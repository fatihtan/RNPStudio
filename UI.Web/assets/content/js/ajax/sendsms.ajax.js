var SendSms = (function () {
    'use strict';

    /*  SendSmsOtherAjax  */
    function SendSmsOtherAjaxBegin() {

        if ($('#sendsms_other_country_id').val() == '') {
            toastr.warning('Ülke seçiniz');
            $('#sendsms_other_country_id').select2('open');
            return false;
        }

        if ($('#sendsms_other_phonenumber').val() == '') {
            $('#sendsms_other_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        if ($('#sendsms_other_content').val() == '') {
            $('#sendsms_other_content').focus();
            toastr.warning('Mesaj metnini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsOtherAjaxSuccess(result) {
        updateSuccess(result);
    }

    function SendSmsOtherAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsOtherAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.hideLoader();
    }

    return {
        SendSmsOtherAjaxBegin:   SendSmsOtherAjaxBegin,
        SendSmsOtherAjaxSuccess: SendSmsOtherAjaxSuccess,
        SendSmsOtherAjaxFailure: SendSmsOtherAjaxFailure
    }
})();