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

    //var skill = "";
    //$("input[name='skill']:checked").each(function () {
    //    skill += $(this).val() + ",";
    //});

    if (sortVal != null) { 
        sortBy = sortVal;
    }

    $("#divLoader").attr('style', 'height:100vh');
    $("#divLoader").removeClass('d-none');

    $.ajax({

        url: "/Pages/bringMissions",
        method: "POST",
        dataType: "html",
        data: { 'sortBy': sortBy, 'missionToSearch': missionToSearch, 'pg': pg, 'country': country, 'cities': cities, 'theme': theme },
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

 