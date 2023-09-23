using HotelDaisy.Data;
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
		public ReservationController(ApplicationDbContext db, UserManager<ApplicationUser> useserMenager)
		{
			_db = db;
			_userManager = useserMenager;
		}

		//GET
		public IActionResult Index()
		{
			return View();
		}

		//POST
		[HttpPost]
		public IActionResult Index(ReservationTime obj)
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

                bool isAvailable = false;
                var apartamentIdGroup = _db.Reservations.GroupBy(o => o.ApartmentId);

                var allApartmentsInReservation = _db.Reservations.Select(r => r.ApartmentId);
                var allApartmentsId = _db.Apartments.Select(a => a.Id);
                List<int> availableApartmentsIds = allApartmentsId.Except(allApartmentsInReservation).ToList();

                if (_db.Reservations.IsNullOrEmpty())
                {
                    availableApartmentsIds = _db.Apartments.Select(o => o.Id).ToList();
                    return RedirectToAction("CreateFromDate", new { sendIds = availableApartmentsIds, sendStart = startDate, sendEnd = endDate });
                }

                foreach (var group in apartamentIdGroup)
                {
                    isAvailable = group.All(o => (startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate));
                    if (isAvailable)
                    {
                        availableApartmentsIds.Add(group.Key);
                    }
                }

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

                    Reservation reservation = new Reservation
                    {
                        StartDate = DateTime.Parse(startDateString),
                        EndDate = DateTime.Parse(endDateString),
                        UserId = userId,
                        ApartmentId = apartmentId
                    };
                    _db.Reservations.Add(reservation);
                    _db.SaveChanges();
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
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
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
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
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
                        Reservation reservation = new Reservation
                        {
                            StartDate = obj.StartDate,
                            EndDate = obj.EndDate,
                            UserId = _userManager.GetUserId(User),
                            ApartmentId = obj.ApartmentId
                        };
                        _db.Reservations.Add(reservation);
                        _db.SaveChanges();
                        return View("Index");
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
