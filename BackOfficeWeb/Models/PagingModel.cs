using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackOfficeWeb.Models
{
    public class PagingModel
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}