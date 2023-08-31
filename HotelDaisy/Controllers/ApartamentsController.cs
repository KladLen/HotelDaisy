using HotelDaisy.Data;
using HotelDaisy.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelDaisy.Controllers
{
	public class ApartamentsController : Controller
	{
		private readonly ApplicationDbContext _db;
		public ApartamentsController(ApplicationDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			IEnumerable<Apartament> objApartamentList = _db.Apartaments;
			return View(objApartamentList);
		}
	}
}
