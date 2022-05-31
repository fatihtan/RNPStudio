using Sky.Log.IServices;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Web.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _iSessionService;

        public SessionController(ISessionService iSessionService)
        {
            this._iSessionService = iSessionService;
        }

        public ActionResult Logout()
        {
            this._iSessionService.Kill(this.HttpContext);
            return RedirectToAction("Dashboard", "Home");
        }
    }
}