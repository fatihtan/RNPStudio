var Customer = (function () {
    'use strict';

    /* UpdateCustomerPersonalInfoAjax */
    function UpdateCustomerPersonalInfoAjaxBegin() {

        if ($('#firstname').val().trim() == '') {
            toastr.warning("Müşteri adını giriniz.");
            $('#firstname').focus();
            return false;
        }

        if ($('#lastname').val().trim() == '') {
            toastr.warning("Müşteri soyadını giriniz.");
            $('#lastname').focus();
            return false;
        }

        if ($('#country_id').val() == '') {
            toastr.warning("Ülke seçiniz.");
            $('#country_id').select2('open');
            return false;
        }

        if ($('#phonenumber').val() == "") {
            toastr.warning("Telefon numarası giriniz.");
            $('#phonenumber').focus();
            return false;
        }
        
        global.showLoader();
        return true;
    }

    function UpdateCustomerPersonalInfoAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdateCustomerPersonalInfoAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UpdateCustomerPersonalInfoAjax */


    /* UpdateCustomerSecuritySettingsAjax */
    function UpdateCustomerSecuritySettingsAjaxBegin() {

        if ($('#email').val() == "") {
            toastr.warning("Müşteri email adresini giriniz.");
            $('#email').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateCustomerSecuritySettingsAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdateCustomerSecuritySettingsAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UpdateCustomerSecuritySettingsAjax */


    /* UpdateCustomerProfileAjax */
    function UpdateCustomerProfileAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdateCustomerProfileAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdateCustomerProfileAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UpdateCustomerProfileAjax */


    /* UpdateSecuritySettingsAjax */
    function UpdateSecuritySettingsAjaxBegin() {
        if ($('#email').val() == "") {
            toastr.warning("Email giriniz.");
            $('#email').focus();
            return false;
        }

        if ($('#password').val() == "") {
            toastr.warning("Şifre giriniz.");
            $('#password').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateSecuritySettingsAjaxSuccess(result) {
        updateSuccess(result);
        if (result.IsSuccess) {
            $('#password').val(result.Data.HashPassword);
        }
    }

    function UpdateSecuritySettingsAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UpdateSecuritySettingsAjax */


    /* UserBalanceSaveAjax */
    function UserBalanceSaveAjaxBegin() {
        if ($('#tk_balance_action').val() == "") {
            toastr.warning("İşlem türü seçiniz.");
            $('#tk_balance_action').select2('open');
            return false;
        }

        if ($('#amount').val() == '') {
            toastr.warning("Bakiye İşlem tutarını giriniz.");
            $('#amount').focus();
            return false;
        }

        if ($('#balance_description').val() == '') {
            toastr.warning("Açıklama giriniz.");
            $('#balance_description').focus();
            return false;
        }

        if ($('#balance_showndescription').val() == '') {
            toastr.warning("Görünen Açıklama giriniz.");
            $('#balance_showndescription').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UserBalanceSaveAjaxSuccess(result) {
        
        if (result.IsSuccess) {
            window.location.href = '/User/Detail/' + result.Data + '/?tab=user_balance&IsSuccess=' + result.IsSuccess + '&Message=' + result.Message;
        }
        else {
            updateSuccess(result);
        }
    }

    function UserBalanceSaveAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UserBalanceSaveAjax */


    /* BillUpdateCoreAjax */
    function BillUpdateCoreAjaxBegin() {

        if ($('#billno').val() == "") {
            toastr.warning("Fatura no giriniz")
            $('#billno').focus();
            return false;
        }

        if ($('#stringeditedat').val() == '') {
            toastr.warning("Fatura düzenlenme tarihini giriniz");
            $('#stringeditedat').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function BillUpdateCoreAjaxSuccess(result) {
        updateSuccess(result);
    }

    function BillUpdateCoreAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BillUpdateCoreAjax */


    /* BillUpdatePriceAjax */
    function BillUpdatePriceAjaxBegin() {
        if ($('#subtotalprice').val() == '') {
            toastr.warning("Alt toplam tutarı giriniz")
            return false;
        }

        if ($('#vatprice').val() == '') {
            toastr.warning("KDV tutarını giriniz")
            return false;
        }

        if ($('#totalprice').val() == '') {
            toastr.warning("Toplam tutarı giriniz")
            return false;
        }

        global.showLoader();
        return true;
    }

    function BillUpdatePriceAjaxSuccess(result) {
        updateSuccess(result);
    }

    function BillUpdatePriceAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BillUpdatePriceAjax */


    /* BillUpdateInformationAjax */
    function BillUpdateInformationAjaxBegin() {
        var isCorporate = $('input[type=radio][name="BillIsCorporate"]:checked').val();

        if (isCorporate == "true") {
            if ($('#billcompanytitle').val() == "") {
                toastr.warning("Şirket ünvanını giriniz");
                $('#billcompanytitle').focus();
                return false;
            }

            if ($('#billtaxno').val() == "") {
                toastr.warning("Vergi numarasını giriniz");
                $('#billtaxno').focus();
                return false;
            }

            if ($('#billtaxoffice').val() == "") {
                toastr.warning("Vergi dairesini giriniz");
                $('#billtaxoffice').focus();
                return false;
            }
        }
        else {
            if ($('#billfullname').val() == "") {
                toastr.warning("Ad Soyad giriniz");
                $('#billfullname').focus();
                return false;
            }
        }

        if ($('#billfulladdress').val() == "") {
            toastr.warning("Fatura adresini giriniz");
            $('#billfulladdress').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function BillUpdateInformationAjaxSuccess(result) {
        updateSuccess(result);
    }

    function BillUpdateInformationAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BillUpdateInformationAjax */


    /* BillInfoSaveAjax */
    function BillInfoSaveAjaxBegin() {

        var isCorporate = $('input[type=radio][name="IsCorporate"]:checked').val();

        if (isCorporate == "true") {
            if ($('#companytitle').val() == "") {
                toastr.warning("Şirket ünvanını giriniz.");
                $('#companytitle').focus();
                return false;
            }

            if ($('#taxno').val() == "") {
                toastr.warning("Vergi numarasını giriniz.");
                $('#taxno').focus();
                return false;
            }

            if ($('#taxoffice').val() == "") {
                toastr.warning("Vergi dairesini giriniz.");
                $('#taxoffice').focus();
                return false;
            }
        }
        else {
            if ($('#fullname').val() == "") {
                toastr.warning("Ad Soyad giriniz.");
                $('#fullname').focus();
                return false;
            }
        }

        if ($('#fulladdress').val() == "") {
            toastr.warning("Fatura adresini giriniz.");
            $('#fulladdress').focus();
            return false;
        }

        global.loaderStart('#loaderProcess');
        return true;
    }

    function BillInfoSaveAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#submit').attr('disabled', 'disabled');
        }
    }

    function BillInfoSaveAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BillInfoSaveAjax */


    /* BillInfoUpdateAjax */
    function BillInfoUpdateAjaxBegin() {

        var isCorporate = $('input[type=radio][name="IsCorporate"]:checked').val();

        if (isCorporate == "true") {
            if ($('#companytitle').val() == "") {
                toastr.warning("Şirket ünvanını giriniz.");
                $('#companytitle').focus();
                return false;
            }

            if ($('#taxno').val() == "") {
                toastr.warning("Vergi numarasını giriniz.");
                $('#taxno').focus();
                return false;
            }

            if ($('#taxoffice').val() == "") {
                toastr.warning("Vergi dairesini giriniz.");
                $('#taxoffice').focus();
                return false;
            }
        }
        else {
            if ($('#fullname').val() == "") {
                toastr.warning("Ad Soyad giriniz.");
                $('#fullname').focus();
                return false;
            }
        }

        if ($('#fulladdress').val() == "") {
            toastr.warning("Fatura adresini giriniz.");
            $('#fulladdress').focus();
            return false;
        }

        global.loaderStart('#loaderProcess');
        return true;
    }

    function BillInfoUpdateAjaxSuccess(result) {
        updateSuccess(result);
    }

    function BillInfoUpdateAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BillInfoUpdateAjax */


    /* BalanceUpdateAjax */
    function BalanceUpdateAjaxBegin() {
        
        if ($('#description').val() == "") {
            toastr.warning("Açıklama bölümünü giriniz.");
            $('#description').focus();
            return false;
        }

        if ($('#shown_description').val() == "") {
            toastr.warning("Görünen açıklama bölümünü giriniz.");
            $('#shown_description').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function BalanceUpdateAjaxSuccess(result) {
        updateSuccess(result);
    }

    function BalanceUpdateAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BalanceUpdateAjax */
    

    /*  SendEmailCustomerBillAjax  */
    function SendEmailCustomerBillAjaxBegin() {
        if ($('#customer_order_bill_id').val() == '') {
            toastr.warning('Fatura ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendemail_customerbill_fullname').val() == '') {
            $('#sendemail_customerbill_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_customerbill_email').val() == '') {
            $('#sendemail_customerbill_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-customerbill input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_customerbill_bccemail').val() == '') {
            $('#sendemail_customerbill_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailCustomerBillAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-email-customerbill-modal').modal('hide');
        }
    }

    function SendEmailCustomerBillAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailCustomerBillAjax  */


    /*  NoteSaveAjax  */
    function NoteSaveAjaxBegin() {
        if ($('#note_content').val() == '') {
            $('#note_content').focus();
            toastr.warning('Not giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function NoteSaveAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#add-note-modal').modal('hide');
        }
    }

    function NoteSaveAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  NoteSaveAjax  */


    /*  SendSmsCustomerBillAjax  */
    function SendSmsCustomerBillAjaxBegin() {
        if ($('#sms_customer_order_bill_id').val() == '') {
            toastr.warning('Fatura ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendsms_customerbill_fullname').val() == '') {
            $('#sendsms_customerbill_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendsms_customerbill_phonenumber').val() == '') {
            $('#sendsms_customerbill_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsCustomerBillAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-sms-customerbill-modal').modal('hide');
        }
    }

    function SendSmsCustomerBillAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsCustomerBillAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.hideLoader();
    }

    return {
        UpdateCustomerPersonalInfoAjaxBegin:   UpdateCustomerPersonalInfoAjaxBegin,
        UpdateCustomerPersonalInfoAjaxSuccess: UpdateCustomerPersonalInfoAjaxSuccess,
        UpdateCustomerPersonalInfoAjaxFailure: UpdateCustomerPersonalInfoAjaxFailure,

        UpdateCustomerSecuritySettingsAjaxBegin:   UpdateCustomerSecuritySettingsAjaxBegin,
        UpdateCustomerSecuritySettingsAjaxSuccess: UpdateCustomerSecuritySettingsAjaxSuccess,
        UpdateCustomerSecuritySettingsAjaxFailure: UpdateCustomerSecuritySettingsAjaxFailure,

        UpdateCustomerProfileAjaxBegin:   UpdateCustomerProfileAjaxBegin,
        UpdateCustomerProfileAjaxSuccess: UpdateCustomerProfileAjaxSuccess,
        UpdateCustomerProfileAjaxFailure: UpdateCustomerProfileAjaxFailure,

        UpdateSecuritySettingsAjaxBegin:   UpdateSecuritySettingsAjaxBegin,
        UpdateSecuritySettingsAjaxSuccess: UpdateSecuritySettingsAjaxSuccess,
        UpdateSecuritySettingsAjaxFailure: UpdateSecuritySettingsAjaxFailure,

        UserBalanceSaveAjaxBegin:   UserBalanceSaveAjaxBegin,
        UserBalanceSaveAjaxSuccess: UserBalanceSaveAjaxSuccess,
        UserBalanceSaveAjaxFailure: UserBalanceSaveAjaxFailure,

        BillUpdateCoreAjaxBegin:   BillUpdateCoreAjaxBegin,
        BillUpdateCoreAjaxSuccess: BillUpdateCoreAjaxSuccess,
        BillUpdateCoreAjaxFailure: BillUpdateCoreAjaxFailure,

        BillUpdatePriceAjaxBegin:   BillUpdatePriceAjaxBegin,
        BillUpdatePriceAjaxSuccess: BillUpdatePriceAjaxSuccess,
        BillUpdatePriceAjaxFailure: BillUpdatePriceAjaxFailure,

        BillUpdateInformationAjaxBegin: BillUpdateInformationAjaxBegin,
        BillUpdateInformationAjaxSuccess: BillUpdateInformationAjaxSuccess,
        BillUpdateInformationAjaxFailure: BillUpdateInformationAjaxFailure,

        BillInfoSaveAjaxBegin:   BillInfoSaveAjaxBegin,
        BillInfoSaveAjaxSuccess: BillInfoSaveAjaxSuccess,
        BillInfoSaveAjaxFailure: BillInfoSaveAjaxFailure,

        BillInfoUpdateAjaxBegin:   BillInfoUpdateAjaxBegin,
        BillInfoUpdateAjaxSuccess: BillInfoUpdateAjaxSuccess,
        BillInfoUpdateAjaxFailure: BillInfoUpdateAjaxFailure,

        BalanceUpdateAjaxBegin:   BalanceUpdateAjaxBegin,
        BalanceUpdateAjaxSuccess: BalanceUpdateAjaxSuccess,
        BalanceUpdateAjaxFailure: BalanceUpdateAjaxFailure,

        SendEmailCustomerBillAjaxBegin:   SendEmailCustomerBillAjaxBegin,
        SendEmailCustomerBillAjaxSuccess: SendEmailCustomerBillAjaxSuccess,
        SendEmailCustomerBillAjaxFailure: SendEmailCustomerBillAjaxFailure,

        NoteSaveAjaxBegin:   NoteSaveAjaxBegin,
        NoteSaveAjaxSuccess: NoteSaveAjaxSuccess,
        NoteSaveAjaxFailure: NoteSaveAjaxFailure,

        SendSmsCustomerBillAjaxBegin:   SendSmsCustomerBillAjaxBegin,
        SendSmsCustomerBillAjaxSuccess: SendSmsCustomerBillAjaxSuccess,
        SendSmsCustomerBillAjaxFailure: SendSmsCustomerBillAjaxFailure
    }
})();