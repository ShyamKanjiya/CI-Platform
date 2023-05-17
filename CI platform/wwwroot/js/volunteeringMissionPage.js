function LoginFirst() {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'You are not Logged-in!!! Login First...',
    })
}

function AddToFavourite(missionId) {
    console.log(missionId)
    $.ajax({
        url: "/Pages/AddToFavourite",
        method: "POST",
        data: { 'missionId': missionId },
        success: function (data) {
            location.reload();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//----------------------------------------------------------------------------------------//

function AddComment(missionId) { 
    var comment_area = $("#comment_area").val();
    
    $.ajax({
        url: "/Pages/AddComment",
        method: "POST",
        dataType: "html",
        data: { 'missionId': missionId, "comment_area": comment_area },
        success: function (data) {
            var newm = $($.parseHTML(data)).find("#commentForLoad").html();
            $("#commentForLoad").html(newm);
            $('#comment_area').val('');
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//----------------------------------------------------------------------------------------//

function Alert(text) {
    Swal.fire({
        icon: 'eror',
        text: text,
        timer: 1500
        })
}

function sendMail(missionId) {

    var recUsersList = [];
    $('.modal-body input[type="checkbox"]:checked').each(function () {
        recUsersList.push($(this).attr("id"));
    });

    $('#divLoader').removeClass('d-none');
    $('.modal-content').addClass('d-none');
    if (recUsersList.length != 0) {
        $.ajax({
            type: 'POST',
            url: '/Pages/RecommandToCoworkers',
            data: { "missionId": missionId, "userIds": recUsersList },
            success: function () {
                $("#divLoader").addClass("d-none");
                $('#modal-content').removeClass('d-none');
                Swal.fire({
                    title: 'Success!',
                    html: 'Mission invite sent succesfully.',
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
                });
                $('#exampleModal').modal('hide');
            },
            error: function () {
                $("#divLoader").addClass("d-none");
                console.log('error');
            },
        });
    }
    else {
        $("#divLoader").addClass("d-none");
        $('.modal-content').removeClass('d-none');
        alterForMail(0);
        $('#divLoader').addClass('d-none');
        $('#modal-content').removeClass('d-none');
        Swal.fire({
            title: 'Alert',
            html: 'Select at least one user!',
            timer: 3000,
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
    }
}

//----------------------------------------------------------------------------------------//

function changeRating(starNum, missionId) {
    var star = document.getElementById('span-' + starNum);
    console.log(star);

    var isStarSelected = star.getAttribute('src').endsWith("star-fill.png");
    var rating = 1;
    for (var i = 1; i <= 5; i++) {
        if (i <= starNum) {
            $("#span-" + i).attr("src", "/images/star-fill.png");
            rating = i;
        } else {
            $("#span-" + i).attr("src", "/images/star-empty.png");
        }
    }

    $.ajax({
        url: "/Pages/UpdateAndAddRate",
        type: "POST",
        data: {
            'missionId': missionId, 'rating': rating
        },
        success: function (data) {
            var newm = $($.parseHTML(data)).find("#avg-rating-part").html();
            $("#avg-rating-part").html(newm);
            console.log(rating);
            console.log("rating is updated succesfully");
        }
    });
}

//----------------------------------------------------------------------------------------//

function applyMission(missionId) {
    $.ajax({
        type: 'POST',
        url: '/Pages/applyMission',
        data: { "missionId": missionId},
        success: function (data) {
            var newm = $($.parseHTML(data)).find("#apply").html();
            $("#apply").html(newm);
            /*Swal.fire({
                title: 'Succes',
                text: 'You have succesfully applied for mission',
            })*/
        },
        error: function () {
            console.log('error');
        },
    });
}

//----------------------------------------------------------------------------------------//

function pendingRating(x) {
    if (x == 0)
    {
        Swal.fire({
            icon: 'error',
            title: 'Pending',
            text: 'You are approval request is being pending!!!'
        })
    }
    else
    {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'You have not applied for this mission yet!!!'
        })
    }    
}

//----------------------------------------------------------------------------------------//

function loadVolunteers(pg, mid) {
    $.ajax({
        type: 'GET',
        url: '/Pages/volunteerPage',
        data: { "pg": pg, 'id': mid },
        success: function (data) {
            $('#changeVolunteer').html("");
            $('#changeVolunteer').html(data);
        },
        error: function () {
            console.log('error');
        },
    });
}