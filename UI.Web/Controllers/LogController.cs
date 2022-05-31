using Sky.Common.DTO;
using Sky.Common.Provider;
using Sky.Common.Utilities;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Web.Infrastructure;

namespace UI.Web.Controllers
{
    public class LogController : BaseController
    {
        private readonly ILogService _iLogService;

        public LogController(
            ISessionService iSessionService,
            ILogService iLogService)
            : base(iSessionService, iLogService)
        {
            this._iLogService = iLogService;
        }

        [HttpPost]
        [Route("Log/URLErrorAdministrator")]
        public JsonResult URLErrorAdministrator(string URL)
        {
            Result result = new Result();

            LogURLErrorAdministrator luea = new LogURLErrorAdministrator()
            {
                URL = URL,
                AdministratorID = this.OutAdministratorID,
                WebsiteID = Settings.WebsiteID,
                Description = BrowserUtility.GetDescriptionAsJSON(),
                IPAddress = IPProvider.GetIPAddress(this.HttpContext),
                CreatedAt = DateTime.UtcNow
            };

            result.IsSuccess = this._iLogService.SaveLogURLErrorAdministrator(luea);

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [Route("Log/NavigationAdministrator")]
        public JsonResult NavigationAdministrator(string URL)
        {
            Result result = new Result();

            LogNavigationAdministrator lna = new LogNavigationAdministrator()
            {
                URL = URL,
                AdministratorID = this.OutAdministratorID,
                WebsiteID = Settings.WebsiteID,
                Description = BrowserUtility.GetDescriptionAsJSON(),
                IPAddress = IPProvider.GetIPAddress(this.HttpContext),
                CreatedAt = DateTime.UtcNow
            };

            result.IsSuccess = this._iLogService.SaveLogNavigationAdministrator(lna);

            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}