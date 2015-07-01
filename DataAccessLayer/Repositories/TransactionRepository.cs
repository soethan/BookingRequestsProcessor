using Booking.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Data.Entity;
using System.Linq;
using System.Text;
using AutoMapper;

namespace DataAccessLayer.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ILog _log;
        private readonly BookingMainDbContext _mainDbContext;
        private readonly BookingRequestsDbContext _bookingRequestsDbContext;

        public TransactionRepository(BookingMainDbContext mainDbContext, BookingRequestsDbContext bookingRequestsDbContext, ILog log)
        {
            Mapper.CreateMap<BookingRequest, BookingRequest>();

            _log = log;
            _mainDbContext = mainDbContext;
            _bookingRequestsDbContext = bookingRequestsDbContext;
            
        }

        public bool UpdateStatus(string requestNumber, string status, string updatedBy)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    //1. Update BookingRequests DB
                    var bookingRequest = _bookingRequestsDbContext.BookingRequests
                                        .Where(b => b.RequestNumber == requestNumber)
                                        .FirstOrDefault();

                    bookingRequest.Status = status;
                    bookingRequest.UpdatedBy = updatedBy;
                    bookingRequest.UpdatedDate = DateTimeOffset.UtcNow;

                    //2. Update Main DB
                    var confirmedBooking = new BookingRequest();
                    Mapper.Map<BookingRequest, BookingRequest>(bookingRequest, confirmedBooking);
                    SaveConfirmedBooking(confirmedBooking, updatedBy);

                    _bookingRequestsDbContext.SaveChanges();
                    _mainDbContext.SaveChanges();

                    transactionScope.Complete();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("UpdateStatus ERROR", ex);
                    return false;
                }
            }
        }

        private void SaveConfirmedBooking(BookingRequest confirmedBooking, string updatedBy)
        {
            confirmedBooking.UpdatedBy = null;
            confirmedBooking.UpdatedDate = null;
            confirmedBooking.CreatedDate = DateTimeOffset.UtcNow;
            confirmedBooking.CreatedBy = updatedBy;
            _mainDbContext.BookingRequests.Add(confirmedBooking);
        }
    }
}
