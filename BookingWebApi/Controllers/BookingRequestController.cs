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
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IBookingRequestRepository _repository;

        public BookingRequestController(IBookingRequestRepository repository)
        {
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

        public HttpResponseMessage Get(string requestNumber)
        {
            var bookingRequest = _repository.GetBookingRequest(requestNumber);
            if (bookingRequest == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, bookingRequest);
        }

        public HttpResponseMessage Put(string requestNumber, UpdateStatusModel model)
        {
            if (_repository.GetBookingRequest(requestNumber) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }
            _repository.UpdateStatus(requestNumber, model.Status, model.UpdatedBy);
            _repository.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
