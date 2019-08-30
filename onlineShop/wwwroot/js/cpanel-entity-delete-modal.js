
function InitItemDelete(entityName) {

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
            url: '/ControlPanel/' + entityName + '/' + itemId + '/Delete',
            method: 'post',
            error: function (xhr) { toastr.error('Failed to delete item. ' + xhr.responseText) },
            success: function (data) {

                //window.location.href = data.redirectUrl;
                let entityDatatable = $('.entity-datatable').DataTable();

                //find row with item id that was deleted
                let targetRow = $('tr td:first-of-type:contains(' + itemId + ')').filter(
                    function () { return ($(this).text() == itemId); }
                ).closest('tr');

                //bind to datatables and remove from DOM
                let targetRowDt = entityDatatable.row(targetRow);
                entityDatatable.row(targetRowDt).remove().draw(false);

                toastr.info("Item deleted.");

            }
        });
    });
}

