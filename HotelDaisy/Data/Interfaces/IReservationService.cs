using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;

namespace HotelDaisy.Data.Interfaces
{
	public interface IReservationService
	{
		IQueryable<IGrouping<int, Reservation>> GroupReservationsByApartmentId();
		List<int> CompareWithReservationsInDb(IQueryable<IGrouping<int, Reservation>> apartmentIdGroup, DateTime startDate, DateTime endDate);
		List<ReservationWithUserFullName> JoinReservationAndUser();
	//	IQueryable GetDatesFromeTimeInterval(IQueryable model, DateTime start, DateTime end);
	}
}