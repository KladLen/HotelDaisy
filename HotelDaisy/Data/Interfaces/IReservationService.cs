using HotelDaisy.Models;

namespace HotelDaisy.Data.Interfaces
{
	public interface IReservationService
	{
		IQueryable<IGrouping<int, Reservation>> GroupReservationsByApartmentId();
	}
}