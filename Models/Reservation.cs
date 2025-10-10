using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelReservationAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int? RoomId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [JsonIgnore]
        public virtual Room? Room { get; set; }
    }
}
