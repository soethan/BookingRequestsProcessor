using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Models
{
    public class BookingRequestKpiListModel
    {
        public List<BookingRequestKpiModel> BookingRequestKpis { get; set; }
        public int TotalPages { get; set; }
    }
}
