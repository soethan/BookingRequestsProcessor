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
        private readonly IBookingRequestRepository _repository;

        public BookingRequestController(ILog log, IBookingRequestRepository repository)
        {
            _log = log;
            _repository = repository;
        }

        public IEnumerable<BookingRequest> Get(int page = 0)
        {
            var query = _repository.GetAll()
                    .OrderBy(b => b.RequestNumber)
                    .Skip(Constants.PAGE_SIZE * page)
                    .Take(Constants.PAGE_SIZE);
            return query.ToList();
        }
        
        public HttpResponseMessage GetBookingRequest(string requestNumber)
        {
            var bookingRequest = _repository.GetBookingRequest(requestNumber);
            if (bookingRequest == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, bookingRequest);
        }
        
        public IEnumerable<BookingRequest> GetStatus(string status)
        {
            var query = _repository.GetAll()
                        .Where(b => (string.IsNullOrEmpty(status) ? true : b.Status == status))
                        .OrderBy(b => b.RequestNumber);
            return query.ToList();
        }

        public HttpResponseMessage GetStatistics()
        { 
            var numberOfBookings = _repository.GetAll()
                        .Count(b => b.Status == "Confirmed");

            var numberOfEnquiries = _repository.GetAll()
                        .Count(b => b.Status == "Enquiry");

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
            if (_repository.GetBookingRequest(requestNumber) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }
            _repository.UpdateStatus(requestNumber, model.Status, model.UpdatedBy);
            _repository.SaveChanges();

            _log.Info(string.Format("Request Number={0};Status={1};Updated by={2};", requestNumber, model.Status, model.UpdatedBy));

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
