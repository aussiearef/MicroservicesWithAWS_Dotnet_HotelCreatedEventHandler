using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SNSEvents;
using HotelCreatedEventHandler.Models;
using Nest;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace HotelCreatedEventHandler;

public class HotelCreatedEventHandler
{
    public async Task Handler(SNSEvent snsEvent)
    {
        Console.WriteLine("Lambda was invoked.");
        var dbClient = new AmazonDynamoDBClient();
        var table = Table.LoadTable(dbClient, "hotel-created-event-ids");

        var host = Environment.GetEnvironmentVariable("host");
        var userName = Environment.GetEnvironmentVariable("userName");
        var password = Environment.GetEnvironmentVariable("password");
        var indexName = Environment.GetEnvironmentVariable("indexName");

        var connSettings = new ConnectionSettings(new Uri(host));
        connSettings.BasicAuthentication(userName, password);
        connSettings.DefaultIndex(indexName);
        connSettings.DefaultMappingFor<Hotel>(m => m.IdProperty(p => p.Id));

        var esClient = new ElasticClient(connSettings);

        if (!(await esClient.Indices.ExistsAsync(indexName)).Exists) await esClient.Indices.CreateAsync(indexName);

        Console.WriteLine($"Found {snsEvent.Records.Count} records in SNS Event");

        foreach (var eventRecord in snsEvent.Records)
        {
            var eventId = eventRecord.Sns.MessageId;
            var foundItem = await table.GetItemAsync(eventId);
            if (foundItem == null)
                await table.PutItemAsync(new Document
                {
                    ["eventId"] = eventId
                });

            var hotel = JsonSerializer.Deserialize<Hotel>(eventRecord.Sns.Message);

            var response = await esClient.IndexDocumentAsync<Hotel>(hotel);
            if (response.Result == Result.Error) Console.WriteLine($"Server Error:{response.ServerError.Error.Reason}");
        }
    }
}