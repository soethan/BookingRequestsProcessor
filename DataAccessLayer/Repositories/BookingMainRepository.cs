using Booking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Repositories
{
    public class BookingMainRepository : IBookingMainRepository
    {
        private readonly BookingMainDbContext _dbContext;

        public BookingMainRepository(BookingMainDbContext dbContext)
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
