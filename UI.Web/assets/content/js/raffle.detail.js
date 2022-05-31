$(document).ready(function () {
    if ($('.nav-tabs li.active').length == 0) {
        $('.nav-tabs li').first().addClass('active');
        $('.tab-content .tab-pane').first().addClass('active');
    }

    $("#tk_raffle_status, #tk_raffle_participation_status").select2({
        allowClear: true,
        placeholder: "Seçiniz",
        width: null
    });

    $('#raffle_started_at, #raffle_ended_at').datetimepicker({
        todayHighlight: true,
        language: 'tr',
        format: 'dd/mm/yyyy hh:ii'
    });

    $("#user_count_per_list").inputmask({
        mask: '9',
        repeat: 12,
        greedy: !1,
        allowMinus: false
    });

    datatableGenerator.responsiveNoPagination('#table_raffle_participant_list', 0, 'desc');
    datatableGenerator.normal('#customer_data_list', 1);
});

$('#select-all').on('change', function () {

    var controlState = $(this).prop('checked');
    var table = document.getElementById('table_raffle_participant_list');

    for (var i = 1; i < table.rows.length; i++) {
        var row = $(table.rows[i]);
        var inputCheckbox = row.find('.select-row');

        if (!inputCheckbox.prop('checked') == controlState) {
            inputCheckbox.click();
        }
    }

});

$('#open-raffle-participant-list-modal').on('click', function () {
    var rows = $('#raffle-participant-list').find('.raffle-participant');

    if (rows.length == 0) {
        toastr.warning('Katılımcı Özetinde herhangi bir kayıt bulunmuyor.');
        return;
    }

    var raffleParticipantList = [];
    for (var i = 0; i < rows.length; i++) {
        var row = $(rows[i]);

        var listNumber = row.find('.summary-raffle-participant-listnumber').text();
        var socialMediaAccountName = row.find('.summary-raffle-participant-social-media-account-name').text();

        var anyListNumber = $.grep(raffleParticipantList, function (val, i) {
            return val.ListNumber == listNumber;
        });

        if (anyListNumber.length == 0) {
            raffleParticipantList.push({
                ListNumber: listNumber,
                SMAList: [socialMediaAccountName]
            });
        }
        else {
            anyListNumber[0].SMAList.push(socialMediaAccountName);
        }
    }

    $('#grouped-raffle-participant-list .grouped-raffle-participant-item').remove();

    for (var i = 0; i < raffleParticipantList.length; i++) {
        var smaList = '';
        for (var j = 0; j < raffleParticipantList[i].SMAList.length; j++) {
            smaList += '@' + raffleParticipantList[i].SMAList[j];

            if (j != raffleParticipantList[i].SMAList.length - 1) {
                smaList += '<br>';
            }
        }

        var tr = $('<tr class="grouped-raffle-participant-item">');
        var tdListNumber = $('<td>').text(raffleParticipantList[i].ListNumber);
        var tdSocialMediaAccount = $('<td class="grouped-raffle-participant-social-media-account-name-list">').html(smaList);
        var tdButtonActions = $('<td class="text-center">').html('<a class="btn btn-sm copy-grouped-social-media-account-list"><i class="fa fa-copy"></i>&nbsp;Kopyala</a>');

        tdListNumber.appendTo(tr);
        tdSocialMediaAccount.appendTo(tr);
        tdButtonActions.appendTo(tr);

        tr.appendTo($('#grouped-raffle-participant-list'));
    }

    $('#copy-raffle-participant-list-modal').modal('show');
});

$('#grouped-raffle-participant-list').on('click', '.copy-grouped-social-media-account-list', function () {
    var smaList = $(this).closest('.grouped-raffle-participant-item').find('.grouped-raffle-participant-social-media-account-name-list').html().replaceAll('<br>', '\r\n');

    var temp = $('<textarea></textarea>');
    $('#copy-raffle-participant-list-modal').append(temp);
    temp.val(smaList).select();
    toastr.info("Sosyal Medya Hesapları Listesi Kopyalandı");
    document.execCommand('copy');
    temp.remove();
});

$('#btn-update-raffle-participation-status-list').on('click', function () {
    var table = document.getElementById('table_raffle_participant_list');

    var selectedList = [];
    for (var i = 1; i < table.rows.length; i++) {
        var row = $(table.rows[i]);
        var inputCheckbox = row.find('.select-row');

        if (inputCheckbox.prop('checked')) {
            selectedList.push(inputCheckbox.data('id'));
        }
    }

    if (selectedList.length == 0) {
        toastr.warning("Güncelleme yapabilmek için tablodan seçim yapmanız gerekmektedir.");
        return false;
    }

    $('#form-update-raffle-participation-status #raffle_participant_list').val(JSON.stringify(selectedList));
    $('#update-raffle-participant-status-modal').modal('show');
});


$('.send-email-raffle-participant').on('click', function () {
    var tkEmailTemplate = $(this).data('tkemailtemplate');
    if (tkEmailTemplate == '' || tkEmailTemplate == undefined || tkEmailTemplate == null || tkEmailTemplate == 0) {
        toastr.warning("Email Template bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var modalTitle;
    if (tkEmailTemplate == $('#TKEmailTemplate_RaffleStarted').val()) {
        modalTitle = "Çekilişimiz Başlamıştır Emaili Gönder";
    }
    else if (tkEmailTemplate == $('#TKEmailTemplate_RaffleEnded').val()) {
        modalTitle = "Çekilişimiz Sona Ermiştir Emaili Gönder";
    }
    else {
        toastr.warning("Email Template ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    $('#sendemail_rafflestatus_tkemailtemplate').val(tkEmailTemplate);
    $('#send-raffle-status-email-modal').find('.modal-title').text(modalTitle)
    $('#send-raffle-status-email-modal').modal('show');
});

$('#send-raffle-status-email-modal').on('hidden.bs.modal', function () {
    $('#sendemail_rafflestatus_tkemailtemplate').val('');
    $('#send-raffle-status-email-modal').find('.modal-title').text('');
});



$('.send-sms-raffle-participant').on('click', function () {
    var tkSmsTemplate = $(this).data('tksmstemplate');
    if (tkSmsTemplate == '' || tkSmsTemplate == undefined || tkSmsTemplate == null || tkSmsTemplate == 0) {
        toastr.warning("Sms Template bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    var modalTitle;
    if (tkSmsTemplate == $('#TKSmsTemplate_RaffleStarted').val()) {
        modalTitle = "Çekilişimiz Başlamıştır Sms'i Gönder";
    }
    else if (tkSmsTemplate == $('#TKSmsTemplate_RaffleEnded').val()) {
        modalTitle = "Çekilişimiz Sona Ermiştir Sms'i Gönder";
    }
    else {
        toastr.warning("Sms Template ID bulunamadı. Sayfayı yenileyip yeniden deneyiniz.");
        return;
    }

    $('#sendsms_rafflestatus_tksmstemplate').val(tkSmsTemplate);
    $('#send-raffle-status-sms-modal').find('.modal-title').text(modalTitle)
    $('#send-raffle-status-sms-modal').modal('show');
});

$('#send-payment-sms-modal').on('hidden.bs.modal', function () {
    $('#sendsms_rafflestatus_tksmstemplate').val('');
    $('#send-raffle-status-sms-modal').find('.modal-title').text('');
});