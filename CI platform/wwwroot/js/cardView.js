let missionToSearch = "";
let sortBy = "";


$(document).ready(function () {
    loadMissions();
});

$("#searchtab").on("keyup", function (e) {
    missionToSearch = $("#searchtab").val().toLowerCase();
    loadMissions();
});

function loadMissions(pg, sortVal) {
    var country = [];
    $('#dropDownCountry').find("input:checked").each(function (i, ob) {
        country.push($(ob).val());
    });

    var cities = [];
    $('#dropDownCity').find("input:checked").each(function (i, ob) {
        cities.push($(ob).val());
    });

    var theme = [];
    $('#dropDownTheme').find("input:checked").each(function (i, ob) {
        theme.push($(ob).val());
    });

    //var skill = "";
    //$("input[name='skill']:checked").each(function () {
    //    skill += $(this).val() + ",";
    //});

    if (sortVal != null) {
        sortBy = sortVal;
    }

    $.ajax({

        url: "/Pages/bringMissionsToGridView",
        method: "POST",
        dataType: "html",
        data: { 'sortBy': sortBy, 'missionToSearch': missionToSearch, 'pg': pg, 'country': country, 'cities': cities, 'theme': theme },
        success: function (data) {
            $('#mission-list').html("");
            $('#mission-list').html(data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}