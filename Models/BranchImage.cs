namespace HotelReservationAPI.Models
{
    public class BranchImage
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string ImageBase64 { get; set; }

        public Branch Branch { get; set; }
    }
}
