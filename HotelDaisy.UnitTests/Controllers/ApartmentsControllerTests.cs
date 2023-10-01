using System.Reflection;
using Xunit;
using Moq;
using HotelDaisy.Data;
using HotelDaisy.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;

namespace HotelDaisy.UnitTests.Controllers
{
	public class ApartmentsControllerTests
	{
		[Fact]
		public void IndexGetAction_GetsApartmentsFromDatabase()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 2, Price = 200 });
				db.SaveChanges();
			}

			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{ 
				result = new ApartmentsController(db).Index();
			}
			
			var viewResult = Assert.IsType<ViewResult>(result);
			var apartments = Assert.IsType<List<Apartment>>(viewResult.Model);
			var apartment = Assert.Single(apartments);
			Assert.NotNull(apartment);
			Assert.True(apartment.Balcony);
			Assert.Equal(2, apartment.NumberOfRooms);
			Assert.Equal(200, apartment.Price);
		}

		[Fact]
		public void EditGetAction_GetApartmentByIdFromDatabase()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 2, Price = 200 });
				db.SaveChanges();
			}

			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ApartmentsController(db).Edit(1);
			}

			var viewResult = Assert.IsType<ViewResult>(result);
			var apartment = Assert.IsType<ApartmentVM>(viewResult.Model);
			Assert.NotNull(apartment);
			Assert.True(apartment.Balcony);
			Assert.Equal(2, apartment.NumberOfRooms);
			Assert.Equal(200, apartment.Price);
		}
	}
}
