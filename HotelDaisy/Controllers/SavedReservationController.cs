using HotelDaisy.Data.Interfaces;
using HotelDaisy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelDaisy.Models.ViewModels;
using HotelDaisy.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using HotelDaisy.Data.Implementations;

namespace HotelDaisy.Controllers
{
    public class SavedReservationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;
        public SavedReservationController(ApplicationDbContext db, UserManager<ApplicationUser> userMenager, IReservationService reservationService)
        {
            _db = db;
            _userManager = userMenager;
            _reservationService = reservationService;
        }

        //GET
        [Authorize]
        public IActionResult UserIndex()
        {
            var ReservationList = _db.Reservations
                .Where(r => r.UserId == _userManager.GetUserId(User)).OrderBy(r => r.StartDate).ToList();

			List<Reservation> upcomingReservations = ReservationList.Where(r => r.StartDate > DateTime.Now).ToList();
			ReservationHistory viewModel = new ReservationHistory()
            {
                UpcomingReservations = upcomingReservations,
				OldReservations = ReservationList.Except(upcomingReservations).ToList()
			};
            return View(viewModel);
        }

        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            var viewModel = _reservationService.JoinReservationAndUser().Where(r => r.StartDate >= DateTime.Now).OrderBy(r => r.StartDate).Take(20).ToList();
            return View(viewModel);
        }

        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Search()
        {
            return View();
        }

        //POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Search(SearchReservation obj)
        {
            if (obj == null)
            {
                return BadRequest();
            }

            var viewModel = _reservationService.JoinReservationAndUser();

            if (obj.SearchOption == "Id")
            {
                var resultId = viewModel.Where(r => r.Id == obj.Id).ToList();

                if (resultId.IsNullOrEmpty())
                {
                    ModelState.AddModelError("", "No reservations with input ID in database");
                }
                else
                {
                    return RedirectToAction("SearchingResults", new { reservations = JsonConvert.SerializeObject(resultId) });
                }
            }
            else if (obj.SearchOption == "Name")
            {
                var resultName = viewModel.Where(r => r.FirstName == obj.Name && r.LastName == obj.LastName).ToList();

                if (resultName.IsNullOrEmpty())
                {
                    ModelState.AddModelError("", "No reservations for input User in database");
                }
                else
                {
                    return RedirectToAction("SearchingResults", new { reservations = JsonConvert.SerializeObject(resultName) });
                }
            }
            else if (obj.SearchOption == "Date")
            {
                var resultDate = _reservationService.GetDatesFromeTimeInterval(viewModel, obj.StartDate, obj.EndDate);

                if (resultDate.IsNullOrEmpty())
                {
                    ModelState.AddModelError("", "No reservations at the input time in database");
                }
                else
                {
                    return RedirectToAction("SearchingResults", new { reservations = JsonConvert.SerializeObject(resultDate) });
                }
            }

            return View(obj);
        }

        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult SearchingResults(string reservations)
        {
            List<ReservationWithUserFullName> reservationList = JsonConvert.DeserializeObject<List<ReservationWithUserFullName>>(reservations);
            return View(reservationList);
        }
    }
}