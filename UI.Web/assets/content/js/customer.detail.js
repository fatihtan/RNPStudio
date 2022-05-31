$(document).ready(function () {
    if ($('.nav-tabs li.active').length == 0) {
        $('.nav-tabs li').first().addClass('active');
        $('.tab-content .tab-pane').first().addClass('active');
    }

    
    $("#city_id, #tk_job_title, #tk_industry").select2({
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

    global.queryStringResult();

    openTab();

    datatableGenerator.responsive('#table_payment_list', 0);
    datatableGenerator.normal('#table_bill_info_list', 8);
    datatableGenerator.normal('#table_customer_bill_list', 5);
    datatableGenerator.normal('#table_social_media_account_list', 1);
    
    datatableGenerator.responsive('#table_raffle_participant_list', 0);
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

function openTab() {
    var tab = global.getParameterByName('tab');
    if (tab != null) {
        $('a[href="#' + tab + '"]').click();
    }
}