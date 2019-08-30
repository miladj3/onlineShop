//
// Below code is based on approach described in the following source: https://www.w3schools.com/howto/howto_js_form_steps.asp 
//

var currentTab = 0;
var selectedDelMethod;
showTab(currentTab);

// Remove 'invalid' class from fields populated
var inputFields = document.querySelectorAll('.form-control');
inputFields.forEach(function (item) {
    item.addEventListener('input', function () {
        this.classList.remove('invalid');
    })
});

// Draw/hide appropriate delivery detail fields based on delivery method selected
function prepDeliveryDetails() {

    var dMethods = document.getElementsByName('DeliveryMethodType');

    for (i = 0; i <= dMethods.length; i++) {
        if (dMethods[i].checked) {
            selectedDelMethod = dMethods[i].value;
            break;
        }
    }

    var ddSpecificFields = document.querySelectorAll('[data-for-del-method]');

    for (i = 0; i < ddSpecificFields.length; i++) {

        if (ddSpecificFields[i].getAttribute('data-for-del-method') != selectedDelMethod) {
            ddSpecificFields[i].style.display = 'none';
            ddSpecificFields[i].classList.remove('required');
            ddSpecificFields[i].previousElementSibling.style.display = 'none';
        } else {
            ddSpecificFields[i].style.display = 'inline-block';
            ddSpecificFields[i].classList.add('required');
            ddSpecificFields[i].previousElementSibling.style.display = 'inline-block';
        }
    }
}

// Payment method preparation based on method selected
function prepPaymentMethods() {

    var pmtMethods = document.getElementsByName('PaymentMethod');

    for (i = 0; i < pmtMethods.length; i++) {

        if (pmtMethods[i].getAttribute("data-lock-for-del-method") == selectedDelMethod) {
            pmtMethods[i].disabled = true;
            pmtMethods[i].checked = false;
        } else {
            pmtMethods[i].disabled = false;
        }
    }
}

// Trigger swtich to tab N
function nextPrev(n) {

    var x = document.getElementsByClassName('tab');

    // don't move on to the next for if some fields are invalid
    if (n == 1 && !validateForm()) return false;

    x[currentTab].style.display = 'none';
    currentTab = currentTab + n;

    if (currentTab < x.length) {
        showTab(currentTab);
    }
}

// Form input validation
function validateForm() {

    var x, y, i, valid = true;
    x = document.getElementsByClassName('tab');
    y = x[currentTab].getElementsByTagName('input');

    // Check if radio box if selected
    let ctrlRadioSets = ['delivery-method-switch', 'payment-method-switch'];

    let errorMsg = x[currentTab].getElementsByClassName('validation-error')[0];

    // Reset error message area
    if (errorMsg != null)
        errorMsg.innerHTML = '';

    for (let q = 0; q < ctrlRadioSets.length; q++) {

        let ctrlSet = x[currentTab].getElementsByClassName(ctrlRadioSets[q]);

        if (ctrlSet.length > 0) {

            let ctrlSetValid;

            for (let w = 0; w < ctrlSet.length; w++) {
                let ctrl = ctrlSet[w];
                if (ctrl.checked) {
                    ctrlSetValid = true;
                }
            }

            if (!ctrlSetValid) {
                valid = false;
                errorMsg.innerHTML = errorMsg.innerHTML + 'Please choose from options available.';
            } 
        }
    }

    // Check terms and conditions agreement
    let agreement = x[currentTab].getElementsByClassName('user-agreement')[0];
    if (agreement != null) {
        if (!agreement.checked) {
            valid = false;
            errorMsg.innerHTML = errorMsg.innerHTML + 'Please accept required agreements.';
        } 
    }

    // Check every input:
    for (i = 0; i < y.length; i++) {

        if (y[i].classList.contains('required') && y[i].value == "") {
            // If a field is empty, aappend Invalid class and set status:
            y[i].className += ' invalid';
            valid = false;
        }
    }

    return valid;
}

// Draw tab with index N
function showTab(n) {

    // Before drawing next tab, make preparations depending on the current stage
    if (n == 1) {
        prepDeliveryDetails();
    }

    if (n == 2) {
        prepPaymentMethods();
    }

    if (n == 3) {
        validateAndGetSummary(drawOrderValidationResult);
    }

    if (n == 4) {
        confirmationKey = document.getElementById('nextBtn').getAttribute('data-confirmation-key');
        submitOrder(confirmationKey, drawOrderSubmitResult);
    }

    // Display the specified tab of the form 
    var x = document.getElementsByClassName('tab');
    x[n].style.display = 'block';

    // Adjust buttons
    if (n == 0) {
        document.getElementById('prevBtn').style.display = 'none';
    } else {
        document.getElementById('prevBtn').style.display = 'inline';
    }

    // Order validation page adjustments
    if (n == (x.length - 2)) {
        // Prevent user from seeing the same message after retrying upon error
        document.getElementById('order-validation-status').innerHTML = '';
        document.getElementById('order-summary-message').innerHTML =
            '<div class="spinner-border text-secondary" role="status"></div>Processing your order...'; 

        // Block 'Submit' button before greenlight recieved from server
        document.getElementById('nextBtn').disabled = true;
        document.getElementById('nextBtn').innerHTML = 'Submit';
        document.getElementById('prevBtn').innerHTML = 'Edit';
    } else {
        document.getElementById('nextBtn').disabled = false;
        document.getElementById('nextBtn').innerHTML = 'Next';
        document.getElementById('prevBtn').innerHTML = 'Previous';
    }

    // Order submission page adjustments
    if (n == x.length - 1) {
        document.getElementById('nextBtn').style.display = 'none';
        document.getElementById('prevBtn').style.display = 'none';
        document.getElementById('closeBtn').style.display = 'block';
    } else {
        document.getElementById('closeBtn').style.display = 'none';
    }

    // Update progress bar
    updateProgressBar(n)
}

// Updates step panel
function updateProgressBar(n) {

    // Remove 'active' class from all steps
    var i, x = document.getElementsByClassName('step');

    for (i = 0; i < x.length; i++) {
        x[i].className = x[i].className.replace(' step-active', '');
    }
    // Append 'active' class to the current step
    x[n].className += ' step-active';
}

// Submit current form inputs for server validation and retrieve order summary with confirmation key before submitting order
function validateAndGetSummary(callback) {

    let form = $('#checkoutForm');

    $.ajax({
        url: '/API/Checkout/Summary',
        data: form.serialize(),
        type: 'POST',
        async: true,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (data) {
            callback(true, data);
        },
        error: function (xhr) {
            callback(false, xhr.responseText);
        }
    });
}

// Submit order using confirmation key
function submitOrder(confirmationKey, callback) {

    // prevent user from closing form before server response arrives
    $('#closeBtn').disabled = true;

    $.ajax({
        url: '/API/Checkout/Confirm/' + confirmationKey,
        type: 'POST',
        async: true,

        success: function (data) {
            callback(true, data.orderId);
        },
        error: function (xhr) {
            callback(false, xhr.responseText);
        }
    });
}

// Draw error response during validation
function drawOrderValidationResult(success, data) {

    if (success) {

        document.getElementById('order-validation-status').innerText = 'Review and Confirm Your Order:';
        document.getElementById('order-validation-status').classList.remove('text-danger');
        document.getElementById('order-summary-message').innerHTML = data.summaryMarkup;
        document.getElementById('nextBtn').setAttribute('data-confirmation-key', data.confirmationKey);
        document.getElementById('nextBtn').disabled = false;

    } else {
        document.getElementById('order-validation-status').innerText = 'Error occured.';
        document.getElementById('order-validation-status').classList.add('text-danger');
        document.getElementById('order-summary-message').innerText = data;
        document.getElementById('nextBtn').setAttribute('data-confirmation-key', '');
        document.getElementById('nextBtn').disabled = true;
    }
}

// Draw server response after order submission
function drawOrderSubmitResult(success, data) {

    if (success) {

        $('#order-submit-status').text('Order Submitted!');
        $('#order-submit-status').removeClass('text-danger');
        $('#order-submit-message').text('Your order ID: ' + data + '. Order confirmation has been sent to your email address.');

        getCartInfo();

    } else {

        $('#order-submit-status').text('Error occured.');
        $('#order-submit-status').addClass('text-danger');
        $('#order-submit-message').text(data);
    }

    $('#closeBtn').disabled = false;
}

// Leave checkout page
function exitCheckout() {
    window.location = '/';
}

