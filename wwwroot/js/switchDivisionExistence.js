$(document).ready(function () {
    
    switchVisibility($("#DivisionsExist").val());

    $("#DivisionsExist").change(function () {
        let selectedOption = $(this).val();
        switchVisibility(selectedOption);
    });
    function switchVisibility(value) {
        switch (value) {
            case 'true': {
                $('#mapHolder').hide();
            } break;
            case 'false': {
                $('#mapHolder').show();
            } break;
            default: {

            } break;
        }
    }
});