// Redirect user to page generating N number of new fields
$(document).on('change', '#ctrl-addFields-number', function () {
    let numFields = $(this).val();

    let subcategoryId = $(this).attr('data-subcategoryId');
    let newUrl = '/ControlPanel/subcategories/' + subcategoryId + '/productDescriptionFields?newFields=' + numFields;
    window.location = newUrl;
})

// Click in item removal control triggers modal
$(document).on('click', '.ctrl-item-delete', function () {
    let itemName = $(this).attr('data-itemName');
    let itemId = $(this).attr('data-itemId');

    $('#itemName').text(itemName);
    $('#itemId').text('(id:' + itemId + ')');
    $('#itemDeleteConfirm').attr('data-itemId', itemId);
    $('#deleteItemModal').modal();
});

// Delete action confirmation within modal
$(document).on('click', '#itemDeleteConfirm', function () {

    let itemId = $(this).attr('data-itemId');
    $('#deleteItemModal').modal('hide');
    $.ajax({
        url: '/ControlPanel/productDescriptionFields/' + itemId + '/delete',
        method: 'post',
        error: function (xhr) { toastr.error('Error occured. ' + xhr.responseText) },
        success: function () {
            var targetRow = $('.ctrl-item-delete[data-itemId=' + itemId + ']').parent().parent().parent().parent();
            targetRow.addClass('debugview');
            targetRow.remove();
            toastr.success('Item deleted.');
        }
    });
});
