using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelDaisy.Models
{
	public class Reservation
	{
		public int Id { get; set; }
		[DisplayName("Start Date")]
		public DateTime? StartDate { get; set; }
		[DisplayName("End Date")]
		public DateTime? EndDate { get; set; }
		public int ClientId { get; set; }
		public Client Client { get; set; }
		public int ApartmentId { get; set; }
		public Apartment Apartment { get; set; }
	}
}
