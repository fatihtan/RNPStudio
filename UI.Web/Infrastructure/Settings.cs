using Sky.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Infrastructure
{
    public class Settings
    {
        public static readonly int TIMEZONE = 3;

        public static readonly TK.TKActor TKActor = TK.TKActor.Administrator;

        public static readonly int WebsiteID = (int)TK.TKWebsite.Studio;

        public static readonly TK.TKWebsite TKWebsite = TK.TKWebsite.Studio;

        public static readonly int SmsSenderTitleID = 1;
    }
}