var SendEmail = (function () {
    'use strict';


    /*  SendEmailOtherAjax  */
    function SendEmailOtherAjaxBegin() {

        if ($('#sendemail_other_subject').val() == '') {
            $('#sendemail_other_subject').focus();
            toastr.warning('Email konusunu giriniz');
            return false;
        }

        if ($('#sendemail_other_fullname').val() == '') {
            $('#sendemail_other_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_other_email').val() == '') {
            $('#sendemail_other_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-other input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_other_bccemail').val() == '') {
            $('#sendemail_other_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailOtherAjaxSuccess(result) {
        updateSuccess(result);
    }

    function SendEmailOtherAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailOtherAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.hideLoader();
    }

    return {
        SendEmailOtherAjaxBegin:   SendEmailOtherAjaxBegin,
        SendEmailOtherAjaxSuccess: SendEmailOtherAjaxSuccess,
        SendEmailOtherAjaxFailure: SendEmailOtherAjaxFailure
    }
})();