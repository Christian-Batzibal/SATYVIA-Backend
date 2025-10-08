using HotelReservationAPI.Models;

namespace HotelReservationAPI.Data
{
    public static class DbSeeder
    {
        public static void SeedRoomImages(AppDbContext context)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "rooms");

            foreach (var room in context.Room.ToList())
            {
                // 📁 Ejemplo: 1Room1, 2Room3...
                string folderName = $"{room.BranchId}Room{room.RoomNumber}";
                string folderPath = Path.Combine(basePath, folderName);

                if (Directory.Exists(folderPath))
                {
                    var imageFiles = Directory.GetFiles(folderPath, "*.*")
                        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                 || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                 || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var file in imageFiles)
                    {
                        byte[] imageBytes = File.ReadAllBytes(file);
                        string base64String = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";

                        if (!context.RoomImage.Any(i => i.RoomId == room.Id && i.ImageBase64 == base64String))
                        {
                            context.RoomImage.Add(new RoomImage
                            {
                                RoomId = room.Id,
                                ImageBase64 = base64String
                            });
                        }
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
