using System.ComponentModel;

namespace HotelDaisy.Models.ViewModels
{
    public class ReservationTime
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
    }
}
