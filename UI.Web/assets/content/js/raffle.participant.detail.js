$(document).ready(function () {
    if ($('.nav-tabs li.active').length == 0) {
        $('.nav-tabs li').first().addClass('active');
        $('.tab-content .tab-pane').first().addClass('active');
    }

    $("#raffle_id, #tk_raffle_participant_type, #tk_raffle_participant_status, #social_media_account_id").select2({
        allowClear: true,
        placeholder: "Seçiniz",
        width: null,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#initial_follower_count, #eventual_follower_count, #makeup_initial_follower_count, #listnumber, #listsequence").inputmask({
        mask: '9',
        repeat: 12,
        greedy: !1,
        allowMinus: false
    });

    if ($('#table_makeup_raffle_participant_list').length) {
        datatableGenerator.responsive('#table_makeup_raffle_participant_list', 0, 'desc');
    }

    $('.input-phone-number').phoneNumberMask();
});

$('#form-update-raffle-participant-follower-screen-shot').ajaxForm({
    beforeSend: function (e) {
        global.showLoader();

        var initialScreenShot = document.getElementById('initial_screen_shot');
        if ($(initialScreenShot).attr('required') != undefined) {
            if (!initialScreenShot.files.length) {
                toastr.warning('Başlangıç ekran görüntüsü seçiniz.');
                return false;
            }

            if (initialScreenShot.files[0].size <= 0) {
                toastr.warning('Lütfen geçerli bir başlangıç ekran görüntüsü seçiniz.');
                return false;
            }
        }

        var eventualScreenShot = document.getElementById('eventual_screen_shot');
        if ($(eventualScreenShot).attr('required') != undefined) {
            if (!eventualScreenShot.files.length) {
                toastr.warning('Bitiş ekran görüntüsü seçiniz.');
                return false;
            }

            if (eventualScreenShot.files[0].size <= 0) {
                toastr.warning('Lütfen geçerli bir bitiş ekran görüntüsü seçiniz.');
                return false;
            }
        }
    },
    complete: function (result) {
        global.hideLoader();

        result = result.responseJSON;

        if (result != undefined) {
            if (result.IsSuccess) {
                toastr.success(result.Message);

                $('.img-initial-screen-shot').attr('src', result.Data.InitialScreenShot);
                $('.img-eventual-screen-shot').attr('src', result.Data.EventualScreenShot);

                $('#initial_screen_shot').closest('.fileinput-exists').find('a[data-dismiss="fileinput"]').click();
                $('#eventual_screen_shot').closest('.fileinput-exists').find('a[data-dismiss="fileinput"]').click();
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


$('#form-save-makeup-raffle-participant').ajaxForm({
    beforeSend: function (e) {
        global.showLoader();

        if ($('#social_media_account_id').val() == '') {
            toastr.warning('Sosyal medya hesabı seçiniz.');
            $('#social_media_account_id').select2('open');
            return false;
        }

        if ($('#raffle_id').val() == '') {
            toastr.warning('Çekiliş paketi seçiniz.');
            return false;
        }

        var initialScreenShot = document.getElementById('makeup_initial_screen_shot');
        if (!initialScreenShot.files.length) {
            toastr.warning('Başlangıç ekran görüntüsü seçiniz.');
            return false;
        }

        if (initialScreenShot.files[0].size <= 0) {
            toastr.warning('Lütfen geçerli bir başlangıç ekran görüntüsü seçiniz.');
            return false;
        }

        if ($('#makeup_initial_follower_count').val() == '') {
            toastr.warning('Mevcut takipçi sayısını giriniz.');
            return false;
        }
    },
    complete: function (result) {
        global.hideLoader();

        result = result.responseJSON;

        if (result != undefined) {
            if (result.IsSuccess) {
                toastr.success(result.Message);

                $('.raffle-participant-ismakeupdefined').html('<span class="label label-sm label-warning">EVET</span>')
                $('#submit-makeup-raffle').attr('disabled', 'disabled');
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


$('.btn-show-image').on('click', function () {
    var imageURL = $(this).find('img').attr('src');
    var title = $(this).data('title');

    $('#screen-shot-title').text(title);
    $('#screen-shot-img').attr('src', imageURL);

    $('#screen-shot-modal').modal('show');
});

$('#form-send-email-raffle-participant-confirmed input[name="UseBCC"]').on('change', function () {
    if (this.value == "true") {
        $('#div-send-email-raffleparticipant-bcc-email').removeClass('hidden');
        $('#sendemail_raffleparticipant_bccemail').attr('required', 'required');
    }
    else if (this.value == "false") {
        $('#div-send-email-raffleparticipant-bcc-email').addClass('hidden');
        $('#sendemail_raffleparticipant_bccemail').removeAttr('required');
    }
});

$('#refresh-social-media-acocunts').on('click', function () {

    var cid = $('#CustomerID').val();

    global.showLoader();
    $.post('/Customer/GetSocialMediaAccountListAjax', { CustomerID: cid })
        .success(function (result) {
            if (result.IsSuccess) {
                if (result.Data != null) {
                    filterSelect2('#social_media_account_id', result.Data.SocialMediaAccountList)
                }
            }
            else {
                toastr.error(result.Message);
            }
        })
        .fail(function () {
            toast.error('Internal Server Error');
        })
        .always(function () {
            global.hideLoader();
        })
});

function filterSelect2(targetSelector, data) {
    $(targetSelector + ' option:not(.ignore-me)').remove();

    if (data != null && data.length > 0) {
        var dt = [];
        data.forEach(function (val, i) {
            dt.push({
                id: val.ID,
                text: val.AccountName + "<span class='label md-skip font-weight-600 label-success pull-right'>" + getTKSocialMediaPlatformName(val.TKSocialMediaPlatform) + "</span>"
            })
        })
        $(targetSelector).select2({
            placeholder: "Seçiniz",
            width: null,
            allowClear: true,
            data: dt,
            "language": {
                "noResults": function () {
                    return "Sonuç Bulunamadı";
                }
            },
            escapeMarkup: function (m) {
                return m;
            }
        })
    }
}

function getTKSocialMediaPlatformName(id) {
    if (id == 1) {
        return 'Instagram';
    }
    else if (id == 2) {
        return 'Facebook';
    }
    else if (id == 3) {
        return 'Twitter';
    }
    else if (id == 4) {
        return 'LinkedIn';
    }
    else {
        return 'Tanımsız'
    }
}