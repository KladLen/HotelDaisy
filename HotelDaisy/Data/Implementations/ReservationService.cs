using HotelDaisy.Data.Interfaces;
using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;

namespace HotelDaisy.Data.Implementations
{
	public class ReservationService : IReservationService
	{
		private readonly ApplicationDbContext _db;

		public ReservationService(ApplicationDbContext db)
		{
			_db = db;
		}

		public IQueryable<IGrouping<int, Reservation>> GroupReservationsByApartmentId()
		{
			return _db.Reservations.GroupBy(r => r.ApartmentId);
		}

		public List<int> CompareWithReservationsInDb(IQueryable<IGrouping<int, Reservation>> apartmentIdGroup, DateTime startDate, DateTime endDate)
		{
			bool isAvailable = false;
			List<int> result = new List<int>();
			foreach (var group in apartmentIdGroup)
			{
				isAvailable = group.All(o => (startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate));
				if (isAvailable)
				{
					result.Add(group.Key);
				}
			}
			return result;
		}

        public List<ReservationWithUserFullName> JoinReservationAndUser()
        {
			var reservation = _db.Reservations.Join(_db.Users, reservation => reservation.UserId, user => user.Id, (reservation, user) => new
			{
				reservation.Id,
				reservation.StartDate,
				reservation.EndDate,
				reservation.ApartmentId,
				user.FirstName,
				user.LastName
			});
			List<ReservationWithUserFullName> reservationAndUser = reservation.Select(item => new ReservationWithUserFullName
			{
				Id = item.Id,
				StartDate = item.StartDate,
				EndDate = item.EndDate,
				ApartmentId = item.ApartmentId,
				FirstName = item.FirstName,
				LastName = item.LastName
			}).ToList();

            return reservationAndUser;
        }

   //     public IQueryable GetDatesFromeTimeInterval(IQueryable model, DateTime start, DateTime end)
   //     {
			//return model.Where(r => ((r.StartDate >= start && r.StartDate <= end) || (r.EndDate <= end && r.EndDate >= start)
			//					|| (r.StartDate <= start && r.EndDate >= end)));
   //     }
    }
}
