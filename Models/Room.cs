using HotelReservationAPI.Enums;

namespace HotelReservationAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }

        public int BranchId { get; set; }

        public Branch? Branch { get; set; }
    }
}
