(function ($) {
    "use strict";

    $.fn.phoneNumberMask = function (options) {

        var settings = $.extend({
            'countryCode': '90',
            'phoneNumber': '(555) 555-5555'
        }, options);

        return this.each(function () {

            if ($.fn.inputmask == undefined) {
                return;
            }

            var $this = $(this);
            var $settings = settings;

            var phoneNumber = $this.data('phoneNumber');
            var countryCode = $this.data('countryCode');

            if (phoneNumber == undefined) {
                phoneNumber = $settings.phoneNumber;
            }

            if (countryCode == undefined) {
                countryCode = $settings.countryCode;
            }

            var mask = '+';

            countryCode += '';
            var stableCountryCode = '';
            for (var i = 0; i < countryCode.length; i++) {
                if (countryCode[i] == '9') {
                    stableCountryCode += '\\' + countryCode[i];
                }
                else {
                    stableCountryCode += countryCode[i];
                }
            }

            mask += stableCountryCode + ' ';
            
            phoneNumber = phoneNumber.replace(new RegExp("[0-9]", "g"), "9");
            mask += phoneNumber;

            console.log(mask);

            $this.inputmask("mask", { mask: mask })
        });
    };
})(jQuery);