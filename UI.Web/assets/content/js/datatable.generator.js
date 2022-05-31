var datatableGenerator = (function () {
    'use strict';

    function normal(selector, orderNum, orderType) {
        if (orderType == undefined || orderType == null || orderType == '') {
            orderType = 'desc';
        }

        if (orderNum == undefined) {
            orderNum = 0;
        }

        if (!$(selector).hasClass('table table-striped table-bordered table-hover table-checkable order-column')) {
            throw "table class is not valid for normal datatable";
            return;
        }

        $(selector).DataTable({
            language: {
                aria: {
                    sortAscending: ": activate to sort column ascending",
                    sortDescending: ": activate to sort column descending"
                },
                emptyTable: "No data available in table",
                info: "Showing _START_ to _END_ of _TOTAL_ records",
                infoEmpty: "No records found",
                infoFiltered: "(filtered1 from _MAX_ total records)",
                lengthMenu: "Show _MENU_",
                search: "Ara:",
                zeroRecords: "Eşleşme Bulunamadı",
                paginate: {
                    previous: "Prev",
                    next: "Next",
                    last: "Last",
                    first: "First"
                }
            },
            buttons: [
                { extend: 'print', className: 'btn default', text: '<i class="fa fa-print"></i>&nbsp;YAZDIR' },
                { extend: 'copy', className: 'btn default', text: '<i class="fa fa-copy"></i>&nbsp;KOPYALA' },
                { extend: 'pdf', className: 'btn default', text: '<i class="fa fa-file-pdf-o"></i>&nbsp;PDF' },
                { extend: 'excel', className: 'btn default', text: '<i class="fa fa-file-excel-o"></i>&nbsp;EXCEL' },
                { extend: 'csv', className: 'btn default', text: '<i class="fa fa-file-text"></i>&nbsp;CSV' }
            ],
            bStateSave: !0,
            columnDefs: [{
                targets: 0,
                orderable: !1,
                searchable: !1
            }],
            lengthMenu: [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
            pageLength: 10,
            pagingType: "bootstrap_full_number",
            order: [[orderNum, orderType]],
            "dom": "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", // horizobtal scrollable datatable
        });
    }

    function responsive(selector, orderNum, orderType) {
        if (orderType == undefined || orderType == null || orderType == '') {
            orderType = 'desc';
        }

        if (orderNum == undefined) {
            orderNum = 0;
        }

        if (!$(selector).hasClass('table table-striped table-bordered table-hover dt-responsive')) {
            throw "table class is not valid for responsive datatable";
            return;
        }

        $(selector).dataTable({
            language: {
                aria: {
                    sortAscending: ": activate to sort column ascending",
                    sortDescending: ": activate to sort column descending"
                },
                emptyTable: "No data available in table",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                infoEmpty: "Kayıt Bulunamadı",
                infoFiltered: "(filtered1 from _MAX_ total entries)",
                lengthMenu: "_MENU_ entries",
                search: "Ara:",
                zeroRecords: "Eşleşme Bulunamadı"
            },
            buttons: [],
            responsive: {
                details: {}
            },
            order: [[orderNum, orderType]],
            lengthMenu: [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
            pageLength: 10,
            dom: "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>"
        });
    }

    function responsiveNoPagination(selector, orderNum, orderType) {
        if (orderType == undefined || orderType == null || orderType == '') {
            orderType = 'desc';
        }

        if (orderNum == undefined) {
            orderNum = 0;
        }

        if (!$(selector).hasClass('table table-striped table-bordered table-hover dt-responsive')) {
            throw "table class is not valid for responsive datatable";
            return;
        }

        $(selector).dataTable({
            language: {
                aria: {
                    sortAscending: ": activate to sort column ascending",
                    sortDescending: ": activate to sort column descending"
                },
                emptyTable: "No data available in table",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                infoEmpty: "Kayıt Bulunamadı",
                infoFiltered: "(filtered1 from _MAX_ total entries)",
                lengthMenu: "_MENU_ entries",
                search: "Ara:",
                zeroRecords: "Eşleşme Bulunamadı"
            },
            buttons: [],
            responsive: {
                details: {}
            },
            columnDefs: [{
                orderable: false,
                targets: 12,
                'checkboxes': {
                    'selectRow': true
                }
            }],
            'select': {
                'style': 'multi'
            },
            order: [[orderNum, orderType]],
            lengthMenu: [[-1], ["All"]],
            pageLength: -1,
            dom: "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>"
        });
    }

    return {
        normal: normal,
        responsive: responsive,
        responsiveNoPagination: responsiveNoPagination
    }
})();