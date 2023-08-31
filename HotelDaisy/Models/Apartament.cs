using System.ComponentModel.DataAnnotations.Schema;

namespace HotelDaisy.Models
{
	public class Apartament
	{
		public int Id { get; set; }
		public int NumberOfRooms { get; set; }
		public bool Balcony { get; set; }
		[Column(TypeName = "decimal(6,2)")]
		public decimal Price { get; set; }
		public Reservation Reservation { get; set; }
		public ICollection<Reservation> Reservations { get; set;}
	}
}
