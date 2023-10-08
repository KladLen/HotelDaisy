using HotelDaisy.Data.Interfaces;
using HotelDaisy.Models;

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
	}
}
