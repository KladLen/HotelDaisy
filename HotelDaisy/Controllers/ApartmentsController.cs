using HotelDaisy.Data;
using HotelDaisy.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelDaisy.Controllers
{
	public class ApartmentsController : Controller
	{
		private readonly ApplicationDbContext _db;
		public ApartmentsController(ApplicationDbContext db)
		{
			_db = db;
		}
		//GET
		public IActionResult Index()
		{
			IEnumerable<Apartment> objApartmentList = _db.Apartments;
			return View(objApartmentList);
		}
	}
}
