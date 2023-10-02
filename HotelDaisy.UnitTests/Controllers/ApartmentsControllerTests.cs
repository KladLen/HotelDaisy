using System.Reflection;
using Xunit;
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

		[Fact]
		public async Task EditPostAction_WhenValidModel_UpdateApartmentInDatabase()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

			ApartmentVM apartmentVM = new ApartmentVM
			{
				Id = 1,
				Balcony = true,
				NumberOfRooms = 1,
				Price = 100
			};

			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 2, Price = 200 });
				db.SaveChanges();
			}

			IActionResult result;
			Apartment apartmentAfterUpdate;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				var controller = new ApartmentsController(db);
				result = await controller.Edit(apartmentVM);
				apartmentAfterUpdate = db.Apartments.FirstOrDefault(x => x.Id == 1);				
			}

			Assert.NotNull(apartmentAfterUpdate);
			Assert.Equal(1, apartmentAfterUpdate.NumberOfRooms);
			Assert.True(apartmentAfterUpdate.Balcony);
			Assert.Equal(100, apartmentAfterUpdate.Price);

			var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToAction.ActionName);
		}

		[Fact]
		public void DeleteGetAction_GetApartmentByIdFromDatabase()
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
				result = new ApartmentsController(db).Delete(1);
			}

			var viewResult = Assert.IsType<ViewResult>(result);
			var apartment = Assert.IsType<ApartmentVM>(viewResult.Model);
			Assert.NotNull(apartment);
			Assert.True(apartment.Balcony);
			Assert.Equal(2, apartment.NumberOfRooms);
			Assert.Equal(200, apartment.Price);
		}

		[Fact]
		public void DeletePostAction_WhenValidModel_DeleteApartmentFromDatabase()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

			ApartmentVM apartmentVM = new ApartmentVM
			{
				Id = 1,
				Balcony = true,
				NumberOfRooms = 1,
				Price = 100
			};

			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.SaveChanges();
			}

			IActionResult result;
			Apartment apartment;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ApartmentsController(db).Delete(apartmentVM);
				apartment = db.Apartments.FirstOrDefault(x => x.Id == 1);
			}

			Assert.Null(apartment);

			var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToAction.ActionName);
		}
	}
}
