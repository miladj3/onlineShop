
function fetchPendingComments(pageNumber) {
    $.get({
        url: '/ControlPanel/GetPendingComments/' + pageNumber,
        success: function (data) {
            $('#comment-list').html(data);
        },
        error: function (xhr) { xhr.responseText; }
    });
}

$(document).ready(function () {
    fetchPendingComments(1);
});

$(document).on('click', '.comment-page-link', function () {
    var targetPage = $(this).attr('data-nav-id');
    fetchPendingComments(targetPage);
});

$(document).on('click', '.ctrl-comment', function () {

    let itemId = $(this).attr('data-comment-id');
    let action = $(this).attr('data-action');

    $.ajax({
        url: '/API/Products/Comments/' + action +'/' + itemId,
        method: 'post',
        error: function (xhr) { toastr.error('Error occured. ' + xhr.responseText) },
        success: function () {
            fetchPendingComments(1);
        }
    });
});



