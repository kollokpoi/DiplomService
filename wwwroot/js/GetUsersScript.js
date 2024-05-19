$(document).ready(() => {
    let selectedIds = $('.DivisionLeaders.Id').val();
    $(document).on("input", '.userSuggestionInput', function () {
        let value = $(this).val();
        var url = "/api/User/UserSuggestion";
        var query = value;
        var parentDiv = $(this).closest('.position-relative');
        idInput = parentDiv.find('.DivisionLeadersId');
        idInput.val('');
        let suggestionsDropdown = parentDiv.find('.suggestionsUserDropdown');
        selectedIds = $('.DivisionLeadersId').val();
        if (query === "") {
            suggestionsDropdown.empty();
            suggestionsDropdown.hide();
            return;
        }
        
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            data: { suggestion: query },
            success: function (result) {
                console.log(result);
                

                suggestionsDropdown.empty();

                if (result.length > 0) {
                    for (let i = 0; i < result.length; i++) {
                        if (!selectedIds.includes(result[i].id)) {
                            suggestionsDropdown.append(`<div class="userSuggestion" data-Id="${result[i].id}">${result[i].userName}</div>`);
                        }
                    }

                    suggestionsDropdown.show();
                } else {
                    suggestionsDropdown.hide();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                suggestionsDropdown.empty();
            }
        });
    });


    $(document).on("click", '.userSuggestion', function () {
        let userId = $(this).data('id');
        let userName = $(this).text();
        
        var parentDiv = $(this).closest('.position-relative');
        let index = parentDiv.index();

        idInput = parentDiv.find('input[name="DivisionLeaders[' + index + '].Id"]');
        idInput.val(userId);

        var userNameInput = parentDiv.find('input[name="DivisionLeaders[' + index + '].UserName"]');
        userNameInput.val(userName);

        let suggestionsDropdown = parentDiv.find('.suggestionsUserDropdown');
        suggestionsDropdown.empty();
        suggestionsDropdown.hide();
        selectedIds = $('.DivisionLeaders.Id').val();
    });

    $('#divisionLeaderAddBtn').on('click', function () {
        getUserSuggestTemplate()
    })
    $(document).on('click', '.deleteBtn', function() {
        var parentDiv = $(this).closest('.my-2');
        parentDiv.remove();
    })
});
function getUserSuggestTemplate() {
    $.ajax({
        type: "GET",
        url: "/Events/GetUserSuggestTemplate",
        contentType: "application/json",
        data: { index: $('.suggestionsUserDropdown').length },
        success: function (result) {

            $('#divisionLeadersList').append(result);

        },
        error: function (xhr, textStatus, errorThrown) {
            suggestionsDropdown.empty();
        }
    });
}