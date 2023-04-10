using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.SNSEvents;
using HotelCreatedEventHandler.Models;
using Nest;

namespace HotelCreatedEventHandler;

public class HotelCreatedEventHandler
{
    public async Task Handler(SNSEvent snsEvent)
    {
        var dbClient = new AmazonDynamoDBClient();
        var table = Table.LoadTable(dbClient, "hotel-created-event-ids");

        var host = Environment.GetEnvironmentVariable("host");
        var userName = Environment.GetEnvironmentVariable("userName");
        var password = Environment.GetEnvironmentVariable("password");
        var indexName = Environment.GetEnvironmentVariable("indexName");

        var connSetting = new ConnectionSettings(new Uri(host));
        connSetting.BasicAuthentication(userName, password);
        connSetting.DefaultIndex(indexName);
        connSetting.DefaultMappingFor<Hotel>(m => m.IdProperty(p => p.Id));

        var client = new ElasticClient(connSetting);

        if (!(await client.Indices.ExistsAsync(indexName)).Exists) await client.Indices.CreateAsync(indexName);

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
            await client.IndexDocumentAsync<Hotel>(hotel);
        }
    }
}