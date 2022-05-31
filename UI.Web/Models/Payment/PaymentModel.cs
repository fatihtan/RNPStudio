using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Payment
{
    public class PaymentModel
    {
        public Sky.Core.DTO.Payment Payment { get; set; }

        public Sky.Core.DTO.Customer Customer { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }


        public List<Sky.Ramesses.DTO.UserBankAccount> UserBankAccountList { get; set; }


        public List<Sky.CMS.DTO.TypeValue> TKPaymentTypeList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKPaymentStatusList { get; set; }

        public List<Sky.CMS.DTO.BankAccount> BankAccountList { get; set; }

        public System.Globalization.CultureInfo TurkishCultureInfo { get; set; }
    }
}