using HotelDaisy.Data.Interfaces;
using HotelDaisy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelDaisy.Models.ViewModels;
using HotelDaisy.Models;
using System.Collections.Generic;

namespace HotelDaisy.Controllers
{
    public class SavedReservationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public SavedReservationController(ApplicationDbContext db, UserManager<ApplicationUser> userMenager)
        {
            _db = db;
            _userManager = userMenager;
        }

        //GET
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
        public IActionResult AdminIndex()
        {
            var reservationList = _db.Reservations.Join(_db.Users, reservation => reservation.UserId, user => user.Id, (reservation, user) => new
            {
                reservation.Id, reservation.StartDate, reservation.EndDate, reservation.ApartmentId, user.FirstName, user.LastName
            }).ToList();
            List<ReservationWithUserFullName> viewModel = reservationList.Select(item => new ReservationWithUserFullName
            {
                Id = item.Id,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                ApartmentId = item.ApartmentId,
                FirstName = item.FirstName,
                LastName = item.LastName
            }).Where(r => r.StartDate >= DateTime.Now).OrderBy(r => r.StartDate).Take(20).ToList();
            return View(viewModel);
        }

        //GET
        public IActionResult Search()
        {
            return View();
        }
    }
}
