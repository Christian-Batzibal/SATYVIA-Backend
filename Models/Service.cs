using System.Collections.Generic;

namespace HotelReservationAPI.Models
{
    public class Service
    {
        public int Id { get; set; }          
        public string Name { get; set; }  
        public string Description { get; set; }
              
        public ICollection<RoomService> RoomServices { get; set; }
    }
}
