function initFroala(selector, height, websiteID, predefinedLinks, fullPage) {
    if (predefinedLinks == undefined) {
        predefinedLinks = [];
    }

     return new FroalaEditor(selector,
        {
            imageUploadParam: 'image',
            imageUploadURL: '/Froala/UploadImage?WebsiteID=' + websiteID,
            imageUploadMethod: 'POST',

            fileUploadParam: 'file',
            fileUploadURL: '/Froala/UploadFile?WebsiteID=' + websiteID,
            fileUploadMethod: 'POST',

            videoUploadParam: 'video',
            videoUploadURL: '/Froala/UploadVideo?WebsiteID=' + websiteID,
            videoUploadMethod: 'POST',

            toolbarButtons: {
                'moreText': {
                    'buttons': ['bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'textColor', 'backgroundColor', 'inlineClass', 'inlineStyle', 'clearFormatting'],
                    'buttonsVisible': 9
                },
                'moreParagraph': {
                    'buttons': ['alignLeft', 'alignCenter', 'alignRight', 'alignJustify', 'formatOL', 'formatUL', 'paragraphFormat', 'paragraphStyle', 'lineHeight', 'outdent', 'indent', 'quote'],
                    'buttonsVisible': 13
                },
                'moreRich': {
                    'buttons': ['insertLink', 'insertTable', 'insertImage', 'insertVideo', 'insertFile'],
                    'buttonsVisible': 5
                },
                'moreStaff': {
                    'buttons': ['emoticons', 'fontAwesome', 'specialCharacters', 'embedly', 'insertHR'],
                    'buttonsVisible': 5
                },
                'moreMisc': {
                    'buttons': ['undo', 'redo', 'fullscreen', 'html', 'print', 'getPDF', 'selectAll', 'help'],
                    'align': 'right',
                    'buttonsVisible': 4
                }
            },
            linkList : predefinedLinks,
            fontList: ["Tahoma, Geneva", "Arial, Helvetica", "Impact, Charcoal"],
            language: "tr",
            height: height,
            enter: FroalaEditor.ENTER_P,
            fontSizeDefaultSelection: '21',
            fontSize: ['8', '10', '12', '14', '16', '18', '19', '20', '21', '22', '23', '27', '30', '35', '47', '51', '60', '72', '96'],
            htmlDoNotWrapTags: ['strong'],
            htmlAllowedEmptyTags: ['a', 'abbr', 'address', 'area', 'article', 'aside', 'audio', 'b', 'base', 'bdi', 'bdo', 'blockquote', 'br', 'button', 'canvas', 'caption', 'cite', 'code', 'col', 'colgroup', 'datalist', 'dd', 'del', 'details', 'dfn', 'dialog', 'div', 'dl', 'dt', 'em', 'embed', 'fieldset', 'figcaption', 'figure', 'footer', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'header', 'hgroup', 'hr', 'i', 'iframe', 'img', 'input', 'ins', 'kbd', 'keygen', 'label', 'legend', 'li', 'link', 'main', 'map', 'mark', 'menu', 'menuitem', 'meter', 'nav', 'noscript', 'object', 'ol', 'optgroup', 'option', 'output', 'p', 'param', 'pre', 'progress', 'queue', 'rp', 'rt', 'ruby', 's', 'samp', 'script', 'style', 'section', 'select', 'small', 'source', 'span', 'strike', 'strong', 'sub', 'summary', 'sup', 'table', 'tbody', 'td', 'textarea', 'tfoot', 'th', 'thead', 'time', 'title', 'tr', 'track', 'u', 'ul', 'var', 'video', 'wbr', 'ins', 'meta'],
            htmlExecuteScripts: false,
            htmlUntouched: true,
            htmlRemoveTags: [],
            htmlAllowedTags: ['a', 'abbr', 'address', 'area', 'article', 'aside', 'audio', 'b', 'base', 'bdi', 'bdo', 'blockquote', 'br', 'button', 'canvas', 'caption', 'cite', 'code', 'col', 'colgroup', 'datalist', 'dd', 'del', 'details', 'dfn', 'dialog', 'div', 'dl', 'dt', 'em', 'embed', 'fieldset', 'figcaption', 'figure', 'footer', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'header', 'hgroup', 'hr', 'i', 'iframe', 'img', 'input', 'ins', 'kbd', 'keygen', 'label', 'legend', 'li', 'link', 'main', 'map', 'mark', 'menu', 'menuitem', 'meter', 'nav', 'noscript', 'object', 'ol', 'optgroup', 'option', 'output', 'p', 'param', 'pre', 'progress', 'queue', 'rp', 'rt', 'ruby', 's', 'samp', 'script', 'style', 'section', 'select', 'small', 'source', 'span', 'strike', 'strong', 'sub', 'summary', 'sup', 'table', 'tbody', 'td', 'textarea', 'tfoot', 'th', 'thead', 'time', 'title', 'tr', 'track', 'u', 'ul', 'var', 'video', 'wbr', 'ins', 'meta'],
            emoticonsStep: 10,
            charCounterCount: true
        }
    );
}
