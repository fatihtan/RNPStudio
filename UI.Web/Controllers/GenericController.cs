using Sky.Common.DTO;
using Sky.Generic.DTO;
using Sky.Generic.IServices;
using Sky.Log.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Web.Infrastructure;
using UI.Web.Models.Generic;

using PermissionNav = Sky.Common.DTO.TK.Project.Studio.TKPermissionNavigation;
using PermissionAct = Sky.Common.DTO.TK.Project.Studio.TKPermissionAction;

namespace UI.Web.Controllers
{
    public class GenericController : BaseController
    {
        private readonly IGenericService _iGenericService;

        public GenericController(
            Sky.SuperUser.IServices.ISessionService iSessionService,
            ILogService iLogService,
            IGenericService iGenericService)
            : base(iSessionService, iLogService)
        {
            this._iGenericService = iGenericService;
        }

        #region ContactMessage
        [PermissionFilter(PermissionNav.Mesajlar)]
        public ActionResult ContactMessageList(bool? IsArchived)
        {
            var model = new ContactMessageListModel();

            IsArchived = model.IsArchived = (IsArchived.HasValue && IsArchived.Value) ? true : false;

            model.ContactMessageList = this._iGenericService.GetContactMessageList(IsArchived.Value);

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [PermissionFilter(PermissionNav.Mesajlar)]
        public ActionResult ContactMessageDetail(int ID)
        {
            var model = new ContactMessageDetailModel();

            var contactMessageItem = this._iGenericService.GetContactMessageByIDMEDIUM(ID);

            if (contactMessageItem == null)
            {
                return RedirectToAction("Oops", "Error");
            }

            if (!contactMessageItem.IsRead)
            {
                if (this._iGenericService.UpdateContactMessageIsRead(ID, true))
                {
                    #region LogTransaction
                    var logContent = new
                    {
                        oldValue = new
                        {
                            IsRead = false
                        },
                        newValue = new
                        {
                            IsRead = true
                        }
                    };

                    base.LogTransaction(
                        Constants.Database.Generic.Value,
                        (int)Constants.Database.Generic.Table.ContactMessage,
                        ID,
                        TK.TKTransactionType.Update,
                        new JavaScriptSerializer().Serialize(logContent),
                        "Mesaj okundu bilgisi güncellendi"
                        );
                    #endregion
                }
            }

            model.ContactMessage = contactMessageItem;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MesajSilme)]
        public JsonResult ContactMessageIsDeletedAjax(int ID)
        {
            Result result = new Result();

            var contactMessageItem = this._iGenericService.GetContactMessageByIDMEDIUM(ID);

            if (contactMessageItem == null)
            {
                result.Message = "Mesaj bulunamadı. Mesajın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iGenericService.UpdateContactMessageIsDeleted(ID, true))
            {
                result.Message = "Mesaj silinemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
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
                Constants.Database.Generic.Value,
                (int)Constants.Database.Generic.Table.ContactMessage,
                ID,
                TK.TKTransactionType.Delete,
                new JavaScriptSerializer().Serialize(logContent),
                "Mesaj silindi"
                );
            #endregion

            result.Message = "Mesaj başarıyla silindi.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.MesajArsivleme)]
        public JsonResult ContactMessageIsArchivedAjax(int ID, bool IsArchived)
        {
            Result result = new Result();

            var contactMessageItem = this._iGenericService.GetContactMessageByIDMEDIUM(ID);

            if (contactMessageItem == null)
            {
                result.Message = "Mesaj bulunamadı. Mesajın daha önce silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            IsArchived = !IsArchived;

            if (contactMessageItem.IsArchived == IsArchived)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iGenericService.UpdateContactMessageIsArchived(ID, IsArchived))
            {
                result.Message =
                    IsArchived ?
                    "Seçilen mesaj arşivlenemedi. Bir hata oluştu. Lütfen tekrar deneyiniz." :
                    "Seçilen mesaj arşivden çıkarılamadı. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    IsArchived = contactMessageItem.IsArchived
                },
                newValue = new
                {
                    IsArchived = IsArchived
                }
            };

            base.LogTransaction(
                Constants.Database.Generic.Value,
                (int)Constants.Database.Generic.Table.ContactMessage,
                ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                IsArchived ?
                "Mesaj arşivlendi." :
                "Mesaj arşivden çıkarıldı."
                );
            #endregion

            result.Message = IsArchived ?
                "Seçilen mesaj başarıyla arşivlendi." :
                "Seçilen mesaj başarıyla arşivden çıkarıldı.";
            result.IsSuccess = true;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Subscribe
        [PermissionFilter(PermissionNav.Aboneler)]
        public ActionResult SubscribeList()
        {
            var model = new SubscribeListModel()
            {
                SubscribeList = this._iGenericService.GetSubscribeListMEDIUM()
            };

            ViewBag.PermissionActionList = this.PermissionActionList;

            return View(model);
        }

        [HttpPost]
        [PermissionFilter(PermissionAct.AboneGuncelleme)]
        public JsonResult SubscribeUpdateAjax(Subscribe subscribe)
        {
            Result result = new Result();

            var subscribeItem = this._iGenericService.GetSubscribeByIDMEDIUM(subscribe.ID);
            if (subscribeItem == null)
            {
                result.Message = "Abone bulunamadı. Abone kaydının daha önceden silinmediğinden emin olun.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            var duplicatedSubscribeItem = this._iGenericService.GetSubscribeByEmail(subscribe.Email);
            if (duplicatedSubscribeItem != null && subscribe.ID != duplicatedSubscribeItem.ID)
            {
                result.Message =
                    string.Format(
                    "{0} email adresi daha önceden kayıt edilmiştir. Farklı bir email ile tekrar deneyiniz.",
                    duplicatedSubscribeItem.Email);

                return Json(result, JsonRequestBehavior.DenyGet);
            }

            subscribe.IsDeleted = !subscribe.IsDeleted;

            if (subscribe.Email == subscribeItem.Email &&
                subscribe.Description == subscribeItem.Description &&
                subscribe.IsSubscribed == subscribeItem.IsSubscribed &&
                subscribe.IsDeleted == subscribeItem.IsDeleted)
            {
                result.NoChanges();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (!this._iGenericService.UpdateSubscribeMANUEL(subscribe))
            {
                result.Message = "İşlem gerçekleştirilemedi. Bir hata oluştu. Lütfen tekrar deneyiniz.";
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            #region LogTransaction
            var logContent = new
            {
                oldValue = new
                {
                    Email = subscribeItem.Email,
                    Description = subscribeItem.Description,
                    IsSubscribed = subscribeItem.IsSubscribed,
                    IsDeleted = subscribeItem.IsDeleted
                },
                newValue = new
                {
                    Email = subscribe.Email,
                    Description = subscribe.Description,
                    IsSubscribed = subscribe.IsSubscribed,
                    IsDeleted = subscribe.IsDeleted
                }
            };

            base.LogTransaction
                (
                Constants.Database.Generic.Value,
                (int)Constants.Database.Generic.Table.Subscribe,
                subscribe.ID,
                TK.TKTransactionType.Update,
                new JavaScriptSerializer().Serialize(logContent),
                "Abone güncellendi"
                );
            #endregion

            result.IsSuccess = true;
            result.Message = "Abone kaydı başarıyla güncellendi.";
            result.Data = subscribe;

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}