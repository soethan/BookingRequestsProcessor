using BookingWebApi.App_Start;
using BookingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BookingWebApi.Controllers
{
    public class UserController : ApiController
    {
        public UserController()
        {

        }
        [HttpPost]
        public HttpResponseMessage Login(UserLogin model)
        {
            //http://www.md5.net/md5-generator/
            //userName:password:secretKey
            var hashValue = HashHelper.Md532Bit(string.Format("{0}{1}{2}", model.UserName, model.Password, "BlogApiSecretKey"), null, false);

            if (model.Hash == hashValue)//&& IsValidUser(userName, password)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { token = Guid.NewGuid().ToString() });//Save generated token in DB
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Login Credentials");
        }
    }
}
