using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserBalanceModel
    {
        public Sky.Ramesses.DTO.User User { get; set; }

        public Sky.Ramesses.DTO.UserBalance UserBalance { get; set; }
    }
}