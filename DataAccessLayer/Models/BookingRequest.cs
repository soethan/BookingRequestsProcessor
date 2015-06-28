using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class BookingRequest
    {
        public int SerialNo{ get; set; }
        public string RequestNumber{ get; set; }
        public string RequestorId{ get; set; }
        public string RequestorFirstName{ get; set; }
        public string RequestorLastName{ get; set; }
        public string RequestorMobilePhoneNo{ get; set; }
        public string RequestorDeskPhoneNo{ get; set; }
        public string RequestorEmail{ get; set; }

        public string PickUpAddress1{ get; set; }
        public string PickUpAddress2{ get; set; }
        public string PickUpAddressCity{ get; set; }
        public string PickUpAddressProvince{ get; set; }
        public string PickUpAddressPostal{ get; set; }
        public string PickUpAddressCountry{ get; set; }
        public decimal PickUpLatitue{ get; set; }
        public decimal PickUpLongitute{ get; set; }

        public string RecipientFirstName{ get; set; }
        public string RecipientLastName{ get; set; }
        public string RecipientMobilePhoneNo{ get; set; }
        public string RecipientDeskPhoneNo{ get; set; }
        public string RecipientEmail{ get; set; }

        public string SendToAddress1{ get; set; }
        public string SendToAddress2{ get; set; }
        public string SendToAddressCity{ get; set; }
        public string SendToAddressProvince{ get; set; }
        public string SendToAddressPostal{ get; set; }
        public string SendToAddressCountry{ get; set; }
        public decimal SendToLatitue{ get; set; }
        public decimal SendToLongitute{ get; set; }

        public string Remarks { get; set; }
        public string Status{ get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
