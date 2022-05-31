var URL = (function () {
    'use strict';

    function ToSEO(val) {
        var retVal =
        val.toLocaleLowerCase('tr-TR')
        .replaceAll('ç', 'c')
        .replaceAll('ı', 'i')
        .replaceAll('ğ', 'g')
        .replaceAll('ö', 'o')
        .replaceAll('ş', 's')
        .replaceAll('ü', 'u')
        .replaceAll('(', '')
        .replaceAll(')', '')
        .replaceAll('&', 've')
        .replaceAll('^','')
        .replaceAll('%', '')
        .replaceAll('#', '')
        .replaceAll('?', '')
        .replaceAll('.', '')
        .replaceAll(',', '')
        .replaceAll('-', '')
        .replaceAll(' ', '')
        .replaceAll('*', '')
        .replaceAll('!', '')
        .replaceAll('$', '')
        .replaceAll('/', '')
        .replaceAll('\\', '')

        return retVal;
    }

    String.prototype.replaceAll = function (str1, str2, ignore) {
        return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
    }

    return {
        ToSEO: ToSEO
    }
})();