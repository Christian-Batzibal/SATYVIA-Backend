using HotelReservationAPI.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public decimal Price { get; set; }
    public int BranchId { get; set; }
    public string? ImagePath { get; set; }
    public Branch? Branch { get; set; }

    public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
}
