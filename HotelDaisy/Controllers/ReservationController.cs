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
		public IActionResult Index(Reservation obj)
		{
			if (obj.StartDate == null || obj.EndDate == null || obj.StartDate >= obj.EndDate) 
			{
				return View();
			}
			DateTime startDate = (DateTime)obj.StartDate;
			DateTime endDate = (DateTime)obj.EndDate;

			if (startDate < DateTime.Now)
			{
				return View();
			}

			List<int> availableApartmentsIds = new List<int>();
            bool isAvailable = false;
            var apartamentIdGroup = _db.Reservations.GroupBy(o => o.ApartmentId);

			List<int> allApartmentsInReservation = _db.Reservations.Select(r => r.ApartmentId).ToList();
			List<int> allApartmentsId = _db.Apartments.Select(a => a.Id).ToList();
			foreach (var id in allApartmentsId)
			{
				if (!allApartmentsInReservation.Contains(id))
				{
					availableApartmentsIds.Add(id);
				}
			}

			if (_db.Reservations.IsNullOrEmpty())
			{
				availableApartmentsIds = _db.Apartments.Select(o => o.Id).ToList();
                return RedirectToAction("CreateFromDate", new { sendIds = availableApartmentsIds, sendStart = startDate, sendEnd = endDate });
            }

            //var availableApartmentsIds = apartamentIdGroup
            //	.Where(g => g.All(o => (startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate))).Select(g => g.Key).ToList();
            foreach (var group in apartamentIdGroup)
			{
				isAvailable = group.All(o => (startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate));
				if (isAvailable)
				{
					availableApartmentsIds.Add(group.Key);
				}
            }

			if (availableApartmentsIds == null)
			{
				return View();
			}

            return RedirectToAction("CreateFromDate", new { sendIds = availableApartmentsIds, sendStart = startDate, sendEnd = endDate });
		}

		//GET
		public IActionResult CreateFromDate(List<int> sendIds, DateTime sendStart, DateTime sendEnd)
		{
			TempData["StartDate"] = sendStart.ToString();
			TempData["EndDate"] = sendEnd.ToString();
			var availableApartments = _db.Apartments.Where(o => sendIds.Contains(o.Id)).ToList();
			var model = new AvailableReservation
			{
                AvailableApartments = availableApartments,
                StartDate = DateOnly.FromDateTime(sendStart),
				EndDate = DateOnly.FromDateTime(sendEnd)
			};
			return View(model);
		}

		//POST
		[HttpPost]
		public IActionResult CreateFromDate(AvailableReservation obj)
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
					ApartmentId = obj.ChosenApartmentId
				};
				_db.Reservations.Add(reservation);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			return RedirectToAction("/Account/Login", new {area = "Identity"});
		}

		////GET
		//public IActionResult Create()
		//{
		//	return View();
		//}

		////POST
		//[HttpPost]
		//public IActionResult Create(Reservation obj)
		//{
		//	return View(obj);
		//}
	}
}
