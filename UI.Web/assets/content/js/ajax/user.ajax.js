var User = (function () {
    'use strict';

    /* UpdateUserPersonalInfoAjax */
    function UpdateUserPersonalInfoAjaxBegin() {

        if ($('#firstname').val() == "") {
            toastr.warning("Ad giriniz.");
            $('#firstname').focus();
            return false;
        }

        if ($('#lastname').val() == "") {
            toastr.warning("Soyad giriniz.");
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

        if ($('#commission_rate').val() == "") {
            toastr.warning("Komisyon oranını giriniz.");
            $('#commission_rate').focus();
            return false;
        }

        if ($('#min_withdraw').val() == "") {
            toastr.warning("Minimum çekim tutarını giriniz.");
            $('#min_withdraw').focus();
            return false;
        }
        
        global.showLoader();

        return true;
    }

    function UpdateUserPersonalInfoAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdateUserPersonalInfoAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UpdateUserPersonalInfoAjax */


    /* UpdateUserProfileAjax */
    function UpdateUserProfileAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdateUserProfileAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdateUserProfileAjaxFailure(result) {
        updateSuccess(result);
    }
    /* UpdateUserProfileAjax */


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
        if (result.IsSuccess && result.Data.HashPassword != null) {
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
    

    /*  SendEmailAccountConfirmationAjax  */
    function SendEmailAccountConfirmationAjaxBegin() {

        if ($('#sendemail_confirmation_fullname').val() == '') {
            $('#sendemail_confirmation_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_confirmation_email').val() == '') {
            $('#sendemail_confirmation_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-account-confirmation input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_confirmation_bccemail').val() == '') {
            $('#sendemail_confirmation_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailAccountConfirmationAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-email-user-account-confirmation-modal').modal('hide');
        }
    }

    function SendEmailAccountConfirmationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailAccountConfirmationAjax  */


    /*  SendEmailUserBillAjax  */
    function SendEmailUserBillAjaxBegin() {
        if ($('#user_order_bill_id').val() == '') {
            toastr.warning('Fatura ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendemail_userbill_fullname').val() == '') {
            $('#sendemail_userbill_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_userbill_email').val() == '') {
            $('#sendemail_userbill_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-userbill input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_userbill_bccemail').val() == '') {
            $('#sendemail_userbill_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailUserBillAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-email-userbill-modal').modal('hide');
        }
    }

    function SendEmailUserBillAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailUserBillAjax  */


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


    /* BankAccountUpdateAjax */
    function BankAccountUpdateAjaxBegin() {
        if ($('#iban').val().replace('_', '') == '') {
            toastr.warning("IBAN Giriniz");
            $('#iban').focus();
            return false;
        }

        if ($('input[type="radio"][name="IncludeTR"]:checked').val() == 'true') {
            if (!IBAN.isValid($('#iban').val())) {
                toastr.warning('Girdiğiniz IBAN numarası geçersizdir. Lütfen kontrol edip yeniden giriniz.');
                $('#iban').focus();
                return false;
            }
        }

        if ($('#owner').val() == '') {
            toastr.warning("Hesap Sahibini Giriniz");
            $('#owner').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function BankAccountUpdateAjaxSuccess(result) {
        updateSuccess(result);
    }

    function BankAccountUpdateAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BankAccountUpdateAjax */


    /* BalanceDoPaymentAjax */
    function BalanceDoPaymentAjaxBegin() {
        if ($('#amount').val() == '') {
            toastr.warning('Ödenecek tutarı giriniz.');
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

    function BalanceDoPaymentAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            var table = $('#table_user_balance_list').DataTable();
            var removeReference = $('#object_' + result.Data.UserBalanceID);

            table
                .row(removeReference)
                .remove()
                .draw();

            $('#do-payment-modal').modal('hide');
        }
    }

    function BalanceDoPaymentAjaxFailure(result) {
        updateSuccess(result);
    }
    /* BalanceDoPaymentAjax */
    

    /*  SaveUserTeamAjax  */
    function SaveUserTeamAjaxBegin() {

        if ($('#name').val() == '') {
            toastr.warning('Takım adı giriniz.');
            $('#name').focus();
            return false;
        }

        if ($('#leader_user_id').val() == '') {
            toastr.warning('Takım lideri seçiniz.');
            $('#leader_user_id').select2('open');
            return false;
        }

        if ($('#commission_rate').val() != '' && parseFloat($('#commission_rate').val()) < 0) {
            toastr.warning('Takım liderinin komisyon oranı sıfırın altında olamaz.');
            $('#commission_rate').select2('open');
            return false;
        }

        if ($('#color_hex').val() == '') {
            toastr.warning('Takım rengi seçiniz.');
            $('#color_hex').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function SaveUserTeamAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            var table = $('#item_list').DataTable();

            var rowNode = table.row.add([
                '',
                result.Data.ID,
                result.Data.Name,
                result.ExtraData.LeaderUserFullName,
                result.ExtraData.CommissionRate,
                result.ExtraData.CreatedAtStr,
                '<a class="btn btn-xs edit-item"><i class="fa fa-edit"></i></a> ' +
                '<a class="btn btn-xs delete-item"><i class="fa fa-remove"></i></a>'
            ]).draw().node();

            $(rowNode).find('td').eq(0).addClass('hidden');
            $(rowNode).find('td').eq(5).addClass('text-center');
            $(rowNode).find('td').eq(6).addClass('text-center');

            $(rowNode).find('td').eq(2).attr('style', 'font-weight:bold; color:' + result.Data.ColorHex);

            $(rowNode)
                .addClass('object-item')
                .data('id', result.Data.ID)
                .attr('id', 'object_' + result.Data.ID);

            SerializedUserTeamList.push({
                ID: result.Data.ID,
                Name: result.Data.Name,
                LeaderUserID: result.Data.LeaderUserID,
                CommissionRate: result.Data.CommissionRate,
                ColorHex: result.Data.ColorHex
            });

            $('#add-user-team-modal').modal('hide');
        }
    }

    function SaveUserTeamAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SaveUserTeamAjax  */


    /*  UpdateUserTeamAjax  */
    function UpdateUserTeamAjaxBegin() {
        if ($('#update_name').val() == "") {
            toastr.warning("Paket adı giriniz");
            $('#update_name').focus();
            return false;
        }

        if ($('#update_leader_user_id').val() == '') {
            toastr.warning('Takım lideri seçiniz.');
            $('#update_leader_user_id').select2('open');
            return false;
        }

        if ($('#update_commission_rate').val() != '' && parseFloat($('#update_commission_rate').val()) < 0) {
            toastr.warning('Takım liderinin komisyon oranı sıfırın altında olamaz.');
            $('#update_commission_rate').select2('open');
            return false;
        }

        if ($('#update_color_hex').val() == '') {
            toastr.warning('Takım rengi seçiniz.');
            $('#update_color_hex').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateUserTeamAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            var table = $('#item_list').DataTable();

            var removeReference = $('#object_' + result.Data.ID);

            var currentData = table.row(removeReference).data();
            currentData[2] = result.Data.Name;
            currentData[3] = result.ExtraData.LeaderUserFullName;
            currentData[4] = result.ExtraData.CommissionRate;
            
            $(removeReference).find('td').eq(2).attr('style', 'font-weight:bold; color:' + result.Data.ColorHex);

            table
                .row(removeReference)
                .data(currentData)
                .draw();

            var itemArr = jQuery.grep(SerializedUserTeamList, function (value) {
                return value.ID == result.Data.ID;
            });

            if (itemArr.length != 0) {
                itemArr[0].Name = result.Data.Name;
                itemArr[0].LeaderUserID = result.Data.LeaderUserID;
                itemArr[0].CommissionRate = result.Data.CommissionRate;
                itemArr[0].ColorHex = result.Data.ColorHex;
            }

            $('#update-object-modal').modal('hide');
        }
    }

    function UpdateUserTeamAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateUserTeamAjax  */


    /*  SendNotificationAjax  */
    function SendNotificationAjaxBegin() {
        if ($('#title').val() == "") {
            toastr.warning("Başlık giriniz");
            $('#title').focus();
            return false;
        }

        if ($('#notification_content').val() == "") {
            toastr.warning("İçerik giriniz");
            $('#notification_content').focus();
            return false;
        }

        if ($('input[type="radio"][name="ReceiverType"]:checked').val() == '') {
            toastr.warning("Alıcı türünü seçiniz.");
            return false;
        }

        if ($('input[type="radio"][name="ReceiverType"]:checked').val() == '1' && $('#user_id').val() == null) {
            toastr.warning('Kullanıcı seçiniz.');
            $('#user_id').select2('open');
            return false;
        }

        if ($('input[type="radio"][name="ReceiverType"]:checked').val() == '2' && $('#user_team_id').val() == null) {
            toastr.warning('Takım seçiniz.');
            $('#user_team_id').select2('open');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendNotificationAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#form-send-notification input, #form-send-notification textarea, #form-send-notification select, #form-send-notification button').attr('disabled', 'disabled');
            $('.select-deselect').remove();
        }

        if (result.Data != null) {
            $('#notification-result').removeClass('hidden');
            $('#notification-result').find('.ReceiverCount').text(result.Data.ReceiverCount);
            $('#notification-result').find('.SuccessfulSendingNotificationCount').text(result.Data.SuccessfulSendingNotificationCount);
            $('#notification-result').find('.SuccessfulSendingSmsCount').text(result.Data.SuccessfulSendingSmsCount);
            $('#notification-result').find('.SuccessfulSendingEmailCount').text(result.Data.SuccessfulSendingEmailCount);
        }
        
    }

    function SendNotificationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendNotificationAjax  */


    /*  SendEmailUserBalanceAjax  */
    function SendEmailUserBalanceAjaxBegin() {

        if ($('#sendemail_userbalance_fullname').val() == '') {
            $('#sendemail_userbalance_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_userbalance_email').val() == '') {
            $('#sendemail_userbalance_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-user-balance input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_userbalance_bccemail').val() == '') {
            $('#sendemail_userbalance_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailUserBalanceAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-email-user-balance-modal').modal('hide');
        }
    }

    function SendEmailUserBalanceAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailUserBalanceAjax  */


    /*  SendSmsUserBillAjax  */
    function SendSmsUserBillAjaxBegin() {
        if ($('#sms_user_order_bill_id').val() == '') {
            toastr.warning('Fatura ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendsms_userbill_fullname').val() == '') {
            $('#sendsms_userbill_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendsms_userbill_phonenumber').val() == '') {
            $('#sendsms_userbill_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsUserBillAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-sms-userbill-modal').modal('hide');
        }
    }

    function SendSmsUserBillAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsUserBillAjax  */


    /*  SendSmsAccountConfirmationAjax  */
    function SendSmsAccountConfirmationAjaxBegin() {

        if ($('#sendsms_confirmation_fullname').val() == '') {
            $('#sendsms_confirmation_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendsms_confirmation_phonenumber').val() == '') {
            $('#sendsms_confirmation_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsAccountConfirmationAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-sms-user-account-confirmation-modal').modal('hide');
        }
    }

    function SendSmsAccountConfirmationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsAccountConfirmationAjax  */


    /*  SendSmsUserBalanceAjax  */
    function SendSmsUserBalanceAjaxBegin() {

        if ($('#sendsms_userbalance_fullname').val() == '') {
            $('#sendsms_userbalance_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendsms_userbalance_phonenumber').val() == '') {
            $('#sendsms_userbalance_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsUserBalanceAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-sms-user-balance-modal').modal('hide');
        }
    }

    function SendSmsUserBalanceAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsUserBalanceAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.hideLoader();
    }

    return {
        UpdateUserPersonalInfoAjaxBegin:   UpdateUserPersonalInfoAjaxBegin,
        UpdateUserPersonalInfoAjaxSuccess: UpdateUserPersonalInfoAjaxSuccess,
        UpdateUserPersonalInfoAjaxFailure: UpdateUserPersonalInfoAjaxFailure,

        UpdateUserProfileAjaxBegin:   UpdateUserProfileAjaxBegin,
        UpdateUserProfileAjaxSuccess: UpdateUserProfileAjaxSuccess,
        UpdateUserProfileAjaxFailure: UpdateUserProfileAjaxFailure,

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

        SendEmailAccountConfirmationAjaxBegin:   SendEmailAccountConfirmationAjaxBegin,
        SendEmailAccountConfirmationAjaxSuccess: SendEmailAccountConfirmationAjaxSuccess,
        SendEmailAccountConfirmationAjaxFailure: SendEmailAccountConfirmationAjaxFailure,

        SendEmailUserBillAjaxBegin:   SendEmailUserBillAjaxBegin,
        SendEmailUserBillAjaxSuccess: SendEmailUserBillAjaxSuccess,
        SendEmailUserBillAjaxFailure: SendEmailUserBillAjaxFailure,

        NoteSaveAjaxBegin:   NoteSaveAjaxBegin,
        NoteSaveAjaxSuccess: NoteSaveAjaxSuccess,
        NoteSaveAjaxFailure: NoteSaveAjaxFailure,

        BankAccountUpdateAjaxBegin:   BankAccountUpdateAjaxBegin,
        BankAccountUpdateAjaxSuccess: BankAccountUpdateAjaxSuccess,
        BankAccountUpdateAjaxFailure: BankAccountUpdateAjaxFailure,

        BalanceDoPaymentAjaxBegin:   BalanceDoPaymentAjaxBegin,
        BalanceDoPaymentAjaxSuccess: BalanceDoPaymentAjaxSuccess,
        BalanceDoPaymentAjaxFailure: BalanceDoPaymentAjaxFailure,

        SaveUserTeamAjaxBegin: SaveUserTeamAjaxBegin,
        SaveUserTeamAjaxSuccess: SaveUserTeamAjaxSuccess,
        SaveUserTeamAjaxFailure: SaveUserTeamAjaxFailure,

        UpdateUserTeamAjaxBegin: UpdateUserTeamAjaxBegin,
        UpdateUserTeamAjaxSuccess: UpdateUserTeamAjaxSuccess,
        UpdateUserTeamAjaxFailure: UpdateUserTeamAjaxFailure,

        SendNotificationAjaxBegin:   SendNotificationAjaxBegin,
        SendNotificationAjaxSuccess: SendNotificationAjaxSuccess,
        SendNotificationAjaxFailure: SendNotificationAjaxFailure,

        SendEmailUserBalanceAjaxBegin:   SendEmailUserBalanceAjaxBegin,
        SendEmailUserBalanceAjaxSuccess: SendEmailUserBalanceAjaxSuccess,
        SendEmailUserBalanceAjaxFailure: SendEmailUserBalanceAjaxFailure,

        SendSmsUserBillAjaxBegin:   SendSmsUserBillAjaxBegin,
        SendSmsUserBillAjaxSuccess: SendSmsUserBillAjaxSuccess,
        SendSmsUserBillAjaxFailure: SendSmsUserBillAjaxFailure,

        SendSmsAccountConfirmationAjaxBegin:   SendSmsAccountConfirmationAjaxBegin,
        SendSmsAccountConfirmationAjaxSuccess: SendSmsAccountConfirmationAjaxSuccess,
        SendSmsAccountConfirmationAjaxFailure: SendSmsAccountConfirmationAjaxFailure,

        SendSmsUserBalanceAjaxBegin:   SendSmsUserBalanceAjaxBegin,
        SendSmsUserBalanceAjaxSuccess: SendSmsUserBalanceAjaxSuccess,
        SendSmsUserBalanceAjaxFailure: SendSmsUserBalanceAjaxFailure,
    }
})();