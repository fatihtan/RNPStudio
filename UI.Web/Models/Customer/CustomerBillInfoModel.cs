using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Customer
{
    public class CustomerBillInfoModel
    {
        public Sky.Core.DTO.Customer Customer { get; set; }

        public Sky.Core.DTO.CustomerBillInfo CustomerBillInfo { get; set; }
    }
}