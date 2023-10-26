using HotelDaisy.Data.Implementations;

namespace HotelDaisy.Models.ViewModels
{
    public class AvailableReservation : TimeInterval
    {
        public List<Apartment> AvailableApartments { get; set; }
    }
}
