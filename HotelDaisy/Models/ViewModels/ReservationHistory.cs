namespace HotelDaisy.Models.ViewModels
{
	public class ReservationHistory
	{
		public List<Reservation> UpcomingReservations { get; set; }
		public List<Reservation> OldReservations { get; set; }
	}
}
