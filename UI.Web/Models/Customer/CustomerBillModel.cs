using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Customer
{
    public class CustomerBillModel
    {
        public Sky.Core.DTO.Customer Customer { get; set; }

        public bool IsCustomerIDSupplied { get; set; }

        public List<Sky.Core.DTO.CustomerBillInfo> CustomerBillInfoList { get; set; }

        public Sky.Core.DTO.CustomerBill CustomerBill { get; set; }
    }
}