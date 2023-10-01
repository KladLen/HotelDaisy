using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelDaisy.Models
{
	public class Apartment
	{
		public int Id { get; set; }
		[DisplayName("Rooms number")]
		public int NumberOfRooms { get; set; }
		public bool Balcony { get; set; }
		[Column(TypeName = "decimal(6,2)")]
		public decimal Price { get; set; }
		public byte[]? Image { get; set; }
		public ICollection<Reservation>? Reservations { get; set;}
	}
}
