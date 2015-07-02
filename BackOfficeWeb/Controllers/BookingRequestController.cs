﻿using System;
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
using Newtonsoft.Json;

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
            var response = _webHelper.GetResponse(string.Format("{0}/GetPendingBookingRequests?page={1}", ConfigurationManager.AppSettings["BookingWebApiUrl"], page), string.Empty, "GET", "text/json", out success);
            var result = JsonConvert.DeserializeObject<BookingListModel>(response);

            SetPagingModel(page, result.TotalPages);

            return View(result.BookingRequests);
        }

        public ActionResult CheckStatus(string status, int page = 0)
        {
            bool success;
            var response = _webHelper.GetResponse(string.Format("{0}/GetStatus?status={1}&page={2}", ConfigurationManager.AppSettings["BookingWebApiUrl"], status, page), string.Empty, "GET", "text/json", out success);
            var result = JsonConvert.DeserializeObject<BookingListModel>(response);

            ViewData["statistics"] = GetBookingStatistics();

            SetPagingModel(page, result.TotalPages);

            return View(result.BookingRequests);
        }

        private void SetPagingModel(int currentPage, int totalPages)
        {
            ViewBag.PagingModel = new PagingModel
            {
                TotalPages = totalPages,
                CurrentPage = currentPage
            };
        }

        private BookingStatisticsModel GetBookingStatistics()
        {
            bool success;
            var response = _webHelper.GetResponse(string.Format("{0}/GetStatistics", ConfigurationManager.AppSettings["BookingWebApiUrl"]), string.Empty, "GET", "text/json", out success);
            var statistics = JsonConvert.DeserializeObject<BookingStatisticsModel>(response);

            return statistics;
        }

        public ActionResult ViewKpi()
        {
            bool success;
            var response = _webHelper.GetResponse(string.Format("{0}/GetBookingProcessKpi", ConfigurationManager.AppSettings["BookingWebApiUrl"]), string.Empty, "GET", "text/json", out success);
            var list = JsonConvert.DeserializeObject<List<BookingRequestKpiModel>>(response);

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
            var bookingRequest = JsonConvert.DeserializeObject<BookingRequest>(response);

            if (bookingRequest == null)
            {
                return HttpNotFound();
            }
            var viewModel = new UpdateStatusViewModel();
            Mapper.Map<BookingRequest, UpdateStatusViewModel>(bookingRequest, viewModel);
            ViewBag.Process = process;
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(UpdateStatusViewModel model)
        {
            bool success;
            var json = JsonConvert.SerializeObject(new UpdateStatusModel { Status = model.Status, UpdatedBy = User.Identity.GetUserName(), ReplyMessage = model.ReplyMessage });
            
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
