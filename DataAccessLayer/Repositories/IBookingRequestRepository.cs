using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IBookingRequestRepository
    {
        IQueryable<BookingRequest> GetAll();
        BookingRequest Create(BookingRequest bookingRequest);
        BookingRequest GetBookingRequest(string requestNumber);
        BookingRequest UpdateStatus(string requestNumber, string status, string updatedBy);
        ResultModel SaveChanges();
    }
}
