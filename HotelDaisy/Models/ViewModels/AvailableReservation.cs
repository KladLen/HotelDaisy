﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HotelDaisy.Models.ViewModels
{
    public class AvailableReservation
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        public List<Apartment> AvailableApartments { get; set; }
 //       public int ChosenApartmentId { get; set; }
    }
}
