namespace HotelReservationAPI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string DPI { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }
    }
}
