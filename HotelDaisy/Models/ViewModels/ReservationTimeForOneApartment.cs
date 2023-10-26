using HotelDaisy.Data.Implementations;

namespace HotelDaisy.Models.ViewModels
{
    public class ReservationTimeForOneApartment : TimeInterval
    {
        public int ApartmentId { get; set; }
    }
}
