using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BackOfficeWeb.Models;
using AutoMapper;
using System.Text;
using Microsoft.AspNet.Identity;
using System.IO;
using log4net;
using Booking.Models;
using System.Web.Script.Serialization;
using Booking.Common;
using System.Configuration;

namespace BackOfficeWeb.Controllers
{
    [Authorize]
    public class BookingRequestController : Controller
    {
        private readonly ILog _log;
        private readonly WebHelper _webHelper;

        public BookingRequestController(ILog log, WebHelper webHelper)
        {
            _log = log;
            _webHelper = webHelper;

            Mapper.CreateMap<BookingRequest, UpdateStatusViewModel>();
        }

        public ActionResult Index(int page = 0)
        {
            bool success;
            var response = _webHelper.GetResponse(string.Format("{0}?page={1}", ConfigurationManager.AppSettings["BookingWebApiUrl"], page), string.Empty, "GET", "text/json", out success);
            var list = (new JavaScriptSerializer().Deserialize(response, typeof(List<BookingRequest>))) as List<BookingRequest>;

            return View(list);
        }

        public ActionResult CheckStatus(string status)
        {
            bool success;
            var response = _webHelper.GetResponse(string.Format("{0}/GetStatus?status={1}", ConfigurationManager.AppSettings["BookingWebApiUrl"], status), string.Empty, "GET", "text/json", out success);
            var list = (new JavaScriptSerializer().Deserialize(response, typeof(List<BookingRequest>))) as List<BookingRequest>;

            return View(list);
        }

        public ActionResult Details(string id, bool process = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bool success;
            var response = _webHelper.GetResponse(string.Format("{0}/GetBookingRequest/{1}", ConfigurationManager.AppSettings["BookingWebApiUrl"], id), string.Empty, "GET", "text/json", out success);
            var bookingRequest = (new JavaScriptSerializer().Deserialize(response, typeof(BookingRequest))) as BookingRequest;

            if (bookingRequest == null)
            {
                return HttpNotFound();
            }
            var viewModel = new UpdateStatusViewModel();
            Mapper.Map<BookingRequest, UpdateStatusViewModel>(bookingRequest, viewModel);
            ViewBag.Process = process;
            return View(viewModel);
        }

        // GET: BookingRequest/Create
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(UpdateStatusViewModel model)
        {
            bool success;
            var json = new JavaScriptSerializer().Serialize(new UpdateStatusModel{ Status = model.Status, UpdatedBy = User.Identity.GetUserName(), ReplyMessage = model.ReplyMessage });
            
            _webHelper.GetResponse(string.Format("{0}/updatestatus/{1}", ConfigurationManager.AppSettings["BookingWebApiUrl"], model.RequestNumber), json, "POST", "text/json", out success);

            if (success)
            {
                return RedirectToAction("Index");
            }
            
            ViewBag.ErrorMessage = "Update Status Error!";
            return RedirectToAction("Details", new { id = model.RequestNumber });
        }
    }
}
