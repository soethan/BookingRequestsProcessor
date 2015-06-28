using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingWebApi.Models
{
    public class UpdateStatusModel
    {
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
    }
}