$(document).ready(function () {

    let dateOfStart = $("input[name='StartDate']").val();
    let dateOfEnd = $("input[name='EndDate']").val();
    let divisionsExists = $("#DivisionExist").val() === 'True';
    let eventId = $("#EventId").val();

     
    $(document).on('change', '.measureUniqueCheckbox', function () {
        let isChecked = $(this).prop("checked");
        var checkboxContainer = $(this).closest(".measureItem");
        var toggleBlock = checkboxContainer.find(".multiDaysPick");

        if (isChecked) {
            toggleBlock.show();
        } else {
            toggleBlock.hide();
        }
    });
    $("#addDateBtn").on("click", function () {
        let inputs = $(this).closest(".multiDaysPick").find('.dateOfMeasureTimeBlock');

        let item = getMeasureDateTemplate(inputs.length);

        if (inputs.length >= 1) {
            $(this).closest(".multiDaysPick").find('#deleteDateBtn').show();
        }
        $(this).closest(".multiDaysPick").find('#multiDaysPickHolder').append(item);
    });
    $("#deleteDateBtn").on("click", function () {
        let inputs = $(this).closest(".multiDaysPick").find('.dateOfMeasureTimeBlock');
        let places = $(this).closest(".multiDaysPick").find('.dateOfMeasurePlaceBlock');
        if (inputs.length > 1) {
            inputs[inputs.length - 1].remove();
            places[places.length - 1].remove();
        }
        if (inputs.length === 2) {
            $(this).closest(".multiDaysPick").find('#deleteDateBtn').hide();
        }
    });
    $(document).on('change', '#measurePeriodicity', function () {
        let value = $(this).val();
        let parentBlock = $(this).closest('.datesBlock');

        if (value === 'true') {
            parentBlock.find('#singleDatePick').show();
            parentBlock.find('#multiplyDatePick').hide();
        } else if (value === 'false') {
            parentBlock.find('#multiplyDatePick').show();
            parentBlock.find('#singleDatePick').hide();
        }
    });
    $(document).on('change', '#measureDatesMode', function () {
        let value = $(this).val();
        let parentBlock = $(this).closest('#multiplyDatePick');

        if (value === 'true') {
            parentBlock.find('#weekDaysPick').show();
            parentBlock.find('#weekDaysPick').addClass('d-flex');
            parentBlock.find('.multiDaysPick').hide();
        } else if (value === 'false') {
            parentBlock.find('.multiDaysPick').show();
            parentBlock.find('#weekDaysPick').removeClass('d-flex');
            parentBlock.find('#weekDaysPick').hide();
        }
    });
    $(document).on('click', '.measureTitle', function () {
        $(this).next(".datesBlock").slideToggle();
    });
    $(document).on('click', '.MeasureAddDateBtn', function () {
        let inputs = $(this).closest(".multiDaysPick").find('.dateOfMeasureTimeBlock');
        let measureIndex = $(this).data('index');

        let item = getMeasureDivisionDateTemplate(inputs.length, measureIndex);

        if (inputs.length >= 1) {
            $(this).closest(".multiDaysPick").find('#deleteDateBtn').show();
        }
        $(this).closest(".multiDaysPick").find('#multiDaysPickHolder').append(item);
    })
    function getMeasureDateTemplate(index) {
        return `<div class="d-flex align-items-center">
            <div class="form-group my-2 dateOfMeasureTimeBlock">
            <label for="MeasureDates[${index}].Datetime" class="control-label"> Укажите дату и время</label>
            <input type="datetime-local" class="login-form__input form-control" data-val="true" data-val-required="Поле обязательно для заполнения."
                id="MeasureDates_${index}__Datetime" name="MeasureDates[${index}].Datetime"
                aria-describedby="MeasureDates_${index}__Datetime-error" aria-invalid="false">
            <span class="text-danger field-validation-valid" data-valmsg-for="MeasureDates[${index}].Datetime" data-valmsg-replace="true"></span>
            </div>
            <div class="form-group ms-3 dateOfMeasurePlaceBlock">
            <label for="MeasureDates[${index}].Place" class="control-label"> Укажите место проведения</label>
            <input name="MeasureDates[${index}].Place" class="login-form__input form-control"/>
            </div>
            </div>`;
    };
    function getMeasureDivisionDateTemplate(index,measureIndex) {
        return `<div class="d-flex align-items-center">
            <div class="form-group my-2 dateOfMeasureTimeBlock">
            <label for="Measures[${measureIndex}].MeasureDates[${index}].Datetime" class="control-label"> Укажите дату и время</label>
            <input type="datetime-local" class="login-form__input form-control" data-val="true" data-val-required="Поле обязательно для заполнения."
                id="Measures_${measureIndex}__MeasureDates_${index}__Datetime" name="Measures[${measureIndex}].MeasureDates[${index}].Datetime"
                aria-describedby="MeasureDates_${index}__Datetime-error" aria-invalid="false">
            <span class="text-danger field-validation-valid" data-valmsg-for="Measures[${measureIndex}].MeasureDates[${index}].Datetime" data-valmsg-replace="true"></span>
            </div>
            <div class="form-group ms-3 dateOfMeasurePlaceBlock">
            <label for="Measures[${measureIndex}].MeasureDates[${index}].Place" class="control-label"> Укажите место проведения</label>
            <input name="Measures[${measureIndex}].MeasureDates[${index}].Place" class="login-form__input form-control"/>
            </div>
            </div>`;
    };
});