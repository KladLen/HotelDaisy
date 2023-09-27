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
			if (apartment == null)
			{
				return NotFound();
			}
			ApartmentVM viewModel = new ApartmentVM
			{
				NumberOfRooms = apartment.NumberOfRooms,
				Balcony = apartment.Balcony,
				Price = apartment.Price,
			};
			return View(viewModel);
		}

		//POST
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(ApartmentVM obj)
		{
			if (ModelState.IsValid)
			{
				var model = _db.Apartments.FirstOrDefault(a => a.Id == obj.Id);
				if (model == null)
				{
					return BadRequest();
				}
				model.NumberOfRooms = obj.NumberOfRooms;
				model.Balcony = obj.Balcony;
				model.Price = obj.Price;

				if (obj.ImageFile != null)
				{
					using (var memoryStream = new MemoryStream())
					{
						await obj.ImageFile.CopyToAsync(memoryStream);
						model.Image = memoryStream.ToArray();
					}
				}
				
				_db.Apartments.Update(model);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View();
		}

		//GET
		[Authorize(Roles = "Admin")]
		public IActionResult Delete(int id)
		{
			Apartment apartment = _db.Apartments.FirstOrDefault(a => a.Id == id);
			if (apartment == null)
			{
				return BadRequest();
			}

			return View(apartment);
		}

		//POST
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Delete(Apartment obj)
		{
			if (ModelState.IsValid)
			{
				_db.Apartments.Remove(obj);
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
