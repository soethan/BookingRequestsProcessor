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
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Booking.Models;
using System.Text;
using NotificationServices.Interfaces;
using System.Configuration;

namespace BookingWebApi.Controllers
{
    public class BookingRequestController : RestrictedApiController
    {
        private readonly ILog _log;
        private readonly IEmailNotification _emailNotification;
        private readonly IBookingRequestRepository _bookingRequestRepository;
        private readonly IBookingMainRepository _mainRepository;
        private readonly ITransactionRepository _transactionRepository;

        public BookingRequestController(ILog log, IEmailNotification emailNotification, IBookingRequestRepository bookingRequestRepository, IBookingMainRepository mainRepository, ITransactionRepository transactionRepository)
        {
            _log = log;
            _emailNotification = emailNotification;
            _bookingRequestRepository = bookingRequestRepository;
            _mainRepository = mainRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<BookingRequest> Get(int page = 0)
        {
            var query = _bookingRequestRepository.GetAll()
                    .OrderBy(b => b.CreatedDate)
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
                        .OrderBy(b => b.CreatedDate);
            return query.ToList();
        }

        public IEnumerable<BookingRequest> GetPendingBookingRequests()
        {
            var query = _bookingRequestRepository.GetAll()
                        .Where(b => b.Status == Constants.BOOKING_STATUS_PENDING)
                        .OrderBy(b => b.CreatedDate);
            return query.ToList();
        }

        public HttpResponseMessage GetStatistics()
        {
            var totalBookings = _bookingRequestRepository.GetAll().Count();

            var numberOfBookings = _bookingRequestRepository.GetAll()
                        .Count(b => b.Status == Constants.BOOKING_STATUS_CONFIRMED);

            var numberOfEnquiries = _bookingRequestRepository.GetAll()
                        .Count(b => b.Status == Constants.BOOKING_STATUS_ENQUIRY);

            int percentageOfBookings = Convert.ToInt32(((decimal)numberOfBookings / totalBookings) * 100);
            int percentageOfEnquiries = Convert.ToInt32(((decimal)numberOfEnquiries / totalBookings) * 100);

            var statistics = new BookingStatisticsModel { 
                NumberOfBookings = numberOfBookings,
                PercentageOfBookings = percentageOfBookings,
                NumberOfEnquiries = numberOfEnquiries,
                PercentageOfEnquiries = percentageOfEnquiries
            };

            return Request.CreateResponse(HttpStatusCode.OK, statistics);
        }

        [HttpPost]
        public HttpResponseMessage UpdateStatus(string requestNumber, UpdateStatusModel model)
        {
            var bookingRequest = _bookingRequestRepository.GetBookingRequest(requestNumber);
            if (bookingRequest == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Booking request does not exist.");
            }

            if (model.Status == Constants.BOOKING_STATUS_CONFIRMED)
            {
                bool isSuccessful = _transactionRepository.UpdateStatus(requestNumber, model.Status, model.UpdatedBy);
                if (!isSuccessful)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                SendEmailToDeliveryOffice(bookingRequest);
            }
            else
            {
                _bookingRequestRepository.UpdateStatus(requestNumber, model.Status, model.UpdatedBy);
                var result = _bookingRequestRepository.SaveChanges();
                if (!result.Success)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                SendEmailForEnquiry(model.ReplyMessage, bookingRequest);
            }

            _log.Info(string.Format("Request Number={0};Status={1};Updated by={2};", requestNumber, model.Status, model.UpdatedBy));

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void SendEmailForEnquiry(string replyMessage, BookingRequest bookingRequest)
        {
            _emailNotification.Send(ConfigurationManager.AppSettings["BackEndEmail"], new List<string> { bookingRequest.RequestorEmail }, string.Format("Enquiry-RequestNo-{0}", bookingRequest.RequestNumber), replyMessage);
        }

        private void SendEmailToDeliveryOffice(BookingRequest bookingRequest)
        {
            var content = new StringBuilder();
            content.Append(string.Format("RequestNumber:{0}<br/>", bookingRequest.RequestNumber));
            content.Append(string.Format("Pickup Location:{0},{1},{2},{3},{4},{5}<br/>", bookingRequest.PickUpAddress1, bookingRequest.PickUpAddress2, bookingRequest.PickUpAddressCity, bookingRequest.PickUpAddressCountry, bookingRequest.PickUpAddressPostal, bookingRequest.PickUpAddressProvince));

            _emailNotification.Send(ConfigurationManager.AppSettings["BackEndEmail"], new List<string> { ConfigurationManager.AppSettings["DeliveryOfficeEmail"] }, string.Format("Parcel Pickup-RequestNo-{0}", bookingRequest.RequestNumber), content.ToString());
        }

        public IEnumerable<BookingRequestKpiModel> GetBookingProcessKpi(int page = 0)
        {
            var list = _bookingRequestRepository.GetAll()
                    .Where(b => b.Status.Equals(Constants.BOOKING_STATUS_ENQUIRY) || b.Status.Equals(Constants.BOOKING_STATUS_CONFIRMED))
                    .OrderBy(b => b.CreatedDate)
                    .Skip(Constants.PAGE_SIZE * page)
                    .Take(Constants.PAGE_SIZE)
                    .Select(b => new BookingRequestKpiModel { RequestNumber = b.RequestNumber, CreatedDate = b.CreatedDate, AttendedDate = b.UpdatedDate.Value, AttendedBy = b.UpdatedBy }).ToList();
            return list;
        }

    }
}
