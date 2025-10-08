using System;
using System.IO;
using System.Linq;
using HotelReservationAPI.Data;
using HotelReservationAPI.Models;

namespace HotelReservationAPI.Data.Seeders
{
    public static class BranchImageSeeder
    {
        public static void Seed(AppDbContext context)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "branches");

            var branches = context.Branch.ToList();
            foreach (var branch in branches)
            {
                string fileName = $"{branch.Id}_{branch.Name.Replace(" ", "_")}";
                string[] possibleExtensions = { ".jpg", ".jpeg", ".png", ".JPG" };

                string filePath = possibleExtensions
                    .Select(ext => Path.Combine(basePath, fileName + ext))
                    .FirstOrDefault(File.Exists);

                if (File.Exists(filePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(filePath);
                    string base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);

                    if (!context.BranchImage.Any(i => i.BranchId == branch.Id))
                    {
                        context.BranchImage.Add(new BranchImage
                        {
                            BranchId = branch.Id,
                            ImageBase64 = base64String
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"Imagen no encontrada: {filePath}");
                }
            }

            context.SaveChanges();
            Console.WriteLine("Imágenes de sucursales insertadas correctamente.");
        }
    }
}
