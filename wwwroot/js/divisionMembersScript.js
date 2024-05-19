$(document).ready(() => {
    let users = [];
    let divisionLeader = $("#DivisionLeader").val() == 'True';
    $(document).on("input", '.userSuggestionInput', function () {
        let value = $(this).val();
        var url = "/api/User/DivisionMemberSuggestion";
        $('#UserId').val('');
        $('#additionalInfo').slideDown()
        let suggestionsDropdown = $('.suggestionsUserDropdown');
        if (value === "") {
            suggestionsDropdown.empty();
            suggestionsDropdown.hide();
            return;
        }

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            data: { suggestion: value, divsionLeader:divisionLeader},
            success: function (result) {
                console.log(result);

                users = result;
                suggestionsDropdown.empty();

                if (result.length > 0) {
                    for (let i = 0; i < result.length; i++) {
                        if (divisionLeader) {
                            suggestionsDropdown.append(`<div class="userSuggestion" data-index="${i}">${result[i].secondName + ' ' + result[i].name + ' ' + result[i].lastName + ' Организация:' + result[i].workingPlace}</div>`);
                        } else {
                            suggestionsDropdown.append(`<div class="userSuggestion" data-index="${i}">${result[i].secondName + ' ' + result[i].name + ' ' + result[i].lastName + ' Образовательное учреждение:' + result[i].workingPlace}</div>`);
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

        let index = parseInt($(this).data('index'))

        $('#UserId').val(users[index].id);
        $('#Name').val(users[index].secondName + ' ' + users[index].name + ' ' + users[index].lastName);
        $('#WorkingPlace').val(users[index].workingPlace);

        $('#additionalInfo').slideUp()

        let suggestionsDropdown = $('.suggestionsUserDropdown');
        suggestionsDropdown.empty();
        suggestionsDropdown.hide();
    });

});