using HotelDaisy.Data.Interfaces;
using System.ComponentModel;

namespace HotelDaisy.Data.Implementations
{
    public class TimeInterval : ITimeInterval
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
    }
}
