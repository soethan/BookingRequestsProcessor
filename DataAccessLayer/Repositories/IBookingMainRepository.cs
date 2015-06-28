using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IBookingMainRepository
    {
        IQueryable<BookingRequest> GetAll();
        BookingRequest Create(BookingRequest bookingRequest);
        BookingRequest GetBookingRequest(string requestNumber);
        ResultModel SaveChanges();
    }
}
