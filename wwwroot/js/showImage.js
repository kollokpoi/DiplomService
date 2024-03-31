$(document).ready(function () {
    $('#previewImageButton').on('change', function () {
        var input = this;

        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#previewImage').attr('src', e.target.result);
            };

            reader.readAsDataURL(input.files[0]);
        }
    });
    $(document).on('change',".previewImageInput", function () {
        var input = this;
        let image = $(this).prev('img');
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                image.attr('src', e.target.result);
            };

            reader.readAsDataURL(input.files[0]);
        }
    });
    $(document).on('click', '.previewImageButton', function () {
        let input = $(this).prev('input[type="file"]');
        let image = $(this).closest('.previewImage');
        input.click();
    });
});