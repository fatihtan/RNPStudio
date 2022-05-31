using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Customer
{
    public class CustomerModel
    {
        public Sky.Core.DTO.Customer Customer { get; set; }

        public Sky.Core.DTO.CustomerProfile CustomerProfile { get; set; }

        public Sky.Core.DTO.CustomerRegisterDetail CustomerRegisterDetail { get; set; }

        public List<Sky.Core.DTO.SocialMediaAccount> SocialMediaAccountList { get; set; }

        public List<Sky.Core.DTO.CustomerBill> CustomerBillList { get; set; }

        public List<Sky.Core.DTO.CustomerBillInfo> CustomerBillInfoList { get; set; }

        public List<Sky.Core.DTO.Payment> PaymentList { get; set; }

        public List<Sky.Core.DTO.RaffleParticipant> RaffleParticipantList { get; set; }

        public Sky.Ramesses.DTO.User CreatorUser { get; set; }


        public List<Sky.CMS.DTO.City> CityList { get; set; }

        public List<Sky.CMS.DTO.Country> CountryList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKJobTitleList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKIndustryList { get; set; }
    }
}