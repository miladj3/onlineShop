// Public Functions ---------------------------------------

// delete item from cart
function deleteItemFromCart(productId, callback) {
    $.ajax({
        url: '/api/cart/removeItem/' + productId,
        type: 'POST',
        async: true,
        contentType: "application/json",
        success: callback,
        error: function (xhr) { toastr.error("Failed to remove item from cart. " + xhr.responseText); }
    });
}

// load/refresh cart info in nav bar
function getCartInfo() {
    $.ajax({
        url: '/cart/info',
        type: 'GET',
        async: true,
        cache: false,
        datatype: 'html',
        contentType: 'application/json',
        success: function (data) {

            $('#cart-info-item-count').text(data.cartItemCount);
            if (data.cartItemCount > 0) {
                $('#cart-info-item-count').removeClass('badge-secondary');
                $('#cart-info-item-count').addClass('badge-danger');
                $('#cart-info-amount').text(data.cartAmount + ' PLN');
            } else {
                $('#cart-info-item-count').removeClass('badge-danger');
                $('#cart-info-item-count').addClass('badge-secondary');
                $('#cart-info-amount').text('');
            }
        },
        error: function (xhr) { toastr.error("Error occured when loading cart preview markup. " + xhr.responseText) }
    });
}

// load/refresh cart preview
function getCartPreview() {
    $.ajax({
        url: '/cart/load/preview/',
        type: 'GET',
        async: true,
        cache: false,
        datatype: 'html',
        contentType: 'application/json',
        success: function (cartMarkupOutput) {

            $('#cartPreviewContent').html(cartMarkupOutput);
            console.log('Cart PREVIEW markup loaded.')
        },
        error: function (xhr) { toastr.error("Error occured when loading cart markup. " + xhr.responseText) }
    });
}

// Events ---------------------------------------

$(document).ready(function () {

    getCartInfo();

    // fetch cart items when cart preview is displayed
    $('#cartInfo').click(function () {
        let preview = $('#cartPreview');

        if (preview.css('display') == 'none') {
            getCartPreview();
            $('#cartPreview').show();
        }
        else {
            $('#cartPreview').hide();
        }
    });

    // close cart preview if clicked outside
    $(document).click(function (e) {
        let preview = $('#cartPreview');
        if (preview.css('display') != 'none') {
            let cartArea = $('.nav-cart-preview');

            if (cartArea.find(e.target).length == 0) {
                $('#cartPreview').hide();
            }
        }
    });

    // add product to cart from product page
    $('#add-to-cart').click(
        function () {
            let productId = $(this).attr('data-product-id');
            let quantity = $('#add-quantity').val();

            $.ajax({
                url: '/api/cart/addItem/' + productId + '/' + quantity,
                type: 'POST',
                contentType: "application/json",
                success: function () {
                    getCartInfo();
                },
                error: function (xhr) { toastr.error("Failed to add item to cart. " + xhr.responseText); }
            });
        });

    // add product to cart from catalog page
    $(document).on('click', '[name="catalog-add-to-cart"]',
        function () {
            let productId = $(this).attr('data-product-id');
            let quantity = 1;
            console.log(productId);
            $.ajax({
                url: '/api/cart/addItem/' + productId + '/' + quantity,
                type: 'POST',
                contentType: "application/json",
                success: function () {
                    getCartInfo();
                },
                error: function (xhr) { toastr.error("Failed to add item to cart. " + xhr.responseText); }
            });
        });

    // remove product from cart
    $(document).on('click', '#remove-from-cart',
        function () {
            let productId = $(this).attr('data-product-id');

            deleteItemFromCart(productId, UpdateUI);

            function UpdateUI() {
                updateFullCartMarkup();
                getCartInfo();
                getCartPreview();
            }
        });
});