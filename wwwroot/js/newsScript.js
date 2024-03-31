$(document).ready(function () {
    $("#addBtn").click(function (e) {
        e.preventDefault();
        let index = $(".sectionBlock").length;

        $.ajax({
            url: '/News/GetParticipantEntry?Index=' + index,
            type: 'GET',
            success: function (partialView) {
                $("#ItemsHolder").append(partialView);
            },
            error: function (error) {
                console.error('Ошибка при выполнении второго запроса:', error);
            }
        });
    });

    $(document).on('click', '.deleteBtn', function () {
        let toDelete = $(this).prev("input");
        let delBoolean = toDelete.val() == 'true';
        let index = $(this).data("index");
        if (delBoolean) {
            toDelete.val('false');
            $(`#Sections_${index}__Name`).prop('readonly', false);
            $(`#Sections_${index}__Description`).prop('readonly', false);
            $(`#Sections_${index}__ImageFile`).prop('readonly', false);
            $(this).parent().css("opacity", "1");
        } else {
            toDelete.val('true');
            $(`#Sections_${index}__Name`).prop('readonly', true);
            $(`#Sections_${index}__Description`).prop('readonly', true);
            $(`#Sections_${index}__ImageFile`).prop('readonly', true);
            $(this).parent().css("opacity", "0.3");
        }
 
    });
})