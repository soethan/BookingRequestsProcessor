using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Booking.Common
{
    public class WebHelper
    {
        private readonly ILog _log;

        public WebHelper(ILog log)
        {
            _log = log;
        }
        public string GetResponse(string requestUrl, string requestJson, string httpMethod, string contentType, out bool success)
        {
            WebRequest request = null;
            WebResponse response = null;
            string responseStr = string.Empty;
            success = false;
            try
            {
                request = WebRequest.Create(requestUrl);
                request.Method = httpMethod;
                request.ContentType = contentType;
                //request.ContentLength = requestJson.Length;

                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.WriteLine(requestJson);
                writer.Close();

                response = request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                responseStr = streamReader.ReadToEnd();
                streamReader.Close();
                success = true;
                _log.Info("API call succeeds");
            }
            catch (WebException webEx)
            {
                _log.Error("ERROR", webEx);
            }
            catch (Exception ex)
            {
                _log.Error("ERROR", ex);
            }
            finally
            {
                if (request != null) request.GetRequestStream().Close();
                if (response != null) response.GetResponseStream().Close();
            }

            return responseStr;
        }
    }
}
