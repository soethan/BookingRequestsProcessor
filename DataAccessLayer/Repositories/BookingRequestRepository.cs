using Booking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Repositories
{
    public class BookingRequestRepository : IBookingRequestRepository
    {
        private readonly BookingRequestsDbContext _dbContext;

        public BookingRequestRepository(BookingRequestsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<BookingRequest> GetAll()
        {
            return _dbContext.BookingRequests.AsQueryable();
        }

        public BookingRequest Create(BookingRequest bookingRequest)
        {
            return _dbContext.BookingRequests.Add(bookingRequest);
        }

        public BookingRequest GetBookingRequest(string requestNumber)
        {
            return GetAll().Where(b => b.RequestNumber == requestNumber).FirstOrDefault();
        }

        public BookingRequest UpdateStatus(string requestNumber, string status, string updatedBy)
        {
            var bookingRequest = GetBookingRequest(requestNumber);
            bookingRequest.Status = status;
            bookingRequest.UpdatedBy = updatedBy;
            bookingRequest.UpdatedDate = DateTimeOffset.UtcNow;
            return bookingRequest;
        }

        public ResultModel SaveChanges()
        {
            var result = new ResultModel { Success = false };
            try
            {
                _dbContext.SaveChanges();
                result.Success = true;
            }
            catch (DbUpdateConcurrencyException)
            {
                result.ErrorMessage = "The data was updated by someone.";
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
            }
            return result;
        }
    }
}
