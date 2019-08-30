$(document).ready(function () {
    $(document).on('change', '.ctrl-role-input', function () {
        $(this).siblings('label').toggleClass('font-weight-bold').toggleClass('text-danger');
        $('#msg-warning').css('display', 'block');
        $('#ctrl-form-submit').removeClass('btn-primary').addClass('btn-danger');
    });

    $(document).on('change', '.ctrl-block-input', function () {
        $('label.ctrl-block-input').toggleClass('font-weight-bold');
    });

    $(document).on('click', '#ctrl-show-role-area', function () {

        let userRoleArea = $('#user-roles-area');

        userRoleArea.toggle();
        let isVisible = userRoleArea.css('display') == "none";
        if (isVisible) {
            $(this).text('Add Roles');
        } else {
            $(this).text('Hide Roles');
            $('html, body').animate({ scrollTop: userRoleArea.offset().top }, 'fast');
        }
    });
});