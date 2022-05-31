using Sky.CMS.IServices;
using Sky.CMS.Services;
using Sky.Common.DTO;
using Sky.Common.Extensions;
using Sky.Core.IServices;
using Sky.Generic.IServices;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.Mors.DTO;
using Sky.Mors.IServices;
using Sky.Ramesses.IServices;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Web.Infrastructure;
using UI.Web.Models.Home;
using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;

namespace UI.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _iUserService;
        private readonly ILogService _iLogService;
        private readonly IGenericService _iGenericService;
        private readonly IGeneralContentService _iGeneralContentService;
        private readonly IPaymentService _iPaymentService;
        private readonly IRaffleService _iRaffleService;
        private readonly ICustomerService _iCustomerService;
        private readonly IDashboardService _iDashboardService;
        private readonly IAdministratorService _iAdministratorService;
        private readonly MorsOperations _morsOperations;

        public HomeController(
            IUserService iUserService,
            Sky.SuperUser.IServices.ISessionService iSessionService,
            ILogService iLogService,
            IGenericService iGenericService,
            IGeneralContentService iGeneralContentService,
            IPaymentService iPaymentService,
            IRaffleService iRaffleService,
            ICustomerService iCustomerService,
            IAdministratorService iAdministratorService,
            IDashboardService iDashboardService,
            MorsOperations morsOperations)
            : base(iSessionService, iLogService)
        {
            this._iUserService = iUserService;
            this._iLogService = iLogService;
            this._iGenericService = iGenericService;
            this._iGeneralContentService = iGeneralContentService;
            this._iPaymentService = iPaymentService;
            this._iRaffleService = iRaffleService;
            this._iCustomerService = iCustomerService;
            this._iAdministratorService = iAdministratorService;
            this._iDashboardService = iDashboardService;
            this._morsOperations = morsOperations;
        }

        [PermissionFilter(PermissionNav.Anasayfa)]
        public ActionResult Dashboard()
        {
            var model = new HomeListModel()
            {
                TotalUserCount = this._iUserService.GetUserTotalCount(),
                ActivityLogListForLeft = new List<ActivityLog>(),
                ActivityLogListForRight = new List<ActivityLog>()
            };

            #region Cards
            var dashboardAdmin = this._iDashboardService.GetDashboardTotalCountForADMINISTRATOR();
            model.TotalCustomerCurrentBalance = this._iUserService.GetUserBalanceACTIVE();
            model.TotalCustomerCount = dashboardAdmin.TotalCustomerCount;
            model.TotalPaymentCount = dashboardAdmin.TotalPaymentCount;
            model.TotalRaffleParticipantCount = dashboardAdmin.TotalRaffleParticipantCount;
            model.PendingRaffleParticipantCount = dashboardAdmin.PendingRaffleParticipantCount;
            model.TotalRaffleCount = dashboardAdmin.TotalRaffleCount;

            model.ContactMessageCount = this._iGenericService.GetContactMessageTotalCount();
            model.LogUserSessionCount = this._iLogService.GetLogTotalCount(TK.TKLog.LogUserSession.ToString());
            model.LogNavigationCount = this._iLogService.GetGLogTotalCount(TK.TKLog.LogNavigation.ToString());
            model.LogNavigationAdministratorCount = this._iLogService.GetGLogTotalCount(TK.TKLog.LogNavigationAdministrator.ToString());
            model.LogURLErrorCount = this._iLogService.GetGLogTotalCount(TK.TKLog.LogURLError.ToString());
            #endregion

            #region Activity Log
            #region Get Limited Data
            List<int> userIDList = new List<int>();
            List<int> administratorIDList = new List<int>();

            var userList = this._iUserService.GetUserListViaLimit(30);
            var administratorList = this._iAdministratorService.GetAdministratorListLIMIT(10);

            var customerList = this._iCustomerService.GetCustomerListLIMIT(30);
            var customerRegisterDetailList = this._iCustomerService.GetCustomerRegisterDetailListByIDList(
                customerList.Select(c => c.ID).ToArray()
                );
            foreach (var item in customerList)
            {
                var tempCustomerRegisterDetail = customerRegisterDetailList.Where(crd => crd.ID == item.ID).FirstOrDefault();
                if (tempCustomerRegisterDetail == null)
                {
                    continue;
                }

                item.CustomerRegisterDetail = tempCustomerRegisterDetail;
            }

            customerList = customerList.Where(c => c.CustomerRegisterDetail != null).ToList();

            var paymentList = this._iPaymentService.GetPaymentListLIMIT(30);
            var paymentNotifiedList = this._iPaymentService.GetPaymentStatusRelationListByTKPaymentStatus(TK.TKPaymentStatus.PaymentNotified, 50);
            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListLIMIT(30);


            var logNavigationList = this._iLogService.GetGLogList(70, TK.TKLog.LogNavigation.ToString(), null, null, null).Cast<LogNavigation>().ToList();
            var logUserSessionList = this._iLogService.GetLogList(70, TK.TKLog.LogUserSession.ToString(), null, null, null).Cast<LogUserSession>().ToList();
            var logNavigationAdministratorList = this._iLogService.GetGLogList(70, TK.TKLog.LogNavigationAdministrator.ToString(), null, null, null).Cast<LogNavigationAdministrator>().ToList();
            var logAdministratorSessionList = this._iLogService.GetGLogList(70, TK.TKLog.LogAdministratorSession.ToString(), null, null, null).Cast<LogAdministratorSession>().ToList();
            #endregion

            #region Get UserID && AdministratorID

            #region FromLeft
            userIDList.AddRange(
                    paymentList.Where(p => p.TKActor == TK.TKActor.User).Select(p => p.ActorID).Distinct().ToList()
                    );

            userIDList.AddRange(
                paymentNotifiedList.Where(p => p.TKActor == TK.TKActor.User).Select(pno => pno.ActorID).Distinct().ToList()
                );

            userIDList.AddRange(
                customerList.Where(c => c.CustomerRegisterDetail.TKActor == TK.TKActor.User).Select(c => c.CustomerRegisterDetail.ActorID).Distinct().ToList()
                );

            userIDList.AddRange(
                raffleParticipantList.Where(p => p.TKActor == TK.TKActor.User).Select(pno => pno.ActorID).Distinct().ToList()
                );

            administratorIDList.AddRange(
                raffleParticipantList.Where(p => p.TKActor == TK.TKActor.Administrator).Select(pno => pno.ActorID).Distinct().ToList()
                );
            #endregion

            #region FromRight
            userIDList.AddRange(
                    logNavigationList.Where(l => l.UserID.HasValue).Select(l => l.UserID.Value).Distinct().ToList()
                    );

            userIDList.AddRange(
                logUserSessionList.Where(l => l.UserID.HasValue).Select(l => l.UserID.Value).Distinct().ToList()
                );

            administratorIDList.AddRange(
                logAdministratorSessionList.Where(l => l.AdministratorID.HasValue).Select(l => l.AdministratorID.Value).Distinct().ToList()
                );

            administratorIDList.AddRange(
                logNavigationAdministratorList.Where(l => l.AdministratorID.HasValue).Select(l => l.AdministratorID.Value).Distinct().ToList()
                );
            #endregion

            #region Prepare User and Administrator List For Activity
            //  Get Latest User's ID as List
            List<int> tempUserIDList = userList.Select(u => u.ID).ToList();

            //  Exclude ID from the collected userIDList
            userIDList = userIDList.Where(uid => !tempUserIDList.Contains(uid)).Distinct().ToList();

            //  Get Latest Administrator's ID as List
            List<int> tempAdministratorIDList = administratorList.Select(a => a.ID).ToList();

            //  Exclude ID from the collected administratorIDList
            administratorIDList = administratorIDList.Where(aid => !tempAdministratorIDList.Contains(aid)).Distinct().ToList();


            //  Get Users from the DB
            var userListForActivity = this._iUserService.GetUserListByIDList(userIDList.ToArray());

            //  Get Administrators from the DB
            var administratorListForActivity = this._iAdministratorService.GetAdministratorListByIDList(administratorIDList.ToArray());


            //  Add Users to Users (latest userList into Collected ID)
            userListForActivity.AddRange(userList);

            //  Add Administrators to Administrators (latest administratorList into Collected ID)
            administratorListForActivity.AddRange(administratorList);
            #endregion

            #endregion

            #region UserFullName & AdministratorFullName Assigment

            #region Left
            foreach (var item in paymentList)
            {
                var tempUser = (userListForActivity.Where(u => u.ID == item.ActorID).FirstOrDefault() ?? new Sky.Ramesses.DTO.User());
                item.ActorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            foreach (var item in paymentNotifiedList)
            {
                var tempUser = (userListForActivity.Where(u => u.ID == item.ActorID).FirstOrDefault() ?? new Sky.Ramesses.DTO.User());
                item.ActorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            foreach (var item in customerList)
            {
                var tempUser = (userListForActivity.Where(u => u.ID == item.CustomerRegisterDetail.ActorID).FirstOrDefault() ?? new Sky.Ramesses.DTO.User());
                item.CustomerRegisterDetail.ActorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            foreach (var item in raffleParticipantList)
            {
                if (item.TKActor == TK.TKActor.User)
                {
                    var tempUser = (userListForActivity.Where(u => u.ID == item.ActorID).FirstOrDefault() ?? new Sky.Ramesses.DTO.User());
                    item.ActorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
                }
                else if (item.TKActor == TK.TKActor.Administrator)
                {
                    var tempUser = (administratorListForActivity.Where(u => u.ID == item.ActorID).FirstOrDefault() ?? new Sky.SuperUser.DTO.Administrator());
                    item.ActorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
                }
            }
            #endregion

            #region Right
            foreach (var item in logNavigationList)
            {
                var tempUser = (userListForActivity.Where(u => u.ID == item.UserID).FirstOrDefault() ?? new Sky.Ramesses.DTO.User());
                item.UserFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            foreach (var item in logUserSessionList)
            {
                var tempUser = (userListForActivity.Where(u => u.ID == item.UserID).FirstOrDefault() ?? new Sky.Ramesses.DTO.User());
                item.UserFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            foreach (var item in logAdministratorSessionList)
            {
                var tempUser = (administratorListForActivity.Where(u => u.ID == item.AdministratorID).FirstOrDefault() ?? new Sky.SuperUser.DTO.Administrator());
                item.AdministratorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            foreach (var item in logNavigationAdministratorList)
            {
                var tempUser = (administratorListForActivity.Where(u => u.ID == item.AdministratorID).FirstOrDefault() ?? new Sky.SuperUser.DTO.Administrator());
                item.AdministratorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            #endregion

            #endregion

            #region Activity Log
            List<ActivityLog> activityLogListForLeft = new List<ActivityLog>();
            List<ActivityLog> activityLogListForRight = new List<ActivityLog>();

            #region For Left

            #region User Transform To ActivityLog
            foreach (var item in userList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Text = string.Format("{0} {1} kayıt oldu.", item.FirstName, item.LastName),
                    Icon = "fa fa-user",
                    IconColor = "success",
                    HasHyperLink = true,
                    HyperLink = string.Format("/User/Detail/{0}", item.ID),
                    CreatedAt = item.CreatedAt
                };
                activityLogListForLeft.Add(al);
            }
            #endregion

            #region Payment Transform To ActivityLog
            foreach (var item in paymentList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Text = string.Format("{0} {1} tutarında ödeme kaydı oluşturdu.", item.ActorFullName, item.AmountPaid.ToThousandSeperatorWithCurrency()),
                    Icon = "fa fa-shopping-cart",
                    IconColor = "warning",
                    HasHyperLink = true,
                    HyperLink = string.Format("/Payment/Detail/{0}", item.ID),
                    CreatedAt = item.CreatedAt
                };
                activityLogListForLeft.Add(al);
            }
            #endregion

            #region PaymentNotified Transform To ActivityLog
            foreach (var item in paymentNotifiedList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Text = string.Format("{0} ödeme bildirimi gönderdi.", item.ActorFullName),
                    Icon = "fa fa-bullhorn",
                    IconColor = "info",
                    HasHyperLink = true,
                    HyperLink = string.Format("/Payment/Detail/{0}#payment_status", item.PaymentID),
                    CreatedAt = item.CreatedAt
                };
                activityLogListForLeft.Add(al);
            }
            #endregion

            #region Customer Transform To ActivityLog
            foreach (var item in customerList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Text = string.Format("{0}, {1} {2} isimli müşteri kaydı oluşturdu.", item.CustomerRegisterDetail.ActorFullName, item.FirstName, item.LastName),
                    Icon = "fa fa-users",
                    IconColor = "success",
                    HasHyperLink = true,
                    HyperLink = string.Format("/Customer/Detail/{0}", item.ID),
                    CreatedAt = item.CreatedAt
                };
                activityLogListForLeft.Add(al);
            }
            #endregion

            #region RaffleParticipant Transform To ActivityLog
            foreach (var item in raffleParticipantList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Text = string.Format("{0} çekiliş ({1}) kaydı oluşturdu.", item.ActorFullName, TypeService.GetNameByValue(item.TKRaffleParticipationType)),
                    Icon = "fa fa-random",
                    IconColor = "default",
                    HasHyperLink = true,
                    HyperLink = string.Format("/Raffle/ParticipantDetail/{0}", item.ID),
                    CreatedAt = item.CreatedAt
                };
                activityLogListForLeft.Add(al);
            }
            #endregion

            #endregion

            #region For Right

            #region LogNavigation Transform To ActivityLog
            foreach (var item in logNavigationList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Icon = "fa fa-line-chart",
                    IconColor = "info",
                    HasHyperLink = false,
                    CreatedAt = item.CreatedAt
                };

                if (!string.IsNullOrEmpty(item.UserFullName))
                {
                    al.Text = string.Format("{0} \"{1}\" adresine tıkladı.", item.UserFullName, item.URL);
                }
                else
                {
                    al.Text = item.Description;
                }

                activityLogListForRight.Add(al);
            }
            #endregion

            #region LogNavigationAdministrator Transform To ActivityLog
            foreach (var item in logNavigationAdministratorList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Icon = "fa fa-user-secret",
                    IconColor = "success",
                    HasHyperLink = false,
                    CreatedAt = item.CreatedAt
                };

                if (!string.IsNullOrEmpty(item.AdministratorFullName))
                {
                    al.Text = string.Format("{0} \"{1}\" adresine tıkladı.", item.AdministratorFullName, item.URL);
                }
                else
                {
                    al.Text = item.Description;
                }

                activityLogListForRight.Add(al);
            }
            #endregion

            #region LogUserSession Transform To ActivityLog
            foreach (var item in logUserSessionList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Icon = "fa fa-key",
                    IconColor = (item.IsSuccess ? "warning" : "danger"),
                    HasHyperLink = false,
                    CreatedAt = item.CreatedAt
                };

                if (!string.IsNullOrEmpty(item.UserFullName))
                {
                    al.Text = string.Format("{0} \"{1}\". Platform: {2}", item.UserFullName, item.Description, item.TKPlatform.ToString());
                }
                else
                {
                    al.Text = string.Format("{0}. Platform: {1}", item.Description, item.TKPlatform.ToString());
                }

                activityLogListForRight.Add(al);
            }
            #endregion

            #region LogAdministratorSession Transform To ActivityLog
            foreach (var item in logAdministratorSessionList)
            {
                ActivityLog al = new ActivityLog()
                {
                    Icon = "fa fa-key",
                    IconColor = (item.IsSuccess ? "warning" : "danger"),
                    HasHyperLink = false,
                    CreatedAt = item.CreatedAt
                };

                if (!string.IsNullOrEmpty(item.AdministratorFullName))
                {
                    al.Text = string.Format("{0} \"{1}\"", item.AdministratorFullName, item.Description);
                }
                else
                {
                    al.Text = string.Format("{0}", item.Description);
                }

                activityLogListForRight.Add(al);
            }
            #endregion

            #endregion


            #region Set DateTimeText
            activityLogListForLeft = activityLogListForLeft.OrderByDescending(al => al.CreatedAt).ToList();
            activityLogListForRight = activityLogListForRight.OrderByDescending(al => al.CreatedAt).ToList();

            DateTime currentUtc = DateTime.UtcNow;
            foreach (var item in activityLogListForLeft)
            {
                TimeSpan span = currentUtc.Subtract(item.CreatedAt);

                if (span.Days != 0)
                {
                    item.DateTimeText = string.Format("{0} gün", span.Days);
                }
                else if (span.Hours != 0)
                {
                    item.DateTimeText = string.Format("{0} saat", span.Hours);
                }
                else if (span.Minutes != 0)
                {
                    item.DateTimeText = string.Format("{0} dk", span.Minutes);
                }
                else if (span.Seconds != 0)
                {
                    item.DateTimeText = "Az önce";
                }
            }

            foreach (var item in activityLogListForRight)
            {
                TimeSpan span = currentUtc.Subtract(item.CreatedAt);

                if (span.Days != 0)
                {
                    item.DateTimeText = string.Format("{0} gün", span.Days);
                }
                else if (span.Hours != 0)
                {
                    item.DateTimeText = string.Format("{0} saat", span.Hours);
                }
                else if (span.Minutes != 0)
                {
                    item.DateTimeText = string.Format("{0} dk", span.Minutes);
                }
                else if (span.Seconds != 0)
                {
                    item.DateTimeText = "Az önce";
                }
            }
            #endregion
            #endregion

            model.ActivityLogListForLeft = activityLogListForLeft;
            model.ActivityLogListForRight = activityLogListForRight;
            #endregion

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.EmailGonder)]
        public ActionResult SendEmail(int? UserID)
        {
            if (UserID.HasValue && UserID.Value != 0)
            {
                var userItem = this._iUserService.GetUserByIDMEDIUM(UserID.Value);
                if (userItem == null)
                {
                    return RedirectToAction("Oops", "Error");
                }

                ViewBag.User = userItem;
            }

            return View();
        }

        [PermissionFilter(PermissionNav.SmsGonder)]
        public ActionResult SendSms(int? UserID)
        {
            if (UserID.HasValue && UserID.Value != 0)
            {
                var userItem = this._iUserService.GetUserByIDMEDIUM(UserID.Value);
                if (userItem == null)
                {
                    return RedirectToAction("Oops", "Error");
                }

                ViewBag.User = userItem;
            }

            ViewBag.CountryList = this._iGeneralContentService.GetCountryList();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [PermissionFilter(PermissionNav.EmailGonder, true)]
        public JsonResult SendEmailOtherAjax(string FullName, string Email, bool UseBCC, string BCCEmail, int EditorType, string ContentFromFroala, string ContentFromTinyMCE, string Subject)
        {
            Result result = new Result();

            #region Validation
            if (string.IsNullOrEmpty(FullName))
            {
                result.Message = "Alıcı Adı Soyadı bilgisini giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(Email))
            {
                result.Message = "Alıcı Email adresini giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (UseBCC && string.IsNullOrEmpty(BCCEmail))
            {
                result.Message = "BCC Email adresini giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Validation (Specific)
            string content = null;
            if (EditorType == 1)
            {
                content = ContentFromFroala;
            }
            else if (EditorType == 2)
            {
                content = ContentFromTinyMCE;
            }
            else
            {
                result.Message = "Farklı bir editör türü algılandı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(content))
            {
                result.Message = "Email içeriğini giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(Subject))
            {
                result.Message = "Email konusunu giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendEmailOtherAjaxEmail(FullName, Email, UseBCC, BCCEmail, content, Subject);

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.SmsGonder, true)]
        public JsonResult SendSmsOtherAjax(int CountryID, string PhoneNumber, string Content)
        {
            Result result = new Result();

            #region Validation
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                result.Message = "Alıcı Telefon Numarasını giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(Content))
            {
                result.Message = "Mesaj metnini giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var countryItem = this._iGeneralContentService.GetCountryByID(CountryID);
            if (countryItem == null)
            {
                result.Message = "Ülke bulunamadı. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendSmsOtherAjaxSms(string.Format("+{0} {1}", countryItem.PhoneCode, PhoneNumber), Content);

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}