var Log = (function () {
    'use strict';

    /*  URLErrorAdministrator  */
    function URLErrorAdministrator() {
        $.post('/Log/URLErrorAdministrator',
            { 'URL': window.location.href })
            .done(function (r) {
                console.log(r);
            })
            .fail(function () {
            })

        return true;
    }
    /*  URLErrorAdministrator  */

    /*  NavigationAdministrator  */
    function NavigationAdministrator() {
        $.post('/Log/NavigationAdministrator',
            { 'URL': window.location.href })
            .done(function (r) {
                console.log(r);
            })
            .fail(function () {
            })

        return true;
    }
    /*  NavigationAdministrator  */

    function updateSuccess(result) {
        if (result.IsSuccess) {
            toastr.success(result.Message);
        } else {
            toastr.error(result.Message);
        }
        global.loaderStop('#loaderProcess');
    }

    return {
        URLErrorAdministrator: URLErrorAdministrator,
        NavigationAdministrator: NavigationAdministrator
    }
})();