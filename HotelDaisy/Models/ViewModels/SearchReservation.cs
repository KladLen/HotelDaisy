using HotelDaisy.Data.Implementations;

namespace HotelDaisy.Models.ViewModels
{
    public class SearchReservation : TimeInterval
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SearchOption { get; set; }
    }
}
