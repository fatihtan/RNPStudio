using Sky.CMS.Services;
using Sky.Common.DTO;
using Sky.Common.Utilities;
using Sky.Core.IServices;
using Sky.Generic.IServices;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.Ramesses.DTO;
using Sky.Ramesses.IServices;
using Sky.SuperUser.DTO;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Web.Infrastructure;
using UI.Web.Models.Statistics;

using Permission = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;

namespace UI.Web.Controllers
{
    public class StatisticsController : BaseController
    {
        private readonly ILogService _iLogService;
        private readonly IUserService _iUserService;
        private readonly IPaymentService _iPaymentService;
        private readonly IRaffleService _iRaffleService;
        private readonly IGenericService _iGenericService;
        private readonly ICustomerService _iCustomerService;
        private readonly IAdministratorService _iAdministratorService;

        public StatisticsController(
            ILogService iLogService,
            Sky.SuperUser.IServices.ISessionService iSessionService,
            IUserService iUserService,
            IPaymentService iPaymentService,
            IRaffleService iRaffleService,
            IGenericService iGenericService,
            ICustomerService iCustomerService,
            IAdministratorService iAdministratorService)
            : base(iSessionService, iLogService)
        {
            this._iLogService = iLogService;
            this._iUserService = iUserService;
            this._iPaymentService = iPaymentService;
            this._iRaffleService = iRaffleService;
            this._iGenericService = iGenericService;
            this._iCustomerService = iCustomerService;
            this._iAdministratorService = iAdministratorService;
        }

        [PermissionFilter(Permission.LogMonitor)]
        public ActionResult LogMonitor(TK.TKLog? TKLog, string Start, string End, int? UserID, int? AdministratorID)
        {
            DateTime? dtStart = null;
            DateTime? dtEnd = null;
            var model = new LogListModel();

            var TKLogList = TypeService.GetAll(TK.TKLog.Undefined);
            model.TKLogList = TKLogList;

            if (TKLog.HasValue && TKLog != TK.TKLog.Undefined)
            {
                if ((!string.IsNullOrEmpty(Start) && string.IsNullOrEmpty(End)) || (string.IsNullOrEmpty(Start) && !string.IsNullOrEmpty(End)))
                {
                    model.IsTKSelected = true;
                    model.SelectedTKLog = TKLog.Value;
                    model.SelectedTKID = (int)TKLog;
                    ViewBag.LogList = new List<Sky.Log.DTO.BaseDTO>();

                    return View(model);
                }

                if (!string.IsNullOrEmpty(Start) && !string.IsNullOrEmpty(End))
                {
                    ValidationUtility.StringToDatetime(Start, "dd/MM/yyyy", out dtStart);
                    ValidationUtility.StringToDatetime(End, "dd/MM/yyyy", out dtEnd);
                }

                var logList = new List<Sky.Log.DTO.BaseDTO>();

                if (this._iLogService.IsGLog(TKLog.Value))
                {
                    logList = this._iLogService.GetGLogList(TKLog.ToString(), dtStart, dtEnd, UserID);
                }
                else
                {
                    logList = this._iLogService.GetLogList(TKLog.ToString(), dtStart, dtEnd, UserID);
                }

                if (UserID.HasValue)
                {
                    var userItem = this._iUserService.GetUserByIDMEDIUM(UserID.Value);
                    model.UserID = UserID;

                    if (userItem == null)
                    {
                        return RedirectToAction("Oops", "Error");
                    }
                    model.User = userItem;
                }
                else if (AdministratorID.HasValue)
                {
                    var administratorItem = this._iAdministratorService.GetAdministratorByID(AdministratorID.Value);
                    model.AdministratorID = AdministratorID;

                    if (administratorItem == null)
                    {
                        return RedirectToAction("Oops", "Error");
                    }
                    model.Administrator = administratorItem;
                }
                else
                {
                    #region UserID could not be provided
                    int[] userIDArray = new int[0];
                    int[] administratorIDArray = new int[0];
                    switch (TKLog)
                    {
                        case TK.TKLog.LogNavigation:
                            userIDArray = logList.Cast<LogNavigation>()
                                .Where(l => l.UserID.HasValue)
                                .Select(l => l.UserID.Value).ToArray();
                            break;
                        case TK.TKLog.LogNavigationAdministrator:
                            administratorIDArray = logList.Cast<LogNavigationAdministrator>()
                                .Where(l => l.AdministratorID.HasValue)
                                .Select(l => l.AdministratorID.Value).ToArray();
                            break;
                        case TK.TKLog.LogPostView:
                            userIDArray = logList.Cast<LogPostView>()
                                .Where(l => l.UserID.HasValue)
                                .Select(l => l.UserID.Value).ToArray();
                            break;
                        case TK.TKLog.LogSearch:
                            userIDArray = logList.Cast<LogSearch>()
                                .Where(l => l.UserID.HasValue)
                                .Select(l => l.UserID.Value).ToArray();
                            break;
                        case TK.TKLog.LogURLError:
                            userIDArray = logList.Cast<LogURLError>()
                                .Where(l => l.UserID.HasValue)
                                .Select(l => l.UserID.Value).ToArray();
                            break;
                        case TK.TKLog.LogFileDownload:
                            userIDArray = logList.Cast<LogFileDownload>()
                                .Where(l => l.UserID.HasValue)
                                .Select(l => l.UserID.Value).ToArray();
                            break;
                        case TK.TKLog.LogIyzicoCheckoutForm:
                            userIDArray = logList.Cast<LogIyzicoCheckoutForm>().Select(l => l.UserID).ToArray();
                            break;
                        case TK.TKLog.LogPayment:
                            userIDArray = logList.Cast<LogPayment>().Select(l => l.UserID).ToArray();
                            break;
                        case TK.TKLog.LogUserSession:
                            userIDArray = logList.Cast<LogUserSession>()
                                .Where(l => l.UserID.HasValue)
                                .Select(l => l.UserID.Value).ToArray();
                            break;
                        default:
                            break;
                    }

                    #region GetUserList
                    userIDArray = userIDArray.Distinct().ToArray();
                    List<User> userList = new List<User>();
                    if (userIDArray.Any())
                    {
                        userList = this._iUserService.GetUserListByIDList(userIDArray);
                    }
                    #endregion

                    #region GetAdministratorList
                    administratorIDArray = administratorIDArray.Distinct().ToArray();
                    List<Administrator> administratorList = new List<Administrator>();
                    if (administratorIDArray.Any())
                    {
                        administratorList = this._iAdministratorService.GetAdministratorListByIDList(administratorIDArray);
                    }
                    #endregion

                    #region UserName Assign
                    foreach (var item in userList)
                    {
                        #region Switch
                        switch (TKLog)
                        {
                            case TK.TKLog.LogNavigation:
                                logList.Cast<LogNavigation>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogPostView:
                                logList.Cast<LogPostView>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogSearch:
                                logList.Cast<LogSearch>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogURLError:
                                logList.Cast<LogURLError>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogFileDownload:
                                logList.Cast<LogFileDownload>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogIyzicoCheckoutForm:
                                logList.Cast<LogIyzicoCheckoutForm>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogPayment:
                                logList.Cast<LogPayment>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            case TK.TKLog.LogUserSession:
                                logList.Cast<LogUserSession>()
                                    .Where(l => l.UserID == item.ID)
                                    .Select(l =>
                                    {
                                        l.UserFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }
                    #endregion

                    #region AdministratorName Assign
                    foreach (var item in administratorList)
                    {
                        #region Switch
                        switch (TKLog)
                        {
                            case TK.TKLog.LogNavigationAdministrator:
                                logList.Cast<LogNavigationAdministrator>()
                                    .Where(l => l.AdministratorID == item.ID)
                                    .Select(l =>
                                    {
                                        l.AdministratorFullName = string.Format("{0} {1}", item.FirstName, item.LastName);
                                        return l;
                                    })
                                    .ToList();
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }
                    #endregion
                    #endregion
                }

                model.Start = dtStart;
                model.End = dtEnd;
                model.SelectedTKLog = TKLog.Value;
                model.IsTKSelected = true;
                model.SelectedTKID = (int)TKLog;
                model.LogList = logList;

                return View(model);
            }

            return View(model);
        }

        [PermissionFilter(Permission.GrafikMonitor)]
        public ActionResult GraphicMonitor()
        {
            GraphicMonitorModel model = new GraphicMonitorModel()
            {
                TKLogList = TypeService.GetAll(TK.TKLog.Undefined),
                TKEntityList = TypeService.GetAll(TK.TKEntity.Undefined)
            };

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(Permission.GrafikMonitor, true)]
        public JsonResult GetData(string Source, int TKData)
        {
            Result result = new Result();

            if (Source != "tk_log" && Source != "tk_entity")
            {
                result.Message = "Tanımlanamayan kaynak türü algılandı. Lütfen sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            TK.TKLog TKLog = TK.TKLog.Undefined;
            TK.TKEntity TKEntity = TK.TKEntity.Undefined;
            if (Source == "tk_log")
            {
                TKLog = (TK.TKLog)TKData;
            }
            else if (Source == "tk_entity")
            {
                TKEntity = (TK.TKEntity)TKData;
            }

            List<GraphicModel> graphicModelList = new List<GraphicModel>();

            if (Source == "tk_log" && TKLog != TK.TKLog.Undefined)
            {
                #region TKLog
                if (TKLog == TK.TKLog.LogNavigation)
                {
                    #region LogNavigation
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogNavigation.ToString(), null, null, null).Cast<LogNavigation>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                if (TKLog == TK.TKLog.LogNavigationAdministrator)
                {
                    #region LogNavigationAdministrator
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogNavigationAdministrator.ToString(), null, null, null).Cast<LogNavigationAdministrator>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogPostView)
                {
                    #region LogPostView
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogPostView.ToString(), null, null, null).Cast<LogPostView>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogSearch)
                {
                    #region LogSearch
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogSearch.ToString(), null, null, null).Cast<LogSearch>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogURLError)
                {
                    #region LogURLError
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogURLError.ToString(), null, null, null).Cast<LogURLError>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogURLErrorAdministrator)
                {
                    #region LogURLErrorAdministrator
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogURLErrorAdministrator.ToString(), null, null, null).Cast<LogURLErrorAdministrator>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogFileDownload)
                {
                    #region LogFileDownload
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogFileDownload.ToString(), null, null, null).Cast<LogFileDownload>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogAdministratorSession)
                {
                    #region LogAdministratorSession
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogAdministratorSession.ToString(), null, null, null).Cast<LogAdministratorSession>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogSparkJob)
                {
                    #region LogSparkJob
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogSparkJob.ToString(), null, null, null).Cast<LogSparkJob>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogUserSession)
                {
                    #region LogUserSession
                    var list = this._iLogService.GetLogListForGraph(TK.TKLog.LogUserSession.ToString(), null, null, null).Cast<LogUserSession>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKLog == TK.TKLog.LogTransaction)
                {
                    #region LogTransaction
                    var list = this._iLogService.GetGLogListForGraph(TK.TKLog.LogTransaction.ToString(), null, null, null).Cast<LogTransaction>().ToList();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                #endregion
            }
            else if (Source == "tk_entity" && TKEntity != TK.TKEntity.Undefined)
            {
                #region TKEntity
                if (TKEntity == TK.TKEntity.User)
                {
                    #region User
                    var list = this._iUserService.GetUserListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.Customer)
                {
                    #region Customer
                    var list = this._iCustomerService.GetCustomerListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.Payment)
                {
                    #region Payment
                    var list = this._iPaymentService.GetPaymentListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.PaymentStatusRelation)
                {
                    #region PaymentStatusRelation
                    var list = this._iPaymentService.GetPaymentStatusRelationListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.Raffle)
                {
                    #region Raffle
                    var list = this._iRaffleService.GetRaffleListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.RaffleParticipant)
                {
                    #region RaffleParticipant
                    var list = this._iRaffleService.GetRaffleParticipantListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.UserBalance)
                {
                    #region UserBalance
                    var list = this._iUserService.GetUserBalanceListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.UserNote)
                {
                    #region UserNote
                    var list = this._iUserService.GetUserNoteListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.CustomerNote)
                {
                    #region CustomerNote
                    var list = this._iCustomerService.GetCustomerNoteListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.UserPasswordInfo)
                {
                    #region UserPasswordInfo
                    var list = this._iUserService.GetUserPasswordInfoListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.UserPasswordReset)
                {
                    #region UserPasswordReset
                    var list = this._iUserService.GetUserPasswordResetListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.ContactMessage)
                {
                    #region ContactMessage
                    var list = this._iGenericService.GetContactMessageListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                else if (TKEntity == TK.TKEntity.Subscribe)
                {
                    #region Subscribe
                    var list = this._iGenericService.GetSubscribeListForGraph();

                    foreach (var item in list)
                    {
                        item.CreatedAt =
                            item.CreatedAt
                            .AddHours(item.CreatedAt.Hour * -1)
                            .AddMinutes(item.CreatedAt.Minute * -1)
                            .AddSeconds(item.CreatedAt.Second * -1)
                            .AddMilliseconds(item.CreatedAt.Millisecond * -1);
                    }

                    list = list.OrderBy(l => l.CreatedAt).ToList();

                    var createdAtGroup = list.GroupBy(g => g.CreatedAt).ToDictionary(d => d.Key, d => d.Select(v => v).ToList());

                    foreach (var item in createdAtGroup)
                    {
                        graphicModelList.Add(new GraphicModel()
                        {
                            x = item.Key.ToString("yyyy-MM-dd"),
                            y = item.Value.Count.ToString()
                        });
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                result.Message = "Tanımlanamayan kaynak türü algılandı. Lütfen sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            result.IsSuccess = true;
            result.Data = new
            {
                GraphicModel = graphicModelList,
                Source = Source,
                TKLog = (int)TKLog,
                TKEntity = (int)TKEntity
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(Permission.LogMonitor, true)]
        public JsonResult GetLogField(int ID, string TableName, string FieldName)
        {
            Result result = new Result();

            try
            {
                if (this._iLogService.IsGLog(TableName))
                {
                    result.Data = this._iLogService.GetGLogFieldByID(ID, TableName, FieldName);
                }
                else
                {
                    result.Data = this._iLogService.GetLogFieldByID(ID, TableName, FieldName);
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}