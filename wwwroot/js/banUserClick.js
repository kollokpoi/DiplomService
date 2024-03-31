var clicked = 0;
var id;

$(document).ready(function () {
    $("#sendButton").click(function () {

        clicked++;
        if (clicked === 1) {
            $("#timeSelect").show();
        } else if (clicked === 2) {
            $("#banForm").submit();
        }
    });
});