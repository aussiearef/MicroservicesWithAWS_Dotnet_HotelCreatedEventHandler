// See https://aka.ms/new-console-template for more information


using System.Text.Json;
using Amazon.Lambda.SNSEvents;
using HotelCreatedEventHandler.Models;

Environment.SetEnvironmentVariable("host", "https://hotels.es.australia-southeast1.gcp.elastic-cloud.com");
Environment.SetEnvironmentVariable("userName", "elastic");
Environment.SetEnvironmentVariable("password", "OQP989jVTMLH3S4ew52gylj9");
Environment.SetEnvironmentVariable("indexName", "event");

var hotel = new Hotel
{
    Name = "Continental",
    CityName = "Paris",
    Price = 100,
    Rating = 4,
    Id = "123",
    UserId = "ABC",
    CreationDateTime = DateTime.Now
};

var snsEvent = new SNSEvent
{
    Records = new List<SNSEvent.SNSRecord>
    {
        new()
        {
            Sns = new SNSEvent.SNSMessage
            {
                MessageId = "100",
                Message = JsonSerializer.Serialize(hotel)
            }
        }
    }
};

var handler = new HotelCreatedEventHandler.HotelCreatedEventHandler();
await handler.Handler(snsEvent);