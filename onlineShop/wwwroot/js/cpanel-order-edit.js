// Deselect and disable incompatible payment method
$(document).ready(function () {

    if ($('.delivery-mode-switch[value="Locker"]').attr("checked") == "checked") {
        $('#BankTransfer.payment-method-control').prop('checked', 'true');
        $('#Cash.payment-method-control').prop('disabled', 'true');
    }
});

$(document).on('click', '.delivery-mode-switch', function () {

    let selectedMethod = $(this).val();

    // Block incompatible payment methods for parcticular delivery mode
    if (selectedMethod.toLowerCase() == "locker") {
        $('#BankTransfer.payment-method-control').prop('checked', 'true');
        $('#Cash.payment-method-control').prop('disabled', 'true');

    } else {
        $('#Cash.payment-method-control').prop('disabled', false);
    }
    // Hide fields not relevant to selected delivery method
    $('.delivery-method-group').each(function () {

        let attr = $(this).attr('data-delivery-method');
        if (attr != 'all' && attr.toLowerCase() != selectedMethod.toLowerCase()) {
            $(this).css("display", "none");
        }
        else {
            $(this).css("display", "block");
        }
    });
});