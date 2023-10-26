using HotelDaisy.Data.Implementations;


namespace HotelDaisy.Models.ViewModels
{
    public class ReservationWithUserFullName : TimeInterval
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
