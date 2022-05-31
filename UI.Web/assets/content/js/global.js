var global = (function () {
    'use strict';

    function loaderStart(selector) {
        $(selector).addClass('is-active')
    }

    function loaderStop(selector) {
        $(selector).removeClass('is-active');
    }

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }

    function queryStringResult() {
        var isSuccess = global.getParameterByName("IsSuccess");
        if (isSuccess == null) {
            return;
        }
        if (isSuccess.toLowerCase() == "true") {
            toastr.success(global.getParameterByName("Message"));
        }
        else if (isSuccess.toLowerCase() == "false") {
            toastr.error(global.getParameterByName("Message"));
        }
    }

    function submitForm(selectorForm, selectorButton) {
        if (selectorButton == undefined) {
            selectorButton = 'button[type="submit"]';
        }
        $(selectorForm).find(selectorButton).trigger('click');
    }

    function showLoader() {
        loaderStart('#loaderProcess')
    }

    function hideLoader() {
        loaderStop('#loaderProcess')
    }

    function pad(num, size) {
        var s = num + "";
        while (s.length < size) s = "0" + s;
        return s;
    }

    return {
        loaderStart: loaderStart,
        loaderStop: loaderStop,
        getParameterByName: getParameterByName,
        queryStringResult: queryStringResult,
        submitForm: submitForm,
        showLoader: showLoader,
        hideLoader: hideLoader,
        pad : pad
    }
})();