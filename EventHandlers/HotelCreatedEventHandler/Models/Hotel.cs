namespace HotelCreatedEventHandler.Models;

public class Hotel
{
    public string Name { get; set; }
    public string CityName { get; set; }
    public int Price { get; set; }
    public int Rating { get; set; }
    public string UserId { get; set; }
    public string Id { get; set; }
    public DateTime CreationDateTime { get; set; }
    public string FileName { get; set; }
}