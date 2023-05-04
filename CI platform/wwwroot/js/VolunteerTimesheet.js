function deleteAlert(timesheetId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't to delete!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc44f',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/User/DeleteTimeSheetData',
                data: { "timesheetId": timesheetId },
                success: function () {
                    Swal.fire(
                        'Deleted!',
                        'Your data has been deleted.',
                        'success'
                    ).then((result) => {
                        if (result.isConfirmed) {
                            location.reload();
                        }
                    });
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

//---------------------------------------------------------------------------------------------------//

function getTimesheetDataForTimeMiss(timesheetId) {
    if (timesheetId > 0) {
        $.ajax({
            type: 'POST',
            url: '/User/GetTimeSheetData',
            data: { "timesheetId": timesheetId },
            success: function (data) {
                $("#editBtnForTimeMiss").modal("toggle");
                //console.log(data);
                $('#timeSheetIdForEdit').val(data.timesheetId);
                $("#missForTimeMissEdit").val(data.missionId).change();
                var dt = data.dateVolunteered;
                dt = dt.split('T');
                $('#dateForTimeMissEdit').val(dt[0]);
                var time = data.time;
                time = time.split(':');
                $('#hrsForTimeMissEdit').val(time[0]);
                $('#minForTimeMissEdit').val(time[1]);
                $('#msgForTimeMissEdit').val(data.notes);
                getDate('#missForTimeMissEdit');
            },
            error: function () {
                console.log('error');
            },
        });
    }
    else {
        Swal.fire(
            'Something Went Wrong',
            'error'
        )
    }
}

//---------------------------------------------------------------------------------------------------//

function getTimesheetDataForGoalMiss(timesheetId) {
    if (timesheetId > 0) {
        $.ajax({
            type: 'POST',
            url: '/User/GetTimeSheetData',
            data: { "timesheetId": timesheetId },
            success: function (data) {
                $("#editBtnForGoalMiss").modal("toggle");
                console.log(data.timesheetId);
                $('#tSIdForEditGoalMiss').val(data.timesheetId);
                $("#missForGoalMissEdit").val(data.missionId).change();
                $("#actForGoalMissEdit").val(data.action);
                var dt = data.dateVolunteered;
                dt = dt.split('T');
                $('#dateForGoalMissEdit').val(dt[0]);
                $('#msgForGoalMissEdit').val(data.notes);
                getDate('#missForGoalMissEdit');
            },
            error: function () {
                console.log('error');
            },
        });
    }
    else {
        Swal.fire(
            'Something Went Wrong',
            'error'
        )
    }
}

//---------------------------------------------------------------------------------------------------//

function getDate(x) {
    var missionId = $(x).val();
    $.ajax({
        type: 'GET',
        url: '/User/getDate',
        data: { "missionId": missionId },
        success: function (data) {
            var startDate = data[0].startDate.split('T')[0];
            var endDate = data[0].endDate.split('T')[0];
            var currentDate = new Date();
            currentDate = currentDate.toISOString().split('T')[0];

            $('.getDate').attr("min", startDate);
            if (currentDate > endDate) {
                $('.getDate').attr("max", endDate);
            }
            else {
                $('.getDate').attr("max", currentDate);
            }
        },
        error: function () {
            console.log('error');
        }
    });
}

//---------------------------------------------------------------------------------------------------//

function timeValidation() {
    var hours = $("#hrsForTimeMiss").val();
    var minutes = $("#minForTimeMiss").val();

    if (hours == "" && minutes == "" || hours == 0 && minutes == "" | hours == "" && minutes == 0){
        document.getElementById("validation").setAttribute("hidden", true);
        document.getElementById("timsheetSubmit").removeAttribute("disabled");
    }
    else if (hours == 0 && minutes == 0) {
        document.getElementById("validation").removeAttribute("hidden");
        document.getElementById("timsheetSubmit").setAttribute("disabled",true);
    }
    else {
        document.getElementById("validation").setAttribute("hidden", true);
        document.getElementById("timsheetSubmit").removeAttribute("disabled");
    }
}


function timeValidationForEdit() {
    var hours = $("#hrsForTimeMissEdit").val();
    var minutes = $("#minForTimeMissEdit").val();

    if (hours == "" && minutes == "" || hours == 0 && minutes == "" | hours == "" && minutes == 0) {
        document.getElementById("validationEdit").setAttribute("hidden", true);
        document.getElementById("timsheetEditSubmit").removeAttribute("disabled");
    }
    else if (hours == 0 && minutes == 0) {
        document.getElementById("validationEdit").removeAttribute("hidden");
        document.getElementById("timsheetEditSubmit").setAttribute("disabled", true);
    }
    else {
        document.getElementById("validationEdit").setAttribute("hidden", true);
        document.getElementById("timsheetEditSubmit").removeAttribute("disabled");
    }
}

//---------------------------------------------------------------------------------------------------//
