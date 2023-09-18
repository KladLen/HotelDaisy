namespace HotelDaisy.Models.ViewModels
{
    public class AvailableReservation
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<Apartment> AvailableApartments { get; set; }
        public int ChosenApartmentId { get; set; }
    }
}
