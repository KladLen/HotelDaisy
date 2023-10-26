using HotelDaisy.Data;
using HotelDaisy.Data.Implementations;
using HotelDaisy.Data.Interfaces;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HotelDaisy.Controllers
{
	public class ReservationController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;
		public ReservationController(ApplicationDbContext db, UserManager<ApplicationUser> userMenager, IReservationService reservationService)
		{
			_db = db;
			_userManager = userMenager;
            _reservationService = reservationService;
		}

		//GET
		public IActionResult Index()
		{
			return View();
		}

		//POST
		[HttpPost]
		public IActionResult Index(TimeInterval obj)
		{
			if (ModelState.IsValid)
			{
                if (obj.StartDate >= obj.EndDate)
                {
                    ModelState.AddModelError("", "Start date must be earlier than end date.");
                    return View();
                }

                DateTime startDate = obj.StartDate;
                DateTime endDate = obj.EndDate;

                if (startDate < DateTime.Now)
                {
                    ModelState.AddModelError("", "The date can't be earlier than today's date.");
                    return View();
                }

                var availableApartmentsIds = _reservationService.InitApartmentIdsList();

				if (_db.Reservations.IsNullOrEmpty())
                {
                    return RedirectToAction("CreateFromDate", new { sendIds = availableApartmentsIds, sendStart = startDate, sendEnd = endDate });
                }

                var idsList = _reservationService.CompareWithReservationsInDb(startDate, endDate);
                availableApartmentsIds.AddRange(idsList);

                if (availableApartmentsIds.IsNullOrEmpty())
                {
                    ModelState.AddModelError("", "No apartments available at this time.");
                    return View();
                }

				return RedirectToAction("CreateFromDate", new { sendIds = availableApartmentsIds, sendStart = startDate, sendEnd = endDate });
            }

            ModelState.AddModelError("", "Input date are not valid.");
			return View();
        }

		//GET
		public IActionResult CreateFromDate(List<int> sendIds, DateTime sendStart, DateTime sendEnd)
        {
            List<Apartment> availableApartments = _db.Apartments.Where(o => sendIds.Contains(o.Id)).ToList();
            var viewModel = new AvailableReservation
            {
                StartDate = sendStart,
                EndDate = sendEnd,
                AvailableApartments = availableApartments
            };
            TempData["StartDate"] = sendStart.ToString();
            TempData["EndDate"] = sendEnd.ToString();
            return View(viewModel);
		}

		//POST
		[HttpPost]
		[Authorize]
		public IActionResult CreateFromDate(int apartmentId)
		{
			if (ModelState.IsValid)
			{
                if (User.Identity.IsAuthenticated)
                {
                    var userId = _userManager.GetUserId(User);
                    var startDateString = TempData["StartDate"] as string;
                    var endDateString = TempData["EndDate"] as string;

                    _reservationService.AddReservation(DateTime.Parse(startDateString), DateTime.Parse(endDateString), userId, apartmentId);

                    return RedirectToAction("Index");
                }

                return RedirectToAction("/Account/Login", new { area = "Identity" });
            }

            return View();
		}

        //GET
        public IActionResult CreateForSelectedApartment(int id)
        {
            var viewModel = new ReservationTimeForOneApartment
            {
                ApartmentId = id,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2)
            };

            return View(viewModel);
        }

        //POST
        [HttpPost, Authorize]
        public IActionResult CreateForSelectedApartment(ReservationTimeForOneApartment obj)
        {
            var viewModel = new ReservationTimeForOneApartment
            {
                ApartmentId = obj.ApartmentId,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2)
            };
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (obj.StartDate >= obj.EndDate)
                    {
                        ModelState.AddModelError("", "Start date must be earlier than end date.");
                        return View(viewModel);
                    }

                    if (obj.StartDate < DateTime.Now)
                    {
                        ModelState.AddModelError("", "The date can't be earlier than today's date.");
                        return View(viewModel);
                    }

                    var isAvailable = _db.Reservations
                        .Where(r => r.ApartmentId == obj.ApartmentId)
                        .All(o => (obj.StartDate <= o.StartDate && obj.EndDate <= o.StartDate) || (obj.StartDate >= o.EndDate && obj.EndDate >= o.EndDate));
                   
                    if (isAvailable)
                    {
                        _reservationService.AddReservation(obj.StartDate, obj.EndDate, _userManager.GetUserId(User), obj.ApartmentId);

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Selected Apartment is not available at this time. Check another dates.");
                    return View(viewModel);
                }
            }

            ModelState.AddModelError("", "Input date are not valid.");
            return View(viewModel);
        }
    }
}
