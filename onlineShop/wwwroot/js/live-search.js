
// redirect to SearchResult action
$(document).ready(function () {
    var invokeCatalogSearch = function () {
        let query = $('#ctrl-search-input').val();
        window.location = "/Catalog/Search?searchString=" + query;
    }

    // invoke search by pressing enter within search input field
    $(document).on('keypress', '#ctrl-search-input', function (e) {
        if (e.keyCode == 13 && $('#ctrl-search-input').val().length > 0) {
            invokeCatalogSearch();
        }
    });

    // invoke search by clicking search button
    $(document).on('click', '#ctrl-search-button', function () {
        if ($('#ctrl-search-input').val().length > 0) {
            invokeCatalogSearch();
        }
    });

    // destroy dropdown
    let destroyDropdown = function () {

        $('#catalogSearch').css('display', 'none');
        $('#catalogSearchResults').html('');
    };

    // close dropdown by clicking window area
    $(window).on('click', function () {
        destroyDropdown();
    });

    // search when typing criteria
    $(document).on('keyup', '#ctrl-search-input', function () {

        if ($(window).width() > 500) {   // disable preview for small screens
            let input = $('#ctrl-search-input');
            if (input.val().length < 3) {

                destroyDropdown();

            } else {

                let query = input.val();

                // fetch list of results
                $.get({
                    url: '/catalog/SearchPreview/' + query,
                    async: true,
                    error: function () { },
                    success: function (data) {

                        if (data.resultCount > 0) {

                            $('#catalogSearchResults').html('');

                            let maxResultCount = 10;

                            // append results to dropdown
                            for (let i = 0; i < Math.min(data.resultCount, maxResultCount); i++) {
                                let productId = data.results[i].Id;

                                let item = document.createElement('a');
                                item.innerHTML = '<span> ' + data.results[i].Name + '</span>' + '<span class="font-italic text-secondary">' + data.results[i].SubcategoryName + '</span > ';
                                item.className = 'search-preview-item';
                                item.href = 'http://localhost:8888/Catalog/Product/' + productId;
                                item.onclick = function (event) { window.location.href = item.href }; // fix for IE11

                                $('#catalogSearchResults').append(item);
                            }

                            // append line "showing X out of Y results"
                            if (data.resultCount > maxResultCount) {
                                let item = document.createElement('span');
                                item.innerHTML = '<span></span><span class="txt0_8em font-italic text-secondary">(showing ' + maxResultCount + ' out of ' + data.resultCount + ' results)</span>';
                                item.className = 'search-preview-item';
                                $('#catalogSearchResults').append(item);
                            }

                            // display dropdown
                            $('#catalogSearch').css('display', 'block');
                        }
                    }
                });
            }
        }
    });
});

