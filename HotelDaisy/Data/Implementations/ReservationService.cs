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
	}
}
