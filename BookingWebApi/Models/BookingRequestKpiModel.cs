using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingWebApi.Models
{
    public class BookingRequestKpiModel
    {
        public string RequestNumber { get; set; }
        public string AttendedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset AttendedDate { get; set; }
        public string TimeTaken
        {
            get
            {
                int totalMins = Convert.ToInt32((AttendedDate - CreatedDate).TotalMinutes);
                return string.Format("{0} hr(s) and {1} min(s)", totalMins / 60, totalMins % 60);
            }
        }
    }
}