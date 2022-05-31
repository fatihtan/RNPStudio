using Sky.Common.Extensions;
using Sky.Common.DTO;
using Sky.Core.DTO;
using Sky.Mors.DTO;
using Sky.Mors.IServices;
using Sky.Ramesses.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Globalization;
using Sky.CMS.Services;

namespace UI.Web.Infrastructure
{
    public class MorsOperations
    {
        private readonly IMorsService _iMorsService;

        public MorsOperations(IMorsService iMorsService)
        {
            this._iMorsService = iMorsService;
        }

        #region Email
        public Result SendEmailUserBillAjaxEmail(string FullName, UserBill userBillItem, string Email, bool UseBCC, string BCCEmail)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.YourBillIsReady,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{BillEditedAt}}",
                    ComponentValue = userBillItem.EditedAt.ToString("dd.MM.yyyy"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                string userBillURL = string.Format("{0}/Fatura?uk={1}", ConfigurationManager.AppSettings["CasaBaseURL"], userBillItem.UniqueKey);

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{BillURL}}",
                    ComponentValue = userBillURL,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailCustomerBillAjaxEmail(string FullName, CustomerBill customerBillItem, string Email, bool UseBCC, string BCCEmail)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.YourBillIsReady,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{BillEditedAt}}",
                    ComponentValue = customerBillItem.EditedAt.ToString("dd.MM.yyyy"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                string userBillURL = string.Format("{0}/C/Fatura?uk={1}", ConfigurationManager.AppSettings["CasaBaseURL"], customerBillItem.UniqueKey);

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{BillURL}}",
                    ComponentValue = userBillURL,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailAccountConfirmationAjaxEmail(TK.TKEmailTemplate TKEmailTemplate, string FullName, string Email, bool UseBCC, string BCCEmail)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TKEmailTemplate,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailOtherAjaxEmail(string FullName, string Email, bool UseBCC, string BCCEmail, string content, string Subject)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.Other,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{EmailContent}}",
                    ComponentValue = content,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Subject}}",
                    ComponentValue = Subject.ToString(),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailNotificationAjaxEmail(string FullName, string Email, string NotificationContent, string Subject)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.Other,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{EmailContent}}",
                    ComponentValue = NotificationContent,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Subject}}",
                    ComponentValue = Subject.ToString(),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailPaymentAjaxEmail(TK.TKEmailTemplate TKEmailTemplate, Payment Payment, RaffleTemplate raffleTemplate, string UserFullName, string CustomerFullName, string FullName, string Email, bool UseBCC, string BCCEmail)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TKEmailTemplate,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{PaymentID}}",
                    ComponentValue = Payment.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{ServiceName}}",
                    ComponentValue = raffleTemplate.Name,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CustomerFullName}}",
                    ComponentValue = CustomerFullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{TotalPrice}}",
                    ComponentValue = Payment.TotalPrice.ToThousandSeperatorWithCurrency(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{AmountPaid}}",
                    ComponentValue = Payment.AmountPaid.ToThousandSeperatorWithCurrency(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{UserFullName}}",
                    ComponentValue = UserFullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{PaymentID}}",
                    ComponentValue = Payment.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailRaffleParticipantConfirmedAjaxEmail(RaffleParticipant raffleParticipant, Raffle raffle, RaffleTemplate raffleTemplate, Customer customer, string CreatorFullName, string FullName, string Email, bool UseBCC, string BCCEmail)
        {
            Result result = new Result();

            CultureInfo TurkishCultureInfo = new CultureInfo("tr-TR");

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.RaffleParticipantConfirmed,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                #region PreRaffleParticipantPart
                string preRaffleParticipantPart = null;
                if (raffleParticipant.TKRaffleParticipationType == TK.TKRaffleParticipationType.Paid)
                {
                    if (raffleParticipant.PaymentID.HasValue)
                    {
                        preRaffleParticipantPart = string.Format("{0} ödeme kaydı numaralı çekiliş katılımınız", raffleParticipant.PaymentID.Value.ToString("00000"));
                    }
                    else
                    {
                        preRaffleParticipantPart = "Ödeme kayıtlı çekiliş katılımınız";
                    }
                }
                else if (raffleParticipant.TKRaffleParticipationType == TK.TKRaffleParticipationType.MakeUp)
                {
                    preRaffleParticipantPart = "Telafi çekiliş katılımınız";
                }
                else if (raffleParticipant.TKRaffleParticipationType == TK.TKRaffleParticipationType.Free)
                {
                    preRaffleParticipantPart = "Ücretsiz çekiliş katılımınız";
                }
                else
                {
                    preRaffleParticipantPart = "Çekiliş katılımınız";
                }
                #endregion

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{PreRaffleParticipantPart}}",
                    ComponentValue = preRaffleParticipantPart,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleStartedAt}}",
                    ComponentValue = raffle.RaffleStartedAt.ToString("dd MMMM yyyy", TurkishCultureInfo),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleEndedAt}}",
                    ComponentValue = raffle.RaffleEndedAt.HasValue ? raffle.RaffleEndedAt.Value.ToString("dd MMMM yyyy", TurkishCultureInfo) : "",
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleTemplateName}}",
                    ComponentValue = raffleTemplate.Name,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{TargetFollowerCount}}",
                    ComponentValue = raffleParticipant.TargetFollowerCount.ToString(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CustomerFullName}}",
                    ComponentValue = string.Format("{0} {1}", customer.FirstName, customer.LastName),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{QualifiedSocialMediaAccount}}",
                    ComponentValue = string.Format("{0}: {1}", raffleParticipant.TKSocialMediaPlatform, raffleParticipant.SocialMediaAccountName),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CreatorFullName}}",
                    ComponentValue = CreatorFullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailUserBalanceAjaxEmail(UserBalance userBalance, string FullName, string Email, bool UseBCC, string BCCEmail)
        {
            Result result = new Result();

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.UserBalanceUpdated,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{UserBalanceID}}",
                    ComponentValue = userBalance.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{TKBalanceActionName}}",
                    ComponentValue = TypeService.GetNameByValue(userBalance.TKBalanceAction),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{ShownDescription}}",
                    ComponentValue = userBalance.ShownDescription,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{PreviousBalance}}",
                    ComponentValue = userBalance.PreviousBalance.ToThousandSeperatorWithCurrency(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Amount}}",
                    ComponentValue = userBalance.Amount.ToThousandSeperatorWithCurrency(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CurrentBalance}}",
                    ComponentValue = userBalance.CurrentBalance.ToThousandSeperatorWithCurrency(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{UserBalanceID}}",
                    ComponentValue = userBalance.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                if (UseBCC)
                {
                    this._iMorsService.SaveEmailComponent(new EmailComponent()
                    {
                        EmailID = emailID,
                        ComponentKey = "{{BCC}}",
                        ComponentValue = BCCEmail,
                        TKEmailComponent = TK.TKEmailComponent.BCC
                    });
                }

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailRaffleStartedAjaxEmail(RaffleParticipant raffleParticipant, Raffle raffle, RaffleTemplate raffleTemplate, SocialMediaAccount socialMediaAccount, string CreatorFullName, string FullName, string Email)
        {
            Result result = new Result();

            CultureInfo TurkishCultureInfo = new CultureInfo("tr-TR");

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.RaffleStarted,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleStartedAt}}",
                    ComponentValue = raffle.RaffleStartedAt.ToString("dd MMMM yyyy", TurkishCultureInfo),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleEndedAt}}",
                    ComponentValue = raffle.RaffleEndedAt.HasValue ? raffle.RaffleEndedAt.Value.ToString("dd MMMM yyyy", TurkishCultureInfo) : "",
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleTemplateName}}",
                    ComponentValue = raffleTemplate.Name,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{TargetFollowerCount}}",
                    ComponentValue = raffleParticipant.TargetFollowerCount.ToString(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CustomerFullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{QuailifiedSocialMediaAccount}}",
                    ComponentValue = socialMediaAccount.GetQualifiedName(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CreatorFullName}}",
                    ComponentValue = CreatorFullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendEmailRaffleEndedAjaxEmail(RaffleParticipant raffleParticipant, Raffle raffle, RaffleTemplate raffleTemplate, SocialMediaAccount socialMediaAccount, string CreatorFullName, string FullName, string Email)
        {
            Result result = new Result();

            CultureInfo TurkishCultureInfo = new CultureInfo("tr-TR");

            #region Mors Connection
            Email email = new Email()
            {
                EmailSenderID = (int)TK.TKSenderEmail.Info,
                EmailTemplateID = (int)TK.TKEmailTemplate.RaffleEnded,
                IsSent = false,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int emailID;
            if (this._iMorsService.SaveEmail(email, out emailID))
            {
                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleStartedAt}}",
                    ComponentValue = raffle.RaffleStartedAt.ToString("dd MMMM yyyy", TurkishCultureInfo),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleEndedAt}}",
                    ComponentValue = raffle.RaffleEndedAt.HasValue ? raffle.RaffleEndedAt.Value.ToString("dd MMMM yyyy", TurkishCultureInfo) : "",
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleTemplateName}}",
                    ComponentValue = raffleTemplate.Name,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{TargetFollowerCount}}",
                    ComponentValue = raffleParticipant.TargetFollowerCount.ToString(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CustomerFullName}}",
                    ComponentValue = FullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{QuailifiedSocialMediaAccount}}",
                    ComponentValue = socialMediaAccount.GetQualifiedName(),
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{CreatorFullName}}",
                    ComponentValue = CreatorFullName,
                    TKEmailComponent = TK.TKEmailComponent.Body
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKEmailComponent = TK.TKEmailComponent.Subject
                });

                this._iMorsService.SaveEmailComponent(new EmailComponent()
                {
                    EmailID = emailID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = Email,
                    TKEmailComponent = TK.TKEmailComponent.Receiver
                });

                result = this._iMorsService.SendEmail(email.UniqueKey);
            }
            #endregion

            return result;
        }
        #endregion

        #region Sms
        public Result SendSmsUserBillAjaxSms(string FullName, UserBill userBillItem, string PhoneNumber)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.YourBillIsReady,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{BillEditedAt}}",
                    ComponentValue = userBillItem.EditedAt.ToString("dd.MM.yyyy"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                string userBillURL = string.Format("{0}/Fatura?uk={1}", ConfigurationManager.AppSettings["CasaBaseURL"], userBillItem.UniqueKey);

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{BillURL}}",
                    ComponentValue = userBillURL,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsCustomerBillAjaxSms(string FullName, CustomerBill customerBillItem, string PhoneNumber)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.YourBillIsReady,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{BillEditedAt}}",
                    ComponentValue = customerBillItem.EditedAt.ToString("dd.MM.yyyy"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                string userBillURL = string.Format("{0}/C/Fatura?uk={1}", ConfigurationManager.AppSettings["CasaBaseURL"], customerBillItem.UniqueKey);

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{BillURL}}",
                    ComponentValue = userBillURL,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsAccountConfirmationAjaxSms(TK.TKSmsTemplate TKSmsTemplate, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TKSmsTemplate,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsOtherAjaxSms(string PhoneNumber, string SmsContent)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.Other,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Body}}",
                    ComponentValue = SmsContent,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsNotificationAjaxSms(List<string> PhoneNumberList, string SmsContent)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.Other,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.NtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Body}}",
                    ComponentValue = SmsContent,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                foreach (var item in PhoneNumberList)
                {
                    this._iMorsService.SaveSmsComponent(new SmsComponent()
                    {
                        SmsID = smsID,
                        ComponentKey = "{{Rec}}",
                        ComponentValue = item,
                        TKSmsComponent = TK.TKSmsComponent.Receiver
                    });
                }

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsPaymentAjaxSms(TK.TKSmsTemplate TKSmsTemplate, Payment Payment, RaffleTemplate raffleTemplate, string UserFullName, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TKSmsTemplate,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{PaymentID}}",
                    ComponentValue = Payment.ID.ToString("00000"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsRaffleParticipantConfirmedAjaxSms(RaffleParticipant raffleParticipant, Raffle raffle, RaffleTemplate raffleTemplate, string CreatorFullName, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            CultureInfo TurkishCultureInfo = new CultureInfo("tr-TR");

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.RaffleParticipantConfirmed,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                #region PreRaffleParticipantPart
                string preRaffleParticipantPart = null;
                if (raffleParticipant.TKRaffleParticipationType == TK.TKRaffleParticipationType.Paid)
                {
                    if (raffleParticipant.PaymentID.HasValue)
                    {
                        preRaffleParticipantPart = string.Format("{0} ödeme kaydı numaralı çekiliş katılımınız", raffleParticipant.PaymentID.Value.ToString("00000"));
                    }
                    else
                    {
                        preRaffleParticipantPart = "Ödeme kayıtlı çekiliş katılımınız";
                    }
                }
                else if (raffleParticipant.TKRaffleParticipationType == TK.TKRaffleParticipationType.MakeUp)
                {
                    preRaffleParticipantPart = "Telafi çekiliş katılımınız";
                }
                else if (raffleParticipant.TKRaffleParticipationType == TK.TKRaffleParticipationType.Free)
                {
                    preRaffleParticipantPart = "Ücretsiz çekiliş katılımınız";
                }
                else
                {
                    preRaffleParticipantPart = "Çekiliş katılımınız";
                }
                #endregion

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{PreRaffleParticipantPart}}",
                    ComponentValue = preRaffleParticipantPart,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsUserBalanceAjaxSms(UserBalance userBalance, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.UserBalanceUpdated,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{UserBalanceID}}",
                    ComponentValue = userBalance.ID.ToString("00000"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{TKBalanceActionName}}",
                    ComponentValue = TypeService.GetNameByValue(userBalance.TKBalanceAction),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{ShownDescription}}",
                    ComponentValue = userBalance.ShownDescription,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{PreviousBalance}}",
                    ComponentValue = userBalance.PreviousBalance.ToThousandSeperatorWithCurrency(),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Amount}}",
                    ComponentValue = userBalance.Amount.ToThousandSeperatorWithCurrency(),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{CurrentBalance}}",
                    ComponentValue = userBalance.CurrentBalance.ToThousandSeperatorWithCurrency(),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsRaffleStartedAjaxSms(RaffleParticipant raffleParticipant, Raffle raffle, RaffleTemplate raffleTemplate, SocialMediaAccount socialMediaAccount, string CreatorFullName, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            CultureInfo TurkishCultureInfo = new CultureInfo("tr-TR");

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.RaffleStarted,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }

        public Result SendSmsRaffleEndedAjaxSms(RaffleParticipant raffleParticipant, Raffle raffle, RaffleTemplate raffleTemplate, SocialMediaAccount socialMediaAccount, string CreatorFullName, string FullName, string PhoneNumber)
        {
            Result result = new Result();

            CultureInfo TurkishCultureInfo = new CultureInfo("tr-TR");

            #region Mors Connection
            Sms sms = new Sms()
            {
                SmsSenderTitleID = Settings.SmsSenderTitleID,
                SmsTemplateID = (int)TK.TKSmsTemplate.RaffleEnded,
                IsSent = false,
                TKSmsSendingType = TK.TKSmsSendingType.ONEtoN,
                TKResultType = TK.TKResultType.NoTrial,
                UniqueKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            int smsID;
            if (this._iMorsService.SaveSms(sms, out smsID))
            {
                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{FullName}}",
                    ComponentValue = FullName,
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{RaffleParticipantID}}",
                    ComponentValue = raffleParticipant.ID.ToString("00000"),
                    TKSmsComponent = TK.TKSmsComponent.Body
                });

                this._iMorsService.SaveSmsComponent(new SmsComponent()
                {
                    SmsID = smsID,
                    ComponentKey = "{{Rec}}",
                    ComponentValue = PhoneNumber,
                    TKSmsComponent = TK.TKSmsComponent.Receiver
                });

                result = this._iMorsService.SendSms(sms.UniqueKey);
            }
            #endregion

            return result;
        }
        #endregion
    }
}