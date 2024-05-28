$(document).ready(function () {

    let dateOfStart = $("input[name='StartDate']").val();
    let dateOfEnd = $("input[name='EndDate']").val();
    let divisionsExists = $("#DivisionExist").val() === 'True';
    var eventForm = $("#eventForm");
    var measureForm = $(".measuresForm");
    var datesblockCopy = $('#singleDatePick')[0].innerHTML;


    $.validator.unobtrusive.parse($(eventForm));

    $("#addBtn").on("click", function () {
        if (CheckMeasuresValid()) {
            addMeasureForm();
        }
    });
    $("#backButton").on("click", function () {
        event.preventDefault(); 

        var confirmation = confirm("Сохранить введенные данные?");

        if (confirmation) {
            if (CheckMeasuresValid()) {
                $("#measuresForm").submit();
                window.location.href = $("#backButton").attr("href") + '?returnUrl=' + window.location.href;
            }
        } else {
            window.location.href = $("#backButton").attr("href") +'?returnUrl='+ window.location.href;
        }
    });
    $('#deleteBtn').on('click', function () {
        if ($('.measureItem').length > 1) {
            $('.measureItem:last').remove();
            $('.measureTitle:last').remove();
            if (measureForm.length < 2) {
                $('#deleteBtn').show();
            }
        }
    });

    $("#addDivisionBtn").on("click", function () {
        if (CheckDivisionValid()) {
            addDivisionForm();
        }
    });
    $("#backDivisionButton").on("click", function () {
        event.preventDefault();

        var confirmation = confirm("Сохранить введенные данные?");

        if (confirmation) {
            if (CheckMeasuresValid()) {
                $("#measuresForm").submit();
                window.location.href = $("#backDivisionButton").attr("href") + '?returnUrl=' + window.location.href;
            }
        } else {
            window.location.href = $("#backDivisionButton").attr("href") + '?returnUrl=' + window.location.href;
        }
    });

    $(document).on('click', '#addDateBtn', function () {
        
        let inputs = $(this).closest(".multiDaysPick").find('.dateOfMeasureBlock');
        let isValid = true;
        inputs.each(function () {
            var input = $(this).find('input');
            var value = input.val();
            if (value === '') {
                isValid = false;
                $(this).find('.text-danger').show();
            } else {
                $(this).find('.text-danger').hide();
            }
        });
        if (!isValid) {
            return;
        }
        var index = $(this).data("index");
        let item = getMeasureDateTemplate(index);
        if (inputs.length >= 1) {
            $(this).closest(".multiDaysPick").find('#deleteDateBtn').show();
        }
        $(this).closest(".multiDaysPick").find('#multiDaysPickHolder').append(item);
    });
    $(document).on('click', '#deleteDateBtn', function () {
        let inputs = $(this).closest(".multiDaysPick").find('.dateOfMeasureBlock');
        if (inputs.length > 1) {
            inputs[inputs.length - 1].remove();
        }
        if (inputs.length === 2) {
            $(this).closest(".multiDaysPick").find('#deleteDateBtn').hide();
        }
    });
    $(document).on('change', '#measurePeriodicity', function () {
        let value = $(this).val();
        let parentBlock = $(this).closest('.datesBlock');

        if (value ==='true') {
            parentBlock.find('#singleDatePick').show();
            parentBlock.find('#multiplyDatePick').hide();
            parentBlock.find('#singleDatePick').append(datesblockCopy);
        } else if (value ==='false') {
            parentBlock.find('#multiplyDatePick').show();
            parentBlock.find('#singleDatePick').hide();
            parentBlock.find('#singleDatePick').empty();
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
            $(this).next(".measureItem").slideToggle("slow");
    });
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
    $(document).on('click', '.divisionMeasureTitle', function () {
        $(this).next(".datesBlock").slideToggle("slow");
    });
    function CheckMeasuresValid() {
        let measuresValid = true;
        $(".measuresForm").each(function () {
            $.validator.unobtrusive.parse($(this));
            if (!$(this).valid()) {
                measuresValid = false;
                return false;
            }
        });
        return measuresValid;
    }
    function CheckDivisionValid() {
        let measuresValid = true;
        $("#divisionsForm").each(function () {
            $.validator.unobtrusive.parse($(this));
            if (!$(this).valid()) {
                measuresValid = false;
                return false;
            }
        });
        return measuresValid;
    }
    function getMeasureFormTemplate(index) {
        return '<p class="measureTitle">Новое мероприятие</p>' +
            '<div class="measureItem">' +
            '<div class="form-group"> ' +
            '<label class="control-label" for="Models_' + index + '__Name">Название*</label>' +
            '<input data-val="true" type="text" value="" class="login-form__input form-control" data-val-required="Обязательно для заполнения" id="Models_' + index + '__Name" name="Models[' + index + '].Name">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Models[' + index + '].Name" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="form-group my-2">' +
            '<label class="control-label" for="Models_' + index + '__Length">Укажите продолжительность*</label>' +
            '<input type="time" value="00:00" class="login-form__input form-control" data-val="true" data-val-required="Обязательно для заполнения" id="Models_' + index + '__Length" name="Models[' + index + '].Length">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].Length">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Models[' + index + '].Length" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="datesBlock">' +
            '<div class="form-group my-2">' +
            '<label class="control-label" for="Models_' + index + '__PeriodicityMode">Частота проведения</label>' +
            '<select class="login-form__input form-control" title="Будет ли отображаться в общем списке" id="measurePeriodicity" data-val="true" data-val-required="Обязательно для заполнения" name="Models[' + index + '].PeriodicityMode">' +
            '<option selected value="true">Одноразовое</option>' +
            '<option value="false">Повторяющееся</option>' +
            '</select>' +
            '</div>' +
            '<div style="display:none" id="multiplyDatePick">' +
            '<div class="form-group my-2">' +
            '<label class="control-label" for="Models_' + index + '__MultiDaysMode">Режим выбора дат</label>' +
            '<select class="login-form__input form-control" title="Будет ли отображаться в общем списке" id="measureDatesMode" data-val="true" data-val-required="The MultiDaysMode field is required." name="Models[' + index + '].MultiDaysMode">' +
            '<option selected value="false">Даты</option>'+
            '<option value="true">Дни недели</option>'+
            '</select>' +
            '</div>' +
            '<div class="form-group my-3 d-flex" id="weekDaysPick" style="display:none!important;">' +
            '<div class="w-25">' +
            '<p>' +
            'Понедельник' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '<p>' +
            'Вторник' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '<p>' +
            'Среда' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '<p>' +
            'Четверг' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '</div>' +
            '<div class="w-25 ms-5">' +
            '<p>' +
            'Пятница' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '<p>' +
            'Суббота' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '<p>' +
            'Воскресенье' +
            '<input required="" value="00:00" class="login-form__input d-inline-block" type="time" id="Models_' + index + '__DaysOfWeek" name="Models[' + index + '].DaysOfWeek">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DaysOfWeek">' +
            '</p>' +
            '</div>' +
            '</div>' +
            '<div class="form-group my-3 multiDaysPick" >' +
            '<div id="multiDaysPickHolder" class="w-25">' +
            '<div class="form-group my-2 dateOfMeasureBlock">' +
            '<label class="control-label" for="Models_' + index + '__Multidates">Укажите дату и время</label>' +
            '<input type="datetime-local" min="' + dateOfStart + '" max="' + dateOfEnd +'" class="login-form__input form-control" id="Models_' + index + '__Multidates" name="Models[' + index + '].Multidates" value="">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].Multidates">' +
            '<span class="text-danger" style="display:none;">Обязательно для заполнения</span>' +
            '</div>' +
            '</div>' +
            '<div class="btn btn-primary me-3" id="deleteDateBtn" style="display:none;">Удалить</div>' +
            '<div class="btn btn-primary" data-index="Models[' + index + '].Multidates" id="addDateBtn">Добавить</div>' +
            '</div>' +
            '</div>' +
            '<div style="" class="form-group my-2" id="singleDatePick">' +
            '<label class="control-label" for="Models_' + index + '__DateTime">Укажите дату и время проведения*</label>' +
            '<input min="' + dateOfStart + '" value="" max="' + dateOfEnd + '" data-val="true" type="datetime-local" class="login-form__input form-control" id="Models_' + index + '__DateTime" name="Models[' + index + '].DateTime">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].DateTime">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Models[0].DateTime" data-valmsg-replace="true"></span>' +
            '</div>' +
            '</div>' +
            '<div class="my-2 measureUniqueBlock">' +
            '<div class="form-check">' +
            '<label class="control-label" for="Models_' + index + '__SameForAll">Общее мероприятие для всех направлений</label>' +
            '<input type="checkbox" class="form-check-input measureUniqueCheckbox" data-val="true" data-val-required="The SameForAll field is required." id="Models_' + index + '__SameForAll" name="Models[' + index + '].SameForAll" value="true">' +
            '</div>' +
            '</div>' +
            '<div class="form-group my-2" id="measurePlaceBlock">' +
            '<label class="control-label" for="Models_' + index + '__Place">Место проведения *</label>' +
            '<input class="login-form__input form-control" type="text" data-val="true" data-val-required="Обязательно для заполнения" id="Models_' + index + '__Place" name="Models[' + index + '].Place" value="">' +
            '<span data-valmsg-replace="true" class="text-danger field-validation-valid" data-valmsg-for="Models[' + index + '].Place"></span>' +
            '</div>' +
            '</div>'
    };
    function getMeasureDivisionsFormTemplate(index) {
        return '<p class="measureTitle">Новое мероприятие</p>' +
            '<div class="measureItem">' +
            '<div class="form-group mb-2">' +
            '<label class="control-label" for="Models_' + index + '__Name">Название*</label>' +
            '<input data-val="true" type="text" value="" class="login-form__input form-control valid" data-val-required="Обязательно для заполнения" id="Models_' + index + '__Name" name="Models[' + index + '].Name" aria-describedby="Models_' + index + '__Name-error" aria-invalid="false">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Models[' + index + '].Name" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="form-group my-2">' +
            '<label class="control-label" for="Models_' + index + '__Length">Укажите продолжительность*</label>' +
            '<input type="time" value="00:00" class="login-form__input form-control valid" data-val="true" data-val-required="Обязательно для заполнения" id="Models_' + index + '__Length" name="Models[' + index + '].Length" aria-describedby="Models_' + index + '__Length-error">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].Length">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Models[' + index + '].Length" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="form-group my-2" id="measurePlaceBlock">' +
            '<label class "control-label" for="Models_' + index + '__Place">Место проведения*</label>' +
            '<input class="login-form__input form-control valid" type="text" data-val="true" data-val-required="Обязательно для заполнения" id="Models_' + index + '__Place" name="Models[' + index + '].Place" value="" aria-describedby="Models_' + index + '__Place-error" aria-invalid="false">' +
            '<span data-valmsg-replace="true" class="text-danger field-validation-valid" data-valmsg-for="Models[' + index + '].Place"></span>' +
            '</div>' +
            '<div class="my-2 measureUniqueBlock">' +
            '<div class="form-check">' +
            '<label class="control-label" for="Models_' + index + '__SameForAll">Общее мероприятие для всех направлений</label>' +
            '<input type="checkbox" class="form-check-input measureUniqueCheckbox valid" data-val="true" data-val-required="The SameForAll field is required." id="Models_' + index + '__SameForAll" name="Models[' + index + '].SameForAll" value="true" aria-describedby="Models[' + index + '].SameForAll-error">' +
            '</div>' +
            '</div>' +
            '<div class="form-group my-3 multiDaysPick" style="display:none">' +
            '<div id="multiDaysPickHolder"  class="w-25">' +
            '<div class="form-group my-2 dateOfMeasureBlock">' +
            '<label class="control-label" for="Models_' + index + '__Multidates">Укажите дату и время</label>' +
            '<input type="datetime-local" min="' + dateOfStart + '" max="' + dateOfEnd +'" class="login-form__input form-control valid" id="Models_0__Multidates" name="Models[' + index + '].Multidates" value="" aria-invalid="false">' +
            '<input name="__Invariant" type="hidden" value="Models[' + index + '].Multidates">' +
            '<span class="text-danger" style="display:none;">Обязательно для заполнения</span>' +
            '</div>' +
            '</div>' +
            '<div class="btn btn-primary me-3" id="deleteDateBtn" style="display:none">Удалить</div>' +
            '<div class="btn btn-primary" data-index="' + index + '" id="addDateBtn">Добавить</div>' +
            '</div>' +
            '</div>';
    };
    function getMeasureDateTemplate(index) {
        return '<div class="form-group my-2 dateOfMeasureBlock">'+
            '<label class="control-label" for="Models_'+index+'__Multidates">Укажите дату и время</label>'+
            '<input type="datetime-local" min="' + dateOfStart + '" max="' + dateOfEnd +'" class="login-form__input form-control valid" id="Models_' + index + '__Multidates" name="Models['+index+'].Multidates"  aria-invalid="false">'+
            '<input name = "__Invariant" type = "hidden" value = "'+index+'"> '+
            '<span class="text-danger" style="display:none;">Обязательно для заполнения</span>'+
            '</div>'
    };
    function getDivisionFormTemplate(measure, index) {
        let result ='<p class="measureTitle">Новое подразделение</p>'+
            '<div class="measureItem">' +
            '<div class="form-group mb-2">' +
            '<label class="control-label" for="Divisions_' + index + '__Named">Название*</label>' +
            '<input data-val="true" type="text" value="" class="login-form__input form-control" data-val-required="Обязательно для заполнения." id="Divisions_' + index + '__Named" name="Divisions[' + index + '].Named">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Divisions[' + index + '].Named" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="form-group my-2">' +
            '<label class="control-label" for="Divisions_' + index + '__Description">Описание</label>' +
            '<textarea class="login-form__input form-control" id="Divisions_' + index + '__Description" name="Divisions[' + index + '].Description"></textarea>' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Divisions[' + index + '].Description" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="form-group my-2">' +
            '<label class="control-label" for="Divisions_' + index + '__MainPlace">Основное место проведения</label>' +
            '<input class="login-form__input form-control" type="text" id="Divisions_' + index + '__MainPlace" name="Divisions[' + index + '].MainPlace" value="">' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="Divisions[' + index + '].MainPlace" data-valmsg-replace="true"></span>' +
            '</div>' +
            '<div class="my-2 mb-3">' +
            '<label class="control-label" for="Divisions_' + index + '__PreviewImageFile">Изображение</label>' +
            '<div class="form-group d-flex align-items-center">' +
            '<img src="/Images/camara.png" alt="Image" id="previewImage" class="item-block__image">' +
            '<div>' +
            '<input id="previewImageButton" accept=".png, .jpg, .jpeg, .gif" type="file" placeholder="" name="Divisions[' + index + '].PreviewImageFile">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '<p class="fw-bold lead">Расписание</p>' +
            '<div class="form-group my-2 ps-3" id="measuresHolder">';
        for (var i = 0; i < measure.length; i++) {
            result += '<input type="hidden" name="Divisions[' + index + '].Measures[' + i + '].Id" value="' + measure[i].id + '"/>' +
                '<input type="hidden" name="Divisions[' + index + '].Measures[' + i + '].Name" value="' + measure[i].name + '""/>'+
                '<p class="divisionMeasureTitle">' + measure[i].name +'</p>'+
                '<div class="datesBlock">' +
                '<label for="Divisions[' + index + '].Measures[' + i + '].Enjoys" class="control-label">Учавствует ли направление в мероприятии</label>' +
                '<input name="Divisions[' + index + '].Measures[' + i + '].Enjoys" type="checkbox" checked class="form-check-input measureUniqueCheckbox" />' +
                '<div class="form-group my-2">' +
                '<label class="control-label" for="Divisions_0__Measures_0__PeriodicityMode">Частота проведения</label>' +
                '<select class="login-form__input form-control" title="Будет ли отображаться в общем списке" id="measurePeriodicity" data-val="true" data-val-required="Обязательно для заполнения" name="Divisions[' + index + '].Measures[' + i +'].PeriodicityMode">' +
                '<option selected="" value="oneTime">Одноразовое</option>' +
                '<option value="multiply">Повторяющееся</option>' +
                '</select>' +
                '</div>' +
                '<div style="display:none" id="multiplyDatePick">' +
                '<div class="form-group my-2">' +
                '<label class="control-label" for="Divisions_0__Measures_0__MultiDaysMode">Режим выбора дат</label>' +
                '<select class="login-form__input form-control" title="Будет ли отображаться в общем списке" id="measureDatesMode" data-val="true" data-val-required="The MultiDaysMode field is required." name="Divisions[' + index + '].Measures[' + i +'].MultiDaysMode">' +
                '<option selected="" value="dates">Даты</option>' +
                '<option value="daysOfWeek">Дни недели</option>' +
                '</select>' +
                '</div>' +
                '<div class="form-group my-3 d-flex" id="weekDaysPick" style="display:none!important">' +
                '<div class="w-25">' +
                '<p>' +
                'Понедельник' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[0]">' +
                '</p>' +
                '<p>' +
                'Вторник' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[1]">' +
                '</p>' +
                '<p>' +
                'Среда' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[2]">' +
                '</p>' +
                '<p>' +
                'Четверг' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[3]">' +
                '</p>' +
                '</div>' +
                '<div class="w-25 ms-5">' +
                '<p>' +
                'Пятница' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[4]">' +
                '</p>' +
                '<p>' +
                'Суббота' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[5]">' +
                '</p>' +
                '<p>' +
                'Воскресенье' +
                '<input value="00:00" class="login-form__input d-inline-block" type="time" name="Divisions[' + index + '].Measures[' + i +'].DaysOfWeek[6]">' +
                '</p>' +
                '</div>' +
                '</div>' +
                '<div class="form-group my-3 multiDaysPick" style="">' +
                '<div id="multiDaysPickHolder" class="w-25">' +
                '<div class="form-group my-2 dateOfMeasureBlock">' +
                '<label class="control-label" for="Divisions_0__Measures_0__Multidates_0_">Укажите дату и время</label>' +
                '<input type="datetime-local" class="login-form__input form-control" id="Divisions_0__Measures_0__Multidates_0_" name="Divisions[' + index + '].Measures[' + i + '].Multidates[0]" value=' + measure[i].multidates[0]+' ><input name="__Invariant" type="hidden" value="Divisions[' + index + '].Measures[' + i +'].Multidates[0]">' +
                '<span class="text-danger" style="display:none;">Обязательно для заполнения</span>' +
                '</div>' +
                '</div>' +
                '<div class="btn btn-primary me-3" id="deleteDateBtn" style="display:none;">Удалить</div>' +
                '<div class="btn btn-primary" data-index="Divisions[' + index + '].Measures[' + i +'].Multidates" id="addDateBtn">Добавить</div>' +
                '</div>' +
                '</div>' +
                '<div style="" class="form-group my-2" id="singleDatePick">' +
                '<div class="form-group my-2" id="singleDatePick">' +
                '<label class="control-label" for="Divisions_0__Measures_0__Multidates_0_">Укажите дату и время проведения*</label>' +
                '<input type="datetime-local" class="login-form__input form-control" id="Divisions_0__Measures_0__Multidates_0_" name="Divisions[' + index + '].Measures[' + i + '].Multidates[0]" value="2023-11-07T00:00:00.000"><input name="__Invariant" type="hidden" value="Divisions[' + index + '].Measures[' + i +'].Multidates[0]">' +
                '<span class="text-danger field-validation-valid" data-valmsg-for="Divisions[' + index + '].Measures[' + i +'].Datetime" data-valmsg-replace="true"></span>' +
                '</div>' +
                '<div class="form-group my-2" id="measurePlaceBlock">'+
                '<label for="Divisions[' + index + '].Measures[' + i + '].Place"  class="control-label">Место проведения *</label>'+
                '<input name="Divisions[' + index + '].Measures[' + i + '].Place" value="'+measure[i].place+'" class="login-form__input form-control" />'+
                '<span asp-validation-for="Models[i].Place" data-valmsg-replace="true" class="text-danger"></span></div>';
        }
        result +='</div></div>'
        return result;
    }
    function addMeasureForm() {
        let length = $('.measureItem').length;
        let item = divisionsExists ? getMeasureDivisionsFormTemplate(length) : getMeasureFormTemplate(length);
        let title = $('.measureTitle:last');
        let t = $('.measureItem:last').find('input:first');
        title.text(t.val());

        $("#measuresHolder").append(item);
        measureForm.push(item);
        $('.measuresForm:last').fadeIn(400);
        if (measureForm.length > 1) {
            $('#deleteBtn').show();
        }
    }
    function addDivisionForm() {

        var currentURL = window.location.href;
        var id = currentURL.split('/').pop();
        $.ajax({
            url: '/Events/GetMeasures/' + id,
            type: 'GET',
            dataType: 'json',
            success: function (measuresData) {
                console.log(measuresData);
                let length = $('.measureItem').length;
                let item = getDivisionFormTemplate(measuresData,length);
                let title = $('.measureTitle:last');
                let t = $('.measureItem:last').find('input:first');
                title.text(t.val());

                $("#divisionsHolder").append(item);
                $('.measureItem:last').fadeIn(400);
                if ($('.measureItem').length > 1) {
                    $('#deleteBtn').show();
                }
            },
            error: function (error) {
                console.error('Ошибка при выполнении второго запроса:', error);
            }
        });

    }
});

