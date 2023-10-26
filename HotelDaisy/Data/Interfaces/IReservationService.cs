using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;

namespace HotelDaisy.Data.Interfaces
{
	public interface IReservationService
	{
		IQueryable<IGrouping<int, Reservation>> GroupReservationsByApartmentId();
		List<int> CompareWithReservationsInDb(IQueryable<IGrouping<int, Reservation>> apartmentIdGroup, DateTime startDate, DateTime endDate);
		List<ReservationWithUserFullName> JoinReservationAndUser();
		public List<T> GetDatesFromeTimeInterval<T>(List<T> model, DateTime start, DateTime end) where T : ITimeInterval;
    }
}