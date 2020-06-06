window.link = {
    copyText: function (text) {
        navigator.clipboard.writeText(text).then(r => {
            //Just let it empty
        })
    }
}
$(function () {
    $("body").tooltip({selector: '[data-toggle=tooltip]'});
})
