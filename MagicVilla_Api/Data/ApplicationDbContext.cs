using MagicVilla_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Api.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<Villa> Villas { get; set; }
    public DbSet<VillaNumber> VillaNumbers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Villa>().HasData(
            new Villa
            {
                Id = 1,
                Name = "Royal Villa",
                Occupancy = 5,
                Details = "Fusce Royal Villa11",
                ImageUrl = "https://example.com/image1.webp",
                Rate = 200,
                Sqft = 500,
                Amenity = "",
                CreatedDate = new DateTime(2025, 9, 29)
            },
            new Villa
            {
                Id = 2,
                Name = "Premium Pool Villa",
                Occupancy = 4,
                Details = "Fusce Premium Pool Villa",
                ImageUrl = "https://example.com/image2.webp",
                Rate = 250,
                Sqft = 600,
                Amenity = "Pool, Wi-Fi",
                CreatedDate = new DateTime(2025, 9, 29)
            },
            new Villa
            {
                Id = 3,
                Name = "Luxury Garden Villa",
                Occupancy = 6,
                Details = "Fusce Luxury Garden Villa",
                ImageUrl = "https://example.com/image3.webp",
                Rate = 300,
                Sqft = 700,
                Amenity = "Garden, Jacuzzi",
                CreatedDate = new DateTime(2025, 9, 29)
            }
        );


    }
}