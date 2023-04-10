// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Amazon.Lambda.SNSEvents;
using HotelCreatedEventHandler.Models;


Environment.SetEnvironmentVariable("host","");
Environment.SetEnvironmentVariable("userName","elastic");
Environment.SetEnvironmentVariable("password","");
Environment.SetEnvironmentVariable("indexName", "event");

var hotel = new Hotel()
{
    Name = "Continental",
    City = "Paris",
    Id = "1",
    Price = 100,
    Rating = 4,
    UserId = "1234",
    CreationDateTime = DateTime.Now
};

var snsEvent = new SNSEvent()
{
    Records = new List<SNSEvent.SNSRecord>()
    {
        new SNSEvent.SNSRecord()
        {
            Sns = new SNSEvent.SNSMessage()
            {
                Message = JsonSerializer.Serialize(hotel),
                MessageId = "123"
            }
        }
    }
};

var handler = new HotelCreatedEventHandler.HotelCreatedEventHandler();
await handler.Handler(snsEvent);

