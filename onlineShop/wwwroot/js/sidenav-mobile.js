// mobile side nav toggle
function toggleMobileNav() {
    var mobileNav = $('#mobileSideNav');

    if (mobileNav.width() > 0) {
        mobileNav.width(0);
    } else {
        mobileNav.width(250);
    }
}