using HotelDaisy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HotelDaisy.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<Apartment> Apartments { get; set; }
//		public DbSet<Client> Clients { get; set; }
		public DbSet<Reservation> Reservations { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Reservation>().HasOne(r => r.User).WithMany(u => u.Reservations).OnDelete(DeleteBehavior.Restrict).HasForeignKey(r => r.UserId);
		}
	}
}
