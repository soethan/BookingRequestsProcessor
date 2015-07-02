using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Models
{
    public class BookingListModel
    {
        public List<BookingRequest> BookingRequests { get; set; }
        public int TotalPages { get; set; }
    }
}
