using HotelDaisy.Controllers;
using HotelDaisy.Data;
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

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2025-01-02"),
				EndDate = DateTime.Parse("2025-01-01")
			};
			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object).Index(reservationTime);
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

			ReservationTime reservationTime = new ReservationTime()
			{
				StartDate = DateTime.Parse("2020-01-01"),
				EndDate = DateTime.Parse("2025-01-01")
			};
			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object).Index(reservationTime);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("The date can't be earlier than today's date.", error.ErrorMessage);
		}

		[Fact]
		public void CreateForSelectedApartmenGetAction_ReturnViewModelWithId()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);

			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

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
				result = new ReservationController(db, userManagerMock.Object)
					.CreateForSelectedApartment(1);
			}
			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var viewModel = Assert.IsType<ReservationTimeForOneApartment>(viewResult.Model);
			Assert.Equal(viewModel.ApartmentId, reservationTime.ApartmentId);
		}
	}
}
