using HotelDaisy.Controllers;
using HotelDaisy.Data;
using HotelDaisy.Data.Implementations;
using HotelDaisy.Data.Interfaces;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;
using Xunit;

namespace HotelDaisy.UnitTests.Controllers
{
	public class ReservationControllerTests
	{
		[Fact]
		public void IndexPostAction_WhenStartDateGreaterThanOrEqualToEndDate_ReturnError()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2025-01-02"),
				EndDate = DateTime.Parse("2025-01-01")
			};
			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(reservationTime);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("Start date must be earlier than end date.", error.ErrorMessage);
			Assert.Equal(viewResult.ViewData.ModelState.ErrorCount, 1);
		}

		[Fact]
		public void IndexPostAction_WhenStartDateSmallerThanTodaysDate_ReturnError()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2020-01-01"),
				EndDate = DateTime.Parse("2025-01-01")
			};
			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(reservationTime);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("The date can't be earlier than today's date.", error.ErrorMessage);
		}

		[Fact]
		public void IndexPostAction_WhenReservationDbIsEmptyAndReservationTimeIsValid_AllApartmentsAvailable()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2025-01-01"),
				EndDate = DateTime.Parse("2025-01-02")
			};

			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Apartment { Balcony = false, NumberOfRooms = 2, Price = 200 });
				db.SaveChanges();
				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(reservationTime);
			}

			List<int> apartmentIds = new List<int> { 1, 2 };

			Assert.NotNull(result);
			var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("CreateFromDate", redirectToAction.ActionName);
			Assert.Equal(reservationTime.StartDate, redirectToAction.RouteValues["sendStart"]);
			Assert.Equal(reservationTime.EndDate, redirectToAction.RouteValues["sendEnd"]);
			Assert.Equal(apartmentIds, redirectToAction.RouteValues["sendIds"]);
		}

		[Fact]
		public void IndexPostAction_WhenReservationTimeIsValidAndAllApartmentsNotAvailable_ReturnError()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2025-01-01"),
				EndDate = DateTime.Parse("2025-01-02")
			};

			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Reservation { StartDate = DateTime.Parse("2025-01-01"), EndDate = DateTime.Parse("2025-01-02"), UserId = 1.ToString(), ApartmentId = 1 });
				db.SaveChanges();

				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(reservationTime);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("No apartments available at this time.", error.ErrorMessage);
		}

		[Fact]
		public void IndexPostAction_WhenReservationTimeIsValid_ReturnsAvailableApartments()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2025-02-01"),
				EndDate = DateTime.Parse("2025-02-02")
			};

			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Apartment { Balcony = false, NumberOfRooms = 2, Price = 200 });
				db.Add(new Reservation { StartDate = DateTime.Parse("2025-03-01"), EndDate = DateTime.Parse("2025-03-02"), UserId = 1.ToString(), ApartmentId = 2 });
				db.SaveChanges();

				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(reservationTime);
			}

			List<int> apartmentIds = new List<int> { 1, 2 };

			Assert.NotNull(result);
			var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("CreateFromDate", redirectToAction.ActionName);
			Assert.Equal(reservationTime.StartDate, redirectToAction.RouteValues["sendStart"]);
			Assert.Equal(reservationTime.EndDate, redirectToAction.RouteValues["sendEnd"]);
			Assert.Equal(apartmentIds, redirectToAction.RouteValues["sendIds"]);
		}

		[Fact]
		public void CreateForSelectedApartmenGetAction_ReturnViewModelWithId()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

			//var mockUser = new Mock<UserManager<ApplicationUser>>();
			//mockUser.Setup(x => x.FindByIdAsync("123"))
			//.ReturnsAsync(new ApplicationUser()
			//{
			//	UserName = "test@email.com",
			//	Id = "123"
			//});


			ReservationTimeForOneApartment reservationTime = new ReservationTimeForOneApartment()
			{ ApartmentId = 1 };

			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object)
					.CreateForSelectedApartment(1);
			}
			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var viewModel = Assert.IsType<ReservationTimeForOneApartment>(viewResult.Model);
			Assert.Equal(viewModel.ApartmentId, reservationTime.ApartmentId);
		}
	}
}
