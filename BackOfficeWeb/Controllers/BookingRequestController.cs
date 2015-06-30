using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataAccessLayer;
using DataAccessLayer.Models;
using BackOfficeWeb.Models;
using AutoMapper;
using NotificationServices.Interfaces;
using System.Text;
using DataAccessLayer.Repositories;
using Microsoft.AspNet.Identity;

namespace BackOfficeWeb.Controllers
{
    [Authorize]
    public class BookingRequestController : Controller
    {
        private BookingRequestsDbContext db = new BookingRequestsDbContext();
        private readonly IEmailNotification _emailNotification;
        IBookingRequestRepository _bookingRequestRepository;
        IBookingMainRepository _mainRepository;

        public BookingRequestController(IEmailNotification emailNotification, IBookingRequestRepository bookingRequestRepository, IBookingMainRepository mainRepository)
        {
            _emailNotification = emailNotification;
            _bookingRequestRepository = bookingRequestRepository;
            _mainRepository = mainRepository;
            Mapper.CreateMap<BookingRequest, UpdateStatusViewModel>();
        }

        // GET: BookingRequest
        public ActionResult Index()
        {
            return View(db.BookingRequests.ToList());
        }

        // GET: BookingRequest/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRequest bookingRequest = db.BookingRequests.Find(id);
            if (bookingRequest == null)
            {
                return HttpNotFound();
            }
            var viewModel = new UpdateStatusViewModel();
            Mapper.Map<BookingRequest, UpdateStatusViewModel>(bookingRequest, viewModel);
            return View(viewModel);
        }

        // GET: BookingRequest/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookingRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RequestNumber,SerialNo,RequestorId,RequestorFirstName,RequestorLastName,RequestorMobilePhoneNo,RequestorDeskPhoneNo,RequestorEmail,PickUpAddress1,PickUpAddress2,PickUpAddressCity,PickUpAddressProvince,PickUpAddressPostal,PickUpAddressCountry,PickUpLatitue,PickUpLongitute,RecipientFirstName,RecipientLastName,RecipientMobilePhoneNo,RecipientDeskPhoneNo,RecipientEmail,SendToAddress1,SendToAddress2,SendToAddressCity,SendToAddressProvince,SendToAddressPostal,SendToAddressCountry,SendToLatitue,SendToLongitute,Remarks,Status")] BookingRequest bookingRequest)
        {
            if (ModelState.IsValid)
            {
                db.BookingRequests.Add(bookingRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookingRequest);
        }

        // GET: BookingRequest/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRequest bookingRequest = db.BookingRequests.Find(id);
            if (bookingRequest == null)
            {
                return HttpNotFound();
            }
            return View(bookingRequest);
        }

        // POST: BookingRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequestNumber,SerialNo,RequestorId,RequestorFirstName,RequestorLastName,RequestorMobilePhoneNo,RequestorDeskPhoneNo,RequestorEmail,PickUpAddress1,PickUpAddress2,PickUpAddressCity,PickUpAddressProvince,PickUpAddressPostal,PickUpAddressCountry,PickUpLatitue,PickUpLongitute,RecipientFirstName,RecipientLastName,RecipientMobilePhoneNo,RecipientDeskPhoneNo,RecipientEmail,SendToAddress1,SendToAddress2,SendToAddressCity,SendToAddressProvince,SendToAddressPostal,SendToAddressCountry,SendToLatitue,SendToLongitute,Remarks,Status")] BookingRequest bookingRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookingRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookingRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(UpdateStatusViewModel model)
        {
            var confirmedBooking = _bookingRequestRepository.UpdateStatus(model.RequestNumber, model.Status, User.Identity.GetUserName());
            _bookingRequestRepository.SaveChanges();

            confirmedBooking.UpdatedBy = null;
            confirmedBooking.UpdatedDate = null;
            confirmedBooking.CreatedDate = DateTimeOffset.UtcNow;
            confirmedBooking.CreatedBy = User.Identity.GetUserName();
            _mainRepository.Create(confirmedBooking);
            _mainRepository.SaveChanges();

            //TODO: refactor call API to update BookingRequests DB. 
            if (confirmedBooking.Status == "Confirmed")
            {
                var content = new StringBuilder();
                content.Append(string.Format("RequestNumber:{0}<br/>", model.RequestNumber));
                content.Append(string.Format("Pickup Location:{0},{1},{2},{3},{4},{5}<br/>", confirmedBooking.PickUpAddress1, confirmedBooking.PickUpAddress2, confirmedBooking.PickUpAddressCity, confirmedBooking.PickUpAddressCountry, confirmedBooking.PickUpAddressPostal, confirmedBooking.PickUpAddressProvince));
                
                _emailNotification.Send("no-reply@alpha.com", new List<string>{"delivery-office@alpha.com"}, "Parcel Pickup", content.ToString());
            }
            else if (confirmedBooking.Status == "Enquiry")
            {
                _emailNotification.Send("no-reply@alpha.com", new List<string> { confirmedBooking.RequestorEmail }, "Enquiry", model.ReplyMessage);
            }
            return RedirectToAction("Index");
        }

        // GET: BookingRequest/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRequest bookingRequest = db.BookingRequests.Find(id);
            if (bookingRequest == null)
            {
                return HttpNotFound();
            }
            return View(bookingRequest);
        }

        // POST: BookingRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            BookingRequest bookingRequest = db.BookingRequests.Find(id);
            db.BookingRequests.Remove(bookingRequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
