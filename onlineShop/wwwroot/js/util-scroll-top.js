
$(document).ready(function () {

    //draw button if current scroll exceed predefined limit
    window.onscroll = function () {

        if (document.documentElement.scrollTop > 200) {
            $('#scrollToTop').css('display', 'block');
        } else {
            $('#scrollToTop').css('display', 'none');
        }
    };

    //scroll to the top of window with animation
    $('#scrollToTop').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 'fast');
    });
});