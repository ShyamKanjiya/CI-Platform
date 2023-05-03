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
                success: function (status) {
                    if (status == true) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted!',
                            text: 'Your data has been deleted.',
                            showConfirmButton: false,
                            timer: 1000,
                        });
                        location.reload();
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'you can not delete this theme because mission is ongoing related to this theme',
                            showConfirmButton: false,
                            timer: 2500
                        }).then((result) => {
                            if (result.isConfirmed) {
                                location.reload();
                            }
                        });
                    }
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
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Your data has been deleted.',
                        showConfirmButton: false,
                        timer: 1000,
                    });
                    location.reload();
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

//---------------------- Mission Application --------------------------//

function approveAndDeclineMissionApplication(MaId, flag) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't to make changes!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc44f',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, change it!'
    }).then((result) => {
        if (result.isConfirmed) {
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
    })
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
    if (CMSId > 0) {
        $.ajax({
            type: 'POST',
            url: '/Admin/GetCMSData',
            data: { "CMSId": CMSId },
            success: function (data) {
                console.log(data);
                $("#editBtnForCMS").modal("toggle");
                $('#CMSIdForEdit').val(data.cmsPageId);
                $("#CMSTitleForEdit").val(data.title);
                tinyMCE.activeEditor.setContent(data.description);
                $("#CMSSlugEdit").val(data.slug);
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

function deleteAlertForCMS(CMSId) {
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
                url: '/Admin/DeleteCMSData',
                data: { "CMSId": CMSId },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Your data has been deleted.',
                        showConfirmButton: false,
                        timer: 1000,
                    });
                    location.reload();
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

//---------------------- Story --------------------------//

function approveAndDeclineStory(SId, flag) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't to make changes!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc44f',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, change it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/Admin/ApproveAndDeclineStory',
                data: { "storyId": SId, "flag": flag },
                success: function (data) {
                    location.reload();
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

function getStoryDetails(SId) {
    $.ajax({
        type: 'POST',
        url: '/Admin/GetStoryDetails',
        data: { "storyId": SId, },
        success: function (data) {
            console.log(data);
            $("#viewStory").modal("toggle");
            $("#userAvatar").attr("src", data.userOfStory.avatar);
            $("#userFName").html(data.userOfStory.firstName);
            $("#userLName").html(data.userOfStory.lastName);
            $("#userVolunteer").html(data.userOfStory.whyIVolunteer);
        },
        error: function () {
            console.log('error');
        },
    });
}

//---------------------- Mission --------------------------//

function deleteAlertForMission(MId) {
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
                url: '/Admin/DeleteMissionData',
                data: { "missionId": MId },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Your data has been deleted.',
                        showConfirmButton: false,
                        timer: 1000,
                    });
                    location.reload();
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

function setEndDate() {
    var start = $("#startDate").val();
    $("#endDate").attr("min", start);
}

function getDeadline() {
    var start = $("#startDate").val();
    var end = $("#endDate").val();

    $("#missionDeadline").attr("min", start);
    $("#missionDeadline").attr("max", end);
}

//---------------------- User --------------------------//

function deleteAlertForUser(UId) {
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
                url: '/Admin/DeleteUserData',
                data: { "userId": UId },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Your data has been deleted.',
                        showConfirmButton: false,
                        timer: 1000,
                    });
                    location.reload();
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

function loadImg(image) {
    if (image.files && image.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#showImg').attr("src", e.target.result);
        };
        reader.readAsDataURL(image.files[0]);
        $('#showImg').show();
    }
}

//---------------------- Banner --------------------------//

function deleteAlertForBanner(BannerId) {
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
                url: '/Admin/DeleteBannerData',
                data: { "bannerId": BannerId },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Your data has been deleted.',
                        showConfirmButton: false,
                        timer: 1000,
                    });
                    location.reload();
                },
                error: function () {
                    console.log('error');
                },
            });
        }
    })
}

function getBannerData(bannerId) {
    if (bannerId > 0) {
        $.ajax({
            type: 'POST',
            url: '/Admin/GetBannerData',
            data: { "bannerId": bannerId },
            success: function (data) {
                console.log(data);
                $("#editBtnForBanner").modal("toggle");
                $('#bannerIdForEdit').val(data.bannerId);
                $("#bannerTextForEdit").val(data.text);
                $("#bannerNumberForEdit").val(data.sortOrder);
                $("#bannerImg").attr("src", data.image);
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

function loadImgForBanner(image) {
    if (image.files && image.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#bannerImg').attr("src", e.target.result);
        };
        reader.readAsDataURL(image.files[0]);
        $('#bannerImg').show();
    }
}

function CheckAvailableNumber() {
    var bannerNumber = $("#bannerNumber").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/CheckNumber',
        data: { "bannerNumber": bannerNumber },
        success: function (data) {
            console.log(data);
            if (data == false) {
                Swal.fire({
                    title: "Enter valid sort number",
                    timer: 1000,
                    timerProgressBar: true,
                    didOpen: () => {
                        Swal.showLoading()
                    },
                    willClose: () => {
                        clearInterval(timerInterval)
                    }
                }).then((result) => {
                    if (result.dismiss === Swal.DismissReason.timer) {
                        console.log("I was closed");
                    }
                })
                $("#cancelBanner").click();
            }
            else {
                $("#formForAddBanner").submit();
            }
        },
        error: function () {
            console.log('error');
        },
    });
}