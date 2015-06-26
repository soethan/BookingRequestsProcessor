using DataAccessLayer.EntityConfigurations;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingRequestsDbContext: DbContext
    {
        public BookingRequestsDbContext()
            : base("name=DefaultConnection")
        {

        }

        public IDbSet<BookingRequest> BookingRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BookingRequestConfiguration());
        }
    }
}
