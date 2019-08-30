
// keep reference to initialized datatable
var onlineShop_entityDatatable; 

// hide part of navigation buttons on smaller screens
$(window).resize(function () {
    var ww = $(window).width();

    if (ww > 1000) {

        $.fn.DataTable.ext.pager.numbers_length = 7;
        onlineShop_entityDatatable.draw();
        $(".first.paginate_button, .last.paginate_button").add();

    } else {

        $.fn.DataTable.ext.pager.numbers_length = 3;
        onlineShop_entityDatatable.draw();
        $(".first.paginate_button, .last.paginate_button").remove();
    };
});

// initialize datatable with basic configuration depending on entity provided
function InitTableForEntity(entityName) {
    
    switch (entityName) {

        case 'Products':

            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false },
                    { "orderable": false, "searchable": false }
                ]
            });
            break;

        case 'Subcategories':
            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                responsive: true,

                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false }
                ]
            });
            break;

        case 'Categories':

            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                responsive: true,

                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false }
                ]
            });
            break;

        case 'Departments':

            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                responsive: true,

                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false },
                    { "orderable": true, "searchable": true },
                    { "orderable": false, "searchable": false }
                ]
            });
            break;

        case 'OrdersAll':

            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                "responsive": true,
                "pagingType": "full_numbers",
                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true }
                ],
                "order": [[0, "desc"]]
            });

            break;

        case 'BlogsAll':

            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                ],
                "order": [[0, "desc"]]
            });
            break;

        case 'UsersAll':

            onlineShop_entityDatatable = $('.entity-datatable').DataTable({
                "responsive": true,
                "columns": [
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true },
                    { "orderable": true, "searchable": true }
                ],
                "order": [[3, "desc"]]
            });
            break;
    }
}