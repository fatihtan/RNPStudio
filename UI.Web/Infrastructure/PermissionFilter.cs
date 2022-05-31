using Sky.Common.DTO;
using Sky.SuperUser.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Web.Infrastructure
{
    public class PermissionFilter : ActionFilterAttribute
    {
        private TK.Project.Studio.TKPermissionAction[] Values;
        private int Value;
        private bool IsPermissionAction;
        private bool IsCollectionControl;
        private bool PretendAction;

        public PermissionFilter(TK.Project.Studio.TKPermissionAction Value)
        {
            this.IsPermissionAction = true;
            this.Value = (int)Value;
        }

        public PermissionFilter(params TK.Project.Studio.TKPermissionAction[] Values)
        {
            this.IsCollectionControl = true;
            this.IsPermissionAction = true;
            this.Values = Values;
        }

        public PermissionFilter(TK.Project.Studio.TKPermissionNavigation Value) : this(Value, false)
        {
            
        }

        public PermissionFilter(TK.Project.Studio.TKPermissionNavigation Value, bool PretendAction)
        {
            this.PretendAction = PretendAction;
            this.Value = (int)Value;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.IsPermissionAction)
            {
                #region PermissionAction
                var sessionValue = filterContext.RequestContext.HttpContext.Session["AdministratorPermissionAction"];
                if (sessionValue == null)
                {
                    string message = "İşleminiz gerçekleştirilemiyor. Lütfen oturumunuzu kapatıp yeniden deneyiniz.";
                    var jsonResult = new JsonResult();
                    jsonResult.Data = new Result()
                    {
                        IsSuccess = false,
                        Message = message
                    };
                    filterContext.Result = jsonResult;
                    return;
                }

                var permissionActionList = (List<AdministratorPermissionAction>)sessionValue;

                if (this.IsCollectionControl)
                {
                    if (!permissionActionList.Any(per => this.Values.Contains((TK.Project.Studio.TKPermissionAction)per.PermissionActionValue)))
                    {
                        string message = string.Format("Yetkisiz işlem algılandı. Bu işlemi gerçekleştirebilmek için gerekli yetki numarası: {0}", this.Value);
                        var jsonResult = new JsonResult();
                        jsonResult.Data = new Result()
                        {
                            IsSuccess = false,
                            Message = message
                        };
                        filterContext.Result = jsonResult;
                        return;
                    }

                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    if (!permissionActionList.Any(per => per.PermissionActionValue == this.Value))
                    {
                        string message = string.Format("Yetkisiz işlem algılandı. Bu işlemi gerçekleştirebilmek için gerekli yetki numarası: {0}", this.Value);
                        var jsonResult = new JsonResult();
                        jsonResult.Data = new Result()
                        {
                            IsSuccess = false,
                            Message = message
                        };
                        filterContext.Result = jsonResult;
                        return;
                    }

                    base.OnActionExecuting(filterContext);
                }
                #endregion
            }
            else
            {
                if (this.PretendAction)
                {
                    #region PermissionNavigation Pretends Action
                    var sessionValue = filterContext.RequestContext.HttpContext.Session["AdministratorPermissionNavigation"];
                    if (sessionValue == null)
                    {
                        string message = "İşleminiz gerçekleştirilemiyor. Lütfen oturumunuzu kapatıp yeniden deneyiniz.";
                        var jsonResult = new JsonResult();
                        jsonResult.Data = new Result()
                        {
                            IsSuccess = false,
                            Message = message
                        };
                        filterContext.Result = jsonResult;
                        return;
                    }

                    var permissionNavigationList = (List<AdministratorPermissionNavigation>)sessionValue;
                    if (!permissionNavigationList.Any(per => per.PermissionNavigationValue == this.Value))
                    {
                        string message = string.Format("Yetkisiz işlem algılandı. Bu işlemi gerçekleştirebilmek için gerekli yetki numarası: {0}", this.Value);
                        var jsonResult = new JsonResult();
                        jsonResult.Data = new Result()
                        {
                            IsSuccess = false,
                            Message = message
                        };
                        filterContext.Result = jsonResult;
                        return;
                    }

                    base.OnActionExecuting(filterContext);
                    #endregion
                }
                else
                {
                    #region PermissionNavigation
                    var sessionValue = filterContext.RequestContext.HttpContext.Session["AdministratorPermissionNavigation"];
                    if (sessionValue == null)
                    {
                        filterContext.RequestContext.HttpContext.Response.StatusCode = 401;
                        filterContext.RequestContext.HttpContext.Response.Redirect("/Error/Oops?message=İşleminiz gerçekleştirilemiyor. Lütfen oturumunuzu kapatıp yeniden deneyiniz.", true);

                        return;
                    }

                    var permissionNavigationList = (List<AdministratorPermissionNavigation>)sessionValue;
                    if (!permissionNavigationList.Any(per => per.PermissionNavigationValue == this.Value))
                    {
                        filterContext.RequestContext.HttpContext.Response.StatusCode = 401;
                        string url = string.Format("/Error/Oops?code=401&message=Yetkisiz işlem algılandı. Bu sayfayı görüntüleyebilmek için gerekli yetki numarası: {0}", this.Value);
                        filterContext.Result = new RedirectResult(url);
                        return;
                    }

                    base.OnActionExecuting(filterContext);
                    #endregion
                }
            }
        }
    }
}