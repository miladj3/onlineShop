// show filters sidebar
function toggleFilters() {
    var filtersContainer = $('.filters-container');
    var filtersToggle = $('.filters-toggle');

    filtersToggle.toggleClass('filters-toggle-hidden');
    filtersToggle.toggleClass('filters-toggle-shown');

    filtersContainer.toggleClass('filters-container-hidden');
    filtersContainer.toggleClass('filters-container-shown');
}

// init and listen price range slider
$(function () {

    let queryMinPrice = $('#query-min-price').val();
    let queryMaxPrice = $('#query-max-price').val();

    let priceLimitMin = parseInt($('#query-min-price').attr('data-l-limit'));
    let priceLimitMax = parseInt($('#query-max-price').attr('data-u-limit'));

    let currentRangeMin = queryMinPrice == 0 ? priceLimitMin : queryMinPrice;
    let currentRangeMax = queryMaxPrice == 0 ? priceLimitMax : queryMaxPrice;

    $("#slider-range").slider({
        range: true,
        min: priceLimitMin,
        max: priceLimitMax,
        values: [currentRangeMin, currentRangeMax],
        slide: function (event, ui) {
            $("#amount").val(ui.values[0] + " - " + ui.values[1] + " PLN");
            $('#query-min-price').val(ui.values[0]);
            $('#query-max-price').val(ui.values[1]);
        }
    });
    $("#amount").val($("#slider-range").slider("values", 0) +
        " - " + $("#slider-range").slider("values", 1) + " PLN");
});