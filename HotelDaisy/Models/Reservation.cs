using System.ComponentModel.DataAnnotations.Schema;

namespace HotelDaisy.Models
{
	public class Reservation
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		[ForeignKey("Client")]
		public int ClientId { get; set; }
		public Client Client { get; set; }
		[ForeignKey("Apartament")]
		public int ApartamentId { get; set; }
		public Apartament Apartament { get; set; }
	}
}
