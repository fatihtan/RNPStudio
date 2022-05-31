using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Customer
{
    public class SocialMediaAccountModel
    {
        public Sky.Core.DTO.Customer Customer { get; set; }

        public Sky.Core.DTO.SocialMediaAccount SocialMediaAccount { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKSocialMediaPlatformList { get; set; }
    }
}