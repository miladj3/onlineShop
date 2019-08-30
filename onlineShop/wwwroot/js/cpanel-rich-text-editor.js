// Upload new image
function uploadFiles() {
    var input = $('#filesToUpload')[0];
    var files = input.files;
    var formData = new FormData();

    for (var i = 0; i < files.length; i++) {
        formData.append("files", files[i]);
    }

    $.ajax({
        url: '/ControlPanel/FileUpload/',
        data: formData,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (imagePaths) {

            let uploaded = $('#filesUploaded');

            for (let i = 0; i < imagePaths.length; i++) {
                uploaded.prepend('<a href="javascript:;" class="ctrl-img-insert">' + imagePaths[i] + '</a>')
                $('#filesUploaded').show();
            }

        },
        error: function (data) {
            toastr.error('Error occured. ' + data.responseText);
        }
    });

    // reset form
    $('#uploadForm').get(0).reset();

}

$(document).ready(function () {
    // CKEDITOR
    CKEDITOR.replace('rich-text-editor');

    // Controls to append image uploaded
    $(document).on('click', '.ctrl-img-insert', function () {
        let imagePath = $(this).text();
        let imagePathNormalized = "/" + imagePath.replace("\\", "/");

        InsertUploadedImage(imagePathNormalized);
    });

    // Inserts image into editor
    function InsertUploadedImage(imgUrl) {
        CKEDITOR.instances['rich-text-editor'].insertHtml('<img src="' + imgUrl + '" />');
    }

    $(document).on('click', '#resetUploaded', function () {
        $('.ctrl-img-insert').remove();
        $('#filesUploaded').hide();
    });
});