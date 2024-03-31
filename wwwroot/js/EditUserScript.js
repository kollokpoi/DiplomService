$(document).ready(function () {
    $(".removeUserBtn").click(function () {
        let index = $(this).data("index");
        let item = $('.item-block').eq(index);
        let divisionId = $(this).data("id");
        let eventId = $('#eventId').val();
        $.ajax({
            url: `/Events/RemoveFromDivision`,
            type: 'POST',
            data: { divisionId: divisionId, eventId: eventId },
            success: function () {
                item.attr('style', 'display: none !important');
            },
            error: function (error) {
                console.error('Ошибка при выполнении запроса:', error);
            }
        });
    });
})