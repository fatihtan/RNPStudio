var Generic = (function () {
    'use strict';

    /*  SubscribeUpdateAjax  */
    function SubscribeUpdateAjaxBegin() {
        if ($('#update_email').val() == "") {
            toastr.warning("Email boş bırakılamaz.");
            $('#update_email').focus();
            return false;
        }

        return true;
    }

    function SubscribeUpdateAjaxSuccess(result) {
        updateSuccess(result);
        if (result.IsSuccess) {

            $('#update_id').val('');
            $('#update_email').val('');
            $('#update_description').val('');
            $('#update_is_subscribed').prop('checked', true).change();
            $('#update_is_deleted').prop('checked', false).change();

            if (result.Data != null) {
                var table = $('#item_list').DataTable();

                var removeReference = $('#object_' + result.Data.ID);

                var currentData = table.row(removeReference).data();
                currentData[2] = result.Data.Email;
                currentData[7] = result.Data.Description;

                if (result.Data.IsSubscribed) {
                    currentData[3] = '<span class="label label-sm label-info">Evet</span>';
                }
                else {
                    currentData[3] = '<span class="label label-sm label-warning">Hayır</span>';
                }

                if (result.Data.IsDeleted) {
                    currentData[4] = '<span class="label label-sm label-danger">Evet</span>';
                }
                else {
                    currentData[4] = '<span class="label label-sm label-success">Hayır</span>';
                }

                table
                    .row(removeReference)
                    .data(currentData)
                    .draw();

                var itemArr = jQuery.grep(SerializedSubscribeList, function (value) {
                    return value.ID == result.Data.ID;
                });

                if (itemArr.length != 0) {
                    itemArr[0].Email = result.Data.Email;
                    itemArr[0].Description = result.Data.Description;
                    itemArr[0].IsDeleted = result.Data.IsDeleted;
                    itemArr[0].IsSubscribed = result.Data.IsSubscribed;
                }
            }

            $('#update-object-modal').modal('hide');
        }
    }

    function SubscribeUpdateAjaxFailure(result) {
        updateSuccess(result);
    }
    /*  SubscribeUpdateAjax  */


    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
    }

    return {
        SubscribeUpdateAjaxBegin:   SubscribeUpdateAjaxBegin,
        SubscribeUpdateAjaxSuccess: SubscribeUpdateAjaxSuccess,
        SubscribeUpdateAjaxFailure: SubscribeUpdateAjaxFailure
    }
})();