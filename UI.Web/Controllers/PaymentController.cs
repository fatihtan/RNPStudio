using Sky.Common.Extensions;
using Sky.CDN.DTO;
using Sky.CDN.IServices;
using Sky.CMS.IServices;
using Sky.CMS.Services;
using Sky.Common.DTO;
using Sky.Common.Provider;
using Sky.Core.DTO;
using Sky.Core.IServices;
using Sky.Log.DTO;
using Sky.Log.IServices;
using Sky.Mors.DTO;
using Sky.Mors.IServices;
using Sky.Ramesses.DTO;
using Sky.Ramesses.IServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Web.Models.Payment;
using UI.Web.Infrastructure;

using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;
using PermissionAct = Sky.Common.DTO.TK.Project.Studio.TKPermissionAction;
using Sky.CMS.DTO;

namespace UI.Web.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _iPaymentService;
        private readonly IUserService _iUserService;
        private readonly ICDNService _iCDNService;
        private readonly ICustomerService _iCustomerService;
        private readonly IRaffleService _iRaffleService;
        private readonly IGeneralContentService _iGeneralContentService;
        private readonly MorsOperations _morsOperations;

        public PaymentController(
            Sky.SuperUser.IServices.ISessionService iSessionService,
            ILogService iLogService,
            IPaymentService iPaymentService,
            IUserService iUserService,
            ICDNService iCDNService,
            IRaffleService iRaffleService,
            ICustomerService iCustomerService,
            IGeneralContentService iGeneralContentService,
            MorsOperations morsOperations)
            : base(iSessionService, iLogService)
        {
            this._iPaymentService = iPaymentService;
            this._iUserService = iUserService;
            this._iCDNService = iCDNService;
            this._iRaffleService = iRaffleService;
            this._iCustomerService = iCustomerService;
            this._iGeneralContentService = iGeneralContentService;
            this._morsOperations = morsOperations;
        }

        [PermissionFilter(PermissionNav.OdemeListesiniGoruntuleme)]
        public ActionResult List(string q, int? Limit)
        {
            var model = new PaymentListModel();

            List<Payment> paymentList = new List<Payment>();

            if (!Limit.HasValue)
            {
                Limit = 200;
            }

            if (!string.IsNullOrEmpty(q))
            {
                var foundedPayment = this._iPaymentService.GetPaymentByIDLARGE(Convert.ToInt32(q));

                if (foundedPayment != null)
                {
                    paymentList.Add(foundedPayment);
                }
            }
            else
            {
                paymentList = this._iPaymentService.GetPaymentListLIMIT(Limit.Value);
            }


            int[] customerIDArray = paymentList.Select(p => p.CustomerID).ToArray();

            var customerList = this._iCustomerService.GetCustomerListByIDList(customerIDArray);

            foreach (var item in paymentList)
            {
                item.Customer = customerList.Where(u => u.ID == item.CustomerID).FirstOrDefault() ?? new Customer();
            }

            model.PaymentList = paymentList;
            model.Limit = Limit.Value;
            model.Query = q;

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.OdemeDetayiniGoruntuleme)]
        public ActionResult Detail(int ID)
        {
            #region Validation
            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(ID);
            if (paymentItem == null)
            {
                throw new Exception("");
            }

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(paymentItem.CustomerID);
            if (customerItem == null)
            {
                throw new Exception("");
            }
            #endregion

            var countryList = this._iGeneralContentService.GetCountryList();
            var bankAccountList = this._iGeneralContentService.GetBankAccountListLARGE();
            var userBankAccountList = this._iUserService.GetUserBankAccountListByUserIDLARGE(paymentItem.ActorID);
            var bankList = this._iGeneralContentService.GetBankListSMALL();

            customerItem.CountryPhoneCode = (countryList.Where(c => c.ID == customerItem.CountryID).FirstOrDefault() ?? new Country()).PhoneCode;

            foreach (var item in userBankAccountList)
            {
                var bankItem = bankList.Where(b => b.ID == item.BankID).FirstOrDefault();
                if (bankItem == null)
                {
                    continue;
                }

                item.BankName = bankItem.Name;
                item.BankLogoURL = bankItem.LogoURL;
            }
            
            #region BankAccount & UserBankAccount Setting
            if (paymentItem.TKPaymentType == TK.TKPaymentType.Transfer && paymentItem.IsCompanyBankAccountReceiver.HasValue)
            {
                if (paymentItem.IsCompanyBankAccountReceiver.Value && paymentItem.BankAccountID.HasValue)
                {
                    paymentItem.BankAccount = bankAccountList.Where(ba => ba.ID == paymentItem.BankAccountID.Value).FirstOrDefault() ?? new BankAccount();
                    paymentItem.BankAccount.BankName = paymentItem.BankAccount.Bank.Name;
                }
                else if (!paymentItem.IsCompanyBankAccountReceiver.Value && paymentItem.UserBankAccountID.HasValue)
                {
                    UserBankAccount tempUserBankAccount = userBankAccountList.Where(uba => uba.ID == paymentItem.UserBankAccountID.Value).FirstOrDefault();

                    if (tempUserBankAccount != null)
                    {
                        paymentItem.BankAccount = new BankAccount()
                        {
                            ID = tempUserBankAccount.ID,
                            BankName = tempUserBankAccount.BankName,
                            IBAN = tempUserBankAccount.IBAN,
                            Owner = tempUserBankAccount.Owner
                        };
                    }
                }
            }
            #endregion

            var socialMediaAccountItem = this._iCustomerService.GetSocialMediaAccountByIDLARGE(paymentItem.SocialMediaAccountID) ?? new SocialMediaAccount();

            if (paymentItem.LeaderTKActor.HasValue)
            {
                if (paymentItem.LeaderTKActor.Value == TK.TKActor.User && paymentItem.LeaderActorID.HasValue)
                {
                    var leaderUserItem = this._iUserService.GetUserByIDMEDIUM(paymentItem.LeaderActorID.Value) ?? new User();
                    paymentItem.LeaderActorFullName = string.Format("{0} {1}", leaderUserItem.FirstName, leaderUserItem.LastName);
                }
            }

            var model = new PaymentModel();

            var paymentStatusRelation = this._iPaymentService.GetPaymentStatusRelationListByPaymentIDLARGE(ID);

            paymentItem.PaymentStatusRelationList = paymentStatusRelation;
            paymentItem.LastTKPaymentStatus = (paymentStatusRelation.LastOrDefault() ?? new PaymentStatusRelation()).TKPaymentStatus;
            paymentItem.RaffleTemplate = this._iRaffleService.GetRaffleTemplateByIDLARGE(paymentItem.RaffleTemplateID) ?? new RaffleTemplate();
            paymentItem.SocialMediaAccount = socialMediaAccountItem;

            if (paymentItem.TKActor == TK.TKActor.User)
            {
                model.User = this._iUserService.GetUserByIDMEDIUM(paymentItem.ActorID);
                model.User.CountryPhoneCode = (countryList.Where(c => c.ID == customerItem.CountryID).FirstOrDefault() ?? new Country()).PhoneCode;
            }

            model.Payment = paymentItem;
            model.Customer = customerItem;

            model.UserBankAccountList = userBankAccountList;
            model.BankAccountList = this._iGeneralContentService.GetBankAccountListMEDIUM();
            model.TKPaymentTypeList = TypeService.GetAll(TK.TKPaymentType.Undefined);
            model.TKPaymentStatusList = TypeService.GetAll(TK.TKPaymentStatus.Undefined);
            model.TurkishCultureInfo = new System.Globalization.CultureInfo("tr-TR");

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        #region Ajax
        [HttpPost]
        [PermissionFilter(PermissionAct.OdemeAktivasyonu)]
        public JsonResult UpdatePaymentActivationAjax(Payment payment)
        {
            Result result = new Result();

            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(payment.ID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            payment.IsCancelled = !payment.IsCancelled;
            payment.IsDeleted = !payment.IsDeleted;

            if (paymentItem.IsCancelled == payment.IsCancelled &&
                paymentItem.IsShown == payment.IsShown &&
                paymentItem.IsActive == payment.IsActive &&
                paymentItem.IsDeleted == payment.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iPaymentService.UpdatePaymentActivation(payment))
            {
                result.Message = "Ödeme aktivasyonu güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    IsCancelled = paymentItem.IsCancelled,
                    IsShown = paymentItem.IsShown,
                    IsActive = paymentItem.IsActive,
                    IsDeleted = paymentItem.IsDeleted
                },
                newValue = new
                {
                    IsCancelled = payment.IsCancelled,
                    IsShown = payment.IsShown,
                    IsActive = payment.IsActive,
                    IsDeleted = payment.IsDeleted
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Payment,
                payment.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Ödeme aktivasyonu güncellendi");
            #endregion

            result.Message = "Ödeme aktivasyonu başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.FinansalOnay, PermissionAct.ListeUzerindeOdemeOnayi)]
        public JsonResult UpdatePaymentFinancialConfirmationAjax(Payment payment, string Description, bool? FromPaymentList)
        {
            Result result = new Result();

            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(payment.ID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (paymentItem.IsFinancialConfirmed)
            {
                result.Message = "Bu ödeme kaydı için daha önce finansal onay verilmiştir. İşleminiz gerçekleştirilmedi.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!payment.IsFinancialConfirmed)
            {
                result.Message = "Finansal onay verebilmeniz için switch'i açmanız gerekmektedir.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            Sky.Ramesses.DTO.User user = null;
            Sky.Ramesses.DTO.User leaderUser = null;
            if (paymentItem.TKActor == TK.TKActor.User)
            {
                user = this._iUserService.GetUserByIDMEDIUM(paymentItem.ActorID);
            }

            if (user == null)
            {
                result.Message = "Ödemeyi oluşturan kullanıcı kaydı bulunamadı. Kullanıcı kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (
                paymentItem.LeaderTKActor.HasValue && 
                paymentItem.LeaderTKActor.Value == TK.TKActor.User && 
                paymentItem.LeaderActorID.HasValue)
            {
                leaderUser = this._iUserService.GetUserByIDMEDIUM(paymentItem.LeaderActorID.Value);

                if (leaderUser == null)
                {
                    result.Message = "Komisyonun tanımlanacağı Takım Lideri Kullanıcı bilgisi bulunamadı. Lütfen sistem geliştiricinizle iletişime geçiniz.";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }

            payment.FinancialConfirmedAt = DateTime.UtcNow;

            if (!this._iPaymentService.UpdatePaymentFinancialConfirmation(payment))
            {
                result.Message = "Ödeme finansal onayı verilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContentPayment = new
            {
                oldValue = new
                {
                    IsFinancialConfirmed = paymentItem.IsFinancialConfirmed,
                    FinancialConfirmedAt = paymentItem.FinancialConfirmedAt
                },
                newValue = new
                {
                    IsFinancialConfirmed = payment.IsFinancialConfirmed,
                    FinancialConfirmedAt = payment.FinancialConfirmedAt
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Payment,
                payment.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContentPayment),
                "Ödeme finansal onayı verildi");
            #endregion

            PaymentStatusRelation psr = new PaymentStatusRelation()
            {
                CustomerID = paymentItem.CustomerID,
                TKActor = TK.TKActor.Administrator,
                ActorID = this.OutAdministratorID,
                PaymentID = paymentItem.ID,
                TKPaymentStatus = TK.TKPaymentStatus.PaymentFinancialConfirmed,
                Description = Description,
                IsActive = true,
                IsDeleted = false,
                IPAddress = IPProvider.GetIPAddress(this.HttpContext),
                CreatedAt = DateTime.UtcNow
            };

            int psrID;
            if (this._iPaymentService.SavePaymentStatusRelation(psr, out psrID))
            {
                psr.ID = psrID;

                #region LogTransaction
                var logContentPaymentStatusRelation = psr;

                base.LogTransaction(Constants.Database.Core.Value,
                    (int)Constants.Database.Core.Table.PaymentStatusRelation,
                    psr.ID,
                    TK.TKTransactionType.Save,
                    new JavaScriptSerializer().Serialize(logContentPaymentStatusRelation),
                    "Ödeme hareketi kaydedildi (Ödeme Onayı)");
                #endregion
            }

            if (user != null)
            {
                #region UserBalance - Creator
                UserBalance userBalanceCreator = new UserBalance();
                userBalanceCreator.UserID = user.ID;

                var activeUserBalance = this._iUserService.GetUserBalanceByUserIDACTIVE(user.ID);
                if (activeUserBalance != null)
                {
                    userBalanceCreator.PreviousBalance = activeUserBalance.CurrentBalance;
                }

                #region Deactivate User Balance
                var activeUserBalanceList = this._iUserService.GetUserBalanceListByUserIDACTIVE(user.ID);
                if (activeUserBalanceList.Any())
                {
                    if (this._iUserService.UpdateUserBalanceDeactive(user.ID))
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
                                Description = "Bakiye pasife alındı (Ödeme Onayı)",
                                CreatedAt = DateTime.UtcNow,
                                IPAddress = IPProvider.GetIPAddress(HttpContext)
                            };

                            logList.Add(lt);
                        }

                        int rows = base.LogTransaction(logList);
                    }
                }
                #endregion

                decimal amountPaid = (paymentItem.AmountPaid * (decimal)paymentItem.UserCommissionRate) / 100;
                userBalanceCreator.CurrentBalance = userBalanceCreator.PreviousBalance + amountPaid;
                userBalanceCreator.Amount = amountPaid;
                userBalanceCreator.Description = Description;
                userBalanceCreator.ShownDescription = string.Format("{0} Numaralı Ödeme Finansal Onayı", paymentItem.ID.ToString("00000"));
                userBalanceCreator.TKBalanceAction = TK.TKBalanceAction.RaffleCommission;
                userBalanceCreator.BalanceActionID = paymentItem.ID;
                userBalanceCreator.IsShown = true;
                userBalanceCreator.IsActive = true;
                userBalanceCreator.IsDeleted = false;
                userBalanceCreator.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                userBalanceCreator.CreatedAt = DateTime.UtcNow;

                int id;
                if (!this._iUserService.SaveUserBalance(userBalanceCreator, out id))
                {
                    result.Message = "Ödeme finansal onayı verildi. Ancak kullanıcı bakiye tanımlama işlemi gerçekleştirilemedi. Lütfen tekrar deneyiniz. Hata Kodu: 1001";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                userBalanceCreator.ID = id;

                #region LogTransaction
                var logContentUserBalance = userBalanceCreator;

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.UserBalance,
                    userBalanceCreator.ID,
                    TK.TKTransactionType.Save,
                    new JavaScriptSerializer().Serialize(logContentUserBalance),
                    "Bakiye kaydedildi (Ödeme Onayı)");
                #endregion
                #endregion
            }

            if (leaderUser != null)
            {
                #region UserBalance - TeamLeader
                UserBalance userBalanceTeamLeader = new UserBalance();
                userBalanceTeamLeader.UserID = leaderUser.ID;

                var activeUserBalance = this._iUserService.GetUserBalanceByUserIDACTIVE(leaderUser.ID);
                if (activeUserBalance != null)
                {
                    userBalanceTeamLeader.PreviousBalance = activeUserBalance.CurrentBalance;
                }

                #region Deactivate User Balance
                var activeUserBalanceList = this._iUserService.GetUserBalanceListByUserIDACTIVE(leaderUser.ID);
                if (activeUserBalanceList.Any())
                {
                    if (this._iUserService.UpdateUserBalanceDeactive(leaderUser.ID))
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
                                Description = "Bakiye pasife alındı (Ödeme Onayı)",
                                CreatedAt = DateTime.UtcNow,
                                IPAddress = IPProvider.GetIPAddress(HttpContext)
                            };

                            logList.Add(lt);
                        }

                        int rows = base.LogTransaction(logList);
                    }
                }
                #endregion

                decimal amountPaid = (paymentItem.AmountPaid * (decimal)paymentItem.LeaderUserCommissionRate) / 100;
                userBalanceTeamLeader.CurrentBalance = userBalanceTeamLeader.PreviousBalance + amountPaid;
                userBalanceTeamLeader.Amount = amountPaid;
                userBalanceTeamLeader.Description = Description;
                userBalanceTeamLeader.ShownDescription = string.Format("{0} Numaralı Ödeme Finansal Onayı (Takım Lideri Komisyonu)", paymentItem.ID.ToString("00000"));
                userBalanceTeamLeader.TKBalanceAction = TK.TKBalanceAction.RaffleCommissionTeamLeader;
                userBalanceTeamLeader.BalanceActionID = paymentItem.ID;
                userBalanceTeamLeader.IsShown = true;
                userBalanceTeamLeader.IsActive = true;
                userBalanceTeamLeader.IsDeleted = false;
                userBalanceTeamLeader.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
                userBalanceTeamLeader.CreatedAt = DateTime.UtcNow;

                int id;
                if (!this._iUserService.SaveUserBalance(userBalanceTeamLeader, out id))
                {
                    result.Message = "Ödeme finansal onayı verildi. Ancak kullanıcı bakiye tanımlama işlemi gerçekleştirilemedi. Lütfen tekrar deneyiniz. Hata Kodu: 1002";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                userBalanceTeamLeader.ID = id;

                #region LogTransaction
                var logContentUserBalance = userBalanceTeamLeader;

                base.LogTransaction(
                    Constants.Database.Ramesses.Value,
                    (int)Constants.Database.Ramesses.Table.UserBalance,
                    userBalanceTeamLeader.ID,
                    TK.TKTransactionType.Save,
                    new JavaScriptSerializer().Serialize(logContentUserBalance),
                    "Bakiye kaydedildi (Ödeme Onayı Takım Lideri Komisyonu)");
                #endregion
                #endregion
            }

            result.Message = "Ödeme finansal onayı başarıyla verildi.";
            result.IsSuccess = true;

            result.Data = new
            {
                FinancialConfirmedAt = payment.FinancialConfirmedAt.Value.ToString("dd MMMM yyyy HH:mm:ss", new CultureInfo("tr-TR")),
                FromPaymentList = FromPaymentList,
                ID = payment.ID
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.OdemeTuruveBankaHesabiGuncelleme)]
        public JsonResult UpdatePaymentTypeAndBankAccountAjax(Payment payment)
        {
            Result result = new Result();

            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(payment.ID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (payment.TKPaymentType == paymentItem.TKPaymentType &&
                payment.IsCompanyBankAccountReceiver == paymentItem.IsCompanyBankAccountReceiver &&
                payment.BankAccountID == paymentItem.BankAccountID &&
                payment.UserBankAccountID == paymentItem.UserBankAccountID)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            //  If payment type is Transfer
            if (payment.TKPaymentType == TK.TKPaymentType.Transfer)
            {
                //  BankAccountItems is set to null
                BankAccount bankAccountItem = null;
                UserBankAccount userBankAccountItem = null;

                if (payment.IsCompanyBankAccountReceiver.HasValue)
                {
                    if (payment.IsCompanyBankAccountReceiver.Value)
                    {
                        if (!payment.BankAccountID.HasValue)
                        {
                            result.Message = "Ödeme yapılan RNP Reklam banka hesabını seçiniz.";
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }

                        #region BankAccount Setting
                        bankAccountItem = this._iGeneralContentService.GetBankAccountByIDLARGE(payment.BankAccountID.Value);
                        if (bankAccountItem == null)
                        {
                            result.Message = "Seçtiğiniz RNP Reklam banka hesabı bulunamadı. RNP Reklam Banka hesabının daha önce silinmediğinden emin olun.";
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                        #endregion

                        payment.UserBankAccountID = null;
                    }
                    else
                    {
                        if (!payment.UserBankAccountID.HasValue)
                        {
                            result.Message = "Ödeme yapılan kullanıcı banka hesabını seçiniz.";
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }

                        #region UserBankAccount Setting
                        userBankAccountItem = this._iUserService.GetUserBankAccountByIDLARGE(payment.UserBankAccountID.Value);
                        if (userBankAccountItem == null)
                        {
                            result.Message = "Seçtiğiniz kullanıcı banka hesabı bulunamadı. Kullanıcı banka hesabının daha önce silinmediğinden emin olun.";
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                        #endregion

                        payment.BankAccountID = null;
                    }
                }
            }
            else
            {
                payment.BankAccountID = null;
                payment.UserBankAccountID = null;
                payment.IsCompanyBankAccountReceiver = null;
            }

            if (!this._iPaymentService.UpdatePaymentPaymentTypeAndBankAccount(payment))
            {
                result.Message = "Ödeme türü ve banka hesap bilgisi güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    TKPaymentType = paymentItem.TKPaymentType,
                    IsCompanyBankAccountReceiver = paymentItem.IsCompanyBankAccountReceiver,
                    BankAccountID = paymentItem.BankAccountID,
                    UserBankAccountID = paymentItem.UserBankAccountID
                },
                newValue = new
                {
                    TKPaymentType = payment.TKPaymentType,
                    IsCompanyBankAccountReceiver = payment.IsCompanyBankAccountReceiver,
                    BankAccountID = payment.BankAccountID,
                    UserBankAccountID = payment.UserBankAccountID
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Payment,
                payment.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Ödeme türü ve banka hesap bilgisi güncellendi");
            #endregion

            result.Message = "Ödeme türü ve banka hesap bilgisi güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.TutarGuncelleme)]
        public JsonResult UpdatePaymentPriceAjax(Payment payment)
        {
            Result result = new Result();

            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(payment.ID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (paymentItem.TotalPrice == payment.TotalPrice &&
                paymentItem.AmountPaid == payment.AmountPaid)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iPaymentService.UpdatePaymentPrice(payment))
            {
                result.Message = "Ödeme kaydı tutar bilgileri güncellenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    TotalPrice = paymentItem.TotalPrice,
                    AmountPaid = paymentItem.AmountPaid
                },
                newValue = new
                {
                    TotalPrice = payment.TotalPrice,
                    AmountPaid = payment.AmountPaid
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.Payment,
                payment.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Ödeme kaydı tutar bilgileri güncellendi");
            #endregion

            result.Message = "Ödeme kaydı tutar bilgileri başarıyla güncellendi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.YeniOdemeHareketiEkleme)]
        public JsonResult PaymentStatusRelationSaveAjax(PaymentStatusRelation paymentStatusRelation, bool InActiveAll, bool DeleteAll)
        {
            Result result = new Result();

            int paymentID = paymentStatusRelation.PaymentID;
            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(paymentID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (paymentStatusRelation.TKPaymentStatus == TK.TKPaymentStatus.Undefined)
            {
                result.Message = "Ödeme durumu seçilmedi. Ödeme durumu seçip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            DeleteAll = !DeleteAll;

            if (InActiveAll || DeleteAll)
            {
                var tempPaymentStatusRelationList = this._iPaymentService.GetPaymentStatusRelationListByPaymentIDLARGE(paymentID);
                var activePaymentStatusRelationList = tempPaymentStatusRelationList.Where(psr => psr.IsActive).ToList();
                var undeltPaymentStatusRelationList = tempPaymentStatusRelationList.Where(psr => !psr.IsDeleted).ToList();

                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<LogTransaction> logList = new List<LogTransaction>();

                #region InActiveAll
                if (activePaymentStatusRelationList.Any() && InActiveAll)
                {
                    if (this._iPaymentService.UpdatePaymentStatusRelationIsActiveByPaymentID(paymentID, false))
                    {
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
                        string serializedLC = jss.Serialize(lc);

                        foreach (var item in activePaymentStatusRelationList)
                        {
                            var logTransaction = new LogTransaction()
                            {
                                DatabaseID = Constants.Database.Core.Value,
                                TableID = (int)Constants.Database.Core.Table.PaymentStatusRelation,
                                RowID = item.ID,
                                ActorID = this.OutAdministratorID,
                                TKActor = TK.TKActor.Administrator,
                                TKTransactionType = TK.TKTransactionType.Update,
                                LogContent = serializedLC,
                                Description = "Ödeme hareketi pasife alındı",
                                CreatedAt = DateTime.UtcNow,
                                IPAddress = IPProvider.GetIPAddress(HttpContext)
                            };

                            logList.Add(logTransaction);
                        }
                    }
                }
                #endregion

                #region DeleteAll
                if (undeltPaymentStatusRelationList.Any() && DeleteAll)
                {
                    if (this._iPaymentService.UpdatePaymentStatusRelationIsDeletedByPaymentID(paymentID, true))
                    {
                        var lc = new
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
                        string serializedLC = jss.Serialize(lc);

                        foreach (var item in undeltPaymentStatusRelationList)
                        {
                            var logTransaction = new LogTransaction()
                            {
                                DatabaseID = Constants.Database.Core.Value,
                                TableID = (int)Constants.Database.Core.Table.PaymentStatusRelation,
                                RowID = item.ID,
                                ActorID = this.OutAdministratorID,
                                TKActor = TK.TKActor.Administrator,
                                TKTransactionType = TK.TKTransactionType.Delete,
                                LogContent = serializedLC,
                                Description = "Ödeme hareketi silindi",
                                CreatedAt = DateTime.UtcNow,
                                IPAddress = IPProvider.GetIPAddress(HttpContext)
                            };

                            logList.Add(logTransaction);
                        }
                    }
                }
                #endregion

                int rows = base.LogTransaction(logList);
            }

            #region ReceiptURL
            string receiptURL = null;
            HttpPostedFileBase httpPostedFile = Request.Files["ReceiptURL"];
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

                HttpPostedFileBase filePosted = Request.Files["ReceiptURL"];
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

                    receiptURL = result.Data.ToString();
                }
                else
                {
                    result.Message = "Data is null";
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
                #endregion
            }
            #endregion

            paymentStatusRelation.CustomerID = paymentItem.CustomerID;
            paymentStatusRelation.ReceiptURL = receiptURL;
            paymentStatusRelation.IsActive = true;
            paymentStatusRelation.IsDeleted = false;
            paymentStatusRelation.IPAddress = IPProvider.GetIPAddress(this.HttpContext);
            paymentStatusRelation.CreatedAt = DateTime.UtcNow;

            int id;
            if (!this._iPaymentService.SavePaymentStatusRelation(paymentStatusRelation, out id))
            {
                result.Message = "Ödeme hareketi kaydedilemedi. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            paymentStatusRelation.ID = id;

            var paymentStatusRelationList = this._iPaymentService.GetPaymentStatusRelationListByPaymentIDLARGE(paymentID);
            paymentStatusRelationList = paymentStatusRelationList.Where(psr => psr.ID != paymentStatusRelation.ID).ToList();

            #region LogTransaction
            var logContent = paymentStatusRelation;

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.PaymentStatusRelation,
                paymentStatusRelation.ID,
                TK.TKTransactionType.Save,
                new JavaScriptSerializer().Serialize(logContent),
                "Ödeme hareketi kaydedildi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Ödeme hareketi başarıyla kaydedildi.";
            result.Data = new
            {
                PaymentStatusRelationList = paymentStatusRelationList,
                PaymentStatusRelation = paymentStatusRelation,
                CreatedAtStr = paymentStatusRelation.CreatedAt.ToString("yyyy.MM.dd HH:mm:ss"),
                TKPaymentStatusName = TypeService.GetNameByValue(paymentStatusRelation.TKPaymentStatus)
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.OdemeHareketleriniGuncelleme)]
        public JsonResult UpdatePaymentStatusRelationAjax(PaymentStatusRelation paymentStatusRelation)
        {
            Result result = new Result();

            var paymentStaturRelationItem = this._iPaymentService.GetPaymentStatusRelationByIDMEDIUM(paymentStatusRelation.ID);
            if (paymentStaturRelationItem == null)
            {
                result.Message = "Ödeme hareketi bulunamadı. Lütfen ödeme hareketinin silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            bool isActive = paymentStatusRelation.IsActive;
            bool isDeleted = !paymentStatusRelation.IsDeleted;
            paymentStatusRelation.IsDeleted = isDeleted;

            if (isActive == paymentStaturRelationItem.IsActive &&
                isDeleted == paymentStaturRelationItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iPaymentService.UpdatePaymentStatusRelationState(paymentStatusRelation.ID, isActive, isDeleted))
            {
                result.Message = "Güncelleme işlemi gerçekleştirilemedi. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    IsActive = isActive,
                    IsDeleted = isDeleted
                },
                newValue = new
                {
                    IsActive = paymentStatusRelation.IsActive,
                    IsDeleted = paymentStaturRelationItem.IsDeleted
                }
            };

            base.LogTransaction(Constants.Database.Core.Value,
                (int)Constants.Database.Core.Table.PaymentStatusRelation,
                paymentStatusRelation.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Ödeme hareketi güncellendi");
            #endregion

            result.IsSuccess = true;
            result.Message = "Ödeme hareketi başarıyla güncellendi.";
            result.Data = new
            {
                PaymentStatusRelation = paymentStatusRelation
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.ListeUzerindeOdemeOnayi)]
        public JsonResult GetPaymentForFinancialConfirmAjax(Payment payment)
        {
            Result result = new Result();

            #region Validation
            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(payment.ID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (paymentItem.IsFinancialConfirmed)
            {
                result.Message = "Bu ödeme kaydı için daha önce finansal onay verilmiştir. İşleminiz gerçekleştirilmedi.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            Sky.Ramesses.DTO.User user = null;
            if (paymentItem.TKActor == TK.TKActor.User)
            {
                user = this._iUserService.GetUserByIDMEDIUM(paymentItem.ActorID);
            }

            if (user == null)
            {
                result.Message = "Ödemeyi oluşturan kullanıcı kaydı bulunamadı. Kullanıcı kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var leaderUserInformation = new Object();

            if (paymentItem.LeaderTKActor.HasValue)
            {
                if (paymentItem.LeaderTKActor.Value == TK.TKActor.User && paymentItem.LeaderActorID.HasValue && paymentItem.LeaderUserCommissionRate.HasValue)
                {
                    var leaderUserItem = this._iUserService.GetUserByIDMEDIUM(paymentItem.LeaderActorID.Value) ?? new User();
                    paymentItem.LeaderActorFullName = string.Format("{0} {1}", leaderUserItem.FirstName, leaderUserItem.LastName);

                    leaderUserInformation = new
                    {
                        FullName = string.Format("{0} (T. Lideri)", paymentItem.LeaderActorFullName),
                        AmountPaid = paymentItem.AmountPaid.ToThousandSeperatorWithCurrency(),
                        CommissionRate = string.Format("%{0}", paymentItem.LeaderUserCommissionRate.Value.ToString("0.00")),
                        CommissionPrice = ((paymentItem.AmountPaid * (decimal)paymentItem.LeaderUserCommissionRate.Value) / 100).ToThousandSeperatorWithCurrency()
                    };
                }
            }
            #endregion

            result.IsSuccess = true;
            result.Data = new
            {
                User = new {
                    FullName = user.GetFullName(),
                    AmountPaid = paymentItem.AmountPaid.ToThousandSeperatorWithCurrency(),
                    CommissionRate = string.Format("%{0}", paymentItem.UserCommissionRate.ToString("0.00")),
                    CommissionPrice = ((paymentItem.AmountPaid * (decimal)paymentItem.UserCommissionRate) / 100).ToThousandSeperatorWithCurrency()
                },
                LeaderUser = leaderUserInformation
            };

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region SendEmail
        [HttpPost]
        [PermissionFilter(PermissionAct.EmailGonder)]
        public JsonResult SendEmailPaymentAjax(TK.TKEmailTemplate TKEmailTemplate, int PaymentID, string FullName, string Email, bool UseBCC, string BCCEmail)
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
            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(PaymentID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKEmailTemplate != TK.TKEmailTemplate.PaymentInProgress &&
                TKEmailTemplate != TK.TKEmailTemplate.PaymentConfirmed &&
                TKEmailTemplate != TK.TKEmailTemplate.PaymentNotConfirmed)
            {
                result.Message = "Tanımlanamayan Email Template algılandı. Sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            RaffleTemplate raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(paymentItem.RaffleTemplateID) ?? new RaffleTemplate() { Name = "Danışmanlık Hizmeti" };
            string userFullName = null;
            if (paymentItem.TKActor == TK.TKActor.User)
            {
                var userItem = this._iUserService.GetUserByIDMEDIUM(paymentItem.ActorID) ?? new User();
                userFullName = string.Format("{0} {1}", userItem.FirstName, userItem.LastName);
            }

            var customerItem = this._iCustomerService.GetCustomerByIDLARGE(paymentItem.CustomerID);
            if (customerItem == null)
            {
                result.Message = "Müşteri kaydı sistemde bulunamadı. Kayıt veritabanından kalıcı olarak silinmiş olabilir. Lütfen sistem geliştiricinizle iletişime geçiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            string customerFullName = string.Format("{0} {1}", customerItem.FirstName, customerItem.LastName);

            #region Function Selection
            if (TKEmailTemplate == TK.TKEmailTemplate.PaymentInProgress)
            {
                result = this._morsOperations.SendEmailPaymentAjaxEmail(TKEmailTemplate, paymentItem, raffleTemplateItem, userFullName, customerFullName, FullName, Email, UseBCC, BCCEmail);
            }
            else if (TKEmailTemplate == TK.TKEmailTemplate.PaymentConfirmed)
            {
                result = this._morsOperations.SendEmailPaymentAjaxEmail(TKEmailTemplate, paymentItem, raffleTemplateItem, userFullName, customerFullName, FullName, Email, UseBCC, BCCEmail);
            }
            else if (TKEmailTemplate == TK.TKEmailTemplate.PaymentNotConfirmed)
            {
                result = this._morsOperations.SendEmailPaymentAjaxEmail(TKEmailTemplate, paymentItem, raffleTemplateItem, userFullName, customerFullName, FullName, Email, UseBCC, BCCEmail);
            }
            #endregion

            if (result.IsSuccess)
            {
                result.Message = "Email başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region SendSms
        [HttpPost]
        [PermissionFilter(PermissionAct.SmsGonder)]
        public JsonResult SendSmsPaymentAjax(TK.TKSmsTemplate TKSmsTemplate, int PaymentID, string FullName, string PhoneNumber)
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
            var paymentItem = this._iPaymentService.GetPaymentByIDLARGE(PaymentID);
            if (paymentItem == null)
            {
                result.Message = "Ödeme kaydı bulunamadı. Ödeme kaydının daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (TKSmsTemplate != TK.TKSmsTemplate.PaymentInProgress &&
                TKSmsTemplate != TK.TKSmsTemplate.PaymentConfirmed &&
                TKSmsTemplate != TK.TKSmsTemplate.PaymentNotConfirmed)
            {
                result.Message = "Tanımlanamayan Sms Template algılandı. Sayfayı yenileyip yeniden deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            #endregion

            RaffleTemplate raffleTemplateItem = this._iRaffleService.GetRaffleTemplateByIDLARGE(paymentItem.RaffleTemplateID) ?? new RaffleTemplate() { Name = "Danışmanlık Hizmeti" };
            string userFullName = null;
            if (paymentItem.TKActor == TK.TKActor.User)
            {
                var userItem = this._iUserService.GetUserByIDMEDIUM(paymentItem.ActorID) ?? new User();
                userFullName = string.Format("{0} {1}", userItem.FirstName, userItem.LastName);
            }

            #region Function Selection
            if (TKSmsTemplate == TK.TKSmsTemplate.PaymentInProgress)
            {
                result = this._morsOperations.SendSmsPaymentAjaxSms(TKSmsTemplate, paymentItem, raffleTemplateItem, userFullName, FullName, PhoneNumber);
            }
            else if (TKSmsTemplate == TK.TKSmsTemplate.PaymentConfirmed)
            {
                result = this._morsOperations.SendSmsPaymentAjaxSms(TKSmsTemplate, paymentItem, raffleTemplateItem, userFullName, FullName, PhoneNumber);
            }
            else if (TKSmsTemplate == TK.TKSmsTemplate.PaymentNotConfirmed)
            {
                result = this._morsOperations.SendSmsPaymentAjaxSms(TKSmsTemplate, paymentItem, raffleTemplateItem, userFullName, FullName, PhoneNumber);
            }
            #endregion

            if (result.IsSuccess)
            {
                result.Message = "Sms başarıyla gönderilmiştir.";
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #endregion
    }
}