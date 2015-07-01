using Booking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityConfigurations
{
    public class BookingRequestConfiguration: EntityTypeConfiguration<BookingRequest>
    {
        public BookingRequestConfiguration()
        {
            HasKey(b => b.RequestNumber);
        }
    }
}
