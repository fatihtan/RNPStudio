$(document).ready(function () {
    if ($('.nav-tabs li.active').length == 0) {
        $('.nav-tabs li').first().addClass('active');
        $('.tab-content .tab-pane').first().addClass('active');
    }

    $("#tk_payment_type, #tk_payment_status").select2({
        allowClear: true,
        placeholder: "Seçiniz",
        width: null
    });

    $("#bank_account_id, #user_bank_account_id").select2({
        allowClear: true,
        placeholder: "Seçiniz",
        width: null,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#totalprice, #amountpaid").inputmask({
        'alias': 'decimal',
        rightAlign: false,
        showMaskOnHover: false,
        'autoGroup': false,
        'digits': 2,
        'digitsOptional': false,
        'placeholder': '0,00',
        autoGroup: true,
        allowMinus: false,
        radixPoint: $('#IsLocal').val() == '' ? "," : "."
    });

    global.queryStringResult();
});

$('#tk_payment_type').on('change', function () {
    var value = $(this).val();

    if (value == '2') {
        $('#is-company-bank-account-receiver-true').hide('slow');
        $('#is-company-bank-account-receiver-false').hide('slow');

        $('#tk-payment-type-transfer').show('slow');
    }
    else {
        $('#tk-payment-type-transfer').hide('slow');

        $('#bank_account_id').removeAttr('required');
        $('#user_bank_account_id').removeAttr('required');

        $('input[type=radio][name="IsCompanyBankAccountReceiver"]:checked').removeAttr('checked');

        $('#bank_account_id').val(null).trigger('change');
        $('#user_bank_account_id').val(null).trigger('change');
    }
});

$('input[type=radio][name="IsCompanyBankAccountReceiver"]').change(function () {
    if (this.value == "true") {
        $('#is-company-bank-account-receiver-true').show('slow');
        $('#is-company-bank-account-receiver-false').hide('slow');

        $('#bank_account_id').attr('required', 'required');
        $('#user_bank_account_id').removeAttr('required');
        $('#user_bank_account_id').val(null).trigger('change');
    }
    else if (this.value == "false") {
        $('#is-company-bank-account-receiver-false').show('slow');
        $('#is-company-bank-account-receiver-true').hide('slow');

        $('#user_bank_account_id').attr('required', 'required');
        $('#bank_account_id').removeAttr('required');
        $('#bank_account_id').val(null).trigger('change');
    }
});

$('#form-save-payment-status-relation-ajax').ajaxForm({
    beforeSend: function (e) {
        global.showLoader();
    },
    complete: function (result) {
        global.hideLoader();

        result = result.responseJSON;

        if (result != undefined) {
            if (result.IsSuccess) {
                toastr.success(result.Message);

                if (result.Data.PaymentStatusRelationList.length) {
                    updateBadgesForPaymentStatusRelation(result.Data.PaymentStatusRelationList);
                    addPaymentStatusRelation(result.Data.PaymentStatusRelation, result.Data.CreatedAtStr, result.Data.TKPaymentStatusName);
                    clearPaymentStatusRelationFields();
                    $('#last-tk-payment-status').text(result.Data.TKPaymentStatusName);
                }
            }
            else {
                toastr.error(result.Message)
            }
        }
        else {
            toastr.error('Internal Server Error');
        }
    }
});

$('#form-send-email-payment input[name="UseBCC"]').on('change', function () {
    if (this.value == "true") {
        $('#div-send-email-payment-bcc-email').removeClass('hidden');
        $('#sendemail_payment_bccemail').attr('required', 'required');
    }
    else if (this.value == "false") {
        $('#div-send-email-payment-bcc-email').addClass('hidden');
        $('#sendemail_payment_bccemail').removeAttr('required');
    }
})

$('.send-payment-email').on('click', function () {
    var tkEmailTemplate = $(this).data('tkemailtemplate');
    if (tkEmailTemplate == '' || tkEmailTemplate == undefined || tkEmailTemplate == null || tkEmailTemplate == 0) {
        toastr.warning("Email Template bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var modalTitle;
    if (tkEmailTemplate == $('#TKEmailTemplate_PaymentInProgress').val()) {
        modalTitle = "Ödemeniz Kontrol Ediliyor Emaili Gönder";
    }
    else if (tkEmailTemplate == $('#TKEmailTemplate_PaymentConfirmed').val()) {
        modalTitle = "Ödemeniz Onaylandı Emaili Gönder";
    }
    else if (tkEmailTemplate == $('#TKEmailTemplate_PaymentNotConfirmed').val()) {
        modalTitle = "Ödemeniz Onaylanamadı Emaili Gönder";
    }
    else {
        toastr.warning("Email Template ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var tkactor = $(this).data('tkactor');
    if (tkactor == $('.TKActor_User').val()) {
        $('#sendemail_payment_fullname').val($('.TKActor_UserFullName').val());
        $('#sendemail_payment_email').val($('#TKActor_UserEmail').val())
    }
    else if (tkactor == $('.TKActor_Customer').val()) {
        $('#sendemail_payment_fullname').val($('.TKActor_CustomerFullName').val());
        $('#sendemail_payment_email').val($('#TKActor_CustomerEmail').val())
    }
    else {
        toastr.warning("TKActor bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    $('#sendemail_payment_tkemailtemplate').val(tkEmailTemplate);
    $('#send-payment-email-modal').find('.modal-title').text(modalTitle)
    $('#send-payment-email-modal').modal('show');
});

$('#send-payment-email-modal').on('hidden.bs.modal', function () {
    $('#sendemail_payment_fullname').val('');
    $('#sendemail_payment_email').val('');
    $('#sendemail_payment_tkemailtemplate').val('');
    $('#send-payment-email-modal').find('.modal-title').text('');
});


$('.send-payment-sms').on('click', function () {
    var tkSmsTemplate = $(this).data('tksmstemplate');
    if (tkSmsTemplate == '' || tkSmsTemplate == undefined || tkSmsTemplate == null || tkSmsTemplate == 0) {
        toastr.warning("Sms Template bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var modalTitle;
    if (tkSmsTemplate == $('#TKSmsTemplate_PaymentInProgress').val()) {
        modalTitle = "Ödemeniz Kontrol Ediliyor Sms'i Gönder";
    }
    else if (tkSmsTemplate == $('#TKSmsTemplate_PaymentConfirmed').val()) {
        modalTitle = "Ödemeniz Onaylandı Sms'i Gönder";
    }
    else if (tkSmsTemplate == $('#TKSmsTemplate_PaymentNotConfirmed').val()) {
        modalTitle = "Ödemeniz Onaylanamadı Sms'i Gönder";
    }
    else {
        toastr.warning("Sms Template ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var tkactor = $(this).data('tkactor');
    if (tkactor == $('.TKActor_User').val()) {
        $('#sendsms_payment_fullname').val($('.TKActor_UserFullName').val());

        $("#sendsms_payment_phonenumber").phoneNumberMask({
            countryCode: $('#TKActor_UserCountryCode').val(),
            phoneNumber: $('#TKActor_UserPhoneNumber').val()
        });

        $("#sendsms_payment_phonenumber").val('+' + $('#TKActor_UserCountryCode').val() + ' ' + $('#TKActor_UserPhoneNumber').val());
    }
    else if (tkactor == $('.TKActor_Customer').val()) {
        $('#sendsms_payment_fullname').val($('.TKActor_CustomerFullName').val());

        $("#sendsms_payment_phonenumber").phoneNumberMask({
            countryCode: $('#TKActor_CustomerCountryCode').val(),
            phoneNumber: $('#TKActor_CustomerPhoneNumber').val()
        });

        $("#sendsms_payment_phonenumber").val('+' + $('#TKActor_CustomerCountryCode').val() + ' ' + $('#TKActor_CustomerPhoneNumber').val());
    }
    else {
        toastr.warning("TKActor bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    $('#sendsms_payment_tksmstemplate').val(tkSmsTemplate);
    $('#send-payment-sms-modal').find('.modal-title').text(modalTitle)
    $('#send-payment-sms-modal').modal('show');
});

$('#send-payment-sms-modal').on('hidden.bs.modal', function () {
    $('#sendsms_payment_fullname').val('');
    $('#sendsms_payment_phonenumber').val('');
    $('#sendsms_payment_tksmstemplate').val('');
    $('#send-payment-sms-modal').find('.modal-title').text('');
});


$('#payment-status-relation-list').on('click', '.update-payment-status-relation', function () {
    var id = $(this).data('id');

    $('#payment_status_update_id').val(id);
    
    var row = $(this).closest('.payment-status-relation-row');
    var isActive = row.find('.payment-status-isactive').data('state');
    var isDeleted = row.find('.payment-status-isdeleted').data('state');

    if (isActive) {
        $('#payment_status_update_is_active').prop('checked', true).change();
    }
    else {
        $('#payment_status_update_is_active').prop('checked', false).change();
    }

    if (isDeleted) {
        $('#payment_status_update_is_deleted').prop('checked', false).change();
    }
    else {
        $('#payment_status_update_is_deleted').prop('checked', true).change();
    }


    $('#payment-status-update-modal').modal('show');
});

function updateBadgesForPaymentStatusRelation(list) {
    for (var i = 0; i < list.length; i++) {
        var payment = list[i];

        var paymentRow = $('#payment-status-relation-list .payment-status-relation-' + payment.ID)
        if (paymentRow.length) {

            if (payment.IsActive) {
                paymentRow.find('.payment-status-isactive').find('.label').removeClass('label-warning');
                paymentRow.find('.payment-status-isactive').find('.label').addClass('label-info');
                paymentRow.find('.payment-status-isactive').find('.label').text('AKTİF');
                paymentRow.find('.payment-status-isactive').attr('data-state', 1);
                paymentRow.find('.payment-status-isactive').data('state', 1);
            }
            else {
                paymentRow.find('.payment-status-isactive').find('.label').removeClass('label-info');
                paymentRow.find('.payment-status-isactive').find('.label').addClass('label-warning');
                paymentRow.find('.payment-status-isactive').find('.label').text('PASİF');
                paymentRow.find('.payment-status-isactive').attr('data-state', 0);
                paymentRow.find('.payment-status-isactive').data('state', 0);
            }


            if (payment.IsDeleted) {
                paymentRow.find('.payment-status-isdeleted').find('.label').removeClass('label-info');
                paymentRow.find('.payment-status-isdeleted').find('.label').addClass('label-danger');
                paymentRow.find('.payment-status-isdeleted').find('.label').text('EVET');
                paymentRow.find('.payment-status-isdeleted').attr('data-state', 1);
                paymentRow.find('.payment-status-isdeleted').data('state', 1);
            }
            else {
                paymentRow.find('.payment-status-isdeleted').find('.label').removeClass('label-danger');
                paymentRow.find('.payment-status-isdeleted').find('.label').addClass('label-info');
                paymentRow.find('.payment-status-isdeleted').find('.label').text('HAYIR');
                paymentRow.find('.payment-status-isdeleted').attr('data-state', 0);
                paymentRow.find('.payment-status-isdeleted').data('state', 0);
            }
        }
    }
}

function addPaymentStatusRelation(PaymentStatusRelation, CreatedAtStr, TKPaymentStatusName) {
    var tr = $(
        '<tr class="payment-status-relation-row payment-status-relation-' + PaymentStatusRelation.ID + '">' +
            '<td class="col-md-2">' + TKPaymentStatusName + '</td>' +
            '<td class="wrap-line">' + (PaymentStatusRelation.Description != null ? PaymentStatusRelation.Description : '') + '</td>' +
            '<td class="col-md-1 text-center">' +
                '<a class="payment-status-isactive update-payment-status-relation" data-id="' + PaymentStatusRelation.ID + '" data-state="' + (PaymentStatusRelation.IsActive ? 1 : 0) + '">' +
                    '<span class="label label-sm label-info">' +
                        'AKTİF' +
                    '</span>' +
                '</a>' +
            '</td>' +
            '<td class="col-md-1 text-center">' +
                '<a class="payment-status-isdeleted update-payment-status-relation" data-id="' + PaymentStatusRelation.ID + '" data-state="' + (PaymentStatusRelation.IsDeleted ? 1 : 0) + '">' +
                    '<span class="label label-sm label-info">' +
                        'HAYIR' +
                    '</span>' +
                '</a>' +
            '</td>' +
            '<td class="text-center">' +
                (PaymentStatusRelation.ReceiptURL != null ? '<a target="_blank" href="' + $('#CDNBaseURL').val() + PaymentStatusRelation.ReceiptURL + '"><i class="fa fa-download"></i>İndir</a>' : '') +
            '</td>' +
            '<td class="col-md-1">' + PaymentStatusRelation.IPAddress + '</td>' +
            '<td class="col-md-1">' + CreatedAtStr + '</td>' +
        '</tr>');

    tr.appendTo($('#payment-status-relation-list tbody'));
}

function clearPaymentStatusRelationFields() {
    $('#tk_payment_status').val(null).trigger('change');
    $('#description').val('');
    $('.fileinput-exists[data-dismiss="fileinput"]').click();
}