using HotelDaisy.Data;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

		//GET
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			return View();
		}

		//POST
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(ApartmentVM obj)
		{
			if (ModelState.IsValid)
			{
				Apartment model = new Apartment
				{
					NumberOfRooms = obj.NumberOfRooms,
					Balcony = obj.Balcony,
					Price = obj.Price,
				};

				if (obj.ImageFile != null)
				{ 
					using (var memoryStream = new MemoryStream())
					{
						await obj.ImageFile.CopyToAsync(memoryStream);
						model.Image = memoryStream.ToArray();
					}
					
					_db.Apartments.Add(model);
					_db.SaveChanges();
				}
			}
			return View();
		}

		//GET
		[Authorize(Roles = "Admin")]
		public IActionResult Edit(int id)
		{
			Apartment apartment = _db.Apartments.FirstOrDefault(a => a.Id == id);
			return View(apartment);
		}

		//POST
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Edit(Apartment obj)
		{
			return View(obj);
		}

		
	}
}
