using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Payment
{
    public class PaymentListModel
    {
        public List<Sky.Core.DTO.Payment> PaymentList { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }
    }
}