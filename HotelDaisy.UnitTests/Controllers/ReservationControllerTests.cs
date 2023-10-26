using HotelDaisy.Controllers;
using HotelDaisy.Data;
using HotelDaisy.Data.Implementations;
using HotelDaisy.Data.Interfaces;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
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

			TimeInterval timeInterval = new TimeInterval()
			{
				StartDate = DateTime.Parse("2025-01-02"),
				EndDate = DateTime.Parse("2025-01-01")
			};
			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(timeInterval);
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

            TimeInterval timeInterval = new TimeInterval()
            {
				StartDate = DateTime.Parse("2020-01-01"),
				EndDate = DateTime.Parse("2025-01-01")
			};
			IActionResult result;
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				result = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object).Index(timeInterval);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("The date can't be earlier than today's date.", error.ErrorMessage);
		}

		[Fact]
		public void IndexPostAction_WhenReservationDbIsEmptyAndReservationTimeIsValid_AllApartmentsAvailable()
		{
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name))
                .AddScoped<IReservationService, ReservationService>()
                .BuildServiceProvider();

			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            TimeInterval timeInterval = new TimeInterval()
            {
				StartDate = DateTime.Parse("2025-01-01"),
				EndDate = DateTime.Parse("2025-01-02")
			};

			IActionResult result;
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Apartment { Balcony = false, NumberOfRooms = 2, Price = 200 });
				db.SaveChanges();
				result = new ReservationController(db, userManagerMock.Object, reservationService).Index(timeInterval);
			}

			List<int> apartmentIds = new List<int> { 1, 2 };

			Assert.NotNull(result);
			var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("CreateFromDate", redirectToAction.ActionName);
			Assert.Equal(timeInterval.StartDate, redirectToAction.RouteValues["sendStart"]);
			Assert.Equal(timeInterval.EndDate, redirectToAction.RouteValues["sendEnd"]);
			Assert.Equal(apartmentIds, redirectToAction.RouteValues["sendIds"]);
		}

		[Fact]
		public void IndexPostAction_WhenReservationTimeIsValidAndAllApartmentsNotAvailable_ReturnError()
		{
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name))
                .AddScoped<IReservationService, ReservationService>()
                .BuildServiceProvider();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            TimeInterval timeInterval = new TimeInterval()
            {
				StartDate = DateTime.Parse("2025-01-01"),
				EndDate = DateTime.Parse("2025-01-02")
			};

			IActionResult result;
			using (var scope = serviceProvider.CreateScope())
			{
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Reservation { StartDate = DateTime.Parse("2025-01-01"), EndDate = DateTime.Parse("2025-01-02"), UserId = 1.ToString(), ApartmentId = 1 });
				db.SaveChanges();

				result = new ReservationController(db, userManagerMock.Object, reservationService).Index(timeInterval);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("No apartments available at this time.", error.ErrorMessage);
		}

		[Fact]
 		public void IndexPostAction_WhenReservationTimeIsValid_ReturnsAvailableApartments()
		{
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name))
                .AddScoped<IReservationService, ReservationService>()
                .BuildServiceProvider();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            TimeInterval timeInterval = new TimeInterval()
            {
				StartDate = DateTime.Now.AddDays(2),
				EndDate = DateTime.Now.AddDays(4)
			};

			IActionResult result;
			using (var scope = serviceProvider.CreateScope())
            {
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Apartment { Balcony = false, NumberOfRooms = 2, Price = 200 });
				db.Add(new Reservation { StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(3), ApartmentId = 1, UserId = 1.ToString()});
				db.SaveChanges();

				result = new ReservationController(db, userManagerMock.Object, reservationService).Index(timeInterval);
			}

			List<int> apartmentIds = new List<int> { 2 };

			Assert.NotNull(result);
			var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("CreateFromDate", redirectToAction.ActionName);
			Assert.Equal(timeInterval.StartDate, redirectToAction.RouteValues["sendStart"]);
			Assert.Equal(timeInterval.EndDate, redirectToAction.RouteValues["sendEnd"]);
			Assert.Equal(apartmentIds, redirectToAction.RouteValues["sendIds"]);
		}

		[Fact]
		public void CreateFromDateGetAction_ReturnViewWithAvailableReservation()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();
			var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			IActionResult result;
			List<Apartment> list = new List<Apartment>();
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				var controller = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object)
				{
					TempData = tempData
				};
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				Apartment apartment = new Apartment { Balcony = false, NumberOfRooms = 2, Price = 200 };
				db.Add(apartment);
				db.SaveChanges();
				list.Add(apartment);
				result = controller.CreateFromDate(new List<int> { 2 }, DateTime.Parse("2025-01-01"), DateTime.Parse("2025-01-02"));
			}
			

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var viewModel = Assert.IsType<AvailableReservation>(viewResult.Model);
			Assert.Equal(DateTime.Parse("2025-01-01"), viewModel.StartDate);
			Assert.Equal(DateTime.Parse("2025-01-02"), viewModel.EndDate);
			Assert.Equal(list.First().Id, 2);
			Assert.Equal(list.First().Price, 200);
		}

		[Fact]
		public void CreateFromDatePostActoion_IfUserLoggedIn_SaveReservation()
		{
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name))
                .AddScoped<IReservationService, ReservationService>()
                .BuildServiceProvider();

			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

			var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "test"));
			userManagerMock.Setup(x => x.GetUserId(user)).Returns("1");

			tempData["StartDate"] = "2025-01-01";
			tempData["EndDate"] = "2025-01-02";
			IActionResult result;
			Reservation reservation = new Reservation();
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();
                var controller = new ReservationController(db, userManagerMock.Object, reservationService)
				{
					TempData = tempData,
					ControllerContext = new ControllerContext
					{
						HttpContext = new DefaultHttpContext { User = user }
					}
				};
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.SaveChanges();

				result = controller.CreateFromDate(1);
				reservation = db.Reservations.FirstOrDefault();
			}

			Assert.NotNull(result);
			Assert.Equal(reservation.StartDate, DateTime.Parse("2025-01-01"));
			Assert.Equal(reservation.EndDate, DateTime.Parse("2025-01-02"));
			Assert.Equal(reservation.UserId, "1");
			Assert.Equal(reservation.ApartmentId, 1);
		}

		[Fact]
		public void CreateForSelectedApartmenGetAction_ReturnViewWithId()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();

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

		[Fact]
		public void CreateForSelectedApartmentPostAction_IfReservationTimeAvailable_SaveReservation()
		{
            var serviceProvider = new ServiceCollection()
				.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name))
				.AddScoped<IReservationService, ReservationService>()
				.BuildServiceProvider();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>
            	(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "test"));
            userManagerMock.Setup(x => x.GetUserId(user)).Returns("1");

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
          
            IActionResult result;
            Reservation reservation = new Reservation();
            ReservationTimeForOneApartment reservationTimeForOneApartment = new ReservationTimeForOneApartment()
            {
                StartDate = DateTime.Parse("2025-01-01"),
                EndDate = DateTime.Parse("2025-01-02"),
                ApartmentId = 1
            };

            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                var controller = new ReservationController(db, userManagerMock.Object, reservationService)
                {
                    TempData = tempData,
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = user }
                    }
                };

                db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.SaveChanges();

				result = controller.CreateForSelectedApartment(reservationTimeForOneApartment);
				reservation = db.Reservations.FirstOrDefault();
			}

			Assert.NotNull(result);
			Assert.Equal(reservationTimeForOneApartment.StartDate, reservation.StartDate);
			Assert.Equal(reservationTimeForOneApartment.EndDate, reservation.EndDate);
			Assert.Equal("1", reservation.UserId);
			Assert.Equal(1, reservation.ApartmentId);
		}

		[Fact]
		public void CreateForSelectedApartmentPostAction_IfApartmentIsReserved_DisplayMessage()
		{
			DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
			optionsBuilder.UseInMemoryDatabase(MethodBase.GetCurrentMethod().Name);
			var userManagerMock = new Mock<UserManager<ApplicationUser>>
				(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var reservationServiceMock = new Mock<IReservationService>();
            

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "test"));
			userManagerMock.Setup(x => x.GetUserId(user)).Returns("1");

			IActionResult result;
			Reservation reservation = new Reservation();
			ReservationTimeForOneApartment reservationTimeForOneApartment = new ReservationTimeForOneApartment()
			{
				StartDate = DateTime.Parse("2025-01-01"),
				EndDate = DateTime.Parse("2025-01-02"),
				ApartmentId = 1
			};
			using (ApplicationDbContext db = new(optionsBuilder.Options))
			{
				var controller = new ReservationController(db, userManagerMock.Object, reservationServiceMock.Object)
				{
					ControllerContext = new ControllerContext
					{
						HttpContext = new DefaultHttpContext { User = user }
					}
				};
				db.Add(new Apartment { Balcony = true, NumberOfRooms = 1, Price = 100 });
				db.Add(new Reservation { StartDate = DateTime.Parse("2025-01-01"), EndDate = DateTime.Parse("2025-01-10"), UserId = "1", ApartmentId = 1 });
				db.SaveChanges();

				result = controller.CreateForSelectedApartment(reservationTimeForOneApartment);
			}

			Assert.NotNull(result);
			var viewResult = Assert.IsType<ViewResult>(result);
			var error = viewResult.ViewData.ModelState[""].Errors[0];
			Assert.Equal("Selected Apartment is not available at this time. Check another dates.", error.ErrorMessage);
		}
	}
}
