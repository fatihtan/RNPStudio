$(document).ready(function () {
    
});
//var navItems = $('.page-sidebar-menu .nav-item');
//if (navItems.length != 0) {
//    //var currentPage = window.location.pathname;
//    //if (currentPage == '/') {
//    //    navItems = $('#home_navigation')
//    //}
//    //else if (
//    //    currentPage == '/Contract/Detail' ||
//    //    currentPage.indexOf('/Contract/Detail/') > -1 ||
//    //    currentPage.indexOf('/Contract/ArticleShortVersion/') > -1 ||
//    //    currentPage.indexOf('/Contract/Articles/') > -1 ||
//    //    currentPage.indexOf('/Contract/NewArticle/') > -1) {
//    //    currentPage = '/Contract/List';
//    //}
//    //else if (currentPage.indexOf('/Contract/Contracts/') > -1 || currentPage == '/Contract/Contracts') {
//    //    currentPage = '/Contract/Categories'
//    //}
//    //else if (currentPage.indexOf('/Contract/Questions/') > -1) {
//    //    currentPage = '/Contract/Questions'
//    //}
//    //else if (currentPage.indexOf('/Contract/VariableDetail/') > -1) {
//    //    currentPage = '/Contract/Variables'
//    //}
//    //else if (currentPage.indexOf('/Contract/Feeds/') > -1 || currentPage.indexOf('/Contract/FeedDetail/') > -1) {
//    //    currentPage = '/Contract/Feeds'
//    //}

//    //for (var i = 0; i < navItems.length; i++) {

//    //    var anchor = navItems[i].children[0].href;

//    //    if (anchor.indexOf(currentPage) >= 0) {
//    //        navItems[i].className += ' active';
//    //        if (navItems[i].parentNode.parentNode.nodeName == 'DIV') {
//    //            var span = document.createElement("span");
//    //            span.className = 'selected';
//    //            navItems[i].firstElementChild.appendChild(span);
//    //        }
//    //        if (navItems[i].parentElement.parentElement.nodeName == 'LI') {
//    //            navItems[i].parentElement.previousElementSibling.children[2].className += ' open';
//    //            navItems[i].parentElement.parentElement.className += ' active open';

//    //            var span = document.createElement("span");
//    //            span.className = 'selected';
//    //            navItems[i].parentElement.parentElement.firstElementChild.appendChild(span);
//    //            break;
//    //        }
//    //    }
//    //}
//}

$(window).bind('beforeunload', function (e) {
    //App.startPageLoading({
    //    animate: !0
    //});
    //App.blockUI();
    App.blockUI({ animate: !0, boxed: 0 })
});
