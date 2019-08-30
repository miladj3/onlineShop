// prevents fixed navbar from overlapping body content
$(document).ready(function () {
    adjustBodyPadding();
})

$(window).resize(function () {
    adjustBodyPadding();
});

function adjustBodyPadding() {
    $('body').css('padding-top', parseInt($('#navbarMain').css("height")));
}