using HotelDaisy.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelDaisy.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
