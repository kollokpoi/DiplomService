$(document).ready(() => {

    let id = $('#EventId').val();
    let index = 1;
    let users = [];
    let selectedIds = [];

    $.validator.addMethod("pattern", function (value, element, params) {
        var pattern = new RegExp(params);
        return this.optional(element) || pattern.test(value);
    }, "Некорректный формат");

    $(document).on('click', '.measureTitle', function () {
        $(this).next(".itemBlock").slideToggle();
    });

    $(document).on('click', '.item-block__title', function () {
        $(this).next(".items-section").slideToggle();
    });

    $(document).on('click', '.deleteButton', function () {
        $('#holder').find('.measureTitle:last').remove();
        $(this).closest('.itemBlock').remove();
    });
    $('#addBtn').click(() => {
        var lastBlock = $('#holder').find('.itemBlock:last');
        if ($('#applicationForm').valid()) {
            var name = lastBlock.find('input[name$=".Name"]').val();
            $('p.measureTitle:last').text(name);
            $('.deleteButton:last').remove();
            $.ajax({
                url: "/EventApplications/GetParticipantEntry?Id=" + id + "&Index=" + index,
                type: "GET",
                success: function (partialView) {
                    $("#holder").append(partialView);
                    index++;
                    $('#holder').find('.itemBlock:last input[data-val="true"]').each(function () {
                        $(this).rules('add', {
                            required: true,
                            pattern: $(this).data('val-regex-pattern'),
                            messages: {
                                required: $(this).attr('data-val-required'),
                                pattern: $(this).data('val-regex')
                            }
                        });
                    });

                    $('#holder').find('.itemBlock:last select[data-val="true"]').each(function () {
                        $(this).rules('add', {
                            required: true,
                            messages: {
                                required: $(this).attr('data-val-required')
                            }
                        });
                    });
                }
            });
        }
    });
    $(document).on('click', '.delete-btn', function () {
        let itemBlock = $(this).closest('.itemBlock');
        let id = itemBlock.find("#dataId").val();

        $.ajax({
            url: "/EventApplications/DeleteData?Id=" + id,
            type: "POST",
            success: function (partialView) {
                let name = itemBlock.prev('.measureTitle').text();

                let commentTextArea = $('#commentTextArea');
                commentTextArea.val(`${commentTextArea.val()}\n Заявка на участие ${name} отклонена по причине:`);

                itemBlock.prev('.measureTitle').remove();
                itemBlock.remove();
                
                if ($(".itemBlock").length==0) {
                    $("#successBtn").remove();
                }
            }
        });
    });

    
    $(document).on("input", '.userSuggestionInput', function () {
        let value = $(this).val();
        var url = "/api/User/ApplicationMemberSuggestion";
        
        let parrentDiv = $(this).closest('.position-relative');
        let suggestionsDropdown = parrentDiv.find('.suggestionsUserDropdown');
        let itemIndex = parrentDiv.index()

        parrentDiv.next('.additionalData').slideDown()
        parrentDiv.find('input[name$=".UserId"]').val('')
        selectedIds = $('input[name$=".UserId"]').val();
        if (value === "") {
            suggestionsDropdown.empty();
            suggestionsDropdown.hide();
            return;
        }

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            data: { suggestion: value},
            success: function (result) {
                users = result;
                suggestionsDropdown.empty();

                if (result.length > 0) {
                    for (let i = 0; i < result.length; i++) {
                        if (!selectedIds.includes(result[i].id)) {
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
        let parrent = $(this).closest('.position-relative');

        if (selectedIds == "") {
            selectedIds = []
        } 
        selectedIds.push(users[index].id);

        let name = parrent.find('input[name$=".Name"]')
        let id = parrent.find('input[name$=".UserId"]')
        id.val(users[index].id);
        name.val(users[index].secondName + ' ' + users[index].name + ' ' + users[index].lastName);

        parrent.next('.additionalData').slideUp()

        let suggestionsDropdown = $('.suggestionsUserDropdown');
        suggestionsDropdown.empty();
        suggestionsDropdown.hide();
    });

});
