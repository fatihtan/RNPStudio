using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Raffle
{
    public class RaffleParticipantModel
    {
        public Sky.Core.DTO.RaffleParticipant RaffleParticipant { get; set; }

        public List<Sky.Core.DTO.Raffle> RaffleList { get; set; }

        public Sky.Core.DTO.Customer Customer { get; set; }

        public List<Sky.Core.DTO.SocialMediaAccount> SocialMediaAccountList { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }

        public Sky.SuperUser.DTO.Administrator Administrator { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKRaffleParticipationTypeList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKRaffleParticipationStatusList { get; set; }

        public List<Sky.Core.DTO.RaffleParticipant> MakeUpRaffleParticipantList { get; set; }

        public System.Globalization.CultureInfo TurkishCultureInfo { get; set; }
    }
}