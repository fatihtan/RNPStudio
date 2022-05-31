using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserBillModel
    {
        public Sky.Ramesses.DTO.User User { get; set; }

        public bool IsUserIDSupplied { get; set; }

        public List<Sky.Ramesses.DTO.UserBillInfo> UserBillInfoList { get; set; }

        public Sky.Ramesses.DTO.UserBill UserBill { get; set; }
    }
}