using Sky.CDN.IServices;
using Sky.CMS.IServices;
using Sky.Common.DTO;
using Sky.Common.Provider;
using Sky.Common.Utilities;
using Sky.Core.DTO;
using Sky.Core.IServices;
using Sky.Log.IServices;
using Sky.Ramesses.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Web.Infrastructure;
using UI.Web.Models.Customer;
using Sky.Ramesses.DTO;
using Sky.SuperUser.IServices;
using Sky.CDN.DTO;
using System.Configuration;

using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;
using PermissionAct = Sky.Common.DTO.TK.Project.Studio.TKPermissionAction;
using Sky.CMS.Services;
using Sky.CMS.DTO;

namespace UI.Web.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _iCustomerService;
        private readonly IUserService _iUserService;
        private readonly IPaymentService _iPaymentService;
        private readonly IGeneralContentService _iGeneralContentService;
        private readonly ITypeService _iTypeService;
        private readonly ICDNService _iCDNService;
        private readonly IRaffleService _iRaffleService;
        private readonly IAdministratorService _iAdministratorService;
        private readonly MorsOperations _morsOperations;

        public CustomerController(
            Sky.SuperUser.IServices.ISessionService iSessionService,
            IUserService iUserService,
            ILogService iLogService,
            ICustomerService iCustomerService,
            IPaymentService iPaymentService,
            IGeneralContentService iGeneralContentService,
            ITypeService iTypeService,
            ICDNService iCDNService,
            IRaffleService iRaffleService,
            MorsOperations morsOperations,
            IAdministratorService iAdministratorService)
            : base(iSessionService, iLogService)
        {
            this._iCustomerService = iCustomerService;
            this._iUserService = iUserService;
            this._iPaymentService = iPaymentService;
            this._iGeneralContentService = iGeneralContentService;
            this._iTypeService = iTypeService;
            this._iCDNService = iCDNService;
            this._iRaffleService = iRaffleService;
            this._morsOperations = morsOperations;
            this._iAdministratorService = iAdministratorService;
        }

        #region Customer
        [PermissionFilter(PermissionNav.MusteriListesiniGoruntuleme)]
        public ActionResult List(string q, int? Limit)
        {
            var model = new CustomerListModel();

            if (!Limit.HasValue)
            {
                Limit = 200;
            }
            
            model.Limit = Limit.Value;

            List<Customer> customerList = null;

            if (!string.IsNullOrEmpty(q))
            {
                customerList = this._iCustomerService.GetCustomerListByEmail(q.Trim());
                model.Query = q.Trim();
                model.Limit = Limit.Value;
            }
            else
            {
                customerList = this._iCustomerService.GetCustomerListLIMIT(Limit.Value);
            }

            var countryList = this._iGeneralContentService.GetCountryList();
            foreach (var item in customerList)
            {
                var tempCountryItem = countryList.Where(c => c.ID == item.CountryID).FirstOrDefault() ?? new Sky.CMS.DTO.Country();

                item.CountryPhoneCode = tempCountryItem.PhoneCode;
                item.CountryName = tempCountryItem.Name;
            }

            model.CustomerList = customerList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.MusteriDetayiniGoruntuleme)]
        public ActionResult Detail(int ID)
        {
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(ID);
            if (customerItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var model = new CustomerModel();

            model.Customer = customerItem;
            model.CustomerProfile = this._iCustomerService.GetCustomerProfileByID(ID) ?? new CustomerProfile();
            model.CustomerRegisterDetail = this._iCustomerService.GetCustomerRegisterDetailByID(ID) ?? new CustomerRegisterDetail();
            model.CustomerBillInfoList = this._iCustomerService.GetCustomerBillInfoListByCustomerIDLARGE(ID);
            model.CustomerBillList = this._iCustomerService.GetCustomerBillListByCustomerIDMEDIUM(ID);
            model.CreatorUser = this._iUserService.GetUserByIDMEDIUM(model.CustomerRegisterDetail.ActorID) ?? new User();

            var raffleParticipantList = this._iRaffleService.GetRaffleParticipantListByCustomerIDLARGE(ID);
            var raffleTemplateList = this._iRaffleService.GetRaffleTemplateListLARGE();
            var raffleList = this._iRaffleService.GetRaffleListByIDList(
                raffleParticipantList.Select(rp => rp.RaffleID).Distinct().ToArray()
                );

            var socialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByCustomerIDLARGE(ID);
            var paymentList = this._iPaymentService.GetPaymentListByCustomerIDLARGE(ID);
            var paymentStatusRelationList = this._iPaymentService.GetPaymentStatusRelationListByPaymentIDList(
                paymentList.Select(p => p.ID).ToArray()
                );

            var countryList = this._iGeneralContentService.GetCountryList();

            var typeValueList = this._iTypeService.GetTypeValueListByTypeKeyTagList(
                    KeyTagProvider.Keys["Customer/Detail"].ToArray()
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

            foreach (var item in paymentList)
            {
                item.PaymentStatusRelationList = paymentStatusRelationList.Where(psr => psr.PaymentID == item.ID).ToList();
                item.RaffleTemplate = raffleTemplateList.Where(rt => rt.ID == item.RaffleTemplateID).FirstOrDefault() ?? new RaffleTemplate();
                item.SocialMediaAccount = socialMediaAccountList.Where(sma => sma.ID == item.SocialMediaAccountID).FirstOrDefault() ?? new SocialMediaAccount();
            }

            model.SocialMediaAccountList = socialMediaAccountList;
            model.RaffleParticipantList = raffleParticipantList;
            model.PaymentList = paymentList;
            model.CityList = this._iGeneralContentService.GetCityList();
            model.CountryList = countryList;
            model.TKJobTitleList =
                typeValueList.Where(tv => tv.TypeKeyID == (int)KeyTagProvider.TKList.TKJobTitle).ToList();
            model.TKIndustryList =
                typeValueList.Where(tv => tv.TypeKeyID == (int)KeyTagProvider.TKList.TKIndustry).ToList();
            model.CityList = this._iGeneralContentService.GetCityList();

            ViewBag.IsMenuClosed = true;
            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.MusteriNotlariniGoruntuleme)]
        public ActionResult NoteList(int ID)
        {
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(ID);
            if (customerItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            var customerNoteList = this._iCustomerService.GetCustomerNoteListByCustomerID(ID);
            int[] administratorIDArray = customerNoteList.Where(cn => cn.TKActor == TK.TKActor.Administrator).Select(cn => cn.ActorID).ToArray();
            int[] userIDArray = customerNoteList.Where(cn => cn.TKActor == TK.TKActor.User).Select(cn => cn.ActorID).ToArray();

            var administratorList = this._iAdministratorService.GetAdministratorListByIDList(administratorIDArray);
            var userList = this._iUserService.GetUserListByIDList(userIDArray);

            foreach (var item in customerNoteList)
            {
                if (item.TKActor == TK.TKActor.Administrator)
                {
                    var tempAdministrator = administratorList.Where(adm => adm.ID == item.ActorID).FirstOrDefault();
                    if (tempAdministrator == null)
                    {
                        continue;
                    }

                    item.ActorFullName = string.Format("{0} {1}", tempAdministrator.FirstName, tempAdministrator.LastName);
                }
                else if (item.TKActor == TK.TKActor.User)
                {
                    var tempUser = userList.Where(adm => adm.ID == item.ActorID).FirstOrDefault();
                    if (tempUser == null)
                    {
                        continue;
                    }

                    item.ActorFullName = string.Format("{0} {1}", tempUser.FirstName, tempUser.LastName);
                }
            }

            var model = new CustomerNoteListModel()
            {
                Customer = customerItem,
                CustomerNoteList = customerNoteList
            };

            return View(model);
        }

        #region Ajax
        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriyeNotEkleme)]
        public JsonResult NoteSaveAjax(CustomerNote customerNote)
        {
            Result result = new Result();

            if (string.IsNullOrEmpty(customerNote.NoteContent))
            {
                result.Message = "Not giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customerNote.CustomerID);
            if (customerItem == null)
            {
                result.Message = "Müşteri bulunamadı. Müşterinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            customerNote.CustomerID = customerNote.CustomerID;
            customerNote.TKActor = Settings.TKActor;
            customerNote.ActorID = this.OutAdministratorID;
            customerNote.IsDeleted = false;
            customerNote.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            customerNote.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iCustomerService.SaveCustomerNote(customerNote, out id))
            {
                result.Message = "Not kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            customerNote.ID = id;

            #region LogTransaction
            var logContent = customerNote;

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerNote,
                customerNote.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Müşteri Notu kaydedildi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Not başarıyla kaydedildi";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriKisiselBilgileriGuncelleme)]
        public JsonResult UpdateCustomerPersonalInfoAjax(Customer customer)
        {
            Result result = new Result();

            #region Validation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customer.ID);
            if (customerItem == null)
            {
                result.Message = "Müşteri bulunamadı. Müşterinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(customer.FirstName))
            {
                result.Message = "Müşteri adını giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(customer.LastName))
            {
                result.Message = "Müşteri soyadını giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (customer.CountryID == 0)
            {
                result.Message = "Ülke seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(customer.PhoneNumber))
            {
                result.Message = "Telefon numarası giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            customer.IsDeleted = !customer.IsDeleted;

            if (customer.FirstName == customerItem.FirstName &&
                customer.LastName == customerItem.LastName &&
                customer.PhoneNumber == customerItem.PhoneNumber &&
                customer.IsActive == customerItem.IsActive &&
                customer.IsDeleted == customerItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iCustomerService.UpdateCustomerCoreADMIN(customer))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var logContent = new
            {
                oldValue = new
                {
                    FirstName = customerItem.FirstName,
                    LastName = customerItem.LastName,
                    CountryID = customerItem.CountryID,
                    PhoneNumber = customerItem.PhoneNumber,
                    IsActive = customerItem.IsActive,
                    IsDeleted = customerItem.IsDeleted
                },
                newValue = new
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    CountryID = customer.CountryID,
                    PhoneNumber = customer.PhoneNumber,
                    IsActive = customer.IsActive,
                    IsDeleted = customer.IsDeleted
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Customer,
                customer.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Müşteri Kişisel Bilgileri Güncellendi"
                );

            result.Message = "Müşteri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriGuvenlikBilgileriniGuncelleme)]
        public JsonResult UpdateCustomerSecuritySettingsAjax(Customer customer)
        {
            Result result = new Result();

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customer.ID);
            if (customerItem == null)
            {
                result.Message = "Müşteri bulunamadı. Müşterinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(customer.Email))
            {
                result.Message = "Müşteri email adresini giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (customer.Email == customerItem.Email)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var anyCustomer = this._iCustomerService.GetCustomerByEmail(customer.Email);
            if (anyCustomer != null && anyCustomer.ID != customer.ID)
            {
                result.Message = string.Format("{0} Email adresi daha önce {1} numaralı müşteri için kullanılmıştır. İşleminiz tamamlanamadı.", customer.Email, anyCustomer.ID);
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iCustomerService.UpdateCustomerEmail(customer))
            {
                result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var logContent = new
            {
                oldValue = new
                {
                    Email = customerItem.Email
                },
                newValue = new
                {
                    Email = customer.Email
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Customer,
                customer.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Müşteri Email Adresi Güncellendi"
                );

            result.Message = "Müşteri email adresi güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriEkBilgileriniGuncelleme)]
        public JsonResult UpdateCustomerProfileAjax(CustomerProfile customerProfile, string BirthdateStr)
        {
            Result result = new Result();

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customerProfile.ID);
            if (customerItem == null)
            {
                result.Message = "Müşteri bulunamadı. Müşterinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            DateTime? dt = null;
            ValidationUtility.StringToDatetime(BirthdateStr, "dd/MM/yyyy", out dt);
            customerProfile.Birthdate = dt;

            var tempCustomerProfile = this._iCustomerService.GetCustomerProfileByID(customerProfile.ID);
            if (tempCustomerProfile != null)
            {
                if (customerProfile.Gender == tempCustomerProfile.Gender &&
                    customerProfile.Birthdate == tempCustomerProfile.Birthdate &&
                    customerProfile.CityID == tempCustomerProfile.CityID &&
                    customerProfile.TKJobTitle == tempCustomerProfile.TKJobTitle &&
                    customerProfile.OtherJobTitle == tempCustomerProfile.OtherJobTitle &&
                    customerProfile.CompanyTitle == tempCustomerProfile.CompanyTitle &&
                    customerProfile.TKIndustry == tempCustomerProfile.TKIndustry &&
                    customerProfile.OtherIndustry == tempCustomerProfile.OtherIndustry)
                {
                    result.NoChanges();
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                if (!this._iCustomerService.UpdateCustomerProfile(customerProfile))
                {
                    result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = new
                {
                    oldValue = new
                    {
                        Gender = tempCustomerProfile.Gender,
                        Birthdate = tempCustomerProfile.Birthdate,
                        CityID = tempCustomerProfile.CityID,
                        TKJobTitle = tempCustomerProfile.TKJobTitle,
                        OtherJobTitle = tempCustomerProfile.OtherJobTitle,
                        CompanyTitle = tempCustomerProfile.CompanyTitle,
                        TKIndustry = tempCustomerProfile.TKIndustry,
                        OtherIndustry = tempCustomerProfile.OtherIndustry
                    },
                    newValue = new
                    {
                        Gender = customerProfile.Gender,
                        Birthdate = customerProfile.Birthdate,
                        CityID = customerProfile.CityID,
                        TKJobTitle = customerProfile.TKJobTitle,
                        OtherJobTitle = customerProfile.OtherJobTitle,
                        CompanyTitle = customerProfile.CompanyTitle,
                        TKIndustry = customerProfile.TKIndustry,
                        OtherIndustry = customerProfile.OtherIndustry
                    }
                };

                base.LogTransaction(
                    Constants.Database.Core.Value,
                    (int)Constants.Database.Core.Table.CustomerProfile,
                    tempCustomerProfile.ID,
                    TK.TKTransactionType.Update,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Müşteri Ek Bilgileri güncellendi"
                    );
                #endregion
            }
            else
            {
                customerProfile.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                customerProfile.CreatedAt = DateTime.UtcNow;

                if (!this._iCustomerService.SaveCustomerProfile(customerProfile))
                {
                    result.Message = "İşleminiz gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                #region LogTransaction
                var logContent = customerProfile;

                base.LogTransaction(
                    Constants.Database.Core.Value,
                    (int)Constants.Database.Core.Table.CustomerProfile,
                    customerProfile.ID,
                    TK.TKTransactionType.Save,
                    new JavaScriptSerializer().Serialize(logContent),
                    "Müşteri Ek Bilgileri kaydedildi"
                    );
                #endregion
            }

            result.Message = "Müşteri ek bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
        #endregion

        #region CustomerBill
        [PermissionFilter(PermissionNav.MusteriFaturaListesiniGoruntuleme)]
        public ActionResult BillList()
        {
            var model = new CustomerBillListModel();

            var customerBillList = this._iCustomerService.GetCustomerBillList();

            var customerList = this._iCustomerService.GetCustomerListByIDList(
                customerBillList
                .Where(b => b.CustomerID.HasValue)
                .Select(b => b.CustomerID.Value)
                .ToArray()
                );

            foreach (var item in customerBillList)
            {
                if (item.CustomerID.HasValue)
                {
                    var tempCustomer = customerList.Where(u => u.ID == item.CustomerID.Value).FirstOrDefault() ?? new Customer();
                    item.CustomerFullName = string.Format("{0} {1}", tempCustomer.FirstName, tempCustomer.LastName);
                }
            }

            model.CustomerBillList = customerBillList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.MusteriFaturasiEkleme)]
        public ActionResult BillSave(int? CustomerID)
        {
            var model = new CustomerBillModel();

            Customer customerItem = null;
            if (CustomerID.HasValue && CustomerID.Value != 0)
            {
                customerItem = this._iCustomerService.GetCustomerByIDLARGE(CustomerID.Value);
                if (customerItem == null)
                {
                    return RedirectToAction("Oops", "Error");
                }

                model.Customer = customerItem;
                model.IsCustomerIDSupplied = true;
            }
            else
            {
                model.Customer = new Customer();
            }

            if (CustomerID.HasValue)
            {
                model.CustomerBillInfoList = this._iCustomerService.GetCustomerBillInfoListByCustomerIDMEDIUM(CustomerID.Value);
            }
            else
            {
                model.CustomerBillInfoList = new List<CustomerBillInfo>();
            }

            return View(model);
        }

        [PermissionFilter(PermissionNav.MusteriFaturaDetayiniGoruntuleme)]
        public ActionResult BillUpdate(int ID)
        {
            var model = new CustomerBillModel();

            var customerBillItem = this._iCustomerService.GetCustomerBillByIDMEDIUM(ID);

            if (customerBillItem == null)
            {
                return RedirectToAction("Opps", "Error");
            }

            if (customerBillItem.CustomerID.HasValue && customerBillItem.CustomerID.Value != 0)
            {
                var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customerBillItem.CustomerID.Value);
                if (customerItem == null)
                {
                    return RedirectToAction("Oops", "Error");
                }

                var countryItem = this._iGeneralContentService.GetCountryByID(customerItem.CountryID) ?? new Country();
                customerItem.CountryPhoneCode = countryItem.PhoneCode;
                customerItem.CountryName = countryItem.Name;

                model.Customer = customerItem;
                model.IsCustomerIDSupplied = true;
            }
            else
            {
                model.Customer = new Customer();
            }

            model.CustomerBill = customerBillItem;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.MusteriFaturasiEkleme, true)]
        public JsonResult BillSaveAjax(CustomerBill CustomerBill, string StringEditedAt)
        {
            Result result = new Result();

            HttpPostedFileBase httpPostedFile = Request.Files["URL"];

            #region Nullable Customer
            int? CustomerID = null;
            Customer customerItem = null;

            if (CustomerBill.CustomerID.HasValue)
            {
                customerItem = this._iCustomerService.GetCustomerByIDLARGE(CustomerBill.CustomerID.Value);
                if (customerItem == null)
                {
                    result.Message = "Müşteri bulunamadı. Müşterinin daha önce silinmediğinden emin olun.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                CustomerID = customerItem.ID;
            }
            #endregion

            #region Validation
            if (httpPostedFile == null || httpPostedFile.ContentLength == 0)
            {
                result.Message = "Fatura dosyası seçilmedi. Dosya seçip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var duplicatedCustomerBillItem = this._iCustomerService.GetCustomerBillByBillNo(CustomerBill.BillNo);

            if (duplicatedCustomerBillItem != null)
            {
                result.Message = string.Format("{0} fatura numarası daha önceden kullanılmıştır. Farklı fatura numarasıyla tekrar deneyiniz.", CustomerBill.BillNo);
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

                CustomerBill.URL = result.Data.ToString();
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

            CustomerBill.UniqueKey = string.Format("{0}{1}", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()).Replace("-", "");
            CustomerBill.EditedAt = editedAt.Value;
            CustomerBill.CreatedAt = DateTime.UtcNow;
            CustomerBill.IsDeleted = !CustomerBill.IsDeleted;
            int id;
            if (!this._iCustomerService.SaveCustomerBill(CustomerBill, out id))
            {
                result.Message = "Fatura bilgileri kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            CustomerBill.ID = id;

            #region Log Transaction
            var logContent = CustomerBill;

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBill,
                CustomerBill.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Fatura kaydedildi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura başarıyla kaydedildi.";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusterininFaturaDosyasiniGuncelleme)]
        public JsonResult BillUpdateURLAjax(int ID)
        {
            Result result = new Result();

            var customerBillItem = this._iCustomerService.GetCustomerBillByIDMEDIUM(ID);

            if (customerBillItem == null)
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

            if (!this._iCustomerService.UpdateCustomerBillURL(customerBillItem.ID, url))
            {
                result.Message = "Fatura dosyası güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var customerbillUrlLog = new
            {
                oldvalue = new
                {
                    URL = customerBillItem.URL
                },
                newValue = new
                {
                    URL = url
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBill,
                customerBillItem.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(customerbillUrlLog),
                "Fatura dosyası güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura dosyası başarıyla güncellendi.";
            result.Data = new
            {
                CustomerBillURL = string.Format("{0}{1}", ConfigurationManager.AppSettings["CDNBaseURL"], url)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusterininFaturaCekirdekBilgileriGuncelleme)]
        public JsonResult BillUpdateCoreAjax(CustomerBill customerBill, string StringEditedAt)
        {
            Result result = new Result();

            var customerBillItem = this._iCustomerService.GetCustomerBillByIDMEDIUM(customerBill.ID);
            if (customerBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var duplicatedItem = this._iCustomerService.GetCustomerBillByBillNo(customerBill.BillNo);
            if (duplicatedItem != null && duplicatedItem.ID != customerBillItem.ID)
            {
                result.Message = string.Format("{0} fatura numarası daha önceden kullanılmıştır. Lütfen başka fatura numarasıyla tekrar deneyiniz.", customerBill.BillNo);
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
                    .AddHours(customerBillItem.EditedAt.Hour)
                    .AddMinutes(customerBillItem.EditedAt.Minute)
                    .AddSeconds(customerBillItem.EditedAt.Second)
                    .AddMilliseconds(customerBillItem.EditedAt.Millisecond);

            customerBill.EditedAt = editedAt.Value;
            customerBill.IsDeleted = !customerBill.IsDeleted;

            if (customerBill.BillNo == customerBillItem.BillNo &&
                customerBill.EditedAt == customerBillItem.EditedAt &&
                customerBill.IsActive == customerBillItem.IsActive &&
                customerBill.IsDeleted == customerBillItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iCustomerService.UpdateCustomerBillCore(customerBill))
            {
                result.Message = "Fatura güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    BillNo = customerBillItem.BillNo,
                    EditedAt = customerBillItem.EditedAt,
                    IsActive = customerBillItem.IsActive,
                    IsDeleted = customerBillItem.IsDeleted
                },
                newValue = new
                {
                    BillNo = customerBill.BillNo,
                    EditedAt = customerBill.EditedAt,
                    IsActive = customerBill.IsActive,
                    IsDeleted = customerBill.IsDeleted
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBill,
                customerBill.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Faturanın çekirdek bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Çekirdek bilgiler başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusterininFaturaBilgileri)]
        public JsonResult BillUpdateInformationAjax(CustomerBill customerBill)
        {
            Result result = new Result();

            var customerBillItem = this._iCustomerService.GetCustomerBillByIDMEDIUM(customerBill.ID);
            if (customerBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (customerBill.BillIsCorporate == customerBillItem.BillIsCorporate &&
                customerBill.BillFullName == customerBillItem.BillFullName &&
                customerBill.BillCompanyTitle == customerBillItem.BillCompanyTitle &&
                customerBill.BillCitizenIdentityNo == customerBillItem.BillCitizenIdentityNo &&
                customerBill.BillTaxNo == customerBillItem.BillTaxNo &&
                customerBill.BillTaxOffice == customerBillItem.BillTaxOffice &&
                customerBill.BillFullAddress == customerBillItem.BillFullAddress &&
                customerBill.BillPhoneNumber == customerBillItem.BillPhoneNumber)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iCustomerService.UpdateCustomerBillInformation(customerBill))
            {
                result.Message = "Fatura güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    BillIsCorporate = customerBillItem.BillIsCorporate,
                    BillFullName = customerBillItem.BillFullName,
                    BillCompanyTitle = customerBillItem.BillCompanyTitle,
                    BillCitizenIdentityNo = customerBillItem.BillCitizenIdentityNo,
                    BillTaxNo = customerBillItem.BillTaxNo,
                    BillTaxOffice = customerBillItem.BillTaxOffice,
                    BillFullAddress = customerBillItem.BillFullAddress,
                    BillPhoneNumber = customerBillItem.BillPhoneNumber
                },
                newValue = new
                {
                    BillIsCorporate = customerBill.BillIsCorporate,
                    BillFullName = customerBill.BillFullName,
                    BillCompanyTitle = customerBill.BillCompanyTitle,
                    BillCitizenIdentityNo = customerBill.BillCitizenIdentityNo,
                    BillTaxNo = customerBill.BillTaxNo,
                    BillTaxOffice = customerBill.BillTaxOffice,
                    BillFullAddress = customerBill.BillFullAddress,
                    BillPhoneNumber = customerBill.BillPhoneNumber
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBill,
                customerBill.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Fatura müşteri bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura bilgileri başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusterininFaturaFiyatBilgileriniGuncelleme)]
        public JsonResult BillUpdatePriceAjax(CustomerBill customerBill)
        {
            Result result = new Result();

            var customerBillItem = this._iCustomerService.GetCustomerBillByIDMEDIUM(customerBill.ID);
            if (customerBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (customerBill.SubTotalPrice == customerBillItem.SubTotalPrice &&
                customerBill.VATPrice == customerBillItem.VATPrice &&
                customerBill.TotalPrice == customerBillItem.TotalPrice)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iCustomerService.UpdateCustomerBillPrice(customerBill))
            {
                result.Message = "Fatura güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    SubTotalPrice = customerBillItem.SubTotalPrice,
                    VATPrice = customerBillItem.VATPrice,
                    TotalPrice = customerBillItem.TotalPrice
                },
                newValue = new
                {
                    SubTotalPrice = customerBill.SubTotalPrice,
                    VATPrice = customerBill.VATPrice,
                    TotalPrice = customerBill.TotalPrice
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBill,
                customerBill.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Fatura fiyat bilgileri güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Fatura bilgileri başarıyla güncellendi.";
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriyeFaturanizHazirEmailiGonder)]
        public JsonResult SendEmailCustomerBillAjax(int CustomerBillID, string FullName, string Email, bool UseBCC, string BCCEmail)
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
            var customerBillItem = this._iCustomerService.GetCustomerBillByIDSMALL(CustomerBillID);
            if (customerBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın pasif ve/veya silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendEmailCustomerBillAjaxEmail(FullName, customerBillItem, Email, UseBCC, BCCEmail);

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriyeFaturanizHazirSmsiGonder)]
        public JsonResult SendSmsCustomerBillAjax(int CustomerBillID, string FullName, string PhoneNumber)
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
            var customerBillItem = this._iCustomerService.GetCustomerBillByIDSMALL(CustomerBillID);
            if (customerBillItem == null)
            {
                result.Message = "Fatura bulunamadı. Faturanın pasif ve/veya silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result = this._morsOperations.SendSmsCustomerBillAjaxSms(FullName, customerBillItem, PhoneNumber);

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region CustomerBillInfo
        [PermissionFilter(PermissionNav.MusteriFaturaBilgisiEkleme)]
        public ActionResult BillInfoSave(int ID)
        {
            var model = new CustomerBillInfoModel();

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(ID);
            if (customerItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            model.Customer = customerItem;

            return View(model);
        }

        [PermissionFilter(PermissionNav.MusteriFaturaBilgisiGuncelleme)]
        public ActionResult BillInfoUpdate(int ID)
        {
            var customerBillInfoItem = this._iCustomerService.GetCustomerBillInfoByIDLARGE(ID);
            if (customerBillInfoItem == null)
            {
                return RedirectToAction("Opps", "Error");
            }

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customerBillInfoItem.CustomerID);
            if (customerItem == null)
            {
                return RedirectToAction("Opps", "Error");
            }

            var model = new CustomerBillInfoModel();

            model.CustomerBillInfo = customerBillInfoItem;
            model.Customer = customerItem;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.MusteriFaturaBilgisiEkleme, true)]
        public JsonResult BillInfoSaveAjax(CustomerBillInfo customerBillInfo)
        {
            Result result = new Result();

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(customerBillInfo.CustomerID);

            if (customerItem == null)
            {
                result.Message = "Müşteri bulunamadı. Müşterinin daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            customerBillInfo.IsDeleted = !customerBillInfo.IsDeleted;
            customerBillInfo.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            customerBillInfo.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iCustomerService.SaveCustomerBillInfo(customerBillInfo, out id))
            {
                result.Message = "Fatura bilgileri kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            customerBillInfo.ID = id;

            #region Log Transaction
            var logContent = customerBillInfo;

            base.LogTransaction
            (
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBillInfo,
                id,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Müşteri fatura bilgisi kaydedildi"
            );
            #endregion

            result.Message = "Müşteri fatura bilgileri başarıyla kaydedildi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionNav.MusteriFaturaBilgisiGuncelleme, true)]
        public JsonResult BillInfoUpdateAjax(CustomerBillInfo customerBillInfo)
        {
            Result result = new Result();

            var customerBillInfoItem = this._iCustomerService.GetCustomerBillInfoByIDLARGE(customerBillInfo.ID);

            if (customerBillInfoItem == null)
            {
                result.Message = "Fatura bilgisi bulunamadı. Fatura bilgisinin silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            customerBillInfo.IsDeleted = !customerBillInfo.IsDeleted;

            if (customerBillInfo.IsCorporate == customerBillInfoItem.IsCorporate &&
                customerBillInfo.CompanyTitle == customerBillInfoItem.CompanyTitle &&
                customerBillInfo.FullName == customerBillInfoItem.FullName &&
                customerBillInfo.CitizenIdentityNo == customerBillInfoItem.CitizenIdentityNo &&
                customerBillInfo.TaxNo == customerBillInfoItem.TaxNo &&
                customerBillInfo.TaxOffice == customerBillInfoItem.TaxOffice &&
                customerBillInfo.PhoneNumber == customerBillInfoItem.PhoneNumber &&
                customerBillInfo.FullAddress == customerBillInfoItem.FullAddress &&
                customerBillInfo.IsActive == customerBillInfoItem.IsActive &&
                customerBillInfo.IsShown == customerBillInfoItem.IsShown &&
                customerBillInfo.IsDeleted == customerBillInfoItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iCustomerService.UpdateCustomerBillInfo(customerBillInfo))
            {
                result.Message = "Fatura bilgileri güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var orderbillLog = new
            {
                oldValue = new
                {
                    IsCorporate = customerBillInfoItem.IsCorporate,
                    CompanyTitle = customerBillInfoItem.CompanyTitle,
                    FullName = customerBillInfoItem.FullName,
                    CitizenIdentityNo = customerBillInfoItem.CitizenIdentityNo,
                    TaxNo = customerBillInfoItem.TaxNo,
                    TaxOffice = customerBillInfoItem.TaxOffice,
                    PhoneNumber = customerBillInfoItem.PhoneNumber,
                    FullAddress = customerBillInfoItem.FullAddress,
                    IsActive = customerBillInfoItem.IsActive,
                    IsShown = customerBillInfoItem.IsShown,
                    IsDeleted = customerBillInfoItem.IsDeleted
                },
                newValue = new
                {
                    IsCorporate = customerBillInfo.IsCorporate,
                    CompanyTitle = customerBillInfo.CompanyTitle,
                    FullName = customerBillInfo.FullName,
                    CitizenIdentityNo = customerBillInfo.CitizenIdentityNo,
                    TaxNo = customerBillInfo.TaxNo,
                    TaxOffice = customerBillInfo.TaxOffice,
                    PhoneNumber = customerBillInfo.PhoneNumber,
                    FullAddress = customerBillInfo.FullAddress,
                    IsActive = customerBillInfo.IsActive,
                    IsShown = customerBillInfo.IsShown,
                    IsDeleted = customerBillInfo.IsDeleted
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.CustomerBill,
                customerBillInfo.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(orderbillLog),
                "Müşteri fatura bilgisi güncellendi"
                );
            #endregion

            result.Message = "Fatura bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region SocialMediaAccount
        [PermissionFilter(PermissionNav.MusteriSosyalMedyaHesabiEkleme)]
        public ActionResult SocialMediaAccountSave(int ID)
        {
            #region CustomerValidation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(ID);

            if (customerItem == null)
            {
                throw new Exception("");
            }
            #endregion

            var model = new SocialMediaAccountModel()
            {
                Customer = customerItem,
                TKSocialMediaPlatformList = TypeService.GetAll(TK.TKSocialMediaPlatform.Undefined)
            };

            return View(model);
        }

        [PermissionFilter(PermissionNav.MusteriSosyalMedyaHesabiGuncelleme)]
        public ActionResult SocialMediaAccountUpdate(int ID)
        {
            var socialMediaAccountItem = this._iCustomerService.GetSocialMediaAccountByIDLARGE(ID);
            if (socialMediaAccountItem == null)
            {
                throw new Exception("");
            }

            #region CustomerValidation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(socialMediaAccountItem.CustomerID);

            if (customerItem == null)
            {
                throw new Exception("");
            }
            #endregion

            var model = new SocialMediaAccountModel()
            {
                SocialMediaAccount = socialMediaAccountItem,
                Customer = customerItem
            };

            return View(model);
        }

        #region Ajax
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [PermissionFilter(PermissionNav.MusteriSosyalMedyaHesabiEkleme, true)]
        public JsonResult SocialMediaAccountSaveAjax(SocialMediaAccount socialMediaAccount)
        {
            Result result = new Result();

            #region CustomerValidation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(socialMediaAccount.CustomerID);

            if (customerItem == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            if (socialMediaAccount.TKSocialMediaPlatform == TK.TKSocialMediaPlatform.Undefined)
            {
                result.Message = "Sosyal Medya Platformu seçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(socialMediaAccount.AccountName))
            {
                result.Message = "Hesap adını giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (socialMediaAccount.FollowerCountInReg < 0)
            {
                result.Message = "Takipçi sayısı sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string screenShotInReg = null;

            HttpPostedFileBase httpPostedFile = Request.Files["ScreenShotInReg"];
            if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
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

                HttpPostedFileBase filePosted = httpPostedFile;
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
                    inputData.TKWebsite = Settings.TKWebsite;
                    inputData.TKActor = Settings.TKActor;
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

                    screenShotInReg = result.Data.ToString();
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
                result.Message = "İlk kayıt ekran görüntüsünü ekleyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            socialMediaAccount.ScreenShotInReg = screenShotInReg;
            socialMediaAccount.TKActor = Settings.TKActor;
            socialMediaAccount.ActorID = this.OutAdministratorID;
            socialMediaAccount.IsActive = true;
            socialMediaAccount.IsDeleted = false;
            socialMediaAccount.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            socialMediaAccount.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iCustomerService.SaveSocialMediaAccount(socialMediaAccount, out id))
            {
                result.Message = "Sosyal medya hesabı kaydedilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            socialMediaAccount.ID = id;

            #region LogContent
            var logContent = socialMediaAccount;
            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.SocialMediaAccount,
                socialMediaAccount.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Sosyal medya hesabı kaydedildi"
                );
            #endregion

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [PermissionFilter(PermissionNav.MusteriSosyalMedyaHesabiGuncelleme, true)]
        public JsonResult SocialMediaAccountUpdateAjax(SocialMediaAccount socialMediaAccount)
        {
            Result result = new Result();

            #region SocialMediaAccount Validation
            var socialMediaAccountItem = this._iCustomerService.GetSocialMediaAccountByIDLARGE(socialMediaAccount.ID);
            if (socialMediaAccountItem == null)
            {
                result.Message = "Sosyal Medya Hesabı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region CustomerValidation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(socialMediaAccountItem.CustomerID);

            if (customerItem == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            #region Input Validation
            if (string.IsNullOrEmpty(socialMediaAccount.AccountName))
            {
                result.Message = "Hesap adını giriniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (socialMediaAccount.FollowerCountInReg < 0)
            {
                result.Message = "Takipçi sayısı sıfırdan daha az bir değer olamaz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            string screenShotInReg = null;

            HttpPostedFileBase httpPostedFile = Request.Files["ScreenShotInReg"];
            if (httpPostedFile == null || httpPostedFile.ContentLength <= 0)
            {
                if (socialMediaAccount.AccountName == socialMediaAccountItem.AccountName &&
                    socialMediaAccount.FollowerCountInReg == socialMediaAccountItem.FollowerCountInReg)
                {
                    result.NoChanges();
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    screenShotInReg = socialMediaAccountItem.ScreenShotInReg;
                }
            }

            if (httpPostedFile != null && httpPostedFile.ContentLength > 0)
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

                HttpPostedFileBase filePosted = httpPostedFile;
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
                    inputData.TKWebsite = Settings.TKWebsite;
                    inputData.TKActor = Settings.TKActor;
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

                    screenShotInReg = result.Data.ToString();
                }
                else
                {
                    result.Message = "İşleminiz gerçekleştirilemiyor. Hata Kodu: CDN-1004";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
            }

            socialMediaAccount.ScreenShotInReg = screenShotInReg;

            if (!this._iCustomerService.UpdateSocialMediaAccountUSER(socialMediaAccount))
            {
                result.Message = "Sosyal medya hesabı güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogContent
            var logContent = new
            {
                oldValue = new
                {
                    AccountName = socialMediaAccountItem.AccountName,
                    FollowerCountInReg = socialMediaAccountItem.FollowerCountInReg,
                    ScreenShotInReg = socialMediaAccountItem.ScreenShotInReg
                },
                newValue = new
                {
                    AccountName = socialMediaAccount.AccountName,
                    FollowerCountInReg = socialMediaAccount.FollowerCountInReg,
                    ScreenShotInReg = socialMediaAccount.ScreenShotInReg
                }
            };

            base.LogTransaction(
                Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.SocialMediaAccount,
                socialMediaAccount.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Sosyal medya hesabı güncellendi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Sosyal medya hesabı başarıyla güncellendi.";

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MusteriSosyalMedyaHesaplari)]
        public JsonResult GetSocialMediaAccountListAjax(int CustomerID)
        {
            Result result = new Result();

            #region CustomerValidation
            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(CustomerID);

            if (customerItem == null)
            {
                result.Message = "Müşteri kaydı bulunamadı. Kaydın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            result.IsSuccess = true;
            result.Data = new
            {
                SocialMediaAccountList = this._iCustomerService.GetSocialMediaAccountListByCustomerIDSMALL(CustomerID)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
        #endregion
    }
}