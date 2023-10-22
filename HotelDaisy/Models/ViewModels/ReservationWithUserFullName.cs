using System.ComponentModel;

namespace HotelDaisy.Models.ViewModels
{
    public class ReservationWithUserFullName
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ApartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
