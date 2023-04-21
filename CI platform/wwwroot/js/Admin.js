//---------------------- Mission Theme --------------------------//

function getMissionThemeData(MtId) {
    if (MtId > 0) {
        $.ajax({
            type: 'POST',
            url: '/Admin/GetMissionThemeData',
            data: { "missionThemeId": MtId },
            success: function (data) {
                $("#editBtnForMissionTheme").modal("toggle");
                $('#missionThemeIdForEdit').val(data.missionThemeId);
                $("#missionThemeTitleEdit").val(data.title);
                if (data.status == 1) {
                    statusInput = document.getElementById("Active");
                }
                else {
                    statusInput = document.getElementById("Inactive");
                }
                statusInput.checked = true;
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

function deleteAlertForMissionTheme(MtId) {
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
                url: '/Admin/DeleteMissionThemeData',
                data: { "missionThemeId": MtId },
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

//---------------------- Skills --------------------------//

function getSkillData(skillId) {
    if (skillId > 0) {
        $.ajax({
            type: 'POST',
            url: '/Admin/GetSkillData',
            data: { "skillId": skillId },
            success: function (data) {
                console.log(data);
                $("#editBtnForSkill").modal("toggle");
                $('#skillIdForEdit').val(data.skillId);
                $("#skillNameEdit").val(data.skillName);
                if (data.status == 1) {
                    statusInput = document.getElementById("Active");
                }
                else {
                    statusInput = document.getElementById("Inactive");
                }
                statusInput.checked = true;
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

function deleteAlertForSkill(skillId) {
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
                url: '/Admin/DeleteSkillData',
                data: { "skillId": skillId },
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

//---------------------- Mission Application --------------------------//

function approveAndDeclineMissionApplication(MaId,flag) {
    $.ajax({
        type: 'POST',
        url: '/Admin/ApproveAndDeclineMissionApplication',
        data: { "missionApplicationId": MaId, "flag": flag },
        success: function (data) {
            location.reload();
        },
        error: function () {
            console.log('error');
        },
    });
}

//---------------------- CMS --------------------------//

tinymce.init({
    selector: 'textarea#CMSDescription',
    plugins: 'preview importcss autosave save directionality fullscreen pagebreak nonbreaking anchor lists wordcount help emoticons',
    menubar: false,
    statusbar: false,
    toolbar: 'undo redo | bold italic strikethrough | alignleft aligncenter alignright alignjustify | superscript subscript removeformat',
    autosave_ask_before_unload: true,
    autosave_interval: "30s",
    autosave_prefix: "{path}{query}-{id}-",
    autosave_restore_when_empty: false,
    autosave_retention: "2m",
    content_css: '//https://www.tiny.cloud/css/codepen.min.css',
    importcss_append: true,
    height: 300,
});

function getCMSData(CMSId) {
    if (skillId > 0) {
        $.ajax({
            type: 'POST',
            url: '/Admin/GetSkillData',
            data: { "CMSIdId": CMSId },
            success: function (data) {
                console.log(data);
                $("#editBtnForSkill").modal("toggle");
                $('#skillIdForEdit').val(data.skillId);
                $("#skillNameEdit").val(data.skillName);
                if (data.status == 1) {
                    statusInput = document.getElementById("Active");
                }
                else {
                    statusInput = document.getElementById("Inactive");
                }
                statusInput.checked = true;
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