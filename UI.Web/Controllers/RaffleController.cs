using Sky.CDN.DTO;
using Sky.CDN.IServices;
using Sky.CMS.DTO;
using Sky.CMS.IServices;
using Sky.CMS.Services;
using Sky.Common.DTO;
using Sky.Common.Extensions;
using Sky.Common.Provider;
using Sky.Common.Utilities;
using Sky.Core.DTO;
using Sky.Core.IServices;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.Mors.IServices;
using Sky.Ramesses.IServices;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Web.Infrastructure;
using UI.Web.Models.Raffle;

using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;
using PermissionAct = Sky.Common.DTO.TK.Project.Studio.TKPermissionAction;

namespace UI.Web.Controllers
{
    public class RaffleController : BaseController
    {
        private readonly IPaymentService _iPaymentService;
        private readonly IUserService _iUserService;
        private readonly ICDNService _iCDNService;
        private readonly ICustomerService _iCustomerService;
        private readonly IRaffleService _iRaffleService;
        private readonly IGeneralContentService _iGeneralContentService;
        private readonly IAdministratorService _iAdministratorService;
        private readonly MorsOperations _morsOperations;

        public RaffleController(
            Sky.SuperUser.IServices.ISessionService iSessionService,
            ILogService iLogService,
            IPaymentService iPaymentService,
            IUserService iUserService,
            ICDNService iCDNService,
            IRaffleService iRaffleService,
            ICustomerService iCustomerService,
            IAdministratorService iAdministratorService,
            IGeneralContentService iGeneralContentService,
            MorsOperations morsOperations)
            : base(iSessionService, iLogService)
        {
            this._iPaymentService = iPaymentService;
            this._iUserService = iUserService;
            this._iCDNService = iCDNService;
            this._iCustomerService = iCustomerService;
            this._iRaffleService = iRaffleService;
            this._iAdministratorService = iAdministratorService;
            this._iGeneralContentService = iGeneralContentService;
            this._morsOperations = morsOperations;
        }

        #region RaffleParticipant
        [PermissionFilter(PermissionNav.CekilisKatilimListesiniGoruntuleme)]
        public ActionResult ParticipantList(int? Limit, TK.TKRaffleParticipationStatus? TKRaffleParticipationStatus, TK.TKRaffleParticipationType? TKRaffleParticipationType)
        {
            List<RaffleParticipant> raffleParticipantList = null;

            if (!Limit.HasValue)
            {
                Limit = 200;
            }

            #region TKRaffleParticipationStatus Control
            if (!TKRaffleParticipationStatus.HasValue || (TKRaffleParticipationStatus.HasValue && TKRaffleParticipationStatus.Value == TK.TKRaffleParticipationStatus.Undefined))
            {
                TKRaffleParticipationStatus = TK.TKRaffleParticipationStatus.Undefined;

                raffleParticipantList = this._iRaffleService.GetRaffleParticipantListLIMIT(Limit.Value);
            }
            else
            {
                raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByTKRaffleParticipationStatusLIMIT(Limit.Value, TKRaffleParticipationStatus.Value);
            }

            if (raffleParticipantList == null)
            {
                throw new Exception("");
            }
            #endregion

            #region TKRaffleParticipationStatus Control
            if (!TKRaffleParticipationType.HasValue || (TKRaffleParticipationType.HasValue && TKRaffleParticipationType.Value == TK.TKRaffleParticipationType.Undefined))
            {
                TKRaffleParticipationType = TK.TKRaffleParticipationType.Undefined;
            }
            else
            {
                raffleParticipantList = raffleParticipantList.Where(rp => rp.TKRaffleParticipationType == TKRaffleParticipationType.Value).ToList();
            }
            #endregion

            #region Raffle & RaffleTemplate Assign
            var raffleTemplateList = this._iRaffleService.GetRaffleTemplateListLARGE();
            var raffleList = this._iRaffleService.GetRaffleListByIDList(
                raffleParticipantList.Select(rp => rp.RaffleID).Distinct().ToArray()
                );

            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByIDList(
                raffleParticipantList.Select(rp => rp.SocialMediaAccountID).ToArray()
                );

            foreach (var item in raffleList)
            {
                item.RaffleTemplate = raffleTemplateList.Where(rt => rt.ID == item.RaffleTemplateID).FirstOrDefault() ?? new RaffleTemplate();
            }
            foreach (var item in raffleParticipantList)
            {
                item.Raffle = raffleList.Where(r => r.ID == item.RaffleID).FirstOrDefault() ?? new Raffle();
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();
            }
            #endregion

            var tkRaffleParticipationTypeList = TypeService.GetAll(TK.TKRaffleParticipationType.Undefined);
            tkRaffleParticipationTypeList.Insert(0, new TypeValue()
            {
                Value = 0,
                Name = "Tüm Katılım Türleri"
            });

            var tkRaffleParticipationStatusList = TypeService.GetAll(TK.TKRaffleParticipationStatus.Undefined);
            tkRaffleParticipationStatusList.Insert(0, new TypeValue()
            {
                Value = 0,
                Name = "Tüm Katılım Durumları"
            });

            var model = new RaffleParticipantListModel()
            {
                RaffleParticipantList = raffleParticipantList,
                TKRaffleParticipationTypeList = tkRaffleParticipationTypeList,
                TKRaffleParticipationStatusList = tkRaffleParticipationStatusList,
                SelectedTKRaffleParticipationType = (int)TKRaffleParticipationType.Value,
                SelectedTKRaffleParticipationStatus = (int)TKRaffleParticipationStatus.Value,
                Limit = Limit.Value
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.CekilisKatilimDetayiniGoruntuleme)]
        public ActionResult ParticipantDetail(int ID)
        {
            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(ID);
            if (raffleParticipantItem == null)
            {
                throw new Exception("");
            }

            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByCustomerIDLARGE(raffleParticipantItem.CustomerID);

            raffleParticipantItem.Customer = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            raffleParticipantItem.Customer.CountryPhoneCode = (this._iGeneralContentService.GetCountryByID(raffleParticipantItem.Customer.CountryID) ?? new Country()).PhoneCode;
            raffleParticipantItem.Raffle = this._iRaffleService.GetRaffleByIDLARGE(raffleParticipantItem.RaffleID) ?? new Raffle();
            raffleParticipantItem.Raffle.RaffleTemplate = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleParticipantItem.Raffle.RaffleTemplateID) ?? new RaffleTemplate();
            raffleParticipantItem.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == raffleParticipantItem.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();

            Sky.Ramesses.DTO.User user = null;
            Sky.SuperUser.DTO.Administrator administrator = null;

            if (raffleParticipantItem.TKActor == TK.TKActor.User)
            {
                user = this._iUserService.GetUserByIDMEDIUM(raffleParticipantItem.ActorID);
            }
            else if (raffleParticipantItem.TKActor == TK.TKActor.Administrator)
            {
                administrator = this._iAdministratorService.GetAdministratorByID(raffleParticipantItem.ActorID);
            }

            List<RaffleParticipant> makeUpRaffleParticipantList = new List<RaffleParticipant>();
            if (raffleParticipantItem.IsMakeUpDefined)
            {
                makeUpRaffleParticipantList = this._iRaffleService.GetRaffleParticipantListByPreviousRaffleParticipantIDLARGE(raffleParticipantItem.ID);
            }

            foreach (var item in makeUpRaffleParticipantList)
            {
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();
            }

            #region Raffle Restriction
            var raffleList = this._iRaffleService.GetRaffleListByTKRaffleStatusSMALL(TK.TKRaffleStatus.PendingParticipant);
            var raffleTemplateList = this._iRaffleService.GetRaffleTemplateListSMALL();
            foreach (var item in raffleList)
            {
                item.RaffleTemplate = raffleTemplateList.Where(rt => rt.ID == item.RaffleTemplateID).FirstOrDefault();
            }
            raffleList = raffleList
                .Where(r =>
                    r.RaffleTemplate != null &&
                    (r.RaffleTemplate.ID == raffleParticipantItem.Raffle.RaffleTemplate.ID || r.RaffleTemplate.Price <= raffleParticipantItem.Raffle.RaffleTemplate.Price))
                .ToList();
            #endregion

            var model = new RaffleParticipantModel()
            {
                RaffleParticipant = raffleParticipantItem,
                User = user,
                Administrator = administrator,
                Customer = raffleParticipantItem.Customer,
                SocialMediaAccountList = socialMediaAccountList.Where(sma => sma.IsActive && !sma.IsDeleted).ToList(),
                RaffleList = raffleList,
                TKRaffleParticipationTypeList = TypeService.GetAll(TK.TKRaffleParticipationType.Undefined),
                TKRaffleParticipationStatusList = TypeService.GetAll(TK.TKRaffleParticipationStatus.Undefined),
                MakeUpRaffleParticipantList = makeUpRaffleParticipantList,
                TurkishCultureInfo = new CultureInfo("tr-TR")
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.TakipciSayisiveEkranGoruntusuGuncelleme)]
        public JsonResult UpdateRaffleParticipantFollowerAndScreenShotAjax(RaffleParticipant raffleParticipant)
        {
            Result result = new Result();

            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(raffleParticipant.ID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customer = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customer == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Müşteri kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string initialScreenShot = null;
            string eventualScreenShot = null;

            #region InitialScreenShot
            HttpPostedFileBase httpPostedFileInitialScreenShot = Request.Files["InitialScreenShot"];
            if (httpPostedFileInitialScreenShot != null && httpPostedFileInitialScreenShot.ContentLength > 0)
            {
                #region Generic Token
                var genericToken = this._iCDNService.GetActiveToken();
                if (genericToken == null)
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1000";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(genericToken.Token))
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1001";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                #region Prepare and Save InputData
                byte[] data = null;
                string fileExtension = null;

                HttpPostedFileBase filePosted = httpPostedFileInitialScreenShot;
                if (filePosted != null && filePosted.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(filePosted.FileName);
                    fileExtension = System.IO.Path.GetExtension(fileName);

                    using (System.IO.Stream inputStream = filePosted.InputStream)
                    {
                        System.IO.MemoryStream memoryStream = inputStream as System.IO.MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new System.IO.MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }
                }

                if (data != null)
                {
                    InputData inputData = new InputData();
                    byte[] bytes = data;

                    inputData.Data = bytes;
                    inputData.UniqueKey = Guid.NewGuid().ToString();
                    inputData.TKWebsite = TK.TKWebsite.AppRNPReklam;
                    inputData.TKActor = TK.TKActor.Administrator;
                    inputData.ActorID = this.OutAdministratorID;
                    inputData.TKResultType = TK.TKResultType.NoTrial;
                    inputData.IsActive = true;
                    inputData.FileExtension = fileExtension;
                    inputData.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                    inputData.CreatedAt = DateTime.UtcNow;

                    if (!this._iCDNService.Save(inputData))
                    {
                        result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1002";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }

                    #region Connect to CDN to Upload File
                    result = this._iCDNService.UploadFile(inputData, System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], genericToken.Token);
                    if (!result.IsSuccess)
                    {
                        result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1003";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    #endregion

                    initialScreenShot = result.Data.ToString();
                }
                else
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1004";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
            }
            else
            {
                if (string.IsNullOrEmpty(raffleParticipantItem.InitialScreenShot))
                {
                    result.Message = "Başlangıç ekran görüntüsünü ekleyiniz.";
                    return base.Json(result, JsonRequestBehavior.DenyGet);
                }

                initialScreenShot = raffleParticipantItem.InitialScreenShot;
            }
            #endregion

            result = new Result();

            #region EventualScreenShot
            HttpPostedFileBase httpPostedFileEventualScreenShot = Request.Files["EventualScreenShot"];
            if (httpPostedFileEventualScreenShot != null && httpPostedFileEventualScreenShot.ContentLength > 0)
            {
                #region Generic Token
                var genericToken = this._iCDNService.GetActiveToken();
                if (genericToken == null)
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1000";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(genericToken.Token))
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1001";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                #region Prepare and Save InputData
                byte[] data = null;
                string fileExtension = null;

                HttpPostedFileBase filePosted = httpPostedFileEventualScreenShot;
                if (filePosted != null && filePosted.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(filePosted.FileName);
                    fileExtension = System.IO.Path.GetExtension(fileName);

                    using (System.IO.Stream inputStream = filePosted.InputStream)
                    {
                        System.IO.MemoryStream memoryStream = inputStream as System.IO.MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new System.IO.MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }
                }

                if (data != null)
                {
                    InputData inputData = new InputData();
                    byte[] bytes = data;

                    inputData.Data = bytes;
                    inputData.UniqueKey = Guid.NewGuid().ToString();
                    inputData.TKWebsite = TK.TKWebsite.AppRNPReklam;
                    inputData.TKActor = TK.TKActor.Administrator;
                    inputData.ActorID = this.OutAdministratorID;
                    inputData.TKResultType = TK.TKResultType.NoTrial;
                    inputData.IsActive = true;
                    inputData.FileExtension = fileExtension;
                    inputData.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                    inputData.CreatedAt = DateTime.UtcNow;

                    if (!this._iCDNService.Save(inputData))
                    {
                        result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1002";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }

                    #region Connect to CDN to Upload File
                    result = this._iCDNService.UploadFile(inputData, System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], genericToken.Token);
                    if (!result.IsSuccess)
                    {
                        result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1003";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    #endregion

                    eventualScreenShot = result.Data.ToString();
                }
                else
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1004";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
            }
            else
            {
                if (string.IsNullOrEmpty(raffleParticipantItem.EventualScreenShot))
                {
                    result.Message = "Bitiş ekran görüntüsünü ekleyiniz.";
                    return base.Json(result, JsonRequestBehavior.DenyGet);
                }

                eventualScreenShot = raffleParticipantItem.EventualScreenShot;
            }
            #endregion

            result = new Result();

            raffleParticipant.InitialScreenShot = initialScreenShot;
            raffleParticipant.EventualScreenShot = eventualScreenShot;

            if (raffleParticipant.InitialScreenShot == raffleParticipantItem.InitialScreenShot &&
                raffleParticipant.EventualScreenShot == raffleParticipantItem.EventualScreenShot &&
                raffleParticipant.InitialFollowerCount == raffleParticipantItem.InitialFollowerCount &&
                raffleParticipant.EventualFollowerCount == raffleParticipantItem.EventualFollowerCount)
            {
                result.NoChanges();
                result.Data = new
                {
                    InitialScreenShot = string.Format("{0}{1}", System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], raffleParticipant.InitialScreenShot),
                    EventualScreenShot = string.Format("{0}{1}", System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], raffleParticipant.EventualScreenShot)
                };

                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleParticipantFollowerCountADMIN(raffleParticipant))
            {
                result.Message = "Takipçi sayısı ve ekran görüntüsü bilgileri güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    InitialFollowerCount = raffleParticipantItem.InitialFollowerCount,
                    InitialScreenShot = raffleParticipantItem.InitialScreenShot,
                    EventualFollowerCount = raffleParticipantItem.EventualFollowerCount,
                    EventualScreenShot = raffleParticipantItem.EventualScreenShot
                },
                newValue = new
                {
                    InitialFollowerCount = raffleParticipant.InitialFollowerCount,
                    InitialScreenShot = raffleParticipant.InitialScreenShot,
                    EventualFollowerCount = raffleParticipant.EventualFollowerCount,
                    EventualScreenShot = raffleParticipant.EventualScreenShot
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleParticipant,
                raffleParticipant.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Takipçi sayısı ve ekran görüntüsü bilgileri güncellendi");
            #endregion

            result.Message = "Takipçi sayısı ve ekran görüntüsü bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;
            result.Data = new
            {
                InitialScreenShot = string.Format("{0}{1}", System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], raffleParticipant.InitialScreenShot),
                EventualScreenShot = string.Format("{0}{1}", System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], raffleParticipant.EventualScreenShot)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisKatilimAktivasyonu)]
        public JsonResult UpdateRaffleParticipantActivationAjax(RaffleParticipant raffleParticipant)
        {
            Result result = new Result();

            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(raffleParticipant.ID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customer = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customer == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Müşteri kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            raffleParticipant.IsDeleted = !raffleParticipant.IsDeleted;

            if (raffleParticipant.IsShown == raffleParticipantItem.IsShown &&
                raffleParticipant.IsActive == raffleParticipantItem.IsActive &&
                raffleParticipant.IsDeleted == raffleParticipantItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleParticipantActivation(raffleParticipant))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    IsShown = raffleParticipantItem.IsShown,
                    IsActive = raffleParticipantItem.IsActive,
                    IsDeleted = raffleParticipantItem.IsDeleted
                },
                newValue = new
                {
                    IsShown = raffleParticipant.IsShown,
                    IsActive = raffleParticipant.IsActive,
                    IsDeleted = raffleParticipant.IsDeleted
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleParticipant,
                raffleParticipant.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Aktivasyon bilgileri güncellendi");
            #endregion

            result.Message = "Aktivasyon bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KatilimTuruveKatilimDurumuGuncelleme)]
        public JsonResult UpdateRaffleParticipantTypeAndStatusAjax(RaffleParticipant raffleParticipant)
        {
            Result result = new Result();

            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(raffleParticipant.ID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customer = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customer == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Müşteri kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleParticipant.TKRaffleParticipationType == raffleParticipantItem.TKRaffleParticipationType &&
                raffleParticipant.TKRaffleParticipationStatus == raffleParticipantItem.TKRaffleParticipationStatus)
            {
                result.NoChanges();
                result.Data = new
                {
                    TKRaffleParticipationTypeName = TypeService.GetNameByValue(raffleParticipant.TKRaffleParticipationType),
                    TKRaffleParticipationStatusName = TypeService.GetNameByValue(raffleParticipant.TKRaffleParticipationStatus)
                };
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleParticipantTypeAndStatus(raffleParticipant))
            {
                result.Message = "Katılım Türü ve Katılım Durumu güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    TKRaffleParticipationType = raffleParticipantItem.TKRaffleParticipationType,
                    TKRaffleParticipationStatus = raffleParticipantItem.TKRaffleParticipationStatus
                },
                newValue = new
                {
                    TKRaffleParticipationType = raffleParticipant.TKRaffleParticipationType,
                    TKRaffleParticipationStatus = raffleParticipant.TKRaffleParticipationStatus
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleParticipant,
                raffleParticipant.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Katılım Türü ve Katılım Durumu güncellendi");
            #endregion

            result.Message = "Katılım Türü ve Katılım Durumu başarıyla güncellendi.";
            result.IsSuccess = true;
            result.Data = new
            {
                TKRaffleParticipationTypeName = TypeService.GetNameByValue(raffleParticipant.TKRaffleParticipationType),
                TKRaffleParticipationStatusName = TypeService.GetNameByValue(raffleParticipant.TKRaffleParticipationStatus)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.ListeKoduOlustur)]
        public JsonResult UpdateRaffleParticipantListNumberAjax(RaffleParticipant raffleParticipant, bool AutoGenerate)
        {
            Result result = new Result();

            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(raffleParticipant.ID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customer = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customer == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Müşteri kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleParticipantItem.TKRaffleParticipationStatus != TK.TKRaffleParticipationStatus.JoinedToRaffle)
            {
                result.Message = string.Format("Katılım durumu \"{0}\" statüsünde olmadığı için işleminiz tamamlanamadı.", TypeService.GetNameByValue(TK.TKRaffleParticipationStatus.JoinedToRaffle));
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!raffleParticipantItem.IsConfirmed)
            {
                result.Message = "İşlem onayı verilmediği için işleminiz tamamlanamadı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            //  To Generate ListCode
            raffleParticipant.RaffleID = raffleParticipantItem.RaffleID;

            if (AutoGenerate)
            {
                var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDSMALL(raffleParticipantItem.RaffleID);
                var raffle = this._iRaffleService.GetRaffleByIDLARGE(raffleParticipant.RaffleID);

                int listNumber = 0, listSequence = 0;
                this.GenerateListCode(raffle, raffleParticipantList, raffleParticipant, ref listNumber, ref listSequence);

                raffleParticipant.ListNumber = listNumber;
                raffleParticipant.ListSequence = listSequence;
            }

            if (raffleParticipant.ListNumber == raffleParticipantItem.ListNumber &&
                raffleParticipant.ListSequence == raffleParticipantItem.ListSequence)
            {
                result.NoChanges();
                result.Data = new
                {
                    ListNumber = raffleParticipant.ListNumber,
                    ListSequence = raffleParticipant.ListSequence,
                    ListCode = raffleParticipant.GetListCode()
                };

                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleParticipantListNumberAndSequence(raffleParticipant))
            {
                result.Message = "Liste numarası ve liste sırası güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    ListNumber = raffleParticipantItem.ListNumber,
                    ListSequence = raffleParticipantItem.ListSequence
                },
                newValue = new
                {
                    ListNumber = raffleParticipant.ListNumber,
                    ListSequence = raffleParticipant.ListSequence
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleParticipant,
                raffleParticipant.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                (AutoGenerate ? "Liste numarası ve liste sırası güncellendi (Auto)" : "Liste numarası ve liste sırası güncellendi (Manuel)"));
            #endregion

            result.Message = "Liste numarası ve liste sırası başarıyla güncellendi.";
            result.IsSuccess = true;
            result.Data = new
            {
                ListNumber = raffleParticipant.ListNumber,
                ListSequence = raffleParticipant.ListSequence,
                ListCode = raffleParticipant.GetListCode()
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.IslemOnayi, PermissionAct.ListeUzerindeIslemOnayi)]
        public JsonResult UpdateRaffleParticipantConfirmationAjax(RaffleParticipant raffleParticipant, bool? FromRaffleParticipantList)
        {
            Result result = new Result();

            #region Validation
            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(raffleParticipant.ID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customer = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customer == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Müşteri kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleParticipantItem.IsConfirmed)
            {
                result.Message = string.Format("Bu kayıt için daha önce {0} tarih ve saatinde işlem onayı verilmiştir.",
                    raffleParticipantItem.ConfirmedAt.Value.ToString("dd MMMM yyyy HH:mm:ss", new CultureInfo("tr-TR")));
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!raffleParticipant.IsConfirmed)
            {
                result.Message = "İşlem onayı verebilmeniz için switch'i açmanız gerekmektedir.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            raffleParticipant.RaffleID = raffleParticipantItem.RaffleID;
            #endregion

            #region Set ListNumber
            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDSMALL(raffleParticipant.RaffleID);
            var raffle = this._iRaffleService.GetRaffleByIDLARGE(raffleParticipantItem.RaffleID);
            int listNumber = 0, listSequence = 0;
            this.GenerateListCode(raffle, raffleParticipantList, raffleParticipant, ref listNumber, ref listSequence);

            raffleParticipant.ListNumber = listNumber;
            raffleParticipant.ListSequence = listSequence;
            #endregion

            raffleParticipant.TKRaffleParticipationStatus = TK.TKRaffleParticipationStatus.JoinedToRaffle;
            raffleParticipant.IsShown = true;
            raffleParticipant.IsActive = true;
            raffleParticipant.IsDeleted = false;
            raffleParticipant.ConfirmedAt = DateTime.UtcNow;

            if (!this._iRaffleService.UpdateRaffleParticipantConfirm(raffleParticipant))
            {
                result.Message = "İşlem onayı verilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    TKRaffleParticipationStatus = raffleParticipantItem.TKRaffleParticipationStatus,
                    ListNumber = raffleParticipantItem.ListNumber,
                    ListSequence = raffleParticipantItem.ListSequence,
                    IsConfirmed = raffleParticipantItem.IsConfirmed,
                    ConfirmedAt = raffleParticipantItem.ConfirmedAt,
                    IsShown = raffleParticipantItem.IsShown,
                    IsActive = raffleParticipantItem.IsActive,
                    IsDeleted = raffleParticipantItem.IsDeleted
                },
                newValue = new
                {
                    TKRaffleParticipationStatus = raffleParticipant.TKRaffleParticipationStatus,
                    ListNumber = raffleParticipant.ListNumber,
                    ListSequence = raffleParticipant.ListSequence,
                    IsConfirmed = raffleParticipant.IsConfirmed,
                    ConfirmedAt = raffleParticipant.ConfirmedAt,
                    IsShown = raffleParticipant.IsShown,
                    IsActive = raffleParticipant.IsActive,
                    IsDeleted = raffleParticipant.IsDeleted
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleParticipant,
                raffleParticipant.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "İşlem onayı verildi");
            #endregion

            result.Message = "İşlem onayı başarıyla verildi.";
            result.IsSuccess = true;
            result.Data = new
            {
                ID = raffleParticipant.ID,
                ListNumber = raffleParticipant.ListNumber,
                ListSequence = raffleParticipant.ListSequence,
                ListCode = raffleParticipant.GetListCode(),
                TKRaffleParticipationStatus = (int)raffleParticipant.TKRaffleParticipationStatus,
                TKRaffleParticipationStatusName = TypeService.GetNameByValue(raffleParticipant.TKRaffleParticipationStatus),
                ConfirmedAtStr = raffleParticipant.ConfirmedAt.Value.ToString("dd MMMM yyyy HH:mm:ss", new CultureInfo("tr-TR")),
                ConfirmedAt = raffleParticipant.ConfirmedAt.Value.ToString("yyyy.MM.dd HH:mm:ss"),
                IsConfirmed = raffleParticipant.IsConfirmed,
                IsShown = raffleParticipant.IsShown,
                IsActive = raffleParticipant.IsActive,
                IsDeleted = !raffleParticipant.IsDeleted,
                FromRaffleParticipantList = FromRaffleParticipantList
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.TelafiTanimlamaIslemi)]
        public JsonResult SaveMakeUpRaffleParticipantAjax(RaffleParticipant raffleParticipant, int RaffleID)
        {
            Result result = new Result();

            #region RaffleParticipant Validation
            if (!raffleParticipant.PreviousRaffleParticipantID.HasValue)
            {
                result.Message = "Önceki çekiliş kaydı bulunamadı. Bir hata oluştu. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(raffleParticipant.PreviousRaffleParticipantID.Value);
            if (raffleParticipantItem == null)
            {
                result.Message = "Önceki çekiliş katılım kaydı bulunamadı. Çekiliş katılım kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region CustomerValidation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customerItem == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Raffle & RaffleTemplate
            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(RaffleID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleItem.RaffleTemplateID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi kaydı bulunamadı. Çekiliş paketi kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Social Media Account
            var socialMediaAccountItem = this._iCustomerService.GetSocialMediaAccountByIDSMALL(raffleParticipant.SocialMediaAccountID);
            if (socialMediaAccountItem == null)
            {
                result.Message = "Sosyal medya hesabı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            string initialScreenShot = null;

            #region InitialScreenShot CDN
            HttpPostedFileBase httpPostedFileInitialScreenShot = Request.Files["InitialScreenShot"];
            if (httpPostedFileInitialScreenShot != null && httpPostedFileInitialScreenShot.ContentLength > 0)
            {
                #region Generic Token
                var genericToken = this._iCDNService.GetActiveToken();
                if (genericToken == null)
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1000";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(genericToken.Token))
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1001";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                #region Prepare and Save InputData
                byte[] data = null;
                string fileExtension = null;

                HttpPostedFileBase filePosted = httpPostedFileInitialScreenShot;
                if (filePosted != null && filePosted.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(filePosted.FileName);
                    fileExtension = System.IO.Path.GetExtension(fileName);

                    using (System.IO.Stream inputStream = filePosted.InputStream)
                    {
                        System.IO.MemoryStream memoryStream = inputStream as System.IO.MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new System.IO.MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }
                }

                if (data != null)
                {
                    InputData inputData = new InputData();
                    byte[] bytes = data;

                    inputData.Data = bytes;
                    inputData.UniqueKey = Guid.NewGuid().ToString();
                    inputData.TKWebsite = TK.TKWebsite.AppRNPReklam;
                    inputData.TKActor = TK.TKActor.User;
                    inputData.ActorID = this.OutAdministratorID;
                    inputData.TKResultType = TK.TKResultType.NoTrial;
                    inputData.IsActive = true;
                    inputData.FileExtension = fileExtension;
                    inputData.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                    inputData.CreatedAt = DateTime.UtcNow;

                    if (!this._iCDNService.Save(inputData))
                    {
                        result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1002";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }

                    #region Connect to CDN to Upload File
                    result = this._iCDNService.UploadFile(inputData, System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], genericToken.Token);
                    if (!result.IsSuccess)
                    {
                        result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1003";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    #endregion

                    initialScreenShot = result.Data.ToString();
                }
                else
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1004";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
            }
            else
            {
                result.Message = "Başlangıç ekran görüntüsünü ekleyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = new Result();

            RaffleParticipant makeUpRaffleParticipant = new RaffleParticipant()
            {
                RaffleID = RaffleID,
                CustomerID = raffleParticipantItem.CustomerID,
                SocialMediaAccountID = socialMediaAccountItem.ID,
                TKSocialMediaPlatform = socialMediaAccountItem.TKSocialMediaPlatform,
                SocialMediaAccountName = socialMediaAccountItem.AccountName,
                ListNumber = 0,
                ListSequence = 0,
                TKActor = Settings.TKActor,
                ActorID = this.OutAdministratorID,
                PaymentID = raffleParticipantItem.PaymentID,
                PreviousRaffleParticipantID = raffleParticipantItem.ID,
                TKRaffleParticipationType = TK.TKRaffleParticipationType.MakeUp,
                TKRaffleParticipationStatus = TK.TKRaffleParticipationStatus.PendingConfirm,
                TargetFollowerCount = raffleTemplateItem.TargetFollowerCount,
                InitialFollowerCount = raffleParticipant.InitialFollowerCount,
                InitialScreenShot = initialScreenShot,
                IsConfirmed = false,
                IsShown = true,
                IsActive = true,
                IsDeleted = false,
                IPAddress = IPProvider.GetIPAddress(this.HttpContext),
                CreatedAt = DateTime.UtcNow
            };

            int rid;
            if (!this._iRaffleService.SaveRaffleParticipant(makeUpRaffleParticipant, out rid))
            {
                result.Message = "Telafi çekilişi kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            makeUpRaffleParticipant.ID = rid;

            #region LogContent RaffleParticipant
            var logContentRaffleParticipant = makeUpRaffleParticipant;
            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleParticipant,
                makeUpRaffleParticipant.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContentRaffleParticipant),
                "Telafi çekiliş kaydı oluşturuldu"
                );
            #endregion

            #region Update IsMakeUpDefined
            if (!raffleParticipantItem.IsMakeUpDefined)
            {
                var isRaffleParticipantSuccess = this._iRaffleService.UpdateRaffleParticipantIsMakeUpDefined(new RaffleParticipant()
                {
                    ID = raffleParticipant.PreviousRaffleParticipantID.Value,
                    IsMakeUpDefined = true
                });

                if (isRaffleParticipantSuccess)
                {
                    #region LogTransaction
                    var logContentPreviousRaffleParticipant = new
                    {
                        oldValue = new
                        {
                            IsMakeUpDefined = false
                        },
                        newValue = new
                        {
                            IsMakeUpDefined = true
                        }
                    };

                    base.LogTransaction(
                        Constants.Database.Core.Value,
                        (int)Constants.Database.Core.Table.RaffleParticipant,
                        raffleParticipant.PreviousRaffleParticipantID.Value,
                        TK.TKTransactionType.Update,
                        new JavaScriptSerializer().Serialize(logContentPreviousRaffleParticipant),
                        "Telafi tanımlandı bilgisi güncellendi");
                    #endregion
                }
            }
            #endregion

            result.IsSuccess = true;
            result.Message = "Telafi çekilişi başarıyla kaydedildi";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisKatiliminizGerceklestiEmailiGonder)]
        public JsonResult SendEmailRaffleParticipantConfirmedAjax(int RaffleParticipantID, string FullName, string Email, bool UseBCC, string BCCEmail)
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

            #region ValidationSpecific
            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(RaffleParticipantID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş katılımı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!raffleParticipantItem.IsConfirmed)
            {
                result.Message = "İşlem onayı verilmediği için bu email gönderilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(raffleParticipantItem.RaffleID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleItem.RaffleTemplateID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(raffleParticipantItem.CustomerID);
            if (customerItem == null)
            {
                result.Message = "Müşteri bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string creatorFullName = null;
            if (raffleParticipantItem.TKActor == TK.TKActor.User)
            {
                var actorItem = this._iUserService.GetUserByIDMEDIUM(raffleParticipantItem.ActorID) ?? new Sky.Ramesses.DTO.User();
                creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
            }
            else if (raffleParticipantItem.TKActor == TK.TKActor.Administrator)
            {
                var actorItem = this._iAdministratorService.GetAdministratorByID(raffleParticipantItem.ActorID);
                creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
            }
            #endregion

            result = this._morsOperations.SendEmailRaffleParticipantConfirmedAjaxEmail(raffleParticipantItem, raffleItem, raffleTemplateItem, customerItem, creatorFullName, FullName, Email, UseBCC, BCCEmail);

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisKatiliminizGerceklestiSmsiGonder)]
        public JsonResult SendSmsRaffleParticipantConfirmedAjax(int RaffleParticipantID, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            #region Validation
            if (string.IsNullOrEmpty(FullName))
            {
                result.Message = "Alıcı Adı Soyadı bilgisini giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                result.Message = "Alıcı Telefon Numarasını giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region ValidationSpecific
            var raffleParticipantItem = this._iRaffleService.GetRaffleParticipantByIDLARGE(RaffleParticipantID);
            if (raffleParticipantItem == null)
            {
                result.Message = "Çekiliş katılımı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!raffleParticipantItem.IsConfirmed)
            {
                result.Message = "İşlem onayı verilmediği için bu sms gönderilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(raffleParticipantItem.RaffleID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleItem.RaffleTemplateID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string creatorFullName = null;
            if (raffleParticipantItem.TKActor == TK.TKActor.User)
            {
                var actorItem = this._iUserService.GetUserByIDMEDIUM(raffleParticipantItem.ActorID) ?? new Sky.Ramesses.DTO.User();
                creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
            }
            else if (raffleParticipantItem.TKActor == TK.TKActor.Administrator)
            {
                var actorItem = this._iAdministratorService.GetAdministratorByID(raffleParticipantItem.ActorID);
                creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
            }
            #endregion

            result = this._morsOperations.SendSmsRaffleParticipantConfirmedAjaxSms(raffleParticipantItem, raffleItem, raffleTemplateItem, creatorFullName, FullName, PhoneNumber);

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region RaffleTemplate
        [PermissionFilter(PermissionNav.CekilisPaketleri)]
        public ActionResult TemplateList()
        {
            var model = new RaffleTemplateListModel()
            {
                RaffleTemplateList = this._iRaffleService.GetRaffleTemplateListMEDIUM()
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisPaketiEkleme)]
        public JsonResult SaveRaffleTemplateAjax(RaffleTemplate raffleTemplate)
        {
            Result result = new Result();

            if (string.IsNullOrEmpty(raffleTemplate.Name))
            {
                result.Message = "Paket adı giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleTemplate.TargetFollowerCount <= 0)
            {
                result.Message = "Hedef takipçi sayısı sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleTemplate.ApplyCommissionRate && raffleTemplate.CommissionRate <= 0)
            {
                result.Message = "Komisyon oranı sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            raffleTemplate.IsDeleted = false;
            raffleTemplate.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iRaffleService.SaveRaffleTemplate(raffleTemplate, out id))
            {
                result.Message = "Çekiliş paketi kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            raffleTemplate.ID = id;

            #region LogTransaction
            var logContent = raffleTemplate;

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleTemplate,
                raffleTemplate.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş paketi kaydedildi"
                );
            #endregion

            result.Message = "Çekiliş paketi başarıyla kaydedildi.";
            result.IsSuccess = true;
            result.Data = raffleTemplate;
            result.ExtraData = new
            {
                PriceStr = raffleTemplate.Price.ToThousandSeperatorWithCurrency(),
                CommissionRateStr = raffleTemplate.CommissionRate.ToString("0.00"),
                CreatedAtStr = raffleTemplate.CreatedAt.ToString("yyyy.MM.dd HH:mm:ss")
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisPaketiDuzenleme)]
        public JsonResult UpdateRaffleTemplateAjax(RaffleTemplate raffleTemplate)
        {
            Result result = new Result();

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDMEDIUM(raffleTemplate.ID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Çekiliş paketinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(raffleTemplate.Name))
            {
                result.Message = "Paket adı giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleTemplate.TargetFollowerCount <= 0)
            {
                result.Message = "Hedef takipçi sayısı sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleTemplate.ApplyCommissionRate && raffleTemplate.CommissionRate <= 0)
            {
                result.Message = "Komisyon oranı sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleTemplate.Name == raffleTemplateItem.Name &&
                raffleTemplate.TargetFollowerCount == raffleTemplateItem.TargetFollowerCount &&
                raffleTemplate.Price == raffleTemplateItem.Price &&
                raffleTemplate.ApplyCommissionRate == raffleTemplateItem.ApplyCommissionRate &&
                raffleTemplate.CommissionRate == raffleTemplateItem.CommissionRate &&
                raffleTemplate.IsActive == raffleTemplateItem.IsActive)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleTemplate(raffleTemplate))
            {
                result.Message = "Çekiliş paketi güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    Name = raffleTemplateItem.Name,
                    TargetFollowerCount = raffleTemplateItem.TargetFollowerCount,
                    Price = raffleTemplateItem.Price,
                    ApplyCommissionRate = raffleTemplateItem.ApplyCommissionRate,
                    CommissionRate = raffleTemplateItem.CommissionRate,
                    IsActive = raffleTemplateItem.IsActive
                },
                newValue = new
                {
                    Name = raffleTemplate.Name,
                    TargetFollowerCount = raffleTemplate.TargetFollowerCount,
                    Price = raffleTemplate.Price,
                    ApplyCommissionRate = raffleTemplate.ApplyCommissionRate,
                    CommissionRate = raffleTemplate.CommissionRate,
                    IsActive = raffleTemplate.IsActive
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleTemplate,
                raffleTemplate.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş paketi güncellendi"
                );
            #endregion

            result.Message = "Çekiliş paketi başarıyla güncellendi.";
            result.IsSuccess = true;
            result.Data = raffleTemplate;
            result.ExtraData = new
            {
                PriceStr = raffleTemplate.Price.ToThousandSeperatorWithCurrency(),
                CommissionRateStr = raffleTemplate.CommissionRate.ToString("0.00")
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisPaketiSilme)]
        public JsonResult UpdateRaffleTemplateIsDeletedAjax(int ID)
        {
            Result result = new Result();

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDMEDIUM(ID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Çekiliş paketinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }


            if (!this._iRaffleService.UpdateRaffleTemplateIsDeleted(ID, true))
            {
                result.Message = "Çekiliş paketi silinemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    IsDeleted = false
                },
                newValue = new
                {
                    IsDeleted = true
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.RaffleTemplate,
                raffleTemplateItem.ID,
                TK.TKTransactionType.Delete,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş paketi silindi"
                );
            #endregion

            result.Message = "Çekiliş paketi başarıyla silindi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Raffle
        [PermissionFilter(PermissionNav.CekilisListeleriniGoruntuleme)]
        public ActionResult List(TK.TKRaffleStatus? TKRaffleStatus)
        {
            List<Raffle> raffleList = null;
            string pageTitle = null;

            if (!TKRaffleStatus.HasValue)
            {
                raffleList = this._iRaffleService.GetRaffleListMEDIUM();
                pageTitle = "Tüm Çekilişler";
            }
            else
            {
                raffleList = this._iRaffleService.GetRaffleListByTKRaffleStatusMEDIUM(TKRaffleStatus.Value);
                pageTitle = TypeService.GetNameByValue(TKRaffleStatus.Value);
            }

            if (raffleList == null)
            {
                throw new Exception("");
            }

            var raffleTemplateList = this._iRaffleService.GetRaffleTemplateListMEDIUM();
            foreach (var item in raffleList)
            {
                item.RaffleTemplate = raffleTemplateList.Where(rt => rt.ID == item.RaffleTemplateID).FirstOrDefault();
            }

            raffleList = raffleList.Where(r => r.RaffleTemplate != null).ToList();

            var model = new RaffleListModel()
            {
                RaffleList = raffleList,
                PageTitle = pageTitle
            };

            return View(model);
        }

        [PermissionFilter(PermissionNav.CekilisPlani)]
        public ActionResult Save(TK.TKRaffleStatus? TKRaffleStatus)
        {
            if (!TKRaffleStatus.HasValue)
            {
                TKRaffleStatus = TK.TKRaffleStatus.Undefined;
            }

            ViewBag.TKRaffleStatus = TKRaffleStatus;
            ViewBag.IsMenuClosed = true;
            ViewBag.RaffleTemplateList = this._iRaffleService.GetRaffleTemplateListSMALL();
            
            var tkRaffleStatusList = TypeService.GetAll(TK.TKRaffleStatus.Undefined);
            tkRaffleStatusList.Insert(0, new Sky.CMS.DTO.TypeValue()
            {
                Name = "Tüm Çekilişler"
            });

            ViewBag.TKRaffleStatusList = tkRaffleStatusList;

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View();
        }

        [PermissionFilter(PermissionNav.CekilisDetayiniGoruntuleme)]
        public ActionResult Detail(int ID)
        {
            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(ID);

            if (raffleItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            raffleItem.CreatedAt = raffleItem.CreatedAt.AddHours(Settings.TIMEZONE);
            
            var countryList = this._iGeneralContentService.GetCountryList();
            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDLARGE(ID);
            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByIDList(
                raffleParticipantList.Select(rp => rp.SocialMediaAccountID).ToArray()
                );

            var smArray = raffleParticipantList.Select(rp => rp.SocialMediaAccountID).Distinct().ToArray();
            var cstArray = raffleParticipantList.Select(rp => rp.CustomerID).Distinct().ToArray();

            var customerList = this._iCustomerService.GetCustomerListByIDList(raffleParticipantList.Select(rp => rp.CustomerID).ToArray());

            foreach (var item in customerList)
            {
                var tempCountryItem = countryList.Where(c => c.ID == item.CountryID).FirstOrDefault() ?? new Country();
                item.CountryPhoneCode = tempCountryItem.PhoneCode;
                item.CountryName = tempCountryItem.Name;
            }

            foreach (var item in raffleParticipantList)
            {
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();
            }

            var model = new RaffleModel()
            {
                Raffle = raffleItem,
                RaffleTemplate = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleItem.RaffleTemplateID),
                RaffleParticipantList = raffleParticipantList,
                TurkishCultureInfo = new CultureInfo("tr-TR"),
                TKRaffleStatusList = TypeService.GetAll(TK.TKRaffleStatus.Undefined),
                TKRaffleParticipationStatusList = TypeService.GetAll(TK.TKRaffleParticipationStatus.Undefined),
                CustomerList = customerList
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisAktivasyonu)]
        public JsonResult UpdateRaffleActivationAjax(Raffle raffle)
        {
            Result result = new Result();

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(raffle.ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            raffle.IsDeleted = !raffle.IsDeleted;

            if (raffle.IsActive == raffleItem.IsActive &&
                raffle.IsDeleted == raffleItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            
            if (!this._iRaffleService.UpdateRaffleActivation(raffle))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }
            
            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    IsActive = raffleItem.IsActive,
                    IsDeleted = raffleItem.IsDeleted
                },
                newValue = new
                {
                    IsActive = raffle.IsActive,
                    IsDeleted = raffle.IsDeleted
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Aktivasyon bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Aktivasyon bilgileri başarıyla güncellendi.";
            result.Data = new
            {
                IsActive = raffle.IsActive,
                IsDeleted = raffle.IsDeleted
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisDurumuGuncelleme)]
        public JsonResult UpdateRaffleStatusAjax(Raffle raffle)
        {
            Result result = new Result();

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(raffle.ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffle.TKRaffleStatus == TK.TKRaffleStatus.Undefined)
            {
                result.Message = "Çekiliş durumunu seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffle.TKRaffleStatus == raffleItem.TKRaffleStatus)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleStatus(raffle))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    TKRaffleStatus = raffleItem.TKRaffleStatus
                },
                newValue = new
                {
                    TKRaffleStatus = raffle.TKRaffleStatus
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş durumu güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Çekiliş durumu başarıyla güncellendi.";
            result.Data = new
            {
                TKRaffleStatusName = TypeService.GetNameByValue(raffle.TKRaffleStatus)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.ListeKapasitesiniGuncelleme)]
        public JsonResult UpdateRaffleUserCountPerListAjax(Raffle raffle)
        {
            Result result = new Result();

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(raffle.ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffle.UserCountPerList <= 0)
            {
                result.Message = "Liste kapasitesi sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffle.UserCountPerList == raffleItem.UserCountPerList)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffleUserCountPerList(raffle))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    UserCountPerList = raffleItem.UserCountPerList
                },
                newValue = new
                {
                    UserCountPerList = raffle.UserCountPerList
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş liste kapasitesi güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Çekiliş liste kapasitesi başarıyla güncellendi.";
            result.Data = new
            {
                UserCountPerList = raffle.UserCountPerList
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.OrtalamaTakipciHesaplama)]
        public JsonResult UpdateRaffleAverageFollowerCountAjax(Raffle raffle)
        {
            Result result = new Result();

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(raffle.ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDLARGE(raffle.ID);
            var filteredList = raffleParticipantList.Where(rp => rp.EventualFollowerCount.HasValue && rp.IsConfirmed && rp.IsShown && rp.IsActive && !rp.IsDeleted).OrderBy(rp => rp.ID).ToList();

            int averageFollowerCount = 0;
            int raffleParticipantCount = 0;

            foreach (var item in filteredList)
            {
                int diff = item.EventualFollowerCount.Value - item.InitialFollowerCount;
                if (diff <= 0)
                {
                    continue;
                }

                averageFollowerCount += diff;
                raffleParticipantCount++;
            }

            if (raffleParticipantCount == 0)
            {
                result.Message = "Hesaplama kuralına uygun katılımcı olmadığı için işlem gerçekleştirilemedi.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            averageFollowerCount = averageFollowerCount / raffleParticipantCount;
            raffle.AverageFollowerCount = averageFollowerCount;

            if (!this._iRaffleService.UpdateRaffleAverageFollowerCount(raffle))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return base.Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    AverageFollowerCount = raffleItem.AverageFollowerCount
                },
                newValue = new
                {
                    AverageFollowerCount = raffle.AverageFollowerCount
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Ortalama takipçi hesaplandı");
            #endregion

            result.IsSuccess = true;
            result.Message = "Ortalama takipçi başarıyla hesaplandı.";
            result.Data = new
            {
                AverageFollowerCount = averageFollowerCount
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.SiralamaYenileme)]
        public JsonResult UpdateRaffleParticipantsNumberAndSequenceAjax(int ID)
        {
            Result result = new Result();

            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDLARGE(ID);
            var filteredList = raffleParticipantList.Where(rp => rp.IsConfirmed && rp.IsShown && rp.IsActive && !rp.IsDeleted).OrderBy(rp => rp.ID).ToList();

            int listNumber = 1;
            int listSequence = 1;

            JavaScriptSerializer jss = new JavaScriptSerializer();

            foreach (var item in filteredList)
            {
                var raffleParticipant = new RaffleParticipant()
                {
                    ID = item.ID,
                    ListNumber = 999,
                    ListSequence = 999
                };

                if (item.TKRaffleParticipationStatus == TK.TKRaffleParticipationStatus.JoinedToRaffle)
                {
                    raffleParticipant.ListNumber = listNumber;
                    raffleParticipant.ListSequence = listSequence;

                    listSequence++;

                    if (raffleItem.UserCountPerList + 1 == listSequence)
                    {
                        listNumber++;
                        listSequence = 1;
                    }
                }

                if (this._iRaffleService.UpdateRaffleParticipantListNumberAndSequence(raffleParticipant))
                {
                    #region LogTransaction
                    var logContent = new
                    {
                        oldValue = new
                        {
                            ListNumber = item.ListNumber,
                            ListSequence = item.ListSequence
                        },
                        newValue = new
                        {
                            ListNumber = raffleParticipant.ListNumber,
                            ListSequence = raffleParticipant.ListSequence
                        }
                    };

                    base.LogTransaction(
                        Constants.Database.Core.Value,
                        (int)Constants.Database.Core.Table.RaffleParticipant,
                        raffleParticipant.ID,
                        TK.TKTransactionType.Update,
                        jss.Serialize(logContent),
                        "Liste numarası ve liste sırası güncellendi (Toplu İşlem)"
                        );
                    #endregion
                }
            }

            #region GetUpdatedList
            raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDLARGE(ID);
            filteredList =
                raffleParticipantList
                .Where(rp => rp.IsConfirmed && rp.IsShown && rp.IsActive && !rp.IsDeleted)
                .OrderBy(rp => rp.ListNumber)
                .ThenBy(rp => rp.ListSequence)
                .ToList();

            filteredList.ForEach(f => 
                f.IPAddress = TypeService.GetNameByValue(f.TKRaffleParticipationStatus)
            );

            filteredList.ForEach(f =>
                f.InitialScreenShot = f.GetListCode()
            );

            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByIDList(
                filteredList.Select(rp => rp.SocialMediaAccountID).Distinct().ToArray()
                );

            foreach (var item in filteredList)
            {
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault();
            }

            filteredList = filteredList.Where(rp => rp.SocialMediaAccount != null).ToList();
            #endregion

            result.IsSuccess = true;
            result.Message = "Çekiliş katılımcılarının liste ve sıra numaraları yeniden oluşturuldu.";
            result.Data = new
            {
                RaffleParticipantList = filteredList
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.CekilisPlani)]
        public JsonResult GetRaffleListAjax(TK.TKRaffleStatus? TKRaffleStatus)
        {
            Result result = new Result();

            List<Raffle> raffleList = null;

            if (!TKRaffleStatus.HasValue || (TKRaffleStatus.HasValue && TKRaffleStatus.Value == TK.TKRaffleStatus.Undefined))
            {
                raffleList = this._iRaffleService.GetRaffleListMEDIUM();
            }
            else
            {
                raffleList = this._iRaffleService.GetRaffleListByTKRaffleStatusMEDIUM(TKRaffleStatus.Value);
            }

            if (raffleList == null)
            {
                result.Message = "Çekilişler Yüklenemedi. Sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleTemplateList = this._iRaffleService.GetRaffleTemplateListMEDIUM();
            foreach (var item in raffleList)
            {
                item.RaffleTemplate = raffleTemplateList.Where(rt => rt.ID == item.RaffleTemplateID).FirstOrDefault();
            }

            raffleList = raffleList.Where(r => r.RaffleTemplate != null).ToList();

            raffleList.ForEach(
                f =>
                    f.CreatedAt = f.CreatedAt.AddHours(Settings.TIMEZONE)
                );

            List<RaffleCalendarModel> raffleCalendarModelList = new List<RaffleCalendarModel>();
            foreach (var item in raffleList)
            {
                RaffleCalendarModel raffleCalendarModel = new RaffleCalendarModel()
                {
                    Raffle = item
                };

                raffleCalendarModel.ID = item.ID;
                raffleCalendarModel.Title = string.Format("{0} - {1}", item.RaffleTemplate.Name, item.ID.ToString("00000"));
                raffleCalendarModel.StartedAtYear = item.RaffleStartedAt.Year;
                raffleCalendarModel.StartedAtMonth = item.RaffleStartedAt.Month - 1;
                raffleCalendarModel.StartedAtDay = item.RaffleStartedAt.Day;
                raffleCalendarModel.StartedAtHour = item.RaffleStartedAt.Hour;
                raffleCalendarModel.StartedAtMinutes = item.RaffleStartedAt.Minute;

                if (item.RaffleEndedAt.HasValue)
                {
                    raffleCalendarModel.EndedAtYear = item.RaffleEndedAt.Value.Year;
                    raffleCalendarModel.EndedAtMonth = item.RaffleEndedAt.Value.Month - 1;
                    raffleCalendarModel.EndedAtDay = item.RaffleEndedAt.Value.Day;
                    raffleCalendarModel.EndedAtHour = item.RaffleEndedAt.Value.Hour;
                    raffleCalendarModel.EndedAtMinutes = item.RaffleEndedAt.Value.Minute;
                }

                if (item.TKRaffleStatus == TK.TKRaffleStatus.PendingParticipant)
                {
                    raffleCalendarModel.BackgroundColor = "yellow";
                }
                else if (item.TKRaffleStatus == TK.TKRaffleStatus.InProgress)
                {
                    raffleCalendarModel.BackgroundColor = "grey";
                }
                else if (item.TKRaffleStatus == TK.TKRaffleStatus.Completed)
                {
                    raffleCalendarModel.BackgroundColor = "green";
                }
                else if (item.TKRaffleStatus == TK.TKRaffleStatus.Cancelled)
                {
                    raffleCalendarModel.BackgroundColor = "red";
                }

                raffleCalendarModelList.Add(raffleCalendarModel);
            }

            result.Data = raffleCalendarModelList;
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.PlanOlusturma)]
        public JsonResult SaveRaffleAjax(Raffle raffle, string RaffleStartedAtStr, string RaffleEndedAtStr)
        {
            Result result = new Result();

            #region Validation
            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDSMALL(raffle.RaffleTemplateID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Çekiliş paketinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(RaffleStartedAtStr))
            {
                result.Message = "Başlangıç tarihini giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffle.UserCountPerList <= 0)
            {
                result.Message = "Liste kapasitesi sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region RaffleStartedAt
            DateTime? dtStartedAt = null;
            try
            {
                if (!ValidationUtility.StringToDatetime(RaffleStartedAtStr, "dd/MM/yyyy HH:mm", out dtStartedAt))
                {
                    result.Message = "Geçersiz başlangıç tarihi girdiniz. Lütfen girdiğiniz tarihin gg/aa/yyyy formatında olduğundan emin olun.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {

            }

            if (!dtStartedAt.HasValue)
            {
                result.Message = "Geçersiz başlangıç tarihi girdiniz. Lütfen girdiğiniz tarihin gg/aa/yyyy formatında olduğundan emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region RaffleEndedAt
            DateTime? dtEndedAt = null;
            if (!string.IsNullOrEmpty(RaffleEndedAtStr))
            {
                try
                {
                    if (!ValidationUtility.StringToDatetime(RaffleEndedAtStr, "dd/MM/yyyy HH:mm", out dtEndedAt))
                    {
                        result.Message = "Geçersiz bitiş tarihi girdiniz. Lütfen girdiğiniz tarihin gg/aa/yyyy formatında olduğundan emin olun.";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
                catch (Exception ex)
                {

                }

                if (!dtStartedAt.HasValue)
                {
                    result.Message = "Geçersiz başlangıç tarihi girdiniz. Lütfen girdiğiniz tarihin gg/aa/yyyy formatında olduğundan emin olun.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            #endregion

            if (dtEndedAt.HasValue && dtStartedAt.Value > dtEndedAt.Value)
            {
                result.Message = "Çekiliş başlangıç tarihi, çekiliş bitiş tarihinden sonra olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            raffle.RaffleTemplate = raffleTemplateItem;
            raffle.RaffleStartedAt = dtStartedAt.Value;
            raffle.RaffleEndedAt = dtEndedAt;
            raffle.TKRaffleStatus = TK.TKRaffleStatus.PendingParticipant;
            raffle.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            raffle.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iRaffleService.SaveRaffle(raffle, out id))
            {
                result.Message = "Çekiliş kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            raffle.ID = id;

            #region LogTransaction
            var logContent = raffle;

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş kaydedildi"
                );
            #endregion

            #region RaffleCalendarModel
            raffle.CreatedAt = raffle.CreatedAt.AddHours(Settings.TIMEZONE);

            RaffleCalendarModel raffleCalendarModel = new RaffleCalendarModel()
            {
                Raffle = raffle
            };

            raffleCalendarModel.ID = raffle.ID;
            raffleCalendarModel.Title = string.Format("{0} - {1}", raffleTemplateItem.Name, raffle.ID.ToString("00000"));
            raffleCalendarModel.StartedAtYear = raffle.RaffleStartedAt.Year;
            raffleCalendarModel.StartedAtMonth = raffle.RaffleStartedAt.Month - 1;
            raffleCalendarModel.StartedAtDay = raffle.RaffleStartedAt.Day;
            raffleCalendarModel.StartedAtHour = raffle.RaffleStartedAt.Hour;
            raffleCalendarModel.StartedAtMinutes = raffle.RaffleStartedAt.Minute;

            if (raffle.RaffleEndedAt.HasValue)
            {
                raffleCalendarModel.EndedAtYear = raffle.RaffleEndedAt.Value.Year;
                raffleCalendarModel.EndedAtMonth = raffle.RaffleEndedAt.Value.Month - 1;
                raffleCalendarModel.EndedAtDay = raffle.RaffleEndedAt.Value.Day;
                raffleCalendarModel.EndedAtHour = raffle.RaffleEndedAt.Value.Hour;
                raffleCalendarModel.EndedAtMinutes = raffle.RaffleEndedAt.Value.Minute;
            }

            if (raffle.TKRaffleStatus == TK.TKRaffleStatus.PendingParticipant)
            {
                raffleCalendarModel.BackgroundColor = "yellow";
            }
            else if (raffle.TKRaffleStatus == TK.TKRaffleStatus.InProgress)
            {
                raffleCalendarModel.BackgroundColor = "grey";
            }
            else if (raffle.TKRaffleStatus == TK.TKRaffleStatus.Completed)
            {
                raffleCalendarModel.BackgroundColor = "green";
            }
            else if (raffle.TKRaffleStatus == TK.TKRaffleStatus.Cancelled)
            {
                raffleCalendarModel.BackgroundColor = "red";
            }
            #endregion

            result.Message = "Çekiliş başarıyla kaydedildi.";
            result.IsSuccess = true;
            result.Data = new
            {
                RaffleCalendarModel = raffleCalendarModel
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.Baslangic_BitisTarihleriniGuncelleme, PermissionAct.SurukleBirak)]
        public JsonResult UpdateRaffleDateTimeAjax(int ID, string RaffleStartedAtStr, string RaffleEndedAtStr)
        {
            Result result = new Result();

            #region Validation
            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(RaffleStartedAtStr))
            {
                result.Message = "Başlangıç tarihi ayrıştırılamadı. Hata Kodu: 1001";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region RaffleStartedAt
            DateTime? dtStartedAt = null;
            try
            {
                string dateTimeFormatter = "dd/MM/yyyy HH:mm:ss";
                if (RaffleStartedAtStr.Count(str => str == ':') == 1)
                    dateTimeFormatter = "dd/MM/yyyy HH:mm";

                if (!ValidationUtility.StringToDatetime(RaffleStartedAtStr, dateTimeFormatter, out dtStartedAt))
                {
                    result.Message = "Başlangıç tarihi ayrıştırılamadı. Hata Kodu: 1002";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {

            }

            if (!dtStartedAt.HasValue)
            {
                result.Message = "Başlangıç tarihi ayrıştırılamadı. Hata Kodu: 1003";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region RaffleEndedAt
            DateTime? dtEndedAt = null;
            if (!string.IsNullOrEmpty(RaffleEndedAtStr))
            {
                try
                {
                    string dateTimeFormatter = "dd/MM/yyyy HH:mm:ss";
                    if (RaffleStartedAtStr.Count(str => str == ':') == 1)
                        dateTimeFormatter = "dd/MM/yyyy HH:mm";

                    if (!ValidationUtility.StringToDatetime(RaffleEndedAtStr, dateTimeFormatter, out dtEndedAt))
                    {
                        result.Message = "Bitiş tarihi ayrıştırılamadı. Hata Kodu: 1001";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                }
                catch (Exception ex)
                {

                }

                if (!dtStartedAt.HasValue)
                {
                    result.Message = "Bitiş tarihi ayrıştırılamadı. Hata Kodu: 1002";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            #endregion

            if (dtEndedAt.HasValue && dtStartedAt.Value > dtEndedAt.Value)
            {
                result.Message = "Çekiliş başlangıç tarihi, çekiliş bitiş tarihinden sonra olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            Raffle raffle = new Raffle()
            {
                ID = ID,
                RaffleStartedAt = dtStartedAt.Value,
                RaffleEndedAt = dtEndedAt
            };

            if (!this._iRaffleService.UpdateRaffleDateTime(raffle))
            {
                result.Message = "Güncelleme işlemi gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    RaffleStartedAt = raffleItem.RaffleStartedAt,
                    RaffleEndedAt = raffleItem.RaffleEndedAt
                },
                newValue = new
                {
                    RaffleStartedAt = raffle.RaffleStartedAt,
                    RaffleEndedAt = raffle.RaffleEndedAt
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Başlangıç ve Bitiş Tarihleri güncellendi");
            #endregion

            CultureInfo turkishCultureInfo = new CultureInfo("tr-TR");

            result.IsSuccess = true;
            result.Message = "Başlangıç ve bitiş tarihleri başarıyla güncellendi.";
            result.Data = new
            {
                RaffleStartedAtStr = raffle.RaffleStartedAt.ToString("dd MMMM yyyy HH:mm:ss", turkishCultureInfo),
                RaffleEndedAtStr = raffle.RaffleEndedAt.HasValue ? raffle.RaffleEndedAt.Value.ToString("dd MMMM yyyy HH:mm:ss", turkishCultureInfo) : ""
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KapasiteveDurumGuncelleme)]
        public JsonResult UpdateRaffleAjax(Raffle raffle)
        {
            Result result = new Result();

            #region Validation
            var raffleItem = this._iRaffleService.GetRaffleByIDMEDIUM(raffle.ID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffle.UserCountPerList <= 0)
            {
                result.Message = "Liste kapasitesi sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            if (raffle.UserCountPerList == raffleItem.UserCountPerList &&
                raffle.IsActive == raffleItem.IsActive)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iRaffleService.UpdateRaffle(raffle))
            {
                result.Message = "Güncelleme işlemi gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    UserCountPerList = raffleItem.UserCountPerList,
                    IsActive = raffleItem.IsActive
                },
                newValue = new
                {
                    UserCountPerList = raffle.UserCountPerList,
                    IsActive = raffle.IsActive
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Raffle,
                raffle.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Çekiliş güncellendi");
            #endregion

            #region RaffleCalendarModel
            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDSMALL(raffleItem.RaffleTemplateID) ?? new RaffleTemplate() { Name = "Undefined Package" };
            int userCountPerList = raffle.UserCountPerList;
            bool isActive = raffle.IsActive;

            raffle = raffleItem;
            raffle.CreatedAt = raffle.CreatedAt.AddHours(Settings.TIMEZONE);
            raffle.UserCountPerList = userCountPerList;
            raffle.IsActive = isActive;

            RaffleCalendarModel raffleCalendarModel = new RaffleCalendarModel()
            {
                Raffle = raffle
            };

            raffleCalendarModel.ID = raffle.ID;
            raffleCalendarModel.Title = string.Format("{0} - {1}", raffleTemplateItem.Name, raffle.ID.ToString("00000"));
            raffleCalendarModel.StartedAtYear = raffle.RaffleStartedAt.Year;
            raffleCalendarModel.StartedAtMonth = raffle.RaffleStartedAt.Month - 1;
            raffleCalendarModel.StartedAtDay = raffle.RaffleStartedAt.Day;
            raffleCalendarModel.StartedAtHour = raffle.RaffleStartedAt.Hour;
            raffleCalendarModel.StartedAtMinutes = raffle.RaffleStartedAt.Minute;

            if (raffle.RaffleEndedAt.HasValue)
            {
                raffleCalendarModel.EndedAtYear = raffle.RaffleEndedAt.Value.Year;
                raffleCalendarModel.EndedAtMonth = raffle.RaffleEndedAt.Value.Month - 1;
                raffleCalendarModel.EndedAtDay = raffle.RaffleEndedAt.Value.Day;
                raffleCalendarModel.EndedAtHour = raffle.RaffleEndedAt.Value.Hour;
                raffleCalendarModel.EndedAtMinutes = raffle.RaffleEndedAt.Value.Minute;
            }

            if (raffle.TKRaffleStatus == TK.TKRaffleStatus.PendingParticipant)
            {
                raffleCalendarModel.BackgroundColor = "yellow";
            }
            else if (raffle.TKRaffleStatus == TK.TKRaffleStatus.InProgress)
            {
                raffleCalendarModel.BackgroundColor = "grey";
            }
            else if (raffle.TKRaffleStatus == TK.TKRaffleStatus.Completed)
            {
                raffleCalendarModel.BackgroundColor = "green";
            }
            else if (raffle.TKRaffleStatus == TK.TKRaffleStatus.Cancelled)
            {
                raffleCalendarModel.BackgroundColor = "red";
            }
            #endregion

            result.IsSuccess = true;
            result.Message = "Çekiliş başarıyla güncellendi.";
            result.Data = new
            {
                RaffleCalendarModel = raffleCalendarModel
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KatilimDurumunuGuncelleme)]
        public JsonResult UpdateRaffleParticipationListStatusAjax(int raffleID, TK.TKRaffleParticipationStatus TKRaffleParticipationStatus, string Description, string RaffleParticipantListJSON)
        {
            Result result = new Result();

            #region Validation
            var raffleItem = this._iRaffleService.GetRaffleByIDMEDIUM(raffleID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş bulunamadı. Çekilişin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var raffleParticipantList = new List<int>();
            try
            {
                raffleParticipantList = jss.Deserialize<List<int>>(RaffleParticipantListJSON);
            }
            catch (Exception)
            {
                result.Message = "Katılım bulunamadı. Lütfen sayfaı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleParticipantList == null)
            {
                result.Message = "Katılım bulunamadı. Lütfen sayfaı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var currentRaffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDLARGE(raffleID);

            var filteredRaffleParticipantList = currentRaffleParticipantList.Where(rp => raffleParticipantList.Contains(rp.ID)).ToList();

            if (filteredRaffleParticipantList.Count() != raffleParticipantList.Count())
            {
                result.Message = "Katılımcılardan biri veya birkaçı bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            List<LogTransaction> logTransactionList = new List<LogTransaction>();
            string processIPAddress = IPProvider.GetIPAddress(this.HttpContext);

            List<int> errorList = new List<int>();
            List<int> successList = new List<int>();

            foreach (var item in filteredRaffleParticipantList)
            {
                RaffleParticipant rp = new RaffleParticipant()
                {
                    ID = item.ID,
                    TKRaffleParticipationStatus = TKRaffleParticipationStatus
                };

                bool isSuccess = this._iRaffleService.UpdateRaffleParticipantStatus(rp);
                if (isSuccess)
                {
                    successList.Add(item.ID);

                    var logContent = new
                    {
                        oldValue = new
                        {
                            TKRaffleParticipationStatus = item.TKRaffleParticipationStatus
                        },
                        newValue = new
                        {
                            TKRaffleParticipationStatus = TKRaffleParticipationStatus
                        }
                    };

                    logTransactionList.Add(new LogTransaction()
                    {
                        DatabaseID = Constants.Database.Core.Value,
                        TableID = (int)Constants.Database.Core.Table.RaffleParticipant,
                        RowID = rp.ID,
                        TKActor = Settings.TKActor,
                        ActorID = this.OutAdministratorID,
                        TKTransactionType = TK.TKTransactionType.Update,
                        LogContent = jss.Serialize(logContent),
                        Description = Description,
                        IPAddress = processIPAddress,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    errorList.Add(rp.ID);
                }
            }

            if (logTransactionList.Any())
            {
                base.LogTransaction(logTransactionList);
            }
            
            result.IsSuccess = true;
            result.Message = "Katılım durumu başarıyla güncellendi.";
            result.Data = new
            {
                ErrorList = errorList,
                SuccessList = successList,
                TKRaffleParticipationStatusName = TypeService.GetNameByValue(TKRaffleParticipationStatus)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.EmailGonderme)]
        public JsonResult SendEmailRaffleStatusAjax(int RaffleID, TK.TKEmailTemplate TKEmailTemplate)
        {
            Result result = new Result();

            #region Validation
            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(RaffleID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!raffleItem.IsActive)
            {
                result.Message = "Çekiliş kaydının Durum özelliği Pasif olduğu için işleme devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleItem.IsDeleted)
            {
                result.Message = "Çekiliş kaydının Silindi özelliği Evet olduğu için işleme devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleItem.IsDeleted)
            {
                result.Message = "Çekiliş kaydının Silindi özelliği Evet olduğu için işleme devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKEmailTemplate != TK.TKEmailTemplate.RaffleStarted && TKEmailTemplate != TK.TKEmailTemplate.RaffleEnded)
            {
                result.Message = "Email Template bulunamadı. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleItem.RaffleTemplateID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Çekiliş paketinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKEmailTemplate == TK.TKEmailTemplate.RaffleStarted)
            {
                if (raffleItem.TKRaffleStatus != TK.TKRaffleStatus.InProgress)
                {
                    result.Message = "Çekiliş kaydı, Devam Ediyor durumunda olmadığı için işleme devam edilemiyor.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            else if (TKEmailTemplate == TK.TKEmailTemplate.RaffleEnded)
            {
                if (raffleItem.TKRaffleStatus != TK.TKRaffleStatus.Completed)
                {
                    result.Message = "Çekiliş kaydı, Tamamlandı durumunda olmadığı için işleme devam edilemiyor.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            #endregion

            #region Filter Raffle Participant
            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDSMALL(RaffleID);
            raffleParticipantList = raffleParticipantList.Where(rp => rp.IsConfirmed).ToList();

            List<RaffleParticipant> filteredRaffleParticipantList = new List<RaffleParticipant>();

            if (TKEmailTemplate == TK.TKEmailTemplate.RaffleStarted)
            {
                filteredRaffleParticipantList = raffleParticipantList.Where(rp => rp.TKRaffleParticipationStatus == TK.TKRaffleParticipationStatus.JoinedToRaffle).ToList();
            }
            else
            {
                filteredRaffleParticipantList = raffleParticipantList.Where(rp => rp.TKRaffleParticipationStatus == TK.TKRaffleParticipationStatus.RaffleCompleted).ToList();
            }

            if (!filteredRaffleParticipantList.Any())
            {
                result.Message = "Uygun katılımcı bulunamadı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            var customerList = this._iCustomerService.GetCustomerListByIDList(filteredRaffleParticipantList.Select(frp => frp.CustomerID).Distinct().ToArray());
            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByIDList(filteredRaffleParticipantList.Select(frp => frp.SocialMediaAccountID).Distinct().ToArray());
            List<int> successList = new List<int>();

            foreach (var item in filteredRaffleParticipantList)
            {
                #region Customer & Social Media Account
                var customerItem = customerList.Where(c => c.ID == item.CustomerID).FirstOrDefault();
                if (customerItem == null || string.IsNullOrEmpty(customerItem.Email))
                    continue;

                var socialMediaAccountItem = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault();
                if (socialMediaAccountItem == null)
                    continue;
                #endregion

                #region CreatorFullName
                string creatorFullName = "";
                if (item.TKActor == TK.TKActor.User)
                {
                    var actorItem = this._iUserService.GetUserByIDMEDIUM(item.ActorID) ?? new Sky.Ramesses.DTO.User();
                    creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
                }
                else if (item.TKActor == TK.TKActor.Administrator)
                {
                    var actorItem = this._iAdministratorService.GetAdministratorByID(item.ActorID);
                    creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
                }
                #endregion

                Result morsResult = null;

                if (TKEmailTemplate == TK.TKEmailTemplate.RaffleStarted)
                {
                    morsResult = 
                        this._morsOperations.SendEmailRaffleStartedAjaxEmail(
                        item, 
                        raffleItem, 
                        raffleTemplateItem, 
                        socialMediaAccountItem,
                        creatorFullName,
                        (string.Format("{0} {1}", customerItem.FirstName, customerItem.LastName)),
                        customerItem.Email);
                }
                else if (TKEmailTemplate == TK.TKEmailTemplate.RaffleEnded)
                {
                    morsResult = 
                        this._morsOperations.SendEmailRaffleEndedAjaxEmail(
                        item, 
                        raffleItem, 
                        raffleTemplateItem,
                        socialMediaAccountItem,
                        creatorFullName,
                        (string.Format("{0} {1}", customerItem.FirstName, customerItem.LastName)),
                        customerItem.Email);
                }

                if (morsResult != null && morsResult.IsSuccess)
                {
                    successList.Add(item.ID);
                }
            }

            result.Message = string.Format("{0}/{1} katılımcıya email gönderilmiştir.", successList.Count, filteredRaffleParticipantList.Count);
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.SmsGonderme)]
        public JsonResult SendSmsRaffleStatusAjax(int RaffleID, TK.TKSmsTemplate TKSmsTemplate)
        {
            Result result = new Result();

            #region Validation
            var raffleItem = this._iRaffleService.GetRaffleByIDLARGE(RaffleID);
            if (raffleItem == null)
            {
                result.Message = "Çekiliş kaydı bulunamadı. Çekiliş kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!raffleItem.IsActive)
            {
                result.Message = "Çekiliş kaydının Durum özelliği Pasif olduğu için işleme devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleItem.IsDeleted)
            {
                result.Message = "Çekiliş kaydının Silindi özelliği Evet olduğu için işleme devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (raffleItem.IsDeleted)
            {
                result.Message = "Çekiliş kaydının Silindi özelliği Evet olduğu için işleme devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKSmsTemplate != TK.TKSmsTemplate.RaffleStarted && TKSmsTemplate != TK.TKSmsTemplate.RaffleEnded)
            {
                result.Message = "Sms Template bulunamadı. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(raffleItem.RaffleTemplateID);
            if (raffleTemplateItem == null)
            {
                result.Message = "Çekiliş paketi bulunamadı. Çekiliş paketinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKSmsTemplate == TK.TKSmsTemplate.RaffleStarted)
            {
                if (raffleItem.TKRaffleStatus != TK.TKRaffleStatus.InProgress)
                {
                    result.Message = "Çekiliş kaydı, Devam Ediyor durumunda olmadığı için işleme devam edilemiyor.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            else if (TKSmsTemplate == TK.TKSmsTemplate.RaffleEnded)
            {
                if (raffleItem.TKRaffleStatus != TK.TKRaffleStatus.Completed)
                {
                    result.Message = "Çekiliş kaydı, Tamamlandı durumunda olmadığı için işleme devam edilemiyor.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            #endregion

            #region Filter Raffle Participant
            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByRaffleIDSMALL(RaffleID);
            raffleParticipantList = raffleParticipantList.Where(rp => rp.IsConfirmed).ToList();

            List<RaffleParticipant> filteredRaffleParticipantList = new List<RaffleParticipant>();

            if (TKSmsTemplate == TK.TKSmsTemplate.RaffleStarted)
            {
                filteredRaffleParticipantList = raffleParticipantList.Where(rp => rp.TKRaffleParticipationStatus == TK.TKRaffleParticipationStatus.JoinedToRaffle).ToList();
            }
            else
            {
                filteredRaffleParticipantList = raffleParticipantList.Where(rp => rp.TKRaffleParticipationStatus == TK.TKRaffleParticipationStatus.RaffleCompleted).ToList();
            }

            if (!filteredRaffleParticipantList.Any())
            {
                result.Message = "Uygun katılımcı bulunamadı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            var customerList = this._iCustomerService.GetCustomerListByIDList(filteredRaffleParticipantList.Select(frp => frp.CustomerID).Distinct().ToArray());
            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByIDList(filteredRaffleParticipantList.Select(frp => frp.SocialMediaAccountID).Distinct().ToArray());
            var countryList = this._iGeneralContentService.GetCountryList();

            foreach (var item in customerList)
            {
                item.CountryPhoneCode = (countryList.Where(c => c.ID == item.CountryID).FirstOrDefault() ?? new Country()).PhoneCode;
            }

            List<int> successList = new List<int>();

            foreach (var item in filteredRaffleParticipantList)
            {
                #region Customer
                var customerItem = customerList.Where(c => c.ID == item.CustomerID).FirstOrDefault();
                if (customerItem == null || string.IsNullOrEmpty(customerItem.PhoneNumber) || string.IsNullOrEmpty(customerItem.CountryPhoneCode))
                    continue;

                var socialMediaAccountItem = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault();
                if (socialMediaAccountItem == null)
                    continue;
                #endregion

                #region CreatorFullName
                string creatorFullName = "";
                if (item.TKActor == TK.TKActor.User)
                {
                    var actorItem = this._iUserService.GetUserByIDMEDIUM(item.ActorID) ?? new Sky.Ramesses.DTO.User();
                    creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
                }
                else if (item.TKActor == TK.TKActor.Administrator)
                {
                    var actorItem = this._iAdministratorService.GetAdministratorByID(item.ActorID);
                    creatorFullName = string.Format("{0} {1}", actorItem.FirstName, actorItem.LastName);
                }
                #endregion

                Result morsResult = null;

                if (TKSmsTemplate == TK.TKSmsTemplate.RaffleStarted)
                {
                    morsResult = 
                        this._morsOperations.SendSmsRaffleStartedAjaxSms(
                        item, 
                        raffleItem, 
                        raffleTemplateItem,
                        socialMediaAccountItem,
                        creatorFullName, 
                        (string.Format("{0} {1}", customerItem.FirstName, customerItem.LastName)),
                        customerItem.GetFullPhoneNumber());
                }
                else if (TKSmsTemplate == TK.TKSmsTemplate.RaffleEnded)
                {
                    morsResult = this._morsOperations.SendSmsRaffleEndedAjaxSms(
                        item,
                        raffleItem,
                        raffleTemplateItem,
                        socialMediaAccountItem,
                        creatorFullName,
                        (string.Format("{0} {1}", customerItem.FirstName, customerItem.LastName)),
                        customerItem.GetFullPhoneNumber());
                }

                if (morsResult != null && morsResult.IsSuccess)
                {
                    successList.Add(item.ID);
                }
            }

            result.Message = string.Format("{0}/{1} katılımcıya sms gönderilmiştir.", successList.Count, filteredRaffleParticipantList.Count);
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Private Functions
        private void GenerateListCode(Raffle raffle, List<RaffleParticipant> raffleParticipantList, RaffleParticipant raffleParticipant, ref int listNumber, ref int listSequence)
        {
            int raffleParticipantCount =
                raffleParticipantList
                    .Where(rp =>
                        rp.IsConfirmed &&
                        rp.TKRaffleParticipationStatus == TK.TKRaffleParticipationStatus.JoinedToRaffle &&
                        rp.ID != raffleParticipant.ID &&
                        rp.ListNumber != 0)
                    .Count();

            listNumber = (raffleParticipantCount / raffle.UserCountPerList) + 1;
            listSequence = (raffleParticipantCount % raffle.UserCountPerList) + 1;
        }
        #endregion
    }
}