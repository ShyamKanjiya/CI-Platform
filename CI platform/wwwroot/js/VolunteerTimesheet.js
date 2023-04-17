function deleteAlert(timeSheetId) {
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
                data: { "tsId": timeSheetId },
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
                var time = data.totalTime;
                time = time.split(':');
                $('#hrsForTimeMissEdit').val(time[0]);
                $('#minForTimeMissEdit').val(time[1]);
                $('#msgForTimeMissEdit').val(data.notes);
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