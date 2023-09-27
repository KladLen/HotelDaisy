using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelDaisy.Models.ViewModels
{
	public class ApartmentVM
	{
		public int Id { get; set; }
		[Range(1, 10, ErrorMessage = "Value must be betwen 1 and 10")]
		[DisplayName("Rooms number")]
		public int NumberOfRooms { get; set; }
		public bool Balcony { get; set; }
		[Column(TypeName = "decimal(6,2)")]
		public decimal Price { get; set; }
		//[DisplayName("Current image")]
		//public byte[]? Image { get; set; }
		[DisplayName("New image")]
		public IFormFile? ImageFile { get; set; }
	}
}
