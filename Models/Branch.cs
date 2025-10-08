namespace HotelReservationAPI.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<Room>? Rooms { get; set; }   

        public ICollection<BranchImage> BranchImages { get; set; }
    }
}
