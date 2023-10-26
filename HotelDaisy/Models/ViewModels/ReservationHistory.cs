namespace HotelDaisy.Models.ViewModels
{
	public class ReservationHistory
	{
        public List<Reservation> UpcomingReservations { get; set; }
		public List<Reservation> OldReservations { get; set; }

        public ReservationHistory(List<Reservation> reservations)
        {
            List<Reservation> upcomingReservations = reservations.Where(r => r.StartDate > DateTime.Now).ToList();
            UpcomingReservations = upcomingReservations;
            OldReservations = reservations.Except(upcomingReservations).ToList();
        }
    }
}