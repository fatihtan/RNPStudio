var Raffle = (function () {
    'use strict';

    /*  UpdateRaffleParticipantActivationAjax  */
    function UpdateRaffleParticipantActivationAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdateRaffleParticipantActivationAjaxSuccess(result) {
        updateSuccess(result);
    }

    function UpdateRaffleParticipantActivationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleParticipantActivationAjax  */


    /*  UpdateRaffleParticipantTypeAndStatusAjax  */
    function UpdateRaffleParticipantTypeAndStatusAjaxBegin() {

        if ($('#tk_raffle_participant_type').val() == '') {
            toastr.warning('Katılım türünü seçiniz.');
            $('#tk_raffle_participant_type').select2('open');
            return false;
        }

        if ($('#tk_raffle_participant_status').val() == '') {
            toastr.warning('Katılım durumunu seçiniz.');
            $('#tk_raffle_participant_status').select2('open');
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleParticipantTypeAndStatusAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('.raffle-participant-type').text(result.Data.TKRaffleParticipationTypeName);
            $('.raffle-participant-status').text(result.Data.TKRaffleParticipationStatusName);
        }
    }

    function UpdateRaffleParticipantTypeAndStatusAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleParticipantTypeAndStatusAjax  */


    /*  UpdateRaffleParticipantListNumberAjax  */
    function UpdateRaffleParticipantListNumberAjaxBegin(m, n) {

        if (n.data.indexOf('AutoGenerate=false') != -1) {
            if ($('#listnumber').val() == '') {
                toastr.warning('Liste numarasını giriniz.');
                $('#listnumber').focus();
                return false;
            }
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleParticipantListNumberAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('.raffle-participant-listnumber').val(result.Data.ListNumber);
            $('.raffle-participant-listsequence').val(result.Data.ListSequence);
            $('.raffle-participant-listcode').text(result.Data.ListCode);
        }
    }

    function UpdateRaffleParticipantListNumberAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleParticipantListNumberAjax  */


    /*  UpdateRaffleParticipantConfirmationAjax  */
    function UpdateRaffleParticipantConfirmationAjaxBegin() {

        if ($('#IsConfirmed').prop('checked') == false) {
            toastr.warning("İşlem onayı verebilmeniz için switch'i açmanız gerekmektedir.");
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleParticipantConfirmationAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {

            if (result.Data.FromRaffleParticipantList) {
                var table = $('#table_raffle_participant_list').DataTable();
                var removeReference = $('#object_' + result.Data.ID);
                var currentData = table.row(removeReference).data();
                currentData[3] = result.Data.ListCode;
                currentData[5] = result.Data.TKRaffleParticipationStatusName;
                currentData[9] = '<span class="label label-sm label-success">EVET</span>';
                currentData[10] = '<span class="label label-sm label-info">AKTİF</span>';
                currentData[12] = '<a class="btn btn-xs text-proper disabled">Onayla</a>';
                currentData[13] = result.Data.ConfirmedAt;

                table
                    .row(removeReference)
                    .data(currentData)
                    .draw();

                $('#raffle-participant-confirmation-modal').modal('hide');
            }
            else {
                $('.raffle-participant-confirmed-false').remove();
                $('.raffle-participant-confirmed-true').removeClass('hidden');

                $('.raffle-participant-confirmed-at').text(result.Data.ConfirmedAtStr);
                $('#tk_raffle_participant_status').val(result.Data.TKRaffleParticipationStatus).change();
                $('.raffle-participant-status').text(result.Data.TKRaffleParticipationStatusName);
                $('.raffle-participant-listnumber').val(result.Data.ListNumber);
                $('.raffle-participant-listsequence').val(result.Data.ListSequence);
                $('.raffle-participant-listcode').text(result.Data.ListCode);
                $('#IsShown').prop('checked', result.Data.IsShown).change();
                $('#IsActive').prop('checked', result.Data.IsActive).change();
                $('#IsDeleted').prop('checked', result.Data.IsDeleted).change();

                if (result.Data.IsConfirmed) {
                    $('.raffle-participant-isconfirmed').html('<span class="label label-sm label-success">EVET</span>');
                }

                if (result.Data.IsShown) {
                    $('.raffle-participant-isshown').html('<span class="label label-sm label-info">EVET</span>');
                }

                if (result.Data.IsActive) {
                    $('.raffle-participant-isactive').html('<span class="label label-sm label-info">AKTİF</span>');
                }

                if (result.Data.IsDeleted) {
                    $('.raffle-participant-isdeleted').html('<span class="label label-sm label-info">HAYIR</span>');
                }
            }
        }
    }

    function UpdateRaffleParticipantConfirmationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleParticipantConfirmationAjax  */


    /*  SaveRaffleTemplateAjax  */
    function SaveRaffleTemplateAjaxBegin() {

        if ($('#name').val() == '') {
            toastr.warning('Paket adı giriniz.');
            $('#name').focus();
            return false;
        }

        if ($('#target_follower_count').val() == '') {
            toastr.warning('Hedef takipçi sayısını giriniz.');
            $('#target_follower_count').focus();
            return false;
        }

        if ($('#price').val() == '') {
            toastr.warning('Fiyat giriniz.');
            $('#price').focus();
            return false;
        }

        if ($('#apply_commission_rate').prop('checked') && $('#commission_rate').val() == '') {
            toastr.warning('Komisyon oranını giriniz.');
            $('#commission_rate').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function SaveRaffleTemplateAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            var table = $('#item_list').DataTable();

            var rowNode = table.row.add([
                '',
                result.Data.ID,
                result.Data.Name,
                result.Data.TargetFollowerCount,
                result.ExtraData.PriceStr,
                result.Data.ApplyCommissionRate ?
                '<span class="label label-sm label-success">AKTİF</span>' : '<span class="label label-sm label-warning">PASİF</span>',
                result.ExtraData.CommissionRateStr,
                result.Data.IsActive ?
                '<span class="label label-sm label-info">AKTİF</span>' : '<span class="label label-sm label-danger">PASİF</span>',
                result.ExtraData.CreatedAtStr,
                '<a class="btn btn-xs edit-item"><i class="fa fa-edit"></i></a> ' +
                '<a class="btn btn-xs delete-item"><i class="fa fa-remove"></i></a>'
            ]).draw().node();

            $(rowNode).find('td').eq(0).addClass('hidden');
            $(rowNode).find('td').eq(5).addClass('text-center');
            $(rowNode).find('td').eq(6).addClass('text-center');
            $(rowNode).find('td').eq(7).addClass('text-center');
            $(rowNode).find('td').eq(8).addClass('text-center');
            $(rowNode).find('td').eq(9).addClass('text-center');

            $(rowNode)
                .addClass('object-item')
                .data('id', result.Data.ID)
                .attr('id', 'object_' + result.Data.ID);

            SerializedRaffleTemplateList.push({
                ID: result.Data.ID,
                Name: result.Data.Name,
                TargetFollowerCount: result.Data.TargetFollowerCount,
                ApplyCommissionRate: result.Data.ApplyCommissionRate,
                CommissionRate: result.Data.CommissionRate,
                Price: result.Data.Price,
                IsActive: result.Data.IsActive
            });

            $('#add-raffle-template-modal').modal('hide');
        }
    }

    function SaveRaffleTemplateAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SaveRaffleTemplateAjax  */


    /*  UpdateRaffleTemplateAjax  */
    function UpdateRaffleTemplateAjaxBegin() {
        if ($('#update_name').val() == "") {
            toastr.warning("Paket adı giriniz");
            $('#update_name').focus();
            return false;
        }

        if ($('#update_target_follower_count').val() == '') {
            toastr.warning('Hedef takipçi sayısını giriniz.');
            $('#update_target_follower_count').focus();
            return false;
        }

        if ($('#update_price').val() == '') {
            toastr.warning('Fiyat giriniz.');
            $('#update_price').focus();
            return false;
        }

        if ($('#update_apply_commission_rate').prop('checked') && $('#update_commission_rate').val() == '') {
            toastr.warning('Komisyon oranını giriniz.');
            $('#update_commission_rate').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleTemplateAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            var table = $('#item_list').DataTable();

            var removeReference = $('#object_' + result.Data.ID);

            var currentData = table.row(removeReference).data();
            currentData[2] = result.Data.Name;
            currentData[3] = result.Data.TargetFollowerCount;
            currentData[4] = result.ExtraData.PriceStr;
            currentData[6] = result.ExtraData.CommissionRateStr;

            if (result.Data.ApplyCommissionRate) {
                currentData[5] = '<span class="label label-sm label-success">AKTİF</span>';
            }
            else {
                currentData[5] = '<span class="label label-sm label-warning">PASİF</span>';
            }

            if (result.Data.IsActive) {
                currentData[7] = '<span class="label label-sm label-info">AKTİF</span>';
            }
            else {
                currentData[7] = '<span class="label label-sm label-danger">PASİF</span>';
            }

            table
                .row(removeReference)
                .data(currentData)
                .draw();

            var itemArr = jQuery.grep(SerializedRaffleTemplateList, function (value) {
                return value.ID == result.Data.ID;
            });

            if (itemArr.length != 0) {
                itemArr[0].Name = result.Data.Name;
                itemArr[0].TargetFollowerCount = result.Data.TargetFollowerCount;
                itemArr[0].Price = result.Data.Price;
                itemArr[0].ApplyCommissionRate = result.Data.ApplyCommissionRate;
                itemArr[0].CommissionRate = result.Data.CommissionRate;
                itemArr[0].IsActive = result.Data.IsActive;
            }

            $('#update-object-modal').modal('hide');
        }
    }

    function UpdateRaffleTemplateAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleTemplateAjax  */


    /*  SaveRaffleAjax  */
    function SaveRaffleAjaxBegin() {

        if ($('#raffle_template_id').val() == '') {
            toastr.warning('Çekiliş paketi seçiniz.');
            $('#raffle_template_id').select2('open');
            return false;
        }

        if ($('#raffle_started_at').val() == '') {
            toastr.warning('Başlangıç tarihini giriniz.');
            $('#raffle_started_at').focus();
            return false;
        }

        if ($('#user_count_per_list').val() == '') {
            toastr.warning('Liste kapastitesini giriniz.');
            $('#user_count_per_list').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function SaveRaffleAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            if (result.Data != null && result.Data.RaffleCalendarModel != null) {
                var calendar = $('#calendar');
                calendar.fullCalendar();

                var event = getEvent(result.Data.RaffleCalendarModel);
                calendar.fullCalendar('renderEvent', event, true);

                var currentCount = $('#raffle-count').data('rafflecount');
                currentCount = parseInt(currentCount) + 1;
                $('#raffle-count').text(currentCount);
                $('#raffle-count').data('rafflecount', currentCount);
            }
        }
    }

    function SaveRaffleAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SaveRaffleAjax  */


    /*  UpdateRaffleAjax  */
    function UpdateRaffleAjaxBegin() {

        if ($('#user_count_per_list').val() == '') {
            toastr.warning('Liste kapastitesini giriniz.');
            $('#user_count_per_list').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            if (result.Data != null && result.Data.RaffleCalendarModel != null) {
                var calendar = $('#calendar');
                calendar.fullCalendar();

                calendar.fullCalendar('removeEvents', [result.Data.RaffleCalendarModel.ID]);

                var event = getEvent(result.Data.RaffleCalendarModel);
                calendar.fullCalendar('renderEvent', event, true);
            }

            $('#update-raffle-modal').modal('hide');
        }
    }

    function UpdateRaffleAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleAjax  */


    /*  UpdateRaffleActivationAjax  */
    function UpdateRaffleActivationAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdateRaffleActivationAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess && result.Data != null) {
            if (result.Data.IsActive) {
                $('.raffle-isactive').html('<span class="label label-sm label-info">AKTİF</span>')
            }
            else {
                $('.raffle-isactive').html('<span class="label label-sm label-warning">PASİF</span>')
            }

            if (result.Data.IsDeleted) {
                $('.raffle-isdeleted').html('<span class="label label-sm label-danger">EVET</span>')
            }
            else {
                $('.raffle-isdeleted').html('<span class="label label-sm label-info">HAYIR</span>')
            }
        }
    }

    function UpdateRaffleActivationAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleActivationAjax  */


    /*  UpdateRaffleAverageFollowerCountAjax  */
    function UpdateRaffleAverageFollowerCountAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdateRaffleAverageFollowerCountAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess && result.Data != null) {
            $('.raffle-average-follower-count').text(result.Data.AverageFollowerCount);
        }
    }

    function UpdateRaffleAverageFollowerCountAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleAverageFollowerCountAjax  */


    /*  UpdateRaffleStatusAjax  */
    function UpdateRaffleStatusAjaxBegin() {

        if ($('#tk_raffle_status').val() == '') {
            toastr.warning("Çekiliş durumu seçiniz.");
            $('#tk_raffle_status').select2('open');
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleStatusAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess && result.Data !== null) {
            $('.raffle-tk-raffle-status').text(result.Data.TKRaffleStatusName);
        }
    }

    function UpdateRaffleStatusAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleStatusAjax  */


    /*  UpdateRaffleDateTimeAjax  */
    function UpdateRaffleDateTimeAjaxBegin() {

        if ($('#raffle_started_at').val() == '') {
            toastr.warning("Başlangıç tarihi giriniz.");
            $('#raffle_started_at').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleDateTimeAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess && result.Data !== null) {
            $('.raffle-started-at').text(result.Data.RaffleStartedAtStr);
            $('.raffle-ended-at').text(result.Data.RaffleEndedAtStr);
        }
    }

    function UpdateRaffleDateTimeAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleDateTimeAjax  */


    /*  UpdateRaffleUserCountPerListAjax  */
    function UpdateRaffleUserCountPerListAjaxBegin() {

        if ($('#user_count_per_list').val() == '') {
            toastr.warning("Liste kapasitesini giriniz.");
            $('#user_count_per_list').focus();
            return false;
        }

        if (parseInt($('#user_count_per_list').val()) <= 0) {
            toastr.warning("Liste kapasitesi sıfır veya sıfırdan daha az bir değer olamaz.");
            $('#user_count_per_list').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleUserCountPerListAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess && result.Data !== null) {
            $('.raffle-user-count-per-list-input').val(result.Data.UserCountPerList);
            $('.raffle-user-count-per-list').text(result.Data.UserCountPerList);
        }
    }

    function UpdateRaffleUserCountPerListAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleUserCountPerListAjax  */


    /*  UpdateRaffleParticipantsNumberAndSequenceAjax  */
    function UpdateRaffleParticipantsNumberAndSequenceAjaxBegin() {
        global.showLoader();
        return true;
    }

    function UpdateRaffleParticipantsNumberAndSequenceAjaxSuccess(result) {
        
        if (result.IsSuccess && result.Data !== null) {
            $('#raffle-participant-list .raffle-participant').remove();

            var table = $('#table_raffle_participant_list').DataTable();

            for (var i = 0; i < result.Data.RaffleParticipantList.length; i++) {
                var item = result.Data.RaffleParticipantList[i];

                var tr = $('<tr class="raffle-participant">').html(
                    '<td class="summary-raffle-participant-id">' + global.pad(item.ID, 5) + '</td>' +
                    '<td class="summary-raffle-participant-listnumber">' + item.ListNumber + '</td>' +
                    '<td class="summary-raffle-participant-listsequence">' + item.ListSequence + '</td>' +
                    '<td class="summary-raffle-participant-social-media-account-name">' + item.SocialMediaAccount.AccountName + '</td>' +
                    '<td class="summary-raffle-participant-tk-raffle-participant-status">' + item.IPAddress + '</td>'
                    );

                tr.attr('id', 'summary-raffle-participant-' + item.ID);
                tr.appendTo($('#raffle-participant-list'));


                //  UPDATE DATATABLE
                var removeReference = $('#object_' + item.ID);
                var currentData = table.row(removeReference).data();
                currentData[3] = item.InitialScreenShot;
                table
                        .row(removeReference)
                        .data(currentData)
                        .draw();

                if (removeReference.find('.select-row').prop('checked')) {
                    removeReference.find('.select-row').click();
                    removeReference.addClass('selected');
                }
            }
        }

        updateSuccess(result);
    }

    function UpdateRaffleParticipantsNumberAndSequenceAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleParticipantsNumberAndSequenceAjax  */


    /*  UpdateRaffleParticipationListStatusAjax  */
    function UpdateRaffleParticipationListStatusAjaxBegin() {

        if ($('#tk_raffle_participation_status').val() == '') {
            toastr.warning('Katılım durumunu seçiniz.');
            $('#tk_raffle_participation_status').select2('open');
            return false;
        }

        if ($('#action_description').val() == '') {
            toastr.warning('Açıklama giriniz.');
            $('#action_description').focus();
            return false;
        }

        global.showLoader();
        return true;
    }

    function UpdateRaffleParticipationListStatusAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess && result.Data !== null) {

            $('#update-raffle-participant-status-modal').modal('hide');

            if (result.Data.ErrorList.length != 0) {

                var errorList = '';

                for (var i = 0; i < result.Data.ErrorList.length; i++) {
                    errorList += result.Data.ErrorList[i];

                    if (result.Data.ErrorList.length != i + 1) {
                        errorList += ', ';
                    }
                }

                $('#error-raffle-participant-list-container').removeClass('hidden');
                $('#error-raffle-participant-list').text(errorList);

                toastr.warning('Bazı katılımlar güncellenemedi. Güncellenemeyen katılım numaraları kırmızı bölge içinde sıralanmıştır.');
            }

            if (result.Data.SuccessList.length != 0) {
                var table = $('#table_raffle_participant_list').DataTable();

                for (var i = 0; i < result.Data.SuccessList.length; i++) {
                    var item = result.Data.SuccessList[i];

                    var removeReference = $('#object_' + item);
                    var currentData = table.row(removeReference).data();
                    currentData[5] = result.Data.TKRaffleParticipationStatusName;

                    table
                        .row(removeReference)
                        .data(currentData)
                        .draw();

                    removeReference.find('.select-row').click();
                    removeReference.addClass('selected');

                    $('#raffle-participant-list #summary-raffle-participant-' + item + ' .summary-raffle-participant-tk-raffle-participant-status').text(result.Data.TKRaffleParticipationStatusName);
                }
            }
        }
    }

    function UpdateRaffleParticipationListStatusAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  UpdateRaffleParticipationListStatusAjax  */


    /*  SendEmailRaffleParticipantConfirmedAjax  */
    function SendEmailRaffleParticipantConfirmedAjaxBegin() {

        if ($('#sendemail_raffleparticipant_fullname').val() == '') {
            $('#sendemail_raffleparticipant_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendemail_raffleparticipant_email').val() == '') {
            $('#sendemail_raffleparticipant_email').focus();
            toastr.warning('Alıcı email adresini giriniz');
            return false;
        }

        if ($('#form-send-email-raffle-participant-confirmed input[name="UseBCC"]:checked').val() == 'true' && $('#sendemail_raffleparticipant_bccemail').val() == '') {
            $('#sendemail_raffleparticipant_bccemail').focus();
            toastr.warning('BCC Email adresini giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailRaffleParticipantConfirmedAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-email-raffle-participant-confirmed-modal').modal('hide');
        }
    }

    function SendEmailRaffleParticipantConfirmedAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailRaffleParticipantConfirmedAjax  */


    /*  SendSmsRaffleParticipantConfirmedAjax  */
    function SendSmsRaffleParticipantConfirmedAjaxBegin() {

        if ($('#sendsms_raffleparticipant_fullname').val() == '') {
            $('#sendsms_raffleparticipant_fullname').focus();
            toastr.warning('Alıcı Adı Soyadı bilgisini giriniz');
            return false;
        }

        if ($('#sendsms_raffleparticipant_phonenumber').val() == '') {
            $('#sendsms_raffleparticipant_phonenumber').focus();
            toastr.warning('Alıcı Telefon Numarasını giriniz');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsRaffleParticipantConfirmedAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-sms-raffle-participant-confirmed-modal').modal('hide');
        }
    }

    function SendSmsRaffleParticipantConfirmedAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsRaffleParticipantConfirmedAjax  */


    /*  SendEmailRaffleStatusAjax  */
    function SendEmailRaffleStatusAjaxBegin() {

        if ($('#sendemail_rafflestatus_tkemailtemplate').val() == '') {
            toastr.warning('Email Template ID bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendemail_rafflestatus_raffleid').val() == '') {
            toastr.warning('Çekiliş ID bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendEmailRaffleStatusAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-raffle-status-email-modal').modal('hide');
        }
    }

    function SendEmailRaffleStatusAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendEmailRaffleStatusAjax  */


    /*  SendSmsRaffleStatusAjax  */
    function SendSmsRaffleStatusAjaxBegin() {

        if ($('#sendsms_rafflestatus_tksmstemplate').val() == '') {
            toastr.warning('Sms Template ID bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        if ($('#sendsms_rafflestatus_raffleid').val() == '') {
            toastr.warning('Çekiliş ID bulunamadı. Lütfen sayfayı yenileyip yeniden deneyiniz.');
            return false;
        }

        global.showLoader();
        return true;
    }

    function SendSmsRaffleStatusAjaxSuccess(result) {
        updateSuccess(result);

        if (result.IsSuccess) {
            $('#send-raffle-status-sms-modal').modal('hide');
        }
    }

    function SendSmsRaffleStatusAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SendSmsRaffleStatusAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.hideLoader();
    }

    return {
        UpdateRaffleParticipantActivationAjaxBegin: UpdateRaffleParticipantActivationAjaxBegin,
        UpdateRaffleParticipantActivationAjaxSuccess: UpdateRaffleParticipantActivationAjaxSuccess,
        UpdateRaffleParticipantActivationAjaxFailure: UpdateRaffleParticipantActivationAjaxFailure,

        UpdateRaffleParticipantTypeAndStatusAjaxBegin: UpdateRaffleParticipantTypeAndStatusAjaxBegin,
        UpdateRaffleParticipantTypeAndStatusAjaxSuccess: UpdateRaffleParticipantTypeAndStatusAjaxSuccess,
        UpdateRaffleParticipantTypeAndStatusAjaxFailure: UpdateRaffleParticipantTypeAndStatusAjaxFailure,

        UpdateRaffleParticipantListNumberAjaxBegin: UpdateRaffleParticipantListNumberAjaxBegin,
        UpdateRaffleParticipantListNumberAjaxSuccess: UpdateRaffleParticipantListNumberAjaxSuccess,
        UpdateRaffleParticipantListNumberAjaxFailure: UpdateRaffleParticipantListNumberAjaxFailure,

        UpdateRaffleParticipantConfirmationAjaxBegin: UpdateRaffleParticipantConfirmationAjaxBegin,
        UpdateRaffleParticipantConfirmationAjaxSuccess: UpdateRaffleParticipantConfirmationAjaxSuccess,
        UpdateRaffleParticipantConfirmationAjaxFailure: UpdateRaffleParticipantConfirmationAjaxFailure,

        SaveRaffleTemplateAjaxBegin: SaveRaffleTemplateAjaxBegin,
        SaveRaffleTemplateAjaxSuccess: SaveRaffleTemplateAjaxSuccess,
        SaveRaffleTemplateAjaxFailure: SaveRaffleTemplateAjaxFailure,

        UpdateRaffleTemplateAjaxBegin: UpdateRaffleTemplateAjaxBegin,
        UpdateRaffleTemplateAjaxSuccess: UpdateRaffleTemplateAjaxSuccess,
        UpdateRaffleTemplateAjaxFailure: UpdateRaffleTemplateAjaxFailure,

        SaveRaffleAjaxBegin: SaveRaffleAjaxBegin,
        SaveRaffleAjaxSuccess: SaveRaffleAjaxSuccess,
        SaveRaffleAjaxFailure: SaveRaffleAjaxFailure,

        UpdateRaffleAjaxBegin: UpdateRaffleAjaxBegin,
        UpdateRaffleAjaxSuccess: UpdateRaffleAjaxSuccess,
        UpdateRaffleAjaxFailure: UpdateRaffleAjaxFailure,

        UpdateRaffleActivationAjaxBegin: UpdateRaffleActivationAjaxBegin,
        UpdateRaffleActivationAjaxSuccess: UpdateRaffleActivationAjaxSuccess,
        UpdateRaffleActivationAjaxFailure: UpdateRaffleActivationAjaxFailure,

        UpdateRaffleAverageFollowerCountAjaxBegin: UpdateRaffleAverageFollowerCountAjaxBegin,
        UpdateRaffleAverageFollowerCountAjaxSuccess: UpdateRaffleAverageFollowerCountAjaxSuccess,
        UpdateRaffleAverageFollowerCountAjaxFailure: UpdateRaffleAverageFollowerCountAjaxFailure,

        UpdateRaffleStatusAjaxBegin: UpdateRaffleStatusAjaxBegin,
        UpdateRaffleStatusAjaxSuccess: UpdateRaffleStatusAjaxSuccess,
        UpdateRaffleStatusAjaxFailure: UpdateRaffleStatusAjaxFailure,

        UpdateRaffleDateTimeAjaxBegin: UpdateRaffleDateTimeAjaxBegin,
        UpdateRaffleDateTimeAjaxSuccess: UpdateRaffleDateTimeAjaxSuccess,
        UpdateRaffleDateTimeAjaxFailure: UpdateRaffleDateTimeAjaxFailure,

        UpdateRaffleUserCountPerListAjaxBegin: UpdateRaffleUserCountPerListAjaxBegin,
        UpdateRaffleUserCountPerListAjaxSuccess: UpdateRaffleUserCountPerListAjaxSuccess,
        UpdateRaffleUserCountPerListAjaxFailure: UpdateRaffleUserCountPerListAjaxFailure,

        UpdateRaffleParticipantsNumberAndSequenceAjaxBegin: UpdateRaffleParticipantsNumberAndSequenceAjaxBegin,
        UpdateRaffleParticipantsNumberAndSequenceAjaxSuccess: UpdateRaffleParticipantsNumberAndSequenceAjaxSuccess,
        UpdateRaffleParticipantsNumberAndSequenceAjaxFailure: UpdateRaffleParticipantsNumberAndSequenceAjaxFailure,

        UpdateRaffleParticipationListStatusAjaxBegin: UpdateRaffleParticipationListStatusAjaxBegin,
        UpdateRaffleParticipationListStatusAjaxSuccess: UpdateRaffleParticipationListStatusAjaxSuccess,
        UpdateRaffleParticipationListStatusAjaxFailure: UpdateRaffleParticipationListStatusAjaxFailure,

        SendEmailRaffleParticipantConfirmedAjaxBegin: SendEmailRaffleParticipantConfirmedAjaxBegin,
        SendEmailRaffleParticipantConfirmedAjaxSuccess: SendEmailRaffleParticipantConfirmedAjaxSuccess,
        SendEmailRaffleParticipantConfirmedAjaxFailure: SendEmailRaffleParticipantConfirmedAjaxFailure,

        SendSmsRaffleParticipantConfirmedAjaxBegin: SendSmsRaffleParticipantConfirmedAjaxBegin,
        SendSmsRaffleParticipantConfirmedAjaxSuccess: SendSmsRaffleParticipantConfirmedAjaxSuccess,
        SendSmsRaffleParticipantConfirmedAjaxFailure: SendSmsRaffleParticipantConfirmedAjaxFailure,

        SendEmailRaffleStatusAjaxBegin:   SendEmailRaffleStatusAjaxBegin,
        SendEmailRaffleStatusAjaxSuccess: SendEmailRaffleStatusAjaxSuccess,
        SendEmailRaffleStatusAjaxFailure: SendEmailRaffleStatusAjaxFailure,

        SendSmsRaffleStatusAjaxBegin:   SendSmsRaffleStatusAjaxBegin,
        SendSmsRaffleStatusAjaxSuccess: SendSmsRaffleStatusAjaxSuccess,
        SendSmsRaffleStatusAjaxFailure: SendSmsRaffleStatusAjaxFailure
    }
})();