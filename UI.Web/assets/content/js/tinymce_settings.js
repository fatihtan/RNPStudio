function initTinymce(selector, height) {
    tinymce.init({
        selector: selector,
        height: height,
        plugins: 'print preview paste importcss searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap emoticons',
        menubar: 'file edit view insert format tools table help',
        toolbar: 'bold italic underline strikethrough | fontselect fontsizeselect formatselect forecolor backcolor | alignleft aligncenter alignright alignjustify | outdent indent | numlist bullist | removeformat | insertfile image media link codesample | fullscreen  code | undo redo',
        fontsize_formats: '8px 9px 10px 11px 12px 13px 14px 15px 16px 17px 18px 19px 20px 21px 22px 23px 24px 36px 48px 56px 72px',
        skin: 'oxide-dark',
        content_css: 'dark',
        browser_spellcheck: true,
        contextmenu: false,
        nonbreaking_force_tab: true,
        image_caption: true,
        image_advtab: true,
        toolbar_sticky: true,
        extended_valid_elements: "script[src|async|defer|type|charset]"
    });
}