using HotelDaisy.Data;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelDaisy.Controllers
{
	public class ReservationController : Controller
	{
		private readonly ApplicationDbContext _db;
		public ReservationController(ApplicationDbContext db)
		{
			_db = db;
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
			var model = new AvailableReservation
			{
                AvailableApartmentsIds = sendIds,
                StartDate = DateOnly.FromDateTime(sendStart),
				EndDate = DateOnly.FromDateTime(sendEnd)
			};
			return View(model);
		}
		//      //GET
		//      public IActionResult Create()
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
