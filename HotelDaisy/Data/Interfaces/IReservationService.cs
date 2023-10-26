using HotelDaisy.Models;
using HotelDaisy.Models.ViewModels;

namespace HotelDaisy.Data.Interfaces
{
	public interface IReservationService
	{
		void AddReservation(DateTime startDate, DateTime endDate, string userId, int apartmentId);
		List<int> InitApartmentIdsList();
		List<int> apartmentInReservationGroupedById();
		List<int> CompareWithReservationsInDb(DateTime startDate, DateTime endDate);
		List<ReservationWithUserFullName> JoinReservationAndUser();
		List<T> GetDatesFromeTimeInterval<T>(List<T> model, DateTime start, DateTime end) where T : ITimeInterval;
    }
}