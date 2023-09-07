using HotelDaisy.Data;
using HotelDaisy.Models;
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
			var availableApartmentsIds = _db.Reservations
				.Where(o => ((startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate)))
				.GroupBy(o => o.ApartmentId).Select(group => group.Key).ToList();

			bool isOverlap = _db.Reservations
				.Any(o => !((startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate)));

			if (isOverlap)
			{
				return View();
			}
//			List<int> ids = availableApartments.ToList();

            return RedirectToAction("CreateFromDate", new { sendIds = availableApartmentsIds });
		}
		//GET
		public IActionResult CreateFromDate(List<int> sendIds)
		{
			return View(sendIds);
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
