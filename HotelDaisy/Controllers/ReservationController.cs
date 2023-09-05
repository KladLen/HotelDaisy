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
			bool isOverlap = _db.Reservations
				.Any(o => !((startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate)));

			if (isOverlap)
			{
				return View();
			}
			return RedirectToAction("Create");
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
