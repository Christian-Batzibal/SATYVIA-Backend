using HotelReservationAPI.Data;
using HotelReservationAPI.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:1841")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JSON Config
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ejecutar seeders automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Seeder de RoomImages (ya existente)
    if (!db.RoomImage.Any())
    {
        DbSeeder.SeedRoomImages(db);
        Console.WriteLine("✅ Imágenes de habitaciones cargadas correctamente.");
    }
    else
    {
        Console.WriteLine("ℹ️ RoomImages ya contiene datos, se omitió el seeder.");
    }

    // Nuevo seeder de BranchImages
    if (!db.BranchImage.Any())
    {
        BranchImageSeeder.Seed(db);
        Console.WriteLine("✅ Imágenes de sucursales cargadas correctamente.");
    }
    else
    {
        Console.WriteLine("ℹ️ BranchImages ya contiene datos, se omitió el seeder.");
    }
}

app.Run();
