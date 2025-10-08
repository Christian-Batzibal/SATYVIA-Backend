namespace HotelReservationAPI.Models
{
    public class RoomImage
    {

        public int Id { get; set; }
        public int RoomId { get; set; }

        public string ImageBase64 { get; set;} = string.Empty;

        public Room Room { get; set; } = null!;
    }
}
