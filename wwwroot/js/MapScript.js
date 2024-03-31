ymaps.ready(init);

function init() {
    var myPlacemark,
        myMap = new ymaps.Map('map', {
            center: [55.753994, 37.622093],
            zoom: 13
        }, {
            searchControlProvider: 'yandex#search'
        });


    myMap.events.add('click', function (e) {
        var coords = e.get('coords');

        if (myPlacemark) {
            myPlacemark.geometry.setCoordinates(coords);
        }

        else {
            myPlacemark = createPlacemark(coords);
            myMap.geoObjects.add(myPlacemark);

            myPlacemark.events.add('dragend', function () {
                getAddress(myPlacemark.geometry.getCoordinates());
            });
        }
        getAddress(coords);
    });


    function createPlacemark(coords) {
        return new ymaps.Placemark(coords, {
            iconCaption: 'поиск...'
        }, {
            preset: 'islands#violetDotIconWithCaption',
            draggable: true
        });
    }

    function getAddress(coords) {
        myPlacemark.properties.set('iconCaption', 'поиск...');
        var url = "http://suggestions.dadata.ru/suggestions/api/4_1/rs/geolocate/address";
        var token = "b39b45a2ac15a8f8eb139365a70c67e73c345ea6";
        var query = { lat: coords[0], lon: coords[1], count: 1 };

        var options = {
            type: "POST",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(query),
            headers: {
                "Accept": "application/json",
                "Authorization": "Token " + token
            },
            success: function (result) {
                console.log(result);
                if (result.suggestions.length > 0) {
                    let parts = result.suggestions[0].value.split(',');
                    console.log(parts);
                    $('#PlaceName').val(result.suggestions[0].value);
                    $('#Longitude').val(coords[1]);
                    $('#Latitude').val(coords[0]);

                    $('#Latitude').val($('#Latitude').val().replace('.', ','));
                    $('#Longitude').val($('#Longitude').val().replace('.', ','));
                    myPlacemark.properties
                        .set({
                            iconCaption: [
                                parts[0] || '',
                                parts[1] || '', parts[2] || ''
                            ].filter(Boolean).join(', '),
                            // В качестве контента балуна задаем строку с адресом объекта.
                            balloonContent: result.suggestions[0].value
                        });
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("error", textStatus, errorThrown);
            }
        };

        $.ajax(options);
    }
    $('#PlaceName').on("input",function () {
        let value = $(this).val();
        var url = "http://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/address";
        var token = "b39b45a2ac15a8f8eb139365a70c67e73c345ea6";
        var query = value;


        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            headers: {
                "Accept": "application/json",
                "Authorization": "Token " + token
            },
            data: JSON.stringify({ query: query }),
            success: function (result) {
                console.log(result);
                let suggestionsDropdown = $('#suggestionsDropdown');

                suggestionsDropdown.empty();

                if (result.suggestions.length > 0) {
                    let suggestions = result.suggestions;
                    for (let i = 0; i < suggestions.length; i++) {
                        suggestionsDropdown.append(`<div class="suggestion" data-lon="${suggestions[i].data.geo_lon}" data-lat="${suggestions[i].data.geo_lat}"> ${suggestions[i].value}</div>`);
                    }

                    suggestionsDropdown.show();
                } else {
                    suggestionsDropdown.hide();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("error", textStatus, errorThrown);
            }
        });
    });

    // Обработка клика на вариант
    $(document).on('click', '.suggestion', function () {
        let thislatitude = $(this).attr('data-lat');
        let thislongitude = $(this).attr('data-lon');
        let selectedValue = $(this).text();
        $('#PlaceName').val(selectedValue);
        $('#suggestionsDropdown').hide();
        $('#Longitude').val(thislongitude.replace('.', ','));
        $('#Latitude').val(thislatitude.replace('.', ','));

        let coords = [parseFloat(thislatitude.replace(',', '.')), parseFloat(thislongitude.replace(',', '.'))];

        let parts = selectedValue.split(',');
        if (!myPlacemark) {
            myPlacemark = createPlacemark(coords);
            myMap.geoObjects.add(myPlacemark);
        }
        myPlacemark.properties
            .set({
                iconCaption: [
                    parts[0] || '',
                    parts[1] || '', parts[2] || ''
                ].filter(Boolean).join(', '),

                balloonContent: selectedValue
            });
        myPlacemark.geometry.setCoordinates(coords);
        myMap.setZoom(16);
        myMap.panTo(coords, { flying: true });
        
    });


    let longitude = parseFloat($('#Longitude').val().replace(',', '.')), latintude = parseFloat($('#Latitude').val().replace(',', '.'));
    if ((longitude >= 0 || longitude <= 180) && (latintude > -90 || latintude < 90)) {
        let coords = [latintude, longitude];
        myMap.setCenter(coords);
        myPlacemark = createPlacemark(coords);
        myMap.geoObjects.add(myPlacemark);
        myMap.setZoom(16);
        getAddress(coords);
        
    } else {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                console.log(position);
                myMap.setCenter([position.coords.latitude, position.coords.longitude]);
            }, function () {
                alert("Geolocation is not supported by this browser or was denied by the user.");
            });
        } else {
            alert("Geolocation is not supported by this browser.");
        }
    }
}
