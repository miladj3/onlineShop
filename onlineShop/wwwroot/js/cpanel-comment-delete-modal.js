
// Item remove control click triggers modal
$(document).on('click', '.ctrl-item-delete', function () {
    let itemName = $(this).attr('data-itemName');
    let itemId = $(this).attr('data-itemId');
    let itemType = $(this).attr('data-itemType');

    $('#deleteItemName').text(itemName);
    $('.deleteItemType').text(itemType);
    $('#deleteItemId').text('(id:' + itemId + ')');

    $('#itemDeleteConfirm').attr('data-itemId', itemId);
    $('#deleteItemModal').modal();
});

// Delete action confirmation within modal
$(document).on('click', '#itemDeleteConfirm', function () {
    let itemId = $(this).attr('data-itemId');
    $('#deleteItemModal').modal('hide');
    $.ajax({
        url: '/API/Products/Comments/Delete/' + itemId,
        method: 'post',
        error: function (xhr) { toastr.error('Error occured. ' + xhr.responseText) },
        success: function () {

            let productId = $('#product-comments').attr('data-product-id');

            fetchComments(productId, 1);
            toastr.success('Item deleted.');
        }
    });
});

