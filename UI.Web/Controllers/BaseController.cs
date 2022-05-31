using Sky.Common.DTO;
using Sky.Common.Provider;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.SuperUser.DTO;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Web.Infrastructure;

using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;

namespace UI.Web.Controllers
{
    public class BaseController : Controller
    {
        public int OutAdministratorID;
        public Administrator OutAdministratorItem;
        private readonly ISessionService _iSessionService;
        private readonly ILogService _iLogService;

        public List<AdministratorPermissionNavigation> PermissionNavigationList;
        public List<AdministratorPermissionAction> PermissionActionList;

        private static List<string> ExcludedURLList = new List<string>()
        {
            "/error/oops",
            "/log/urlerroradministrator",
            "/log/navigationadministrator"
        };

        public BaseController(ISessionService iSessionService, ILogService iLogService)
        {
            this._iSessionService = iSessionService;
            this._iLogService = iLogService;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            try
            {
                int id;
                if (!this._iSessionService.CheckIsValid(filterContext.HttpContext, out id))
                {
                    filterContext.Result = new RedirectResult(ConfigurationManager.AppSettings["SuperUserURL"]);
                    return;
                }

                List<AdministratorPermissionNavigation> permissionNavigationList;
                if (!this._iSessionService.CheckAdministratorPermissionNavigationIsValid(filterContext.HttpContext, Settings.TKWebsite, out permissionNavigationList))
                {
                    filterContext.Result = new RedirectResult(ConfigurationManager.AppSettings["SuperUserURL"]);
                    return;
                }

                List<AdministratorPermissionAction> permissionActionList;
                if (!this._iSessionService.CheckAdministratorPermissionActionIsValid(filterContext.HttpContext, Settings.TKWebsite, out permissionActionList))
                {
                    filterContext.Result = new RedirectResult(ConfigurationManager.AppSettings["SuperUserURL"]);
                    return;
                }

                if (!ExcludedURLList.Contains(Request.Url.AbsolutePath.ToLower()))
                {
                    new PermissionFilter(PermissionNav.StudioGirisYetkisi).OnActionExecuting(filterContext);
                }

                this.OutAdministratorID = id;
                this.OutAdministratorItem = this._iSessionService.GetAdministrator(filterContext.HttpContext, out id);
                ViewBag.AdministratorItem = this.OutAdministratorItem;

                this.PermissionActionList = permissionActionList;
                this.PermissionNavigationList = permissionNavigationList;
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult(ConfigurationManager.AppSettings["SuperUserURL"]);
                return;
            }
        }

        public void LogTransaction(int DatabaseID, int TableID, int RowID, TK.TKTransactionType TKTransactionType, Object LogContent, string Description)
        {
            var logTransaction = new LogTransaction()
            {
                DatabaseID = DatabaseID,
                TableID = TableID,
                RowID = RowID,
                ActorID = this.OutAdministratorID,
                TKActor = TK.TKActor.Administrator,
                TKTransactionType = TKTransactionType,
                LogContent = (string)LogContent,
                Description = Description,
                CreatedAt = DateTime.UtcNow,
                IPAddress = IPProvider.GetIPAddress(HttpContext)
            };

            this._iLogService.SaveLogTransaction(logTransaction);
        }

        public int LogTransaction(List<LogTransaction> logTransactionList)
        {
            int affectedRows = 0;

            this._iLogService.SaveLogTransaction(logTransactionList, out affectedRows);

            return affectedRows;
        }
    }
}