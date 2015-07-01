using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingWebApi.App_Start
{
    public class Constants
    {
        public const int PAGE_SIZE = 10000;
        public const string BOOKING_STATUS_PENDING = "Pending";
        public const string BOOKING_STATUS_CONFIRMED = "Confirmed";
        public const string BOOKING_STATUS_ENQUIRY = "Enquiry";
    }
}