
// ############################################################################# PUBLIC FUNCTIONS

function updateFullCartMarkup() {
    $.ajax({
        url: '/cart/load/full',
        type: 'GET',
        async: true,
        cache: false,
        datatype: 'html',
        contentType: 'application/json',
        success: function (cartFullMarkup) {
            $('#cart-full-area').html(cartFullMarkup);
            console.log('Full cart markup loaded.')
        },
        error: function (xhr) { alert("Error occured when loading full cart markup. " + xhr.responseText) }
    });
};

function updateItemQuantity(productId, quantity, _callback) {
    $.ajax({
        url: '/api/cart/updateItem/' + productId + '/' + quantity,
        type: 'POST',
        async: true,
        contentType: "application/json",
        success: _callback,
        error: function (xhr) { alert("Failed to update item in cart. " + xhr.responseText); }
    });

}

// ############################################################################# EVENT HANDLERS

$(document).ready(function () {

    $(document).on('click', '#remove-from-cart-full',
        function () {
            let productId = $(this).attr('data-product-id');

            deleteItemFromCart(productId, UpdateUI);

            function UpdateUI() {
                updateFullCartMarkup();
                getCartInfo();
                getCartPreview();
            }
        });

    $(document).on('click', '#item-quantity-control',
        function () {

            let control = $(this);
            let controlAction = control.attr('data-action');
            let itemProductId = control.attr('data-product-id');

            let itemQuantity = parseInt($('#item-quantity-for-product-' + itemProductId).val());

            if (controlAction == 'reduce') {

                if (itemQuantity > 1) {
                    itemQuantity--;
                }
            }
            else if (controlAction == 'increase') {
                itemQuantity++;
            }

            $('#item-quantity-for-product-' + itemProductId).val(itemQuantity);

            updateItemQuantity(itemProductId, itemQuantity, UpdateUI);
          
            function UpdateUI() {
                updateFullCartMarkup();
                getCartInfo();
                getCartPreview();
            }
        });
});