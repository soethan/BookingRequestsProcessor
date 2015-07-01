using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Models
{
    public class BookingStatisticsModel
    {
        public int NumberOfBookings { get; set; }
        public int PercentageOfBookings { get; set; }
        public int NumberOfEnquiries { get; set; }
        public int PercentageOfEnquiries { get; set; }
    }
}