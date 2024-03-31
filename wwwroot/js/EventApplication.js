$(document).ready(() => {

    let id = $('#EventId').val();
    let index = 1;

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
            var secondName = lastBlock.find('input[name$=".SecondName"]').val();
            var name = lastBlock.find('input[name$=".Name"]').val();
            $('p.measureTitle:last').text(secondName + ' ' + name);
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
});