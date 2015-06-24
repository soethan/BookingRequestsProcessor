using FileHelpers;
using System;
using System.Collections.Generic;

namespace BookingRequestsProcessor.Models
{
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    [IgnoreLast(1)]
    public class BookingRequest
    {
        public int SerialNo;
        public string RequestNumber;
        public string RequestorId;
        public string RequestorFirstName;
        public string RequestorLastName;
        public string RequestorMobilePhoneNo;
        public string RequestorDeskPhoneNo;
        public string RequestorEmail;

        public string PickUpAddress1;
        public string PickUpAddress2;
        public string PickUpAddressCity;
        public string PickUpAddressProvince;
        public string PickUpAddressPostal;
        public string PickUpAddressCountry;
        public decimal PickUpLatitue;
        public decimal PickUpLongitute;

        public string RecipientFirstName;
        public string RecipientLastName;
        public string RecipientMobilePhoneNo;
        public string RecipientDeskPhoneNo;
        public string RecipientEmail;

        public string SendToAddress1;
        public string SendToAddress2;
        public string SendToAddressCity;
        public string SendToAddressProvince;
        public string SendToAddressPostal;
        public string SendToAddressCountry;
        public decimal SendToLatitue;
        public decimal SendToLongitute;

        public string Remarks { get; set; }

    }
}
