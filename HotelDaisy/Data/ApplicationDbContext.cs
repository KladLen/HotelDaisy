using HotelDaisy.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelDaisy.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<Apartament> Apartaments { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
	}
}
