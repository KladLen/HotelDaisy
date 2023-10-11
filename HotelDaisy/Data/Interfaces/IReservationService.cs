using HotelDaisy.Models;

namespace HotelDaisy.Data.Interfaces
{
	public interface IReservationService
	{
		IQueryable<IGrouping<int, Reservation>> GroupReservationsByApartmentId();
		List<int> CompareWithReservationsInDb(IQueryable<IGrouping<int, Reservation>> apartmentIdGroup, DateTime startDate, DateTime endDate);
	}
}