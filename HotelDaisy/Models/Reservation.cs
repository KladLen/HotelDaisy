using System.ComponentModel.DataAnnotations.Schema;

namespace HotelDaisy.Models
{
	public class Reservation
	{
		public int Id { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		//[ForeignKey("Client")]
		public int ClientId { get; set; }
		public Client Client { get; set; }
		//[ForeignKey("Apartament")]
		public int ApartmentId { get; set; }
		public Apartment Apartment { get; set; }
	}
}
