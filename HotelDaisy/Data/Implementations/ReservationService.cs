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

        public void AddReservation(DateTime startDate, DateTime endDate, string userId, int apartmentId)
        {
            Reservation reservation = new Reservation
			{
				StartDate = startDate,
				EndDate = endDate,
				UserId = userId,
				ApartmentId = apartmentId
			};
			_db.Reservations.Add(reservation);
			_db.SaveChanges();
        }

		public List<int> apartmentInReservationGroupedById()
		{
            return _db.Reservations.Select(r => r.ApartmentId).Distinct().ToList();
        }

        public List<int> InitApartmentIdsList()
		{
            var allApartmentsId = _db.Apartments.Select(a => a.Id).ToList();
            return allApartmentsId.Except(apartmentInReservationGroupedById()).ToList();
        }

		public List<int> CompareWithReservationsInDb(DateTime startDate, DateTime endDate)
		{
			bool isAvailable = false;
			List<int> result = new List<int>();
			foreach (var apartmentId in apartmentInReservationGroupedById())
			{
                isAvailable = _db.Reservations
                    .Where(r => r.ApartmentId == apartmentId)
                    .All(o => (startDate <= o.StartDate && endDate <= o.StartDate) || (startDate >= o.EndDate && endDate >= o.EndDate));
                if (isAvailable)
				{
					result.Add(apartmentId);
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

		public List<T> GetDatesFromeTimeInterval<T>(List<T> model, DateTime start, DateTime end) where T : ITimeInterval
        {
            return model.Where(r => (r.StartDate >= start && r.StartDate <= end)
                                || (r.EndDate <= end && r.EndDate >= start)
                                || (r.StartDate <= start && r.EndDate >= end)).ToList();
        }

        
    }
}
