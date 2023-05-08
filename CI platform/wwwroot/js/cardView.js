let missionToSearch = "";
let sortBy = "";
let exploreBy = "";


$(document).ready(function () {
    loadMissions();
});

$("#searchtab").on("keyup", function (e) {
    missionToSearch = $("#searchtab").val().toLowerCase();
    loadMissions();
});

function loadMissions(pg, sortVal,exploreVal) {
    var country = [];
    console.log(country);
    $('#dropDownCountry').find("input:checked").each(function (i, ob) {
        country.push($(ob).val());
        console.log(country);
    });

    var cities = [];
    $('#dropDownCity').find("input:checked").each(function (i, ob) {
        cities.push($(ob).val());
    });

    var theme = [];
    $('#dropDownTheme').find("input:checked").each(function (i, ob) {
        theme.push($(ob).val());
    });

    var skill = [];
    $('#dropDownSkill').find("input:checked").each(function (i, ob) {
        skill.push($(ob).val());
    });

    if (sortVal != null) { 
        sortBy = sortVal;
    }

    if (exploreVal != null) {
        exploreBy = exploreVal;
    }

    $("#divLoader").attr('style', 'height:100vh');
    $("#divLoader").removeClass('d-none');

    $.ajax({

        url: "/Pages/bringMissions",
        method: "POST",
        dataType: "html",
        data: { 'sortBy': sortBy, 'exploreBy': exploreBy, 'missionToSearch': missionToSearch, 'pg': pg, 'country': country, 'cities': cities, 'theme': theme, 'skill': skill },
        success: function (data) {
            $("#divLoader").addClass("d-none");
            $('#mission-list').html("");
            $('#mission-list').html(data);
        },
        error: function (error) {
            $("#divLoader").addClass("d-none");
            console.log(error);
        }
    });
}

//--------------------------------------------------------------------------------------------------------//

 