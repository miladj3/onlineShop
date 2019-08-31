
// wait for images to load
$(window).on('load', function () {
    $('.sp-wrap').smoothproducts();
});

// load comments
$(document).ready(function () {
    let productId = $('#published-comments').attr('data-product-id');
    fetchComments(productId, 1);
});

function gotoComments() {
    $('#product-comments a').tab('show');
    $('html, body').animate({
        scrollTop: parseInt($("#product-comments").offset().top)
    }, 250);
}

function NotifyWhenAvailable(productId, guestEmail, action) {

    $.ajax({
        url: '/API/Product/AvailabilityNotification/' + action + '/' + productId + '/' + guestEmail,
        type: 'post',
        async: true,
        success: function () {

            let notificationArea = $('#product-notification-area');

            if (guestEmail != '') {
                notificationArea.html('<span class="text-success text-center d-block mt-2"><i class="fa fa-check"></i> We will inform you once product is available.</span>');
            }
            else {
                if (action == "add") {
                    notificationArea.html('<a href="javascript:;" id="product-inform-available" class="text-success text-center d-block mt-2" data-action="remove"><i class="fa fa-check"></i> In watchlist</a>');
                } else {
                    notificationArea.html('<a href="javascript:;" id="product-inform-available" class="text-warning text-center d-block mt-2" data-action="add"><i class="fa fa-info-circle"></i> Inform when available</a>');
                }
            }

        },
        error: function (xhr) { toastr.error(xhr.responseText);  }
    });
};

function fetchComments(productId, pageNumber) {
    $.get({
        url: '/Catalog/Product/' + productId + '/comments/' + pageNumber,
        success: function (data) {
            $('#published-comments').html(data);

            if ($('.product-rating-input').attr('data-rating-value') != 0) {
                $('.product-rating-input').hide();
            }

        },
        error: function (xhr) { xhr.responseText; }
    });
}

$(document).ready(

    function () {

        $(document).on('click', '#notification-email-confirm', function () {

            let notificationCtrl = $('#product-notification-area');

            let email = $('#notification-email-input').val();
            let productId = notificationCtrl.attr('data-product-id');

            NotifyWhenAvailable(productId, email, "Add");
            $('#product-notification-modal').modal('hide');
        });

        // product availability notification
        $(document).on('click', '#product-inform-available', function () {

            let notificationCtrl = $('#product-notification-area');

            if (notificationCtrl.attr('data-guest-user') == "True") {
                $('#product-notification-modal').modal('show');

            } else {
                let productId = notificationCtrl.attr('data-product-id');
                let action = $(this).attr('data-action');

                NotifyWhenAvailable(productId, "", action);
            }
        });

        // product rating & comment
        $('.product-rating-control').hover(function () {
            $(this).prevAll('.product-rating-control').toggleClass('rating-hovered');
            $(this).toggleClass('rating-hovered');
        });

        $(document).on('click', '.product-rating-control', function () {

            var control = $(this);
            var selectedRating;
            if (control.hasClass('rating-selected')) {
                selectedRating = 0;
            }
            else {
                selectedRating = control.attr('value');

                control.siblings('.product-rating-control').removeClass('rating-selected');
                control.siblings('.product-rating-control').removeClass('rating-active');
                control.removeClass('rating-active');
            }

            control.toggleClass('rating-selected');
            control.toggleClass('rating-active');

            control.prevAll('.product-rating-control').toggleClass('rating-active');
            $('.product-rating-input').attr('data-rating-value', selectedRating);

        });

        $(document).on('click', '.comment-page-link', function () {
            var productId = $('#product-comments').attr('data-product-id');
            var targetPage = $(this).attr('data-nav-id');

            fetchComments(productId, targetPage);
        });

        $(document).on('click', '#product-comment-submit', function () {
            let productId = $(this).attr('data-product-id');
            let commentText = $('#product-comment-text').val();

            let isGuestUser = $('#product-comment-guest-name').length > 0;
            let guestUserName = '';
            
            if (isGuestUser) {
                guestUserName = isGuestUser ? $('#product-comment-guest-name').val() : "";
            }

            let selectedRating = $('.product-rating-input').attr('data-rating-value');

            if (typeof (selectedRating) === 'undefined') {
                selectedRating = 0;
            }

            let isFormValid = true;

            if (commentText.length < 5) {
                toastr.error("Your comment is too short.");
                isFormValid = false;
            } else if (commentText.length > 1000) {
                toastr.error("Your comment is too long.");
                isFormValid = false;
            }

            if (isGuestUser && guestUserName.trim().length == 0) {
                toastr.error("Please provide your name.");
                isFormValid = false;
            }

            if (isFormValid) {

                let dataToSend = { "commentText": commentText, "guestUserName": guestUserName };
                var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();

                $.ajax({
                    url: '/api/products/comments/add/' + productId + '/' + selectedRating,
                    type: 'post',
                    data: JSON.stringify(dataToSend),
                    contentType: 'application/json; charset=utf-8',
                    async: true,
                    headers: { "X-XSRF-TOKEN": antiForgeryToken },
                    success: function () {

                        $('#product-comment-text').val('');
                        $('#comment-area').html('<span class="text-success">Thank you for your comment!</span><span class="text-success d-block">It will be published after validaiton.</span>');

                        fetchComments(productId, 1);

                        $.get({
                            url: '/Catalog/Product/' + productId + '/ratingSummary/',
                            success: function (data) {
                                $('.product-rating-summary-content').html(data);
                            },
                            error: function (xhr) { toastr.error(xhr.responseText); }
                        });
                    },
                    error: function (xhr) { toastr.error(xhr.responseText); }
                });
            }
        })
    }
);
