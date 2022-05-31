using Sky.Common.DTO;
using Sky.Log.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Web.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(
            Sky.SuperUser.IServices.ISessionService iSessionService,
            ILogService iLogService)
            : base(iSessionService, iLogService)
        {

        }

        public ActionResult Oops(Result result, string code)
        {
            ViewBag.Result = result;
            ViewBag.StatusCode = code;

            return View();
        }
    }
}