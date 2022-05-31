using IbanNet;
using Sky.CDN.DTO;
using Sky.CDN.IServices;
using Sky.CMS.DTO;
using Sky.CMS.IServices;
using Sky.CMS.Services;
using Sky.Common.DTO;
using Sky.Common.Provider;
using Sky.Common.Utilities;
using Sky.Core.DTO;
using Sky.Core.IServices;
using Sky.Generic.IServices;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.Mors.IServices;
using Sky.Ramesses.DTO;
using Sky.Ramesses.IServices;
using Sky.SuperUser.DTO;
using Sky.SuperUser.IServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Web.Infrastructure;
using UI.Web.Models.User;

using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;
using PermissionAct = Sky.Common.DTO.TK.Project.Studio.TKPermissionAction;
using Sky.Mors.DTO;
using System.Globalization;

namespace UI.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _iUserService;
        private readonly IGenericService _iGenericService;
        private readonly IPaymentService _iPaymentService;
        private readonly IGeneralContentService _iGeneralContentService;
        private readonly ITypeService _iTypeService;
        private readonly ICDNService _iCDNService;
        private readonly ICustomerService _iCustomerService;
        private readonly IRaffleService _iRaffleService;
        private readonly IAdministratorService _iAdministratorService;
        private readonly MorsOperations _morsOperations;

        public UserController(
            Sky.SuperUser.IServices.ISessionService iSessionService,
            ILogService iLogService,
            IUserService iUserService,
            IGenericService iGenericService,
            IPaymentService iPaymentService,
            IGeneralContentService iGeneralContentService,
            ITypeService iTypeService,
            ICDNService iCDNService,
            ICustomerService iCustomerService,
            IRaffleService iRaffleService,
            IAdministratorService iAdministratorService,
            MorsOperations morsOperations)
            : base(iSessionService, iLogService)
        {
            this._iUserService = iUserService;
            this._iGenericService = iGenericService;
            this._iPaymentService = iPaymentService;
            this._iGeneralContentService = iGeneralContentService;
            this._iTypeService = iTypeService;
            this._iCDNService = iCDNService;
            this._iCustomerService = iCustomerService;
            this._iRaffleService = iRaffleService;
            this._iAdministratorService = iAdministratorService;
            this._morsOperations = morsOperations;
        }

        #region User
        [PermissionFilter(PermissionNav.KullaniciListesiniGoruntuleme)]
        public ActionResult List(string q, int? Limit)
        {
            var model = new UserListModel();

            if (!Limit.HasValue)
            {
                Limit = 200;
            }

            model.Limit = Limit.Value;

            if (!string.IsNullOrEmpty(q))
            {
                model.UserList = this._iUserService.GetUserListByEmail(q.Trim());
                model.Query = q.Trim();

                return View(model);
            }

            model.UserList = this._iUserService.GetUserListViaLimit(Limit.Value);
            
            return View(model);
        }

        [PermissionFilter(PermissionNav.KullaniciDetayiniGoruntuleme)]
        public ActionResult Detail(int ID)
        {
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var model = new UserModel();

            model.User = userItem;
            model.UserProfile = this._iUserService.GetUserProfileByID(ID) ?? new UserProfile();
            model.UserPasswordInfo = this._iUserService.GetUserPasswordInfoByUserID(ID) ?? new UserPasswordInfo();
            model.UserRegisterDetail = this._iUserService.GetUserRegisterDetailByID(ID) ?? new UserRegisterDetail();
            model.UserBillInfoList = this._iUserService.GetUserBillInfoListByUserIDLARGE(ID);
            model.UserBillList = this._iUserService.GetUserBillListByUserIDMEDIUM(ID);
            model.ContactMessageList = this._iGenericService.GetContactMessageListByUserIDMEDIUM(ID);
            model.UserBalanceList = this._iUserService.GetUserBalanceListByUserIDMEDIUM(ID);
            model.CustomerList = this._iCustomerService.GetCustomerListByActorLARGE(TK.TKActor.User, ID);

            var countryList = this._iGeneralContentService.GetCountryList();
            var userBankAccountList = this._iUserService.GetUserBankAccountListByUserIDMEDIUM(ID);
            var bankList = this._iGeneralContentService.GetBankListMEDIUM();
            var userTeamList = this._iUserService.GetUserTeamList();

            var countryItem = countryList.Where(c => c.ID == userItem.CountryID).FirstOrDefault() ?? new Country();

            userItem.CountryPhoneCode = countryItem.PhoneCode;
            userItem.CountryName = countryItem.Name;

            model.UserTeamList = userTeamList;

            if (model.UserProfile.UserTeamID.HasValue)
            {
                model.UserProfile.UserTeam = userTeamList.Where(ut => ut.ID == model.UserProfile.UserTeamID.Value).FirstOrDefault() ?? new UserTeam();
            }

            foreach (var item in model.CustomerList)
            {
                var tempCountry = countryList.Where(c => c.ID == item.CountryID).FirstOrDefault() ?? new Country();
                item.CountryPhoneCode = tempCountry.PhoneCode;
                item.CountryName = tempCountry.Name;
            }

            foreach (var item in userBankAccountList)
            {
                item.BankName = (bankList.Where(b => b.ID == item.BankID).FirstOrDefault() ?? new Bank()).Name;
            }

            model.UserBankAccountList = userBankAccountList;

            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByActorLARGE(TK.TKActor.User, ID);
            var raffleTemplateList = this._iRaffleService.GetRaffleTemplateListLARGE();
            var raffleList = this._iRaffleService.GetRaffleListByIDList(
                raffleParticipantList.Select(rp => rp.RaffleID).Distinct().ToArray()
                );

            var paymentList = this._iPaymentService.GetPaymentListByActorLARGE(TK.TKActor.User, ID);
            var paymentStatusRelationList = this._iPaymentService.GetPaymentStatusRelationListByPaymentIDList(
                paymentList.Select(p => p.ID).ToArray()
                );

            int[] socialMediaAccountIDArray = paymentList.Select(p => p.SocialMediaAccountID).ToArray();
            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByIDList(socialMediaAccountIDArray);

            var typeValueList = this._iTypeService.GetTypeValueListByTypeKeyTagList(
                    KeyTagProvider.Keys["User/Detail"].ToArray()
                    );

            foreach (var item in paymentList)
            {
                item.PaymentStatusRelationList = paymentStatusRelationList.Where(psr => psr.PaymentID == item.ID).ToList();
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();
            }

            foreach (var item in raffleList)
            {
                item.RaffleTemplate = raffleTemplateList.Where(rt => rt.ID == item.RaffleTemplateID).FirstOrDefault() ?? new RaffleTemplate();
            }

            foreach (var item in raffleParticipantList)
            {
                item.Raffle = raffleList.Where(r => r.ID == item.RaffleID).FirstOrDefault() ?? new Raffle();
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();
            }

            if (model.UserRegisterDetail.ReferralUserID.HasValue)
            {
                var referralUserItem = this._iUserService.GetUserByIDMEDIUM(model.UserRegisterDetail.ReferralUserID.Value) ?? new User();
                model.UserRegisterDetail.ReferralUserFullName = string.Format("{0} {1}", referralUserItem.FirstName, referralUserItem.LastName);
            }

            model.RaffleParticipantList = raffleParticipantList;
            model.ActiveUserBalance = model.UserBalanceList.Where(ub => ub.IsShown && ub.IsActive && !ub.IsDeleted).FirstOrDefault() ?? new UserBalance();
            
            model.PaymentList = paymentList;

            model.CountryList = countryList;
            model.CityList = this._iGeneralContentService.GetCityList();

            model.TKBalanceActionList = TypeService.GetAll(TK.TKBalanceAction.Undefined);
            model.TKJobTitleList =
                typeValueList.Where(tv => tv.TypeKeyID == (int)KeyTagProvider.TKList.TKJobTitle).ToList();
            model.TKIndustryList =
                typeValueList.Where(tv => tv.TypeKeyID == (int)KeyTagProvider.TKList.TKIndustry).ToList();
            model.CityList = this._iGeneralContentService.GetCityList();
            model.TKLogList = TypeService.GetAll(TK.TKLog.Undefined, true);

            ViewBag.IsMenuClosed = true;
            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullanicininEncryptedSifreleriniGoruntuleme)]
        public ActionResult PasswordList(int ID)
        {
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var model = new UserPasswordListModel()
            {
                UserPasswordInfoList = this._iUserService.GetUserPasswordInfoListByUserID(ID),
                User = userItem
            };

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullanicininSifreSifirlamaTalepleriniGoruntuleme)]
        public ActionResult PasswordResetList(int ID)
        {
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var model = new UserPasswordResetListModel()
            {
                UserPasswordResetList = this._iUserService.GetUserPasswordResetListByUserID(ID),
                User = userItem
            };
            return View(model);
        }

        [PermissionFilter(PermissionNav.KullanicininNotlariniGoruntuleme)]
        public ActionResult NoteList(int ID)
        {
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var model = new UserNoteListModel()
            {
                UserNoteList = this._iUserService.GetUserNoteListByUserID(ID),
                User = userItem
            };

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciyaNotEkleme)]
        public JsonResult NoteSaveAjax(UserNote userNote)
        {
            Result result = new Result();

            if (string.IsNullOrEmpty(userNote.NoteContent))
            {
                result.Message = "Not giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var userItem = this._iUserService.GetUserByIDMEDIUM(userNote.UserID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userNote.UserID = userNote.UserID;
            userNote.IsDeleted = false;
            userNote.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            userNote.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iUserService.SaveUserNote(userNote, out id))
            {
                result.Message = "Not kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userNote.ID = id;

            #region LogTransaction
            var logContent = userNote;

            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserNote,
                userNote.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Kullanıcı Notu kaydedildi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Not başarıyla kaydedildi";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciGirisBilgileriniGuncelleme)]
        public JsonResult UpdateSecuritySettingsAjax(int ID, string Email, string Password)
        {
            Result result = new Result();

            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            var userPasswordInfoItem = this._iUserService.GetUserPasswordInfoByUserID(ID);

            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(Email))
            {
                result.Message = "Email adresi giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(Password))
            {
                result.Message = "Şifre giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var anyDuplicatedUser = this._iUserService.GetUserByEmail(Email);
            if (anyDuplicatedUser != null && anyDuplicatedUser.ID != ID)
            {
                result.Message = string.Format("{0} email adresi daha önceden başka bir hesap için kullanılmaktadır. Lütfen başka bir email ile yeniden deneyiniz.", Email);
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userItem.Email == Email && (userPasswordInfoItem != null && userPasswordInfoItem.HashPassword == Password))
            {
                result.Data = new
                {
                    HashPassword = userPasswordInfoItem.HashPassword
                };
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userItem.Email != Email)
            {
                if (!this._iUserService.UpdateUserEmail(ID, Email))
                {
                    result.Message = "Email güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = new
                {
                    oldValue = new
                    {
                        Email = userItem.Email
                    },
                    newValue = new
                    {
                        Email = Email
                    }
                };

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.User,
                    userItem.ID,
                    TK.TKTransactionType.Update,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Kullanıcı Email Adresi Güncellendi"
                    );
                #endregion
            }

            string hashPassword = null;
            if (userPasswordInfoItem == null || userPasswordInfoItem.HashPassword != Password)
            {
                this._iUserService.UpdateUserPasswordInfoIsDeleted(ID, true);

                string saltPassword = Guid.NewGuid().ToString().Replace("-", "");
                hashPassword = EncryptionUtility.MD5(Password, saltPassword);
                int id;
                bool isSuccess = this._iUserService.SaveUserPasswordInfo(new UserPasswordInfo()
                {
                    UserID = ID,
                    SaltPassword = saltPassword,
                    HashPassword = hashPassword,
                    Description = string.Format("Created By Administrator: {0}, {1} {2}",
                    this.OutAdministratorID,
                    this.OutAdministratorItem.FirstName,
                    this.OutAdministratorItem.LastName),
                    TKPlatform = TK.TKPlatform.Web,
                    IsDeleted = false,
                    IPAddress = IPProvider.GetIPAddress(this.HttpContext),
                    CreatedAt = DateTime.UtcNow
                }, out id);

                if (!isSuccess)
                {
                    result.Message = "Yeni şifre kaydedilemedi. Eski şifre silindiği için kullanıcı girişi kapalı. Sistem geliştiricisiyle iletişime geçiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }

            result.Data = new
            {
                HashPassword = hashPassword
            };
            result.Message = "Email ve şifre başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciKisiselBilgileriniGuncelleme)]
        public JsonResult UpdateUserPersonalInfoAjax(User user)
        {
            Result result = new Result();

            #region Validation
            var userItem = this._iUserService.GetUserByIDMEDIUM(user.ID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(user.FirstName))
            {
                result.Message = "Ad giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                result.Message = "Soyad giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                result.Message = "Telefon numarası giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var countryItem = this._iGeneralContentService.GetCountryByID(user.CountryID);
            if (countryItem == null)
            {
                result.Message = "Ülke seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            user.IsDeleted = !user.IsDeleted;

            if (user.FirstName == userItem.FirstName &&
                user.LastName == userItem.LastName &&
                user.CountryID == userItem.CountryID &&
                user.PhoneNumber == userItem.PhoneNumber &&
                user.CommissionRate == userItem.CommissionRate &&
                user.MinWithdraw == userItem.MinWithdraw &&
                user.IsActive == userItem.IsActive &&
                user.IsDeleted == userItem.IsDeleted)
            {
                result.IsSuccess = true;
                result.Message = "Herhangi bir değişiklik algılanmadı";

                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserADMIN(user))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var logContent = new
            {
                oldValue = new
                {
                    FirstName = userItem.FirstName,
                    LastName = userItem.LastName,
                    CountryID = userItem.CountryID,
                    PhoneNumber = userItem.PhoneNumber,
                    CommissionRate = userItem.CommissionRate,
                    MinWithdraw = userItem.MinWithdraw,
                    IsActive = userItem.IsActive,
                    IsDeleted = userItem.IsDeleted
                },
                newValue = new
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CountryID = user.CountryID,
                    PhoneNumber = user.PhoneNumber,
                    CommissionRate = user.CommissionRate,
                    MinWithdraw = user.MinWithdraw,
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted
                }
            };

            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.User,
                user.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Kullanıcı Kişisel Bilgileri Güncellendi"
                );

            result.Message = "Kullanıcı başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciEkBilgileriGuncelleme)]
        public JsonResult UpdateUserProfileAjax(UserProfile userProfile, string BirthdateStr)
        {
            Result result = new Result();

            var userItem = this._iUserService.GetUserByIDMEDIUM(userProfile.ID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            DateTime? dt = null;
            ValidationUtility.StringToDatetime(BirthdateStr, "dd/MM/yyyy", out dt);
            userProfile.Birthdate = dt;

            var tempUserProfile = this._iUserService.GetUserProfileByID(userProfile.ID);
            if (tempUserProfile != null)
            {
                if (userProfile.UserTeamID == tempUserProfile.UserTeamID &&
                    userProfile.Gender == tempUserProfile.Gender &&
                    userProfile.Birthdate == tempUserProfile.Birthdate &&
                    userProfile.CityID == tempUserProfile.CityID &&
                    userProfile.TKJobTitle == tempUserProfile.TKJobTitle &&
                    userProfile.OtherJobTitle == tempUserProfile.OtherJobTitle &&
                    userProfile.CompanyTitle == tempUserProfile.CompanyTitle &&
                    userProfile.TKIndustry == tempUserProfile.TKIndustry &&
                    userProfile.OtherIndustry == tempUserProfile.OtherIndustry)
                {
                    result.NoChanges();
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (!this._iUserService.UpdateUserProfile(userProfile))
                {
                    result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = new
                {
                    oldValue = new
                    {
                        UserTeamID = tempUserProfile.UserTeamID,
                        Gender = tempUserProfile.Gender,
                        Birthdate = tempUserProfile.Birthdate,
                        CityID = tempUserProfile.CityID,
                        TKJobTitle = tempUserProfile.TKJobTitle,
                        CompanyTitle = tempUserProfile.CompanyTitle,
                        TKIndustry = tempUserProfile.TKIndustry
                    },
                    newValue = new
                    {
                        UserTeamID = userProfile.UserTeamID,
                        Gender = userProfile.Gender,
                        Birthdate = userProfile.Birthdate,
                        CityID = userProfile.CityID,
                        TKJobTitle = userProfile.TKJobTitle,
                        CompanyTitle = userProfile.CompanyTitle,
                        TKIndustry = userProfile.TKIndustry
                    }
                };

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.UserProfile,
                    tempUserProfile.ID,
                    TK.TKTransactionType.Update,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Kullanıcı Ek Bilgileri güncellendi"
                    );
                #endregion
            }
            else
            {
                userProfile.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                userProfile.CreatedAt = DateTime.UtcNow;

                if (!this._iUserService.SaveUserProfile(userProfile))
                {
                    result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = userProfile;

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.UserProfile,
                    userProfile.ID,
                    TK.TKTransactionType.Save,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Kullanıcı Ek Bilgileri kaydedildi"
                    );
                #endregion
            }

            result.Message = "Kullanıcı ek bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullanicininProfilFotografiGuncelleme)]
        public JsonResult UpdateProfileURLAjax(int ID)
        {
            Result result = new Result();

            string profileURL = null;

            HttpPostedFileBase httpPostedFile = Request.Files["ProfileURL"];
            if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
            {
                #region Generic Token
                var genericToken = this._iCDNService.GetActiveToken();
                if (genericToken == null)
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Aktif GenericToken bulunamadı.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrEmpty(genericToken.Token))
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Aktif GenericToken boş.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                #region Prepare and Save InputData
                byte[] data = null;
                string fileExtension = null;

                HttpPostedFileBase filePosted = Request.Files["ProfileURL"];
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
                        result.Message = "İşleminiz gerçekleştirilemiyor. CDN istek hatası.";
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }

                    #region Connect to CDN to Upload File
                    result = this._iCDNService.UploadFile(inputData, System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], genericToken.Token);
                    if (!result.IsSuccess)
                    {
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    #endregion

                    profileURL = result.Data.ToString();
                }
                else
                {
                    result.Message = "Data is null";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
            }

            var tempUserProfile = this._iUserService.GetUserProfileByID(ID);
            if (tempUserProfile != null)
            {
                if (tempUserProfile.ProfileURL == profileURL)
                {
                    result.NoChanges();
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (!this._iUserService.UpdateUserProfileProfileURL(ID, profileURL))
                {
                    result.Message = "Profil fotoğrafı güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = new
                {
                    oldValue = new
                    {
                        ProfileURL = tempUserProfile.ProfileURL
                    },
                    newValue = new
                    {
                        ProfileURL = profileURL
                    }
                };

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.UserProfile,
                    tempUserProfile.ID,
                    TK.TKTransactionType.Update,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Kullanıcı Ek Bilgileri Profil Fotoğrafı Güncellendi"
                    );
                #endregion
            }
            else
            {
                UserProfile userProfile = new UserProfile();
                userProfile.ID = ID;
                userProfile.ProfileURL = profileURL;
                userProfile.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                userProfile.CreatedAt = DateTime.UtcNow;

                if (!this._iUserService.SaveUserProfile(userProfile))
                {
                    result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = userProfile;

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.UserProfile,
                    userProfile.ID,
                    TK.TKTransactionType.Save,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Kullanıcı Ek Bilgileri Profil Fotoğrafıyla Kaydedildi"
                    );
                #endregion
            }

            result.IsSuccess = true;
            result.Message = "Profil fotoğrafı başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.HesapOnayiEmailiGonder)]
        public JsonResult SendEmailAccountConfirmationAjax(int ID, TK.TKEmailTemplate TKEmailTemplate, string FullName, string Email, bool UseBCC, string BCCEmail)
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
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKEmailTemplate != TK.TKEmailTemplate.UserAccountConfirmed && TKEmailTemplate != TK.TKEmailTemplate.UserAccountNotConfirmed)
            {
                result.Message = "Email Template bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendEmailAccountConfirmationAjaxEmail(TKEmailTemplate, FullName, Email, UseBCC, BCCEmail);

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.HesapBakiyenizGuncellendiEmailiGonder)]
        public JsonResult SendEmailUserBalanceAjax(int ID, string FullName, string Email, bool UseBCC, string BCCEmail)
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
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var userBalanceItem = this._iUserService.GetUserBalanceByUserIDACTIVE(ID);
            if (userBalanceItem == null)
            {
                result.Message = "Kullanıcının aktif bir bakiye kaydı olmadığı için işleminize devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendEmailUserBalanceAjaxEmail(userBalanceItem, FullName, Email, UseBCC, BCCEmail);

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.HesapBakiyenizGuncellendiSmsiGonder)]
        public JsonResult SendSmsUserBalanceAjax(int ID, string FullName, string PhoneNumber)
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

            #region Validation (Specific)
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var userBalanceItem = this._iUserService.GetUserBalanceByUserIDACTIVE(ID);
            if (userBalanceItem == null)
            {
                result.Message = "Kullanıcının aktif bir bakiye kaydı olmadığı için işleminize devam edilemiyor.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendSmsUserBalanceAjaxSms(userBalanceItem, FullName, PhoneNumber);

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.HesapOnayiSmsiGonder)]
        public JsonResult SendSmsAccountConfirmationAjax(int ID, TK.TKSmsTemplate TKSmsTemplate, string FullName, string PhoneNumber)
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

            #region Validation (Specific)
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKSmsTemplate != TK.TKSmsTemplate.UserAccountConfirmed && TKSmsTemplate != TK.TKSmsTemplate.UserAccountNotConfirmed)
            {
                result.Message = "Sms Template bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendSmsAccountConfirmationAjaxSms(TKSmsTemplate, FullName, PhoneNumber);

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region UserBankAccount
        [PermissionFilter(PermissionNav.KullanicininBankaHesabiDetayiniGoruntuleme)]
        public ActionResult BankAccountDetail(int ID)
        {
            var userBankAccountItem = this._iUserService.GetUserBankAccountByIDMEDIUM(ID);
            if (userBankAccountItem == null)
            {
                throw new Exception();
            }

            var bankItem = this._iGeneralContentService.GetBankByIDLARGE(ID);
            if (bankItem == null)
            {
                throw new Exception();
            }

            userBankAccountItem.BankName = bankItem.Name;
            userBankAccountItem.BankLogoURL = bankItem.LogoURL;

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBankAccountItem.UserID);
            if (userItem == null)
            {
                throw new Exception();
            }

            var model = new UserBankAccountModel()
            {
                User = userItem,
                UserBankAccount = userBankAccountItem
            };

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciBankaHesabiniGuncelleme)]
        public JsonResult BankAccountUpdateAjax(UserBankAccount userBankAccount, bool IncludeTR)
        {
            Result result = new Result();

            var userBankAccountItem = this._iUserService.GetUserBankAccountByIDMEDIUM(userBankAccount.ID);
            if (userBankAccountItem == null)
            {
                result.Message = "Seçilen banka hesabı daha önce silinmiş olabilir. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBankAccount.IBAN))
            {
                result.Message = "IBAN Giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (IncludeTR)
            {
                IIbanValidator validator = new IbanValidator();
                ValidationResult validationResult = validator.Validate(userBankAccount.IBAN);
                if (!validationResult.IsValid)
                {
                    result.Message = "Girdiğiniz IBAN numarası geçersizdir. Lütfen kontrol edip yeniden giriniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }

            if (string.IsNullOrEmpty(userBankAccount.Owner))
            {
                result.Message = "Hesap Sahibini Giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!string.IsNullOrEmpty(userBankAccount.IBAN))
            {
                userBankAccount.IBAN = userBankAccount.IBAN.Replace("_", "");
            }

            if (userBankAccount.AccountNumber == userBankAccountItem.AccountNumber &&
                userBankAccount.Branch == userBankAccountItem.Branch &&
                userBankAccount.IBAN == userBankAccountItem.IBAN &&
                userBankAccount.Owner == userBankAccountItem.Owner &&
                userBankAccount.Sequence == userBankAccountItem.Sequence &&
                userBankAccount.IsShown == userBankAccountItem.IsShown &&
                userBankAccount.IsActive == userBankAccountItem.IsActive)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBankAccountADMIN(userBankAccount))
            {
                result.Message = "Banka hesabı güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    AccountNumber = userBankAccountItem.AccountNumber,
                    Branch = userBankAccountItem.Branch,
                    IBAN = userBankAccountItem.IBAN,
                    Owner = userBankAccountItem.Owner,
                    Sequence = userBankAccountItem.Sequence,
                    IsShown = userBankAccountItem.IsShown,
                    IsActive = userBankAccountItem.IsActive
                },
                newValue = new
                {
                    AccountNumber = userBankAccount.AccountNumber,
                    Branch = userBankAccount.Branch,
                    IBAN = userBankAccount.IBAN,
                    Owner = userBankAccount.Owner,
                    Sequence = userBankAccount.Sequence,
                    IsShown = userBankAccount.IsShown,
                    IsActive = userBankAccount.IsActive
                }
            };

            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBankAccount,
                userBankAccount.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Kullanıcı banka hesabı güncellendi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Banka hesabı başarıyla güncellendi.";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciBankaHesabiniSilme)]
        public JsonResult BankAccountUpdateIsDeletedAjax(int ID)
        {
            Result result = new Result();

            var userBankAccountItem = this._iUserService.GetUserBankAccountByIDMEDIUM(ID);
            if (userBankAccountItem == null)
            {
                result.Message = "Seçilen banka hesabı daha önce silinmiş olabilir. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBankAccountIsDeleted(ID, true))
            {
                result.Message = "Banka hesabı silinemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
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
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBankAccount,
                ID,
                TK.TKTransactionType.Delete,
                new JavaScriptSerializer().Serialize(logContent),
                "Kullanıcı banka hesabı silindi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Banka hesabı başarıyla silindi.";

            return Json(result, JsonRequestBehavior.DenyGet);

        }
        #endregion

        #region UserBill
        [PermissionFilter(PermissionNav.KullaniciFaturaListesiniGoruntuleme)]
        public ActionResult BillList()
        {
            var model = new UserBillListModel();

            var userBillList = this._iUserService.GetUserBillList();

            var userList = this._iUserService.GetUserListByIDList(
                userBillList
                .Where(b => b.UserID.HasValue)
                .Select(b => b.UserID.Value)
                .ToArray()
                );

            foreach (var item in userBillList)
            {
                if (item.UserID.HasValue)
                {
                    var tempUser = userList.Where(u => u.ID == item.UserID.Value).FirstOrDefault() ?? new User();
                    item.UserFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
                }
            }

            model.UserBillList = userBillList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullaniciFaturasiEkleme)]
        public ActionResult BillSave(int? UserID)
        {
            var model = new UserBillModel();

            User userItem = null;
            if (UserID.HasValue && UserID.Value != 0)
            {
                userItem = this._iUserService.GetUserByIDMEDIUM(UserID.Value);
                if (userItem == null)
                {
                    return RedirectToAction("Oops", "Error");
                }

                model.User = userItem;
                model.IsUserIDSupplied = true;
            }
            else
            {
                model.User = new User();
            }

            if (UserID.HasValue)
            {
                model.UserBillInfoList = this._iUserService.GetUserBillInfoListByUserIDMEDIUM(UserID.Value);
            }
            else
            {
                model.UserBillInfoList = new List<UserBillInfo>();
            }

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullaniciFaturaDetayiniGoruntuleme)]
        public ActionResult BillUpdate(int ID)
        {
            var model = new UserBillModel();

            var userBillItem = this._iUserService.GetUserBillByIDMEDIUM(ID);

            if (userBillItem == null)
            {
                return RedirectToAction("Opps", "Error");
            }

            if (userBillItem.UserID.HasValue && userBillItem.UserID.Value != 0)
            {
                var userItem = this._iUserService.GetUserByIDMEDIUM(userBillItem.UserID.Value);
                if (userItem == null)
                {
                    return RedirectToAction("Oops", "Error");
                }

                var countryItem = this._iGeneralContentService.GetCountryByID(userItem.CountryID) ?? new Country();
                userItem.CountryPhoneCode = countryItem.PhoneCode;
                userItem.CountryName = countryItem.Name;

                model.User = userItem;
                model.IsUserIDSupplied = true;
            }
            else
            {
                model.User = new User();
            }

            model.UserBill = userBillItem;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.KullaniciFaturasiEkleme, true)]
        public JsonResult BillSaveAjax(UserBill UserBill, string StringEditedAt)
        {
            Result result = new Result();

            HttpPostedFileBase httpPostedFile = Request.Files["URL"];

            #region Nullable User
            int? UserID = null;
            User userItem = null;

            if (UserBill.UserID.HasValue)
            {
                userItem = this._iUserService.GetUserByIDMEDIUM(UserBill.UserID.Value);
                if (userItem == null)
                {
                    result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                UserID = userItem.ID;
            }
            #endregion

            #region Validation
            if (httpPostedFile == null || httpPostedFile.ContentLength == 0)
            {
                result.Message = "Fatura dosyası seçilmedi. Dosya seçip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var duplicatedUserBillItem = this._iUserService.GetUserBillByBillNo(UserBill.BillNo);

            if (duplicatedUserBillItem != null)
            {
                result.Message = string.Format("{0} fatura numarası daha önceden kullanılmıştır. Farklı fatura numarasıyla tekrar deneyiniz.", UserBill.BillNo);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Generic Token
            var genericToken = this._iCDNService.GetActiveToken();
            if (genericToken == null)
            {
                result.Message = "İşleminiz gerçekleştirilemiyor. Aktif GenericToken bulunamadı";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(genericToken.Token))
            {
                result.Message = "İşleminiz gerçekleştirilemiyor. Aktif GenericToken boş.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Prepare and Save InputData
            byte[] data = null;
            string fileExtension = null;

            if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(httpPostedFile.FileName);
                fileExtension = System.IO.Path.GetExtension(fileName);

                using (System.IO.Stream inputStream = httpPostedFile.InputStream)
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
                    result.Message = "İşleminiz gerçekleştirilemiyor. CDN istek hatası.";
                    return Json(result, JsonRequestBehavior.DenyGet);

                }

                #region Connect to CDN to Upload File
                result = this._iCDNService.UploadDocument(inputData, System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], genericToken.Token);
                if (!result.IsSuccess)
                {
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                UserBill.URL = result.Data.ToString();
            }
            else
            {
                result.Message = "Data is null";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            DateTime? editedAt;
            if (!Sky.Common.Utilities.ValidationUtility.StringToDatetime(StringEditedAt, "dd/MM/yyyy", out editedAt))
            {
                result.Message = "Düzenlenme tarihi geçerli formatta değil.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!editedAt.HasValue)
            {
                result.Message = "Düzenlenme tarihi başarıyla dönüştürüldü ancak değeri bulunamadı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            UserBill.UniqueKey = string.Format("{0}{1}", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()).Replace("-", "");
            UserBill.EditedAt = editedAt.Value;
            UserBill.CreatedAt = DateTime.UtcNow;
            UserBill.IsDeleted = !UserBill.IsDeleted;
            int id;
            if (!this._iUserService.SaveUserBill(UserBill, out id))
            {
                result.Message = "Fatura bilgileri kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            UserBill.ID = id;

            #region Log Transaction
            var logContent = UserBill;

            base.LogTransaction(Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBill,
                UserBill.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Fatura kaydedildi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura başarıyla kaydedildi.";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullanicininFaturaDosyasiniGuncelleme)]
        public JsonResult BillUpdateURLAjax(int ID)
        {
            Result result = new Result();

            var userBillItem = this._iUserService.GetUserBillByIDMEDIUM(ID);

            if (userBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            HttpPostedFileBase httpPostedFile = Request.Files["URL"];
            if (httpPostedFile == null || httpPostedFile.ContentLength == 0)
            {
                result.Message = "Fatura dosyası boş bırakılamaz. Lütfen bir dosya seçip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string url;

            #region Generic Token
            var genericToken = this._iCDNService.GetActiveToken();
            if (genericToken == null)
            {
                result.Message = "İşleminiz gerçekleştirilemiyor. Aktif GenericToken bulunamadı";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrEmpty(genericToken.Token))
            {
                result.Message = "İşleminiz gerçekleştirilemiyor. Aktif GenericToken boş.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Prepare and Save InputData
            byte[] data = null;
            string fileExtension = null;

            if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(httpPostedFile.FileName);
                fileExtension = System.IO.Path.GetExtension(fileName);

                using (System.IO.Stream inputStream = httpPostedFile.InputStream)
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
                    result.Message = "İşleminiz gerçekleştirilemiyor. CDN istek hatası.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region Connect to CDN to Upload File
                result = this._iCDNService.UploadDocument(inputData, System.Configuration.ConfigurationManager.AppSettings["CDNBaseURL"], genericToken.Token);
                if (!result.IsSuccess)
                {
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion

                url = result.Data.ToString();
            }
            else
            {
                result.Message = "Data is null";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            if (!this._iUserService.UpdateUserBillURL(userBillItem.ID, url))
            {
                result.Message = "Fatura dosyası güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var userbillUrlLog = new
            {
                oldvalue = new
                {
                    URL = userBillItem.URL
                },
                newValue = new
                {
                    URL = url
                }
            };

            base.LogTransaction(Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBill,
                userBillItem.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(userbillUrlLog),
                "Fatura dosyası güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura dosyası başarıyla güncellendi.";
            result.Data = new
            {
                UserBillURL = string.Format("{0}{1}", ConfigurationManager.AppSettings["CDNBaseURL"], url)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullanicininFaturaCekirdekBilgileriniGuncelleme)]
        public JsonResult BillUpdateCoreAjax(UserBill userBill, string StringEditedAt)
        {
            Result result = new Result();

            var userBillItem = this._iUserService.GetUserBillByIDMEDIUM(userBill.ID);
            if (userBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var duplicatedItem = this._iUserService.GetUserBillByBillNo(userBill.BillNo);
            if (duplicatedItem != null && duplicatedItem.ID != userBillItem.ID)
            {
                result.Message = string.Format("{0} fatura numarası daha önceden kullanılmıştır. Lütfen başka fatura numarasıyla tekrar deneyiniz.", userBill.BillNo);
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            DateTime? editedAt;
            if (!Sky.Common.Utilities.ValidationUtility.StringToDatetime(StringEditedAt, "dd/MM/yyyy", out editedAt))
            {
                result.Message = "Düzenlenme tarihi geçerli formatta değil.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!editedAt.HasValue)
            {
                result.Message = "Düzenlenme tarihi başarıyla dönüştürüldü ancak değeri bulunamadı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            editedAt = editedAt.Value
                    .AddHours(userBillItem.EditedAt.Hour)
                    .AddMinutes(userBillItem.EditedAt.Minute)
                    .AddSeconds(userBillItem.EditedAt.Second)
                    .AddMilliseconds(userBillItem.EditedAt.Millisecond);

            userBill.EditedAt = editedAt.Value;
            userBill.IsDeleted = !userBill.IsDeleted;

            if (userBill.BillNo == userBillItem.BillNo &&
                userBill.EditedAt == userBillItem.EditedAt &&
                userBill.IsActive == userBillItem.IsActive &&
                userBill.IsDeleted == userBillItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBillCore(userBill))
            {
                result.Message = "Fatura güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    BillNo = userBillItem.BillNo,
                    EditedAt = userBillItem.EditedAt,
                    IsActive = userBillItem.IsActive,
                    IsDeleted = userBillItem.IsDeleted
                },
                newValue = new
                {
                    BillNo = userBill.BillNo,
                    EditedAt = userBill.EditedAt,
                    IsActive = userBill.IsActive,
                    IsDeleted = userBill.IsDeleted
                }
            };

            base.LogTransaction(Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBill,
                userBill.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Faturanın çekirdek bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Çekirdek bilgiler başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullanicininFaturaBilgileriniGuncelleme)]
        public JsonResult BillUpdateInformationAjax(UserBill userBill)
        {
            Result result = new Result();

            var userBillItem = this._iUserService.GetUserBillByIDMEDIUM(userBill.ID);
            if (userBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userBill.BillIsCorporate == userBillItem.BillIsCorporate &&
                userBill.BillFullName == userBillItem.BillFullName &&
                userBill.BillCompanyTitle == userBillItem.BillCompanyTitle &&
                userBill.BillCitizenIdentityNo == userBillItem.BillCitizenIdentityNo &&
                userBill.BillTaxNo == userBillItem.BillTaxNo &&
                userBill.BillTaxOffice == userBillItem.BillTaxOffice &&
                userBill.BillFullAddress == userBillItem.BillFullAddress &&
                userBill.BillPhoneNumber == userBillItem.BillPhoneNumber)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBillInformation(userBill))
            {
                result.Message = "Fatura güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    BillIsCorporate = userBillItem.BillIsCorporate,
                    BillFullName = userBillItem.BillFullName,
                    BillCompanyTitle = userBillItem.BillCompanyTitle,
                    BillCitizenIdentityNo = userBillItem.BillCitizenIdentityNo,
                    BillTaxNo = userBillItem.BillTaxNo,
                    BillTaxOffice = userBillItem.BillTaxOffice,
                    BillFullAddress = userBillItem.BillFullAddress,
                    BillPhoneNumber = userBillItem.BillPhoneNumber
                },
                newValue = new
                {
                    BillIsCorporate = userBill.BillIsCorporate,
                    BillFullName = userBill.BillFullName,
                    BillCompanyTitle = userBill.BillCompanyTitle,
                    BillCitizenIdentityNo = userBill.BillCitizenIdentityNo,
                    BillTaxNo = userBill.BillTaxNo,
                    BillTaxOffice = userBill.BillTaxOffice,
                    BillFullAddress = userBill.BillFullAddress,
                    BillPhoneNumber = userBill.BillPhoneNumber
                }
            };

            base.LogTransaction(Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBill,
                userBill.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Fatura kullanıcı bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura bilgileri başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullanicininFaturaFiyatBilgileriniGuncelleme)]
        public JsonResult BillUpdatePriceAjax(UserBill userBill)
        {
            Result result = new Result();

            var userBillItem = this._iUserService.GetUserBillByIDMEDIUM(userBill.ID);
            if (userBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userBill.SubTotalPrice == userBillItem.SubTotalPrice &&
                userBill.VATPrice == userBillItem.VATPrice &&
                userBill.TotalPrice == userBillItem.TotalPrice)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBillPrice(userBill))
            {
                result.Message = "Fatura güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    SubTotalPrice = userBillItem.SubTotalPrice,
                    VATPrice = userBillItem.VATPrice,
                    TotalPrice = userBillItem.TotalPrice
                },
                newValue = new
                {
                    SubTotalPrice = userBill.SubTotalPrice,
                    VATPrice = userBill.VATPrice,
                    TotalPrice = userBill.TotalPrice
                }
            };

            base.LogTransaction(Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBill,
                userBill.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Fatura fiyat bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura bilgileri başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciyaFaturanizHazirEmailiGonder)]
        public JsonResult SendEmailUserBillAjax(int UserBillID, string FullName, string Email, bool UseBCC, string BCCEmail)
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
            var userBillItem = this._iUserService.GetUserBillByIDSMALL(UserBillID);
            if (userBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın pasif ve/veya silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendEmailUserBillAjaxEmail(FullName, userBillItem, Email, UseBCC, BCCEmail);

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciyaFaturanizHazirSmsiGonder)]
        public JsonResult SendSmsUserBillAjax(int UserBillID, string FullName, string PhoneNumber)
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

            #region Validation (Specific)
            var userBillItem = this._iUserService.GetUserBillByIDSMALL(UserBillID);
            if (userBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın pasif ve/veya silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendSmsUserBillAjaxSms(FullName, userBillItem, PhoneNumber);

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region UserBillInfo
        [PermissionFilter(PermissionNav.KullaniciFaturaBilgisiEkleme)]
        public ActionResult BillInfoSave(int ID)
        {
            var model = new UserBillInfoModel();

            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            model.User = userItem;

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullaniciFaturaBilgisiGuncelleme)]
        public ActionResult BillInfoUpdate(int ID)
        {
            var userBillInfoItem = this._iUserService.GetUserBillInfoByIDLARGE(ID);
            if (userBillInfoItem == null)
            {
                return RedirectToAction("Opps", "Error");
            }

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBillInfoItem.UserID);
            if (userItem == null)
            {
                return RedirectToAction("Opps", "Error");
            }

            var model = new UserBillInfoModel();

            model.UserBillInfo = userBillInfoItem;
            model.User = userItem;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.KullaniciFaturaBilgisiEkleme, true)]
        public JsonResult BillInfoSaveAjax(UserBillInfo userBillInfo)
        {
            Result result = new Result();

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBillInfo.UserID);

            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userBillInfo.IsDeleted = !userBillInfo.IsDeleted;
            userBillInfo.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            userBillInfo.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iUserService.SaveUserBillInfo(userBillInfo, out id))
            {
                result.Message = "Fatura bilgileri kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userBillInfo.ID = id;

            #region Log Transaction
            var logContent = userBillInfo;

            base.LogTransaction
            (
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBillInfo,
                id,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Kullanıcı fatura bilgisi kaydedildi"
            );
            #endregion

            result.Message = "Kullanıcı fatura bilgileri başarıyla kaydedildi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.KullaniciFaturaBilgisiGuncelleme, true)]
        public JsonResult BillInfoUpdateAjax(UserBillInfo userBillInfo)
        {
            Result result = new Result();

            var userBillInfoItem = this._iUserService.GetUserBillInfoByIDLARGE(userBillInfo.ID);

            if (userBillInfoItem == null)
            {
                result.Message = "Fatura bilgisi bulunamadı. Fatura bilgisinin silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userBillInfo.IsDeleted = !userBillInfo.IsDeleted;

            if (userBillInfo.IsCorporate == userBillInfoItem.IsCorporate &&
                userBillInfo.CompanyTitle == userBillInfoItem.CompanyTitle &&
                userBillInfo.FullName == userBillInfoItem.FullName &&
                userBillInfo.CitizenIdentityNo == userBillInfoItem.CitizenIdentityNo &&
                userBillInfo.TaxNo == userBillInfoItem.TaxNo &&
                userBillInfo.TaxOffice == userBillInfoItem.TaxOffice &&
                userBillInfo.PhoneNumber == userBillInfoItem.PhoneNumber &&
                userBillInfo.FullAddress == userBillInfoItem.FullAddress &&
                userBillInfo.IsActive == userBillInfoItem.IsActive &&
                userBillInfo.IsShown == userBillInfoItem.IsShown &&
                userBillInfo.IsDeleted == userBillInfoItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBillInfo(userBillInfo))
            {
                result.Message = "Fatura bilgileri güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    IsCorporate = userBillInfoItem.IsCorporate,
                    CompanyTitle = userBillInfoItem.CompanyTitle,
                    FullName = userBillInfoItem.FullName,
                    CitizenIdentityNo = userBillInfoItem.CitizenIdentityNo,
                    TaxNo = userBillInfoItem.TaxNo,
                    TaxOffice = userBillInfoItem.TaxOffice,
                    PhoneNumber = userBillInfoItem.PhoneNumber,
                    FullAddress = userBillInfoItem.FullAddress,
                    IsActive = userBillInfoItem.IsActive,
                    IsShown = userBillInfoItem.IsShown,
                    IsDeleted = userBillInfoItem.IsDeleted
                },
                newValue = new
                {
                    IsCorporate = userBillInfo.IsCorporate,
                    CompanyTitle = userBillInfo.CompanyTitle,
                    FullName = userBillInfo.FullName,
                    CitizenIdentityNo = userBillInfo.CitizenIdentityNo,
                    TaxNo = userBillInfo.TaxNo,
                    TaxOffice = userBillInfo.TaxOffice,
                    PhoneNumber = userBillInfo.PhoneNumber,
                    FullAddress = userBillInfo.FullAddress,
                    IsActive = userBillInfo.IsActive,
                    IsShown = userBillInfo.IsShown,
                    IsDeleted = userBillInfo.IsDeleted
                }
            };

            base.LogTransaction(Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBill,
                userBillInfo.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Kullanıcı fatura bilgisi güncellendi"
                );
            #endregion

            result.Message = "Fatura bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region UserBalance
        [PermissionFilter(PermissionNav.KullaniciOdemeleri)]
        public ActionResult BalanceListNotZero()
        {
            var userBalanceList = this._iUserService.GetUserBalanceListACTIVE();
            userBalanceList = userBalanceList.Where(ub => ub.CurrentBalance > 0).ToList();

            var userIDList = userBalanceList.Select(u => u.UserID).ToArray();

            var userList = this._iUserService.GetUserListByIDList(userIDList);

            foreach (var item in userBalanceList)
            {
                var tempUser = userList.Where(u => u.ID == item.UserID).FirstOrDefault();
                if (tempUser == null)
                {
                    continue;
                }

                item.UserFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            var model = new UserBalanceListModel()
            {
                UserBalanceList = userBalanceList
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullaniciBakiyeDetayiniGuncelleme)]
        public ActionResult BalanceDetail(int ID)
        {
            var userBalance = this._iUserService.GetUserBalanceByIDMEDIUM(ID);
            if (userBalance == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBalance.UserID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var model = new UserBalanceModel()
            {
                UserBalance = userBalance,
                User = userItem
            };
            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciOdemeleriniGerceklestirme)]
        public JsonResult GetUserBalanceAjax(int ID)
        {
            Result result = new Result();

            #region Validation
            var userBalanceItem = this._iUserService.GetUserBalanceByIDMEDIUM(ID);
            if (userBalanceItem == null)
            {
                result.Message = "İşlem kaydı bulunamadı. İşlem kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userBalanceItem.IsShown)
            {
                result.Message = "İşlem kaydının Görünür özelliği daha önceden Hayır olarak güncellenmiştir. Bu kayıt için ödeme gerçekleştirilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userBalanceItem.IsActive)
            {
                result.Message = "İşlem kaydının Durum özelliği daha önceden Pasif olarak güncellenmiştir. Bu kayıt için ödeme gerçekleştirilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userBalanceItem.IsDeleted)
            {
                result.Message = "İşlem kaydının Silindi özelliği daha önceden Evet olarak güncellenmiştir. Bu kayıt için ödeme gerçekleştirilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBalanceItem.UserID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            var userBankAccountList = this._iUserService.GetUserBankAccountListByUserIDSMALL(userBalanceItem.UserID);
            var bankList = this._iGeneralContentService.GetBankListMEDIUM();

            foreach (var item in userBankAccountList)
            {
                var tempBank = bankList.Where(b => b.ID == item.BankID).FirstOrDefault();

                if (tempBank == null)
                {
                    continue;
                }

                item.BankName = tempBank.Name;
            }

            result.IsSuccess = true;
            result.Data = new
            {
                UserBalance = userBalanceItem,
                UserBankAccountList = userBankAccountList,
                User = userItem
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.KullaniciOdemeleriniGerceklestirme)]
        public JsonResult BalanceDoPaymentAjax(UserBalance userBalance)
        {
            Result result = new Result();

            #region Validation
            var userBalanceItem = this._iUserService.GetUserBalanceByIDMEDIUM(userBalance.ID);
            if (userBalanceItem == null)
            {
                result.Message = "İşlem kaydı bulunamadı. İşlem kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userBalanceItem.IsShown)
            {
                result.Message = "İşlem kaydının Görünür özelliği daha önceden Hayır olarak güncellenmiştir. Bu kayıt için ödeme gerçekleştirilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userBalanceItem.IsActive)
            {
                result.Message = "İşlem kaydının Durum özelliği daha önceden Pasif olarak güncellenmiştir. Bu kayıt için ödeme gerçekleştirilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userBalanceItem.IsDeleted)
            {
                result.Message = "İşlem kaydının Silindi özelliği daha önceden Evet olarak güncellenmiştir. Bu kayıt için ödeme gerçekleştirilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBalance.UserID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var activeUserBalance = this._iUserService.GetUserBalanceByUserIDACTIVE(userBalance.UserID);
            if (activeUserBalance == null)
            {
                result.Message = "Kullanıcının aktif bir işlem kaydı bulunmamaktadır. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (activeUserBalance.ID != userBalance.ID)
            {
                result.Message = "Kullanıcının bakiye hareketlerinde anomali (aykırılık) bulunuyor. Lütfen sorunu çözümleyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userBalance.Amount <= 0)
            {
                result.Message = "Ödenecek tutar sıfır veya sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBalance.Description))
            {
                result.Message = "Açıklama boş bırakılamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBalance.ShownDescription))
            {
                result.Message = "Görünen Açıklama boş bırakılamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Deactivate User Balance
            var activeUserBalanceList = this._iUserService.GetUserBalanceListByUserIDACTIVE(userBalance.UserID);
            if (activeUserBalanceList.Any())
            {
                if (this._iUserService.UpdateUserBalanceDeactive(userBalance.UserID))
                {
                    List<LogTransaction> logList = new List<LogTransaction>();
                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    var lc = new
                    {
                        oldValue = new
                        {
                            IsActive = true
                        },
                        newValue = new
                        {
                            IsActive = false
                        }
                    };

                    string serializedLogContent = jss.Serialize(lc);

                    foreach (var item in activeUserBalanceList)
                    {
                        LogTransaction lt = new LogTransaction()
                        {
                            DatabaseID = Constants.Database.Ramesses.Value,
                            TableID = (int)Constants.Database.Ramesses.Table.UserBalance,
                            RowID = item.ID,
                            ActorID = this.OutAdministratorID,
                            TKActor = Settings.TKActor,
                            TKTransactionType = TK.TKTransactionType.Update,
                            LogContent = serializedLogContent,
                            Description = "Bakiye pasife alındı (Kullanıcı Ödemesi)",
                            CreatedAt = DateTime.UtcNow,
                            IPAddress = IPProvider.GetIPAddress(HttpContext)
                        };

                        logList.Add(lt);
                    }

                    int rows = base.LogTransaction(logList);
                }
            }
            #endregion

            userBalance.PreviousBalance = activeUserBalance.CurrentBalance;
            userBalance.CurrentBalance = userBalance.PreviousBalance - userBalance.Amount;
            userBalance.Amount = userBalance.Amount * (-1);
            userBalance.TKBalanceAction = TK.TKBalanceAction.Payment;
            userBalance.IsShown = true;
            userBalance.IsActive = true;
            userBalance.IsDeleted = false;
            userBalance.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            userBalance.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iUserService.SaveUserBalance(userBalance, out id))
            {
                result.Message = "Bakiye kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            userBalance.ID = id;

            #region LogTransaction
            var logContent = userBalance;
            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBalance,
                userBalance.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Bakiye kaydedildi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Kullanıcı ödemesi başarıyla gerçekleşti.";
            result.Data = new
            {
                UserBalanceID = userBalanceItem.ID
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.KullaniciBakiyeDetayiniGuncelleme, true)]
        public JsonResult BalanceUpdateAjax(UserBalance userBalance)
        {
            Result result = new Result();

            var userBalanceItem = this._iUserService.GetUserBalanceByIDMEDIUM(userBalance.ID);
            if (userBalanceItem == null)
            {
                result.Message = "Bakiye işlem kaydı bulunamadı. Bakiye işlem kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBalance.Description))
            {
                result.Message = "Açıklama bölümünü giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBalance.ShownDescription))
            {
                result.Message = "Görünen açıklama bölümünü giriniz";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userBalance.IsDeleted = !userBalance.IsDeleted;

            if (userBalance.Description == userBalanceItem.Description &&
                userBalance.ShownDescription == userBalanceItem.ShownDescription &&
                userBalance.IsShown == userBalanceItem.IsShown &&
                userBalance.IsActive == userBalanceItem.IsActive &&
                userBalance.IsDeleted == userBalanceItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserBalance(userBalance))
            {
                result.Message = "Bakiye işlem kaydı güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    Description = userBalanceItem.Description,
                    ShownDescription = userBalanceItem.ShownDescription,
                    IsShown = userBalanceItem.IsShown,
                    IsActive = userBalanceItem.IsActive,
                    IsDeleted = userBalanceItem.IsDeleted
                },
                newValue = new
                {
                    Description = userBalance.Description,
                    ShownDescription = userBalance.ShownDescription,
                    IsShown = userBalance.IsShown,
                    IsActive = userBalance.IsActive,
                    IsDeleted = userBalance.IsDeleted
                }
            };

            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBalance,
                userBalance.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Bakiye işlem kaydı güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Bakiye işlem kaydı başarıyla güncellendi";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.YeniBakiyeKaydiEkleme)]
        public JsonResult UserBalanceSaveAjax(UserBalance userBalance)
        {
            Result result = new Result();

            CultureInfo EnglishCultureInfo = new CultureInfo("en-US");
            userBalance.CurrentBalance = Convert.ToDecimal(Request.Form["CurrentBalance"], EnglishCultureInfo);

            if (userBalance.TKBalanceAction == TK.TKBalanceAction.Undefined)
            {
                result.Message = "Tanımlanamayan işlem türü algılandı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userBalance.CurrentBalance < 0)
            {
                result.Message = "Mevcut bakiye sıfırdan düşük olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBalance.Description))
            {
                result.Message = "Açıklama boş bırakılamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userBalance.ShownDescription))
            {
                result.Message = "Görünen Açıklama boş bırakılamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var userItem = this._iUserService.GetUserByIDMEDIUM(userBalance.UserID);
            if (userItem == null)
            {
                result.Message = "Kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var activeUserBalance = this._iUserService.GetUserBalanceByUserIDACTIVE(userBalance.UserID);
            if (activeUserBalance != null)
            {
                if (activeUserBalance.ID != userBalance.ID)
                {
                    result.Message = "İşlem yaptığınız sayfa güncel olmayan kullanıcı bakiye verilerini içeriyor. Lütfen sayfayı yenileyip tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (activeUserBalance.CurrentBalance != userBalance.CurrentBalance - userBalance.Amount)
                {
                    //result.Message = "Bakiye kaydı sistem üzerindeki kayıtla eşleşmiyor. Lütfen sayfayı yenileyip tekrar deneyiniz=>" + userBalance.CurrentBalance + "-" + userBalance.Amount;

                    

                    result.Message = "RF: " + Request.Form["CurrentBalance"] + " RF: " + Request.Form["Amount"] + " <=> " + userBalance.CurrentBalance + "-" + userBalance.Amount;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                userBalance.ID = 0;
                userBalance.PreviousBalance = activeUserBalance.CurrentBalance;
            }

            #region Deactivate User Balance
            var activeUserBalanceList = this._iUserService.GetUserBalanceListByUserIDACTIVE(userBalance.UserID);
            if (activeUserBalanceList.Any())
            {
                if (this._iUserService.UpdateUserBalanceDeactive(userBalance.UserID))
                {
                    List<LogTransaction> logList = new List<LogTransaction>();
                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    var lc = new
                    {
                        oldValue = new
                        {
                            IsActive = true
                        },
                        newValue = new
                        {
                            IsActive = false
                        }
                    };

                    string serializedLogContent = jss.Serialize(lc);

                    foreach (var item in activeUserBalanceList)
                    {
                        LogTransaction lt = new LogTransaction()
                        {
                            DatabaseID = Constants.Database.Ramesses.Value,
                            TableID = (int)Constants.Database.Ramesses.Table.UserBalance,
                            RowID = item.ID,
                            ActorID = this.OutAdministratorID,
                            TKActor = TK.TKActor.Administrator,
                            TKTransactionType = TK.TKTransactionType.Update,
                            LogContent = serializedLogContent,
                            Description = "Bakiye pasife alındı (Manuel Kayıt)",
                            CreatedAt = DateTime.UtcNow,
                            IPAddress = IPProvider.GetIPAddress(HttpContext)
                        };

                        logList.Add(lt);
                    }

                    int rows = base.LogTransaction(logList);
                }
            }
            #endregion

            userBalance.IsShown = true;
            userBalance.IsActive = true;
            userBalance.IsDeleted = false;
            userBalance.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            userBalance.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iUserService.SaveUserBalance(userBalance, out id))
            {
                result.Message = "Bakiye kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            userBalance.ID = id;

            #region LogTransaction
            var logContent = userBalance;
            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserBalance,
                userBalance.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Bakiye kaydedildi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Bakiye başarıyla kaydedildi.";
            result.Data = userBalance.UserID;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region UserTeam
        [PermissionFilter(PermissionNav.TakimYonetimi)]
        public ActionResult TeamManager()
        {
            var userList = this._iUserService.GetUserListSMALL();
            var userTeamList = this._iUserService.GetUserTeamList();

            foreach (var item in userTeamList)
            {
                var tempUser = (userList.Where(u => u.ID == item.LeaderUserID).FirstOrDefault() ?? new User());
                item.LeaderUserFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
            }

            var model = new UserTeamListModel()
            {
                UserTeamList = userTeamList,
                UserList = userList
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.TakimOzeti)]
        public ActionResult TeamDashboard(int? ID, bool? IncludeLeader, string Start, string End)
        {
            var model = new UserTeamDashboardModel();

            DateTime start, end;
            DateTime? nStart = null, nEnd = null;
            if(!string.IsNullOrEmpty(Start) && !string.IsNullOrEmpty(End))
            {
                ValidationUtility.StringToDatetime(Start, "dd/MM/yyyy", out nStart);
                ValidationUtility.StringToDatetime(End, "dd/MM/yyyy", out nEnd);
            }

            if (!nStart.HasValue && !nEnd.HasValue)
            {
                DateTime tempStart = DateTime.UtcNow;
                tempStart = tempStart.AddDays(-(int)tempStart.DayOfWeek + 1);

                start = new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
                end = start.AddDays(7);
            }
            else
            {
                start = nStart.Value;
                end = nEnd.Value;
            }

            IncludeLeader = IncludeLeader.HasValue && IncludeLeader.Value;

            var userTeamList = this._iUserService.GetUserTeamList();
            UserTeam selectedUserTeam = null;
            List<User> userList = new List<User>();

            if (ID.HasValue && ID.Value != 0)
            {
                selectedUserTeam = userTeamList.Where(u => u.ID == ID.Value).FirstOrDefault();
                if (selectedUserTeam == null)
                {
                    throw new Exception();
                }

                userList = this._iUserService.GetUserListByUserTeamIDMEDIUM(ID.Value);
                if (!IncludeLeader.Value)
                {
                    userList = userList.Where(u => u.ID != selectedUserTeam.LeaderUserID).ToList();
                }
                
                int []userIDArray = userList.Select(u => u.ID).ToArray();
                var paymentList = this._iPaymentService.GetPaymentListByActorIDList(TK.TKActor.User, userIDArray, start, end);
                var confirmedPaymentList = paymentList.Where(p => p.IsFinancialConfirmed && !p.IsCancelled).ToList();
                var customerList = this._iCustomerService.GetCustomerListByActorIDList(TK.TKActor.User, userIDArray, start, end);

                model.TotalUserTeamBalance = this._iUserService.GetUserBalanceByUserIDListACTIVE(userIDArray);
                model.SelectedUserTeamID = ID.Value;
                model.SelectedUserTeam = selectedUserTeam;
                model.IsUserTeamSelected = true;
                model.PaymentList = paymentList;
                model.ConfirmedPaymentList = confirmedPaymentList;
                model.CustomerList = customerList;
            }

            model.UserTeamList = userTeamList;
            model.UserList = userList;
            model.Start = start;
            model.End = end;
            model.IncludeLeader = IncludeLeader.Value;

            ViewBag.IsMenuClosed = true;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.TakimEkleme)]
        public JsonResult SaveUserTeamAjax(UserTeam userTeam)
        {
            Result result = new Result();

            if (string.IsNullOrEmpty(userTeam.Name))
            {
                result.Message = "Takım adı giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userTeam.LeaderUserID == 0)
            {
                result.Message = "Takım lideri seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userTeam.CommissionRate < 0)
            {
                result.Message = "Takım liderinin komisyon oranı sıfırın altında olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userTeam.ColorHex))
            {
                result.Message = "Takım rengi seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            User userItem = this._iUserService.GetUserByIDMEDIUM(userTeam.LeaderUserID);

            if (userItem == null)
            {
                result.Message = "Takım lideri olarak seçtiğiniz kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userItem.IsActive)
            {
                result.Message = "Seçilen kullanıcı pasif olduğu için lider olarak seçilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userItem.IsDeleted)
            {
                result.Message = "Seçilen kullanıcının silindi özelliği evet olduğu için lider olarak seçilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            userTeam.IsDeleted = false;
            userTeam.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iUserService.SaveUserTeam(userTeam, out id))
            {
                result.Message = "Takım kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            userTeam.ID = id;

            #region LogTransaction
            var logContent = userTeam;

            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserTeam,
                userTeam.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Takım kaydedildi"
                );
            #endregion

            result.Message = "Takım başarıyla kaydedildi.";
            result.IsSuccess = true;
            result.Data = userTeam;
            result.ExtraData = new
            {
                LeaderUserFullName = string.Format("{0} {1}", userItem.FirstName, userItem.LastName),
                CommissionRate = userTeam.CommissionRate,
                CreatedAtStr = userTeam.CreatedAt.ToString("yyyy.MM.dd HH:mm:ss")
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.TakimDuzenleme)]
        public JsonResult UpdateUserTeamAjax(UserTeam userTeam)
        {
            Result result = new Result();

            var userTeamItem = this._iUserService.GetUserTeamByID(userTeam.ID);
            if (userTeamItem == null)
            {
                result.Message = "Takım bulunamadı. Takımın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userTeam.Name))
            {
                result.Message = "Takım adı giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userTeam.LeaderUserID == 0)
            {
                result.Message = "Takım lideri seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userTeam.CommissionRate < 0)
            {
                result.Message = "Takım liderinin komisyon oranı sıfırın altında olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(userTeam.ColorHex))
            {
                result.Message = "Takım rengi seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userTeam.Name == userTeamItem.Name &&
                userTeam.LeaderUserID == userTeamItem.LeaderUserID &&
                userTeam.CommissionRate == userTeamItem.CommissionRate &&
                userTeam.ColorHex == userTeamItem.ColorHex)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            User userItem = this._iUserService.GetUserByIDMEDIUM(userTeam.LeaderUserID);
            if (userItem == null)
            {
                result.Message = "Takım lideri olarak seçtiğiniz kullanıcı bulunamadı. Kullanıcının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userItem.IsActive)
            {
                result.Message = "Seçilen kullanıcı pasif olduğu için lider olarak seçilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (userItem.IsDeleted)
            {
                result.Message = "Seçilen kullanıcının silindi özelliği evet olduğu için lider olarak seçilemez.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserTeam(userTeam))
            {
                result.Message = "Takım güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    Name = userTeamItem.Name,
                    LeaderUserID = userTeamItem.LeaderUserID,
                    CommissionRate = userTeamItem.CommissionRate,
                    ColorHex = userTeamItem.ColorHex
                },
                newValue = new
                {
                    Name = userTeam.Name,
                    LeaderUserID = userTeam.LeaderUserID,
                    CommissionRate = userTeam.CommissionRate,
                    ColorHex = userTeam.ColorHex
                }
            };

            base.LogTransaction(
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserTeam,
                userTeam.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Takım güncellendi"
                );
            #endregion

            result.Message = "Takım başarıyla güncellendi.";
            result.IsSuccess = true;
            result.Data = userTeam;
            result.ExtraData = new
            {
                LeaderUserFullName = string.Format("{0} {1}", userItem.FirstName, userItem.LastName),
                CommissionRate = userTeam.CommissionRate
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.TakimSilme)]
        public JsonResult UpdateUserTeamIsDeletedAjax(int ID)
        {
            Result result = new Result();

            var userTeamItem = this._iUserService.GetUserTeamByID(ID);
            if (userTeamItem == null)
            {
                result.Message = "Takım bulunamadı. Takımın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iUserService.UpdateUserTeamIsDeleted(ID, true))
            {
                result.Message = "Takım silinemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
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
                Constants.Database.Ramesses.Value,
                (int)Constants.Database.Ramesses.Table.UserTeam,
                userTeamItem.ID,
                TK.TKTransactionType.Delete,
                new JavaScriptSerializer().Serialize(logContent),
                "Takım silindi"
                );
            #endregion

            result.Message = "Takım başarıyla silindi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region UserNotification
        [PermissionFilter(PermissionNav.BildirimYonetimi)]
        public ActionResult NotificationManager()
        {
            var model = new UserNotificationManagerModel()
            {
                UserTeamList = this._iUserService.GetUserTeamList(),
                UserList = this._iUserService.GetUserListSMALL()
            };

            return View(model);
        }

        [PermissionFilter(PermissionNav.KullanicininBildirimleriniGoruntuleme)]
        public ActionResult NotificationList(int ID)
        {
            var userItem = this._iUserService.GetUserByIDMEDIUM(ID);
            if (userItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var userNotificationList = this._iUserService.GetUserNotificationListByUserIDMEDIUM(ID);
            var administratorList = this._iAdministratorService.GetAdministratorListByIDList(
                userNotificationList.Where(un => un.AdministratorID.HasValue).Select(un => un.AdministratorID.Value).ToArray()
                );

            foreach (var item in userNotificationList)
            {
                if (item.AdministratorID.HasValue)
                {
                    var tempAdministrator = administratorList.Where(a => a.ID == item.AdministratorID.Value).FirstOrDefault() ?? new Administrator();
                    item.AdministratorFullName = string.Format("{0} {1}", tempAdministrator.FirstName, tempAdministrator.LastName);
                }
                else
                {
                    item.AdministratorFullName = "Sistem";
                }
            }

            var model = new UserNotificationListModel()
            {
                UserNotificationList = userNotificationList,
                User = userItem
            };

            ViewBag.IsMenuClosed = true;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.BildirimYonetimi, true)]
        public JsonResult SendNotificationAjax(int ReceiverType, int[] UserID, int[] UserTeamID, string Title, string NotificationContent, bool SendNotification, bool SendEmail, bool SendSms)
        {
            Result result = new Result();

            List<User> userList = new List<User>();

            if (string.IsNullOrEmpty(Title))
            {
                result.Message = "Başlık giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(NotificationContent))
            {
                result.Message = "İçerik giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            //  1 => User
            if (ReceiverType == 1)
            {
                if (UserID == null || !UserID.Any())
                {
                    result.Message = "Kullanıcı seçiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                userList = this._iUserService.GetUserListByIDList(UserID);
            }
            //  2 => UserTeam
            else if (ReceiverType == 2)
            {
                if (UserTeamID == null || !UserTeamID.Any())
                {
                    result.Message = "Takım seçiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                foreach (var item in UserTeamID)
                {
                    userList.AddRange(this._iUserService.GetUserListByUserTeamIDSMALL(item));
                }
            }
            else
            {
                result.Message = "Alıcı Seçimi Yapınız";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!userList.Any())
            {
                result.Message = "Herhangi bir alıcı bulunamadı.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            int notificationSuccessCounter = 0;
            int smsSuccessCounter = 0;
            int emailSuccessCounter = 0;
            string processIPAddress = IPProvider.GetIPAddress(this.HttpContext);

            if (SendNotification)
            {
                foreach (var item in userList)
                {
                    int unID;
                    var userNotification = new UserNotification()
                    {
                        UserID = item.ID,
                        Title = Title,
                        NotificationContent = NotificationContent,
                        TKNotificationType = TK.TKNotificationType.AdministratorMessage,
                        AdministratorID = this.OutAdministratorID,
                        IsRead = false,
                        IsShown = true,
                        IsActive = true,
                        IsDeleted = false,
                        IPAddress = processIPAddress,
                        CreatedAt = DateTime.UtcNow
                    };

                    if (this._iUserService.SaveUserNotification(userNotification, out unID))
                    {
                        notificationSuccessCounter++;
                    }
                }
            }

            if (SendSms)
            {
                var phoneNumberList = userList.Where(u => !string.IsNullOrEmpty(u.PhoneNumber)).Select(u => u.PhoneNumber).ToList();
                var smsResult = this._morsOperations.SendSmsNotificationAjaxSms(phoneNumberList, NotificationContent);

                smsSuccessCounter = smsResult.IsSuccess ? userList.Count : 0;
            }

            if (SendEmail)
            {
                foreach (var item in userList)
                {
                    var emailResult = this._morsOperations.SendEmailNotificationAjaxEmail(
                        string.Format("{0} {1}", item.FirstName, item.LastName),
                        item.Email,
                        NotificationContent,
                        Title
                        );

                    if (emailResult.IsSuccess)
                        emailSuccessCounter++;
                }
            }

            result.IsSuccess = true;
            result.Message = string.Format("{0} adet kullanıcıya bildirim gönderilmiştir.", notificationSuccessCounter);
            result.Data = new
            {
                ReceiverCount = userList.Count,
                SuccessfulSendingNotificationCount = notificationSuccessCounter,
                SuccessfulSendingSmsCount = smsSuccessCounter,
                SuccessfulSendingEmailCount = emailSuccessCounter
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}