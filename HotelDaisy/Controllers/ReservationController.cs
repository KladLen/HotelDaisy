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
            return View();
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
