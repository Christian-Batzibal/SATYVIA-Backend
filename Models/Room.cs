using HotelReservationAPI.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public decimal Price { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int RoomNumber { get; set; }
    public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
}
