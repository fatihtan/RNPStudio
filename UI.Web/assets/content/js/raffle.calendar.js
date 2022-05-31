global.showLoader();

$(document).ready(function () {
    $("#raffle_template_id").select2({
        allowClear: true,
        placeholder: "Seçiniz",
        width: null
    });

    $('#raffle_started_at, #raffle_ended_at').datetimepicker({
        todayHighlight: true,
        language: 'tr',
        format: 'dd/mm/yyyy hh:ii'
    });

    $("#user_count_per_list, #update_user_count_per_list").inputmask({
        mask: '9',
        repeat: 12,
        greedy: !1,
        allowMinus: false
    });

    getRaffleList();
});

function initCalendar(events) {
    var h = {};

    if (App.isRTL()) {
        if ($('#calendar').parents(".portlet").width() <= 720) {
            $('#calendar').addClass("mobile");
            h = {
                right: 'title, prev, next',
                center: '',
                left: 'agendaDay, agendaWeek, month, today'
            };
        } else {
            $('#calendar').removeClass("mobile");
            h = {
                right: 'title',
                center: '',
                left: 'agendaDay, agendaWeek, month, today, prev, next'
            };
        }
    } else {
        if ($('#calendar').parents(".portlet").width() <= 720) {
            $('#calendar').addClass("mobile");
            h = {
                left: 'title, prev, next',
                center: '',
                right: 'today, month, agendaWeek, agendaDay'
            };
        } else {
            $('#calendar').removeClass("mobile");
            h = {
                left: 'title',
                center: '',
                right: 'prev, next, today, month, agendaWeek, agendaDay'
            };
        }
    }

    var initDrag = function (el) {
        // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
        // it doesn't need to have a start or end
        var eventObject = {
            title: $.trim(el.text()) // use the element's text as the event title
        };
        // store the Event Object in the DOM element so we can get to it later
        el.data('eventObject', eventObject);
        // make the event draggable using jQuery UI
        el.draggable({
            zIndex: 999,
            revert: true, // will cause the event to go back to its
            revertDuration: 0 //  original position after the drag
        });
    };

    $('#calendar').fullCalendar('destroy'); // destroy the calendar
    $('#calendar').fullCalendar({ //re-initialize the calendar
        header: h,
        defaultView: 'month', // change default view with available options from http://arshaw.com/fullcalendar/docs/views/Available_Views/ 
        slotMinutes: 15,
        lang: 'tr',
        editable: true,
        droppable: true, // this allows things to be dropped onto the calendar !!!
        events: events,
        displayEventTime: false,
        eventRender: function (event, element, view) {
            if (event.isActive) {
                element.find('.fc-content').append('<i class="pull-right font-green fa fa-thumbs-up"></span> ');
            }
            else {
                element.find('.fc-content').append('<i class="pull-right font-red fa fa-thumbs-down"></span> ');
            }

            $(element).tooltip({
                trigger: 'hover',
                container: 'body',
                title: 'Liste Kapasitesi: ' + event.userCountPerList
            });
        },
        eventClick: function (event) {
            $('#update_id').val(event.id);
            $('#go-to-raffle').attr('href', '/Raffle/Detail/' + event.id);
            $('#update_user_count_per_list').val(event.userCountPerList);
            $('#update_is_active').prop('checked', event.isActive).change();

            $('#update-raffle-modal').modal('show');
        },
        eventDragStop: function (event, dayDelta, revertFunc) {
            $('.tooltip.fade.in').remove()
        },
        eventDrop: function (event, dayDelta, revertFunc) {

            var ID, RaffleStartedAtStr, RaffleEndedAtStr;

            if (event.start == null || event.start._d == null) {
                toastr.warning('Başlangıç tarihi ayrıştırılamadı. Lütfen sayfayı yenileyip tekrar deneyiniz.');
                revertFunc();
                return false;
            }
            else {
                RaffleStartedAtStr = event.start._d.toLocaleString('tr-TR').replaceAll('.', '/')
            }

            if (event.end != null && event.end._d != null) {
                RaffleEndedAtStr = event.end._d.toLocaleString('tr-TR').replaceAll('.', '/')
            }

            ID = event.id;

            global.showLoader();

            $.post('/Raffle/UpdateRaffleDateTimeAjax',
                { 'ID': ID, 'RaffleStartedAtStr': RaffleStartedAtStr, 'RaffleEndedAtStr': RaffleEndedAtStr })
                .success(function (r) {
                    if (r.IsSuccess) {
                        toastr.success('Çekiliş başlangıç ve bitiş tarihleri başarıyla güncellendi.');
                        $('.tooltip.fade.in').remove();
                    }
                    else {
                        toastr.warning(r.Message);
                        revertFunc();
                        return false;
                    }
                })
                .fail(function () {
                    toastr.error("Internal Server Error");
                    revertFunc();
                    return false;
                })
                .always(function () {
                    global.hideLoader();
                })

        },
        eventResize: function (event, dayDelta, revertFunc) {

            var ID, RaffleStartedAtStr, RaffleEndedAtStr;

            if (event.start == null || event.start._d == null) {
                toastr.warning('Başlangıç tarihi ayrıştırılamadı. Lütfen sayfayı yenileyip tekrar deneyiniz.');
                revertFunc();
                return false;
            }
            else {
                RaffleStartedAtStr = event.start._d.toLocaleString('tr-TR').replaceAll('.', '/')
            }

            if (event.end != null && event.end._d != null) {
                RaffleEndedAtStr = event.end._d.toLocaleString('tr-TR').replaceAll('.', '/')
            }

            ID = event.id;

            global.showLoader();

            $.post('/Raffle/UpdateRaffleDateTimeAjax',
                { 'ID': ID, 'RaffleStartedAtStr': RaffleStartedAtStr, 'RaffleEndedAtStr': RaffleEndedAtStr })
                .success(function (r) {
                    if (r.IsSuccess) {
                        toastr.success('Çekiliş başlangıç ve bitiş tarihleri başarıyla güncellendi.');
                    }
                    else {
                        toastr.warning(r.Message);
                        revertFunc();
                        return false;
                    }
                })
                .fail(function () {
                    toastr.error("Internal Server Error");
                    revertFunc();
                    return false;
                })
                .always(function () {
                    global.hideLoader();
                })
        }
    });
}

function getRaffleList() {

    var TKRaffleStatus = $('#TKRaffleStatus').val();

    $.post('/Raffle/GetRaffleListAjax',
        { 'TKRaffleStatus': TKRaffleStatus })
        .success(function (r) {
            if (r.IsSuccess) {
                var events = convertDataToEvents(r.Data);
                initCalendar(events);
                $('#raffle-count').text(r.Data.length);
                $('#raffle-count').data('rafflecount', r.Data.length);
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

function convertDataToEvents(data) {
    var events = [];
    for (var i = 0; i < data.length; i++) {
        events.push(getEvent(data[i]));
    }
    return events;
}

function getEvent(raffleCalendarModel) {

    var event;

    if (raffleCalendarModel.Raffle.RaffleEndedAt == null) {
        event = {
            id: raffleCalendarModel.ID,
            title: raffleCalendarModel.Title,
            isActive: raffleCalendarModel.Raffle.IsActive,
            userCountPerList: raffleCalendarModel.Raffle.UserCountPerList,
            start: new Date(
                raffleCalendarModel.StartedAtYear,
                raffleCalendarModel.StartedAtMonth,
                raffleCalendarModel.StartedAtDay,
                raffleCalendarModel.StartedAtHour,
                raffleCalendarModel.StartedAtMinutes),
            backgroundColor: App.getBrandColor(raffleCalendarModel.BackgroundColor),
            allDay: true,
            eventResizableFromStart: true
        };
    }
    else {
        event = {
            id: raffleCalendarModel.ID,
            title: raffleCalendarModel.Title,
            isActive: raffleCalendarModel.Raffle.IsActive,
            userCountPerList: raffleCalendarModel.Raffle.UserCountPerList,
            start: new Date(
                raffleCalendarModel.StartedAtYear,
                raffleCalendarModel.StartedAtMonth,
                raffleCalendarModel.StartedAtDay,
                raffleCalendarModel.StartedAtHour,
                raffleCalendarModel.StartedAtMinutes),
            end: new Date(
                raffleCalendarModel.EndedAtYear,
                raffleCalendarModel.EndedAtMonth,
                raffleCalendarModel.EndedAtDay,
                raffleCalendarModel.EndedAtHour,
                raffleCalendarModel.EndedAtMinutes),
            backgroundColor: App.getBrandColor(raffleCalendarModel.BackgroundColor),
            allDay: true,
            eventResizableFromStart: true
        };
    }

    return event;
}

$('#update-raffle-modal').on('hidden.bs.modal', function () {
    $('#update_id').val('');
    $('#go-to-raffle').removeAttr('href');
    $('#update_user_count_per_list').val('');
    $('#update_is_active').prop('checked', true).change();
});

$('#tk_raffle_status').on('change', function () {
    var id = $(this).val();
    window.location.href = '/Raffle/Save?TKRaffleStatus=' + id;
});

$('.btn-left-menu').on('click', function () {
    $(this).attr('disabled', 'disabled');

    var state = $(this).data('state');

    if (state == 1) {
        $(this).data('state', 0);

        //animateCSS('#calendar-right-side', 'slideInRight', function () {
        //    $('#calendar-right-side').addClass('col-md-12');
        //});

        //animateCSS('#calendar-left-side', 'zoomOut', function () {
        //    $('#calendar-left-side').hide();
        //});

        $('#calendar-left-side').toggleClass('col-md-3 col-md-1');
        $('.hide-on-close').toggleClass('hidden')
        $('#calendar-right-side').toggleClass('col-md-9 col-md-11');


        //$('#calendar-left-side').toggleClass('animated zoomOut');
        //setTimeout(function () {
        //    $('#calendar-left-side').toggleClass('hidden');
        //}, 400);
        //setTimeout(function () {
        //    $('#calendar-right-side').toggleClass('col-md-12 animated zoomIn');
        //}, 50);
        
        
        

    }
    else {
        $(this).data('state', 1);
    }


    //if (isShown == "0") {   //  Göster
    //    $('.group-title').addClass('hidden');
    //    $('#field-wrapper').removeClass('initial-float margin-auto');
    //    animateCSS('#field-wrapper', 'slideInRight', function () {

    //    });

    //    $('#article-wrapper').show();
    //    animateCSS('#article-wrapper', 'zoomIn', function () {

    //        item.removeAttr('disabled');
    //    });

    //    $(this).text('Metni Gizle');
    //    $(this).data('isshown', "1");
    //}
    //else if (isShown == "1") {  //  Gizle
    //    $('.group-title').removeClass('hidden');
    //    animateCSS('#article-wrapper', 'zoomOut', function () {
    //        $('#article-wrapper').hide();
    //    });

    //    animateCSS('#field-wrapper', 'slideOutRight', function () {
    //        $('#field-wrapper').addClass('initial-float margin-auto');

    //        item.removeAttr('disabled');
    //    });

    //    $(this).text('Metni Göster');
    //    $(this).data('isshown', "0");

    //}
});

function animateCSS(element, animationName, callback) {
    var node = document.querySelector(element)
    node.classList.add('animated', animationName)

    function handleAnimationEnd() {
        node.classList.remove('animated', animationName)
        node.removeEventListener('animationend', handleAnimationEnd)

        if (typeof callback === 'function') callback()
    }

    node.addEventListener('animationend', handleAnimationEnd)
}