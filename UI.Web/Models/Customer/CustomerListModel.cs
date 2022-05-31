using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Customer
{
    public class CustomerListModel
    {
        public List<Sky.Core.DTO.Customer> CustomerList { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }
    }
}