namespace HotelDaisy.Models.ViewModels
{
    public class SearchReservation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SearchOption { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
