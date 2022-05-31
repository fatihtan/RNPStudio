var Payment = (function () {
    'use strict';

    /*  UpdatePaymentActivationAjax  */
    function UpdatePaymentActivationAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdatePaymentActivationAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdatePaymentActivationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdatePaymentActivationAjax  */


    /*  UpdatePaymentFinancialConfirmationAjax  */
    function UpdatePaymentFinancialConfirmationAjaxBegin() {

        if ($('#is_financial_confirmed').prop('checked') == false) {
            toastr.warning("Finansal onay verebilmeniz için switch'i açmanız gerekmektedir.");
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdatePaymentFinancialConfirmationAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            if (result.Data.FromPaymentList) {

                var table = $('#item_list').DataTable();
                var removeReference = $('#object_' + result.Data.ID);
                var currentData = table.row(removeReference).data();
                currentData[5] = '<span class="label label-sm label-info">EVET</span>';
                currentData[7] = 'Finansal Onay Verildi';
                currentData[9] =
                    '<a class="btn btn-xs text-proper disabled">Onayla</a> ' +
                    '<a href="/Payment/Detail/' + result.Data.ID + '" class="btn btn-xs text-proper">Git</a>';

                $(removeReference).find('td').eq(7).removeClass('font-green');
                $(removeReference).find('td').eq(7).removeClass('font-red');

                table
                    .row(removeReference)
                    .data(currentData)
                    .draw();

                $('#financial-confirmation-modal').modal('hide');
            }
            else {
                $('.payment-financial-confirmed-false').remove();

                $('.financial-confirmed-at').text(result.Data.FinancialConfirmedAt);
                $('.payment-financial-confirmed-true').removeClass('hidden');
            }
        }
    }

    function UpdatePaymentFinancialConfirmationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdatePaymentFinancialConfirmationAjax  */


    /*  UpdatePaymentTypeAndBankAccountAjax  */
    function UpdatePaymentTypeAndBankAccountAjaxBegin() {

        if ($('#tk_payment_type').val() == '2') {
            if ($('input[type="radio"][name="IsCompanyBankAccountReceiver"]:checked').val() == undefined) {
                toastr.warning('Ödeme yapılan banka türünü seçiniz.');
                return false;
            }

            if ($('input[type="radio"][name="IsCompanyBankAccountReceiver"]:checked').val() == 'true') {
                if ($('#bank_account_id').val() == '') {
                    toastr.warning('RNP Reklam banka hesaplarından banka seçimi yapınız.');
                    $('#bank_account_id').select2('open');
                    return false;
                }
            }

            if ($('input[type="radio"][name="IsCompanyBankAccountReceiver"]:checked').val() == 'false') {
                if ($('#user_bank_account_id').val() == '') {
                    toastr.warning('Kullanıcı banka hesaplarından banka seçimi yapınız.');
                    $('#user_bank_account_id').select2('open');
                    return false;
                }
            }
        }

        global.showLoader();
        return true;
    }

    function UpdatePaymentTypeAndBankAccountAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdatePaymentTypeAndBankAccountAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdatePaymentTypeAndBankAccountAjax  */


    /*  UpdatePaymentPriceAjax  */
    function UpdatePaymentPriceAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdatePaymentPriceAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdatePaymentPriceAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdatePaymentPriceAjax  */


    /*  UpdatePaymentStatusRelationAjax  */
    function UpdatePaymentStatusRelationAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdatePaymentStatusRelationAjaxSuccess(result) {
        updateSuccess(result);
        if (result.IsSuccess) {
            if (result.Data != null) {
                var row = $('.payment-status-relation-' + result.Data.PaymentStatusRelation.ID);
                var isActiveBadge = row.find('.payment-status-isactive');
                var isDeletedBadge = row.find('.payment-status-isdeleted');

                if (result.Data.PaymentStatusRelation.IsActive) {
                    isActiveBadge.find('.label').removeClass('label-warning');
                    isActiveBadge.find('.label').addClass('label-info');
                    isActiveBadge.find('.label').text('AKTİF');
                    isActiveBadge.attr('data-state', 1);
                    isActiveBadge.data('state', 1);
                }
                else {
                    isActiveBadge.find('.label').removeClass('label-info');
                    isActiveBadge.find('.label').addClass('label-warning');
                    isActiveBadge.find('.label').text('PASİF');
                    isActiveBadge.attr('data-state', 0);
                    isActiveBadge.data('state', 0);
                }

                if (result.Data.PaymentStatusRelation.IsDeleted) {
                    isDeletedBadge.find('.label').removeClass('label-info');
                    isDeletedBadge.find('.label').addClass('label-danger');
                    isDeletedBadge.find('.label').text('EVET');
                    isDeletedBadge.attr('data-state', 1);
                    isDeletedBadge.data('state', 1);
                }
                else {
                    isDeletedBadge.find('.label').removeClass('label-danger');
                    isDeletedBadge.find('.label').addClass('label-info');
                    isDeletedBadge.find('.label').text('HAYIR');
                    isDeletedBadge.attr('data-state', 0);
                    isDeletedBadge.data('state', 0);
                }
            }
            
            $('#payment-status-update-modal').modal('hide');
        }
    }

    function UpdatePaymentStatusRelationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdatePaymentStatusRelationAjax  */


    /*  UpdatePaymentBalanceConfirmAjax  */
    function UpdatePaymentBalanceConfirmAjaxBegin() {

        if ($('#description_balance_confirm').val() == '') {
            $('#description_balance_confirm').focus();
            toastr.warning('Bakiye Onaylama işlemine devam edebilmeniz için açıklama bölümünü doldurmalısınız.');
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdatePaymentBalanceConfirmAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('.payment-balance-confirmation-false').remove();
            $('.payment-balance-confirmation-true').removeClass('hidden');
        }
    }

    function UpdatePaymentBalanceConfirmAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdatePaymentBalanceConfirmAjax  */


    /*  SendEmailPaymentAjax  */
    function SendEmailPaymentAjaxBegin() {
        if ($('#sendemail_payment_tkemailtemplate').val() == '') {
            toastr.warning('Tanımlanamayan Email Template algılandı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendemail_payment_paymentid').val() == '') {
            toastr.warning('Ödeme kaydına ait ID bilgisi bulunamadı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendemail_payment_fullname').val() == '') {
            $('#sendemail_payment_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_payment_email').val() == '') {
            $('#sendemail_payment_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-payment input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_payment_bccemail').val() == '') {
            $('#sendemail_payment_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailPaymentAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-payment-email-modal').modal('hide');
        }
    }

    function SendEmailPaymentAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailPaymentAjax  */


    /*  SendSmsPaymentAjax  */
    function SendSmsPaymentAjaxBegin() {
        if ($('#sendsms_payment_tksmstemplate').val() == '') {
            toastr.warning('Tanımlanamayan Sms Template algılandı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendsms_payment_paymentid').val() == '') {
            toastr.warning('Ödeme kaydına ait ID bilgisi bulunamadı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendsms_payment_fullname').val() == '') {
            $('#sendsms_payment_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendsms_payment_phonenumber').val() == '') {
            $('#sendsms_payment_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsPaymentAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-payment-sms-modal').modal('hide');
        }
    }

    function SendSmsPaymentAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsPaymentAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.loaderStop('#loaderProcess');
    }

    return {
        UpdatePaymentActivationAjaxBegin:   UpdatePaymentActivationAjaxBegin,
        UpdatePaymentActivationAjaxSuccess: UpdatePaymentActivationAjaxSuccess,
        UpdatePaymentActivationAjaxFailure: UpdatePaymentActivationAjaxFailure,

        UpdatePaymentFinancialConfirmationAjaxBegin:   UpdatePaymentFinancialConfirmationAjaxBegin,
        UpdatePaymentFinancialConfirmationAjaxSuccess: UpdatePaymentFinancialConfirmationAjaxSuccess,
        UpdatePaymentFinancialConfirmationAjaxFailure: UpdatePaymentFinancialConfirmationAjaxFailure,

        UpdatePaymentTypeAndBankAccountAjaxBegin:   UpdatePaymentTypeAndBankAccountAjaxBegin,
        UpdatePaymentTypeAndBankAccountAjaxSuccess: UpdatePaymentTypeAndBankAccountAjaxSuccess,
        UpdatePaymentTypeAndBankAccountAjaxFailure: UpdatePaymentTypeAndBankAccountAjaxFailure,

        UpdatePaymentPriceAjaxBegin:   UpdatePaymentPriceAjaxBegin,
        UpdatePaymentPriceAjaxSuccess: UpdatePaymentPriceAjaxSuccess,
        UpdatePaymentPriceAjaxFailure: UpdatePaymentPriceAjaxFailure,

        UpdatePaymentStatusRelationAjaxBegin:   UpdatePaymentStatusRelationAjaxBegin,
        UpdatePaymentStatusRelationAjaxSuccess: UpdatePaymentStatusRelationAjaxSuccess,
        UpdatePaymentStatusRelationAjaxFailure: UpdatePaymentStatusRelationAjaxFailure,

        UpdatePaymentBalanceConfirmAjaxBegin:   UpdatePaymentBalanceConfirmAjaxBegin,
        UpdatePaymentBalanceConfirmAjaxSuccess: UpdatePaymentBalanceConfirmAjaxSuccess,
        UpdatePaymentBalanceConfirmAjaxFailure: UpdatePaymentBalanceConfirmAjaxFailure,

        SendEmailPaymentAjaxBegin:   SendEmailPaymentAjaxBegin,
        SendEmailPaymentAjaxSuccess: SendEmailPaymentAjaxSuccess,
        SendEmailPaymentAjaxFailure: SendEmailPaymentAjaxFailure,

        SendSmsPaymentAjaxBegin:   SendSmsPaymentAjaxBegin,
        SendSmsPaymentAjaxSuccess: SendSmsPaymentAjaxSuccess,
        SendSmsPaymentAjaxFailure: SendSmsPaymentAjaxFailure
    }
})();