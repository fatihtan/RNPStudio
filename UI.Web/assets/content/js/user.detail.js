$(document).ready(function () {
    if ($('.nav-tabs li.active').length == 0) {
        $('.nav-tabs li').first().addClass('active');
        $('.tab-content .tab-pane').first().addClass('active');
    }

    $("#city_id, #tk_job_title, #tk_industry, #tk_balance_action, #user_team_id").select2({
        allowClear: true,
        placeholder: "Seçiniz",
        width: null
    });

    CDNBaseURL = $('#CDNBaseURL').val();

    $("#country_id").select2({
        placeholder: '<i class="fa fa-map-marker"></i>&nbsp;Ülke Seçiniz',
        templateResult: function (state, el) {
            if (!state.id) {
                return state.text;
            }

            var $state = $(
                '<span><img src="' + CDNBaseURL + state.element.dataset.url + '" class="img-flag" /> ' + state.text + ' +' + state.element.dataset.code + '</span>'
            )

            return $state;
        },
        templateSelection: function (state, el) {
            if (!state.id) {
                return state.text;
            }

            var $state = $(
                '<span><img src="' + CDNBaseURL + state.element.dataset.url + '" class="img-flag" /> +' + state.element.dataset.code + '</span>'
            )

            return $state;
        },
        width: 'auto',
        escapeMarkup: function (m) {
            return m;
        }
    });

    $('#country_id').trigger('change');

    $('#birthdate').datepicker({
        autoclose: true,
        language: 'tr',
        format: 'dd/mm/yyyy'
    })

    $("#birthdate").inputmask("d/m/y", {
        autoUnmask: false
    });

    $('.input-phone-number').phoneNumberMask();

    $("#amount, #commission_rate, #min_withdraw").inputmask({
        'alias': 'decimal',
        rightAlign: false,
        showMaskOnHover: true,
        'autoGroup': false,
        'digits': 2,
        'digitsOptional': false,
        'placeholder': '0,00',
        autoGroup: true,
        radixPoint: $('#IsLocal').val() == '' ? "," : "."
    });

    $("#userbillinfo_id").select2({
        placeholder: "Seçiniz",
        width: null,
        escapeMarkup: function (m) {
            return m;
        }
    });

    global.queryStringResult();

    openTab();

    datatableGenerator.responsive('#table_payment_list', 0);
    datatableGenerator.normal('#table_bill_info_list', 8);
    datatableGenerator.normal('#table_user_bill_list', 5);
    datatableGenerator.normal('#table_message_list', 7);
    datatableGenerator.responsive('#table_user_balance_list', 0);
    datatableGenerator.responsive('#table_raffle_participant_list', 0);
    datatableGenerator.normal('#table_customer_list', 1);
    datatableGenerator.normal('#table_bank_account_list', 1);
});

$('#country_id').on('change', function () {
    if (this.value == 228) {
        $("#phonenumber").inputmask("mask", { mask: "(999) 999-9999" });
    }
    else {
        $("#phonenumber").inputmask("mask", { mask: "9", repeat: "15", greedy: !1 });
    }
})

$('#phonenumber').on('keyup', function (e) {
    if ($('#country_id').val() == 228) {
        if ((this.value == '' && e.which == 48) || this.value.startsWith('(0')) {
            $('#phonenumber').val('');
        }
    }
});

$('#tk_job_title').on('change', function () {
    var id = $(this).val();

    if (id == $('#OtherTKJobTitle').val()) {
        $('#div_other_job_title').removeClass('hidden');
    }
    else {
        $('#div_other_job_title').addClass('hidden');
    }
});

$('#tk_industry').on('change', function () {
    var id = $(this).val();

    if (id == $('#OtherTKIndustry').val()) {
        $('#div_other_industry').removeClass('hidden');
    }
    else {
        $('#div_other_industry').addClass('hidden');
    }
});

$('.copy-casa').on('click', function () {
    var url = $(this).data('url');
    if (url == undefined || url == '') {
        toastr.warning("UniqueKey veya CasaURL Bulunamadı");
        return;
    }

    var temp = $('<input></input>');
    $('body').append(temp);
    temp.val(url).select();
    toastr.info("Casa Fatura bağlantısı kopyalandı");
    document.execCommand('copy');
    temp.remove();
});

$('#form-profile-photo-save-ajax').ajaxForm({
    beforeSend: function (e) {
        global.showLoader();
    },
    complete: function (result) {
        global.hideLoader();

        result = result.responseJSON;

        if (result.IsSuccess) {
            toastr.success(result.Message);
        }
        else {
            toastr.error(result.Message)
        }
    }
});

$('#amount').on('keypress, keyup', function () {

    var amount = this.value;
    if (amount == '') {
        amount = 0;
    }

    if (amount != 0) {
        amount = amount.replaceAll(',', '.');
        amount = parseFloat(amount);
    }
    
    var currentBalance = $('#hidden_balance_current_balance').val();
    if (currentBalance == '') {
        currentBalance = 0;
    }

    currentBalance = parseFloat(currentBalance);
    currentBalance = currentBalance + amount;

    $('#balance_current_balance').val(currentBalance.toFixed(2));
});

$('.copy-iban').on('click', function () {
    var iban = $(this).data('iban');

    var temp = $('<input>');
    $('body').append(temp);
    temp.val(iban).select();
    document.execCommand('copy');
    temp.remove();

    toastr.info('IBAN kopyalandı.');
});

$('#table_bank_account_list').on('click', '.delete-bank-account', function () {
    var item = $(this).closest('.bank-account-item');
    var id = item.data('id');

    if (id == null || id == 'null' || id == undefined || id == '') {
        toastr.warning("Seçilen banka hesabı silinemiyor. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var iban = item.find('.iban').text();

    $('#delete_bank_account_id').val(id);
    $('#bank_account_name').text(iban);
    $('#delete-bank-account-modal').modal('show');
});

$("#delete-bank-account-modal").on("hidden.bs.modal", function () {
    $('#delete_bank_account_id').val('');
    $('#bank_account_name').text('');
});

function DeleteBankAccount() {
    var id = $('#delete_bank_account_id').val();

    if (id == null || id == 'null' || id == undefined || id == '') {
        toastr.warning("Banka hesabı silinemiyor. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    global.showLoader();

    $.post('/User/BankAccountUpdateIsDeletedAjax',
        { 'ID': id })
        .success(function (r) {
            if (r.IsSuccess) {

                var table = $('#table_bank_account_list').DataTable();
                var removeReference = $('#bank_account_id_' + id);

                table
                    .row(removeReference)
                    .remove()
                    .draw();

                $('#delete-bank-account-modal').modal('hide');

                toastr.success(r.Message);
            }
            else {
                toastr.warning(r.Message);
            }
        })
        .fail(function () {
            toastr.error("Internal Server Error");
        })
        .always(function () {
            global.hideLoader();
        })
}

$('.send-email-user-account-confirmation').on('click', function () {
    var tkemailtemplate = $(this).data('tkemailtemplate');

    if (tkemailtemplate == $('#TKEmailTemplate_UserAccountConfirmed').val()) {
        $('#user-account-confirmation-modal-title').text('Hesabınız Onaylandı Emaili Gönder');
    }
    else if (tkemailtemplate == $('#TKEmailTemplate_UserAccountNotConfirmed').val()) {
        $('#user-account-confirmation-modal-title').text('Hesabınız Onaylanamadı Emaili Gönder');
    }
    else {
        toastr.warning('Email Template bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.');
        return;
    }

    $('#TKEmailTemplate').val(tkemailtemplate);

    $('#send-email-user-account-confirmation-modal').modal('show');
});

$("#send-email-user-account-confirmation-modal").on("hidden.bs.modal", function () {
    $('#user-account-confirmation-modal-title').text('');
});

$('.send-sms-user-account-confirmation').on('click', function () {
    var tksmstemplate = $(this).data('tksmstemplate');

    if (tksmstemplate == $('#TKSmsTemplate_UserAccountConfirmed').val()) {
        $('#sms-user-account-confirmation-modal-title').text("Hesabınız Onaylandı Sms'i Gönder");
    }
    else if (tksmstemplate == $('#TKSmsTemplate_UserAccountNotConfirmed').val()) {
        $('#sms-user-account-confirmation-modal-title').text("Hesabınız Onaylanamadı Sms'i Gönder");
    }
    else {
        toastr.warning('Sms Template bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.');
        return;
    }

    $('#TKSmsTemplate').val(tksmstemplate);

    $('#send-sms-user-account-confirmation-modal').modal('show');
});

$("#send-sms-user-account-confirmation-modal").on("hidden.bs.modal", function () {
    $('#sms-user-account-confirmation-modal-title').text('');
});

$('#form-send-email-account-confirmation input[name="UseBCC"]').on('change', function () {
    if (this.value == "true") {
        $('#div-send-email-confirmation-bcc-email').removeClass('hidden');
        $('#sendemail_confirmation_bccemail').attr('required', 'required');
    }
    else if (this.value == "false") {
        $('#div-send-email-confirmation-bcc-email').addClass('hidden');
        $('#sendemail_confirmation_bccemail').removeAttr('required');
    }
});

$('#form-send-email-user-balance input[name="UseBCC"]').on('change', function () {
    if (this.value == "true") {
        $('#div-send-email-userbalance-bcc-email').removeClass('hidden');
        $('#sendemail_userbalance_bccemail').attr('required', 'required');
    }
    else if (this.value == "false") {
        $('#div-send-email-userbalance-bcc-email').addClass('hidden');
        $('#sendemail_userbalance_bccemail').removeAttr('required');
    }
});

function openTab() {
    var tab = global.getParameterByName('tab');
    if (tab != null) {
        $('a[href="#' + tab + '"]').click();
    }
}