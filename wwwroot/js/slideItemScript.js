$(document).ready(() => {

    $(document).on('click', '.measureTitle', function () {
        $(this).next(".items-section").slideToggle();
    });

});