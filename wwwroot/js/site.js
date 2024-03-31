$(document).ready(function () {
    $('.circled-search__input').on('input', function () {
        var searchText = $(this).val().toLowerCase();
        $('.item-block').each(function () {
            var title = $(this).find('.name').text().toLowerCase();

            if (title.indexOf(searchText) !== -1) {
                $(this).removeClass('hide');
            } else {
                $(this).addClass('hide');
            }
        });
    });

    $(".circled-search__input").on("focus", function () {
        $(this).closest(".circled-search__block").addClass("focused");
    });

    $(".circled-search__input").on("blur", function () {
        var $input = $(this);
        var $block = $input.closest(".circled-search__block");

        if ($input.val().trim() === "") {
            $block.removeClass("focused");
        }
    });

    function showItemsOnScroll() {
        var scrollTop = parseFloat($(window).scrollTop()); //прокручено
        var windowHeight = parseFloat($(window).height());//высота окна

        if (scrollTop > 100) {
            // Показать кнопку
            $('.fixed-block__image').fadeIn();
        } else {
            // Скрыть кнопку
            $('.fixed-block__image').fadeOut();
        }
        // При нажатии на кнопку
        $('.fixed-block').click(function () {
            $(window).scroll({
                top: 0,
                behavior: 'smooth' // Плавная прокрутка
            });
        });

        $('.item-block').each(function () {
            var elementTop = parseFloat($(this).offset().top);//верхнее положение блока

            if (!isNaN(elementTop) && (elementTop - scrollTop) < windowHeight && $(this).hasClass('animated')) {
                $(this).removeClass('animated');
            } else if (!isNaN(elementTop) && (elementTop - scrollTop) > windowHeight && !$(this).hasClass('animated')) {
                $(this).addClass('animated');
            }
        });

        if (scrollTop > windowHeight * 1.2) {
            $('.pop-up-header').addClass('visible');
        } else if ($('.pop-up-header').hasClass('visible')) {
            $('.pop-up-header').removeClass('visible');
        }
    }

    $(window).on('scroll', function () {
        showItemsOnScroll();
    });
    $(document).ready(function () {
        showItemsOnScroll();
    });

    $("#faq").click(function () {
        $("#faq-body").toggleClass('visible');
    })
});