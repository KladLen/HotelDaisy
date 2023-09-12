namespace HotelDaisy.Models.ViewModels
{
    public class AvailableReservation
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<int> AvailableApartmentsIds { get; set; }
    }
}
