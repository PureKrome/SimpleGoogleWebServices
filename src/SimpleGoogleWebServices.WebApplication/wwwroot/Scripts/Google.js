﻿
function getGoogleGeocodeResults(address, postcode, key) {

    // Clean up UI.
    displayResult("");
    displayErrorMessage("");

    // Ajax post it!
    $.getJSON("/google/geocode",
        {
            'address': address,
            'postcode' : postcode,
            'key': key
        })
        .done((data) => { displayResult(data) })
        .fail((qXhr, textStatus, errorThrown) => {
            $("#errorMessage").text(qXhr.responseText);
        });
}

function getGoogleCleanUpResults(address, key) {

    // Clean up UI.
    displayResult("");
    displayErrorMessage("");

    // Ajax post it!
    $.getJSON("/google/cleanup",
            {
                'address': address,
                'key': key
            })
        .done((data) => { displayResult(data) })
        .fail((qXhr, textStatus, errorThrown) => {
            $("#errorMessage").text(qXhr.responseText);
        });
}

function displayResult(jsonString) {
    var result = "";
    if (jsonString !== "") {
        //var zz = JSON.parse(jsonString);
        //var x = JSON.stringify(JSON.parse(jsonString), null, 2);
        result = JSON.stringify(jsonString, null, 2);
    } else {
        result = jsonString;
    }
    $("#result").text(result);
}

function displayErrorMessage(text) {
    $("#result").text(text);
}