// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ContectUsGetData() {
    $.ajax({
        url: "/User/ContectUs",
        method: "GET",
        success: function (data) {
            document.getElementById("contectName").value = data.userDetails.firstName + " " + data.userDetails.lastName;
            document.getElementById("contectEmail").value = data.userDetails.email;
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function ContectUsPostData() {
    var subject = $("#contectSubject").val();
    var message = $("#contectMessage").val();
    if (subject != '' && message != '') {
        $.ajax({
            url: "/User/ContectUs",
            method: "POST",
            data: { 'subject': subject, 'message': message },
            success: function (data) {
                $('#contectUs').modal('hide');
                Swal.fire({
                    title: 'Your message has been sent.',
                    timer: 2000,
                    didOpen: () => {
                        Swal.showLoading()
                        const b = Swal.getHtmlContainer().querySelector('b')
                        timerInterval = setInterval(() => {
                            b.textContent = Swal.getTimerLeft()
                        }, 100)
                    },
                    willClose: () => {
                        clearInterval(timerInterval)
                    }
                })
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    else {
        Swal.fire('Please first fill the form');
    }
}

function isRead(NotallId) {
    if (NotallId > 0) {
        $.ajax({
            type: 'POST',
            url: '/Common/MarkNotAsRead',
            data: { "notId": NotallId },
            success: function () {
                $('#notifType').text('');
                $('.notifList').text('');
                loadNotification();
            },
            error: function () {
                console.log('error');
            },
        });
    }
}

$(document).ready(function () {
    loadNotification();
});

function loadNotification() {
    $.ajax({
        type: 'GET',
        url: '/Common/GetNotificationData',
        success: function (data) {
            console.log(data);
            $('#notifCount').text(data.notificationCount);
            $.each(data.notificationTypeList, function (index, notifType) {
                $('#notifType').append(`<li class="form-check mb-2 p-0 d-flex align-items-center justify-content-between">
                                   <label class="form-check-label ms-3" for="">`+ notifType.notifType + `</label>
                                <input class="form-check-input" type="checkbox" value="`+ notifType.nottypeId + `">
                            </li>
                        `);
            });

            $.each(data.notificationToUserList, function (index, notifdata) {
                $('.notifList').append(`<tr class="border-1 d-flex align-items-center" style="height: 60px;">
                                <td style="width: 10%;"><i class="bi bi-chat-text fs-5"></i></td>
                                <td style="width: 80%;"><a href="`+ notifdata.url + `" class="text-decoration-none text-black d-flex"><small>` + notifdata.notification + `</small></a></td>
                                <td class="text-end" id="markAsReadIcon`+ notifdata.notspecId + `" style="width: 10%;"></td>
                            </tr>
                            `);
                if (notifdata.isread == 0) {
                    $("#markAsReadIcon" + notifdata.notspecId).append(`<button class="border-0 bg-transparent" onclick="isRead(` + notifdata.notspecId + `)">
                                <i class="bi bi-check-circle-fill fs-5" style="color: #F88634;"></i>
                                </button>`);
                }
                else {
                    $('#markAsReadIcon' + notifdata.notspecId).append(`<i class="bi bi-check-circle-fill fs-5" style="color: #b0b0b0;"></i>`);
                }
            });

            $("#notifType li input[type='checkbox']").each(function () {
                // If the checkbox's value is in the list of IDs, check it
                if ($.inArray(parseInt($(this).val()), data.userNotifPrefList) !== -1) {
                    $(this).prop("checked", true);
                }
            });
        },
        error: function () {
            console.log('error');
        },
    });
}

function savePref() {
    var userNotifPref = [];
    $('#notifType li').find("input:checked").each(function (i, obj) {
        userNotifPref.push(parseInt($(obj).val()));
    });
    console.log(userNotifPref);
    if (userNotifPref.length > 0) {
        $.ajax({
            type: 'POST',
            url: '/Common/SaveUserNotificationPreferences',
            data: { "notifPref": userNotifPref },
            success: function () {
                Alert('preference saved successfully');
                $('#notifType').text('');
                $('.notifList').text('');
                loadNotification();
            },
            error: function () {
                console.log('error');
            },
        });
    }
}
