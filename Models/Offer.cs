namespace HotelReservationAPI.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
