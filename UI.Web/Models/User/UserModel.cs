using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserModel
    {
        public Sky.Ramesses.DTO.User User { get; set; }

        public Sky.Ramesses.DTO.UserProfile UserProfile { get; set; }

        public Sky.Ramesses.DTO.UserPasswordInfo UserPasswordInfo { get; set; }

        public Sky.Ramesses.DTO.UserRegisterDetail UserRegisterDetail { get; set; }

        public List<Sky.Ramesses.DTO.UserBill> UserBillList { get; set; }

        public List<Sky.Ramesses.DTO.UserBillInfo> UserBillInfoList { get; set; }

        public List<Sky.Ramesses.DTO.UserBankAccount> UserBankAccountList { get; set; }

        public List<Sky.Generic.DTO.ContactMessage> ContactMessageList { get; set; }

        public List<Sky.Core.DTO.Payment> PaymentList { get; set; }

        public List<Sky.Core.DTO.RaffleParticipant> RaffleParticipantList { get; set; }

        public List<Sky.Ramesses.DTO.UserBalance> UserBalanceList { get; set; }

        public Sky.Ramesses.DTO.UserBalance ActiveUserBalance { get; set; }

        public List<Sky.Core.DTO.Customer> CustomerList { get; set; }

        public List<Sky.Ramesses.DTO.UserTeam> UserTeamList { get; set; }


        public List<Sky.CMS.DTO.Country> CountryList { get; set; }

        public List<Sky.CMS.DTO.City> CityList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKBalanceActionList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKJobTitleList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKIndustryList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKLogList { get; set; }
    }
}