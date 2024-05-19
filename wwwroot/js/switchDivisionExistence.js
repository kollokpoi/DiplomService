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
                $('#divisionLeadersHolder').hide();
                $('#divisionLeadersList').empty();
            } break;
            case 'false': {
                $('#mapHolder').show();
                $('#divisionLeadersHolder').show();
                if ($('#divisionLeadersList').children().length == 0) {
                    getUserSuggestTemplate();
                }
            } break;
            default: {

            } break;
        }
    }
});