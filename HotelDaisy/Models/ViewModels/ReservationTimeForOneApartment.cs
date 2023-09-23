using System.ComponentModel;

namespace HotelDaisy.Models.ViewModels
{
    public class ReservationTimeForOneApartment
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        public int ApartmentId { get; set; }
    }
}
