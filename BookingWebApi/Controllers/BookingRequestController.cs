using BookingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using BookingWebApi.App_Start;
using log4net;
using System.Reflection;
using BookingWebApi.Filters;
using DataAccessLayer.Repositories;
using DataAccessLayer.Models;

namespace BookingWebApi.Controllers
{
    public class BookingRequestController : RestrictedApiController
    {
        private readonly ILog _log;
        private readonly IBookingRequestRepository _bookingRequestRepository;
        private readonly IBookingMainRepository _mainRepository;

        public BookingRequestController(ILog log, IBookingRequestRepository bookingRequestRepository, IBookingMainRepository mainRepository)
        {
            _log = log;
            _bookingRequestRepository = bookingRequestRepository;
            _mainRepository = mainRepository;
        }

        public IEnumerable<BookingRequest> Get(int page = 0)
        {
            var query = _bookingRequestRepository.GetAll()
                    .OrderBy(b => b.RequestNumber)
                    .Skip(Constants.PAGE_SIZE * page)
                    .Take(Constants.PAGE_SIZE);
            return query.ToList();
        }
        
        public HttpResponseMessage GetBookingRequest(string requestNumber)
        {
            var bookingRequest = _bookingRequestRepository.GetBookingRequest(requestNumber);
            if (bookingRequest == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, bookingRequest);
        }
        
        public IEnumerable<BookingRequest> GetStatus(string status)
        {
            var query = _bookingRequestRepository.GetAll()
                        .Where(b => (string.IsNullOrEmpty(status) ? true : b.Status == status))
                        .OrderBy(b => b.RequestNumber);
            return query.ToList();
        }

        public HttpResponseMessage GetStatistics()
        { 
            var numberOfBookings = _bookingRequestRepository.GetAll()
                        .Count(b => b.Status == Constants.BOOKING_STATUS_CONFIRMED);

            var numberOfEnquiries = _bookingRequestRepository.GetAll()
                        .Count(b => b.Status == Constants.BOOKING_STATUS_ENQUIRY);

            var percentageOfBookings = (numberOfBookings / (numberOfBookings + numberOfEnquiries)) * 100;
            var percentageOfEnquiries = (numberOfEnquiries / (numberOfBookings + numberOfEnquiries)) * 100;

            var statistics = new BookingStatisticsModel { 
                NumberOfBookings = numberOfBookings,
                PercentageOfBookings = percentageOfBookings,
                NumberOfEnquiries = numberOfEnquiries,
                PercentageOfEnquiries = percentageOfEnquiries
            };

            return Request.CreateResponse(HttpStatusCode.OK, statistics);
        }

        public HttpResponseMessage Put(string requestNumber, UpdateStatusModel model)
        {
            if (_bookingRequestRepository.GetBookingRequest(requestNumber) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }
            var confirmedBooking = _bookingRequestRepository.UpdateStatus(requestNumber, model.Status, model.UpdatedBy);
            _bookingRequestRepository.SaveChanges();

            SaveConfirmedBooking(confirmedBooking, model.UpdatedBy);

            _log.Info(string.Format("Request Number={0};Status={1};Updated by={2};", requestNumber, model.Status, model.UpdatedBy));

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public IEnumerable<BookingRequestKpiModel> GetBookingProcessKpi(int page = 0)
        {
            var list = _bookingRequestRepository.GetAll()
                    .Where(b => b.Status.Equals(Constants.BOOKING_STATUS_ENQUIRY) || b.Status.Equals(Constants.BOOKING_STATUS_CONFIRMED))
                    .OrderBy(b => b.RequestNumber)
                    .Skip(Constants.PAGE_SIZE * page)
                    .Take(Constants.PAGE_SIZE)
                    .Select(b => new BookingRequestKpiModel { RequestNumber = b.RequestNumber, CreatedDate = b.CreatedDate, AttendedDate = b.UpdatedDate.Value, AttendedBy = b.UpdatedBy }).ToList();
            return list;
        }

        private void SaveConfirmedBooking(BookingRequest confirmedBooking, string updatedBy)
        {
            confirmedBooking.UpdatedBy = null;
            confirmedBooking.UpdatedDate = null;
            confirmedBooking.CreatedDate = DateTimeOffset.UtcNow;
            confirmedBooking.CreatedBy = updatedBy;
            _mainRepository.Create(confirmedBooking);
            _mainRepository.SaveChanges();
        }
    }
}
