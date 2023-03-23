let slideIndex = 1;
showSlides(slideIndex);

// Next/previous controls
function plusSlides(n) {
    showSlides(slideIndex += n);
}

// Thumbnail image controls
function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    let dots = document.getElementsByClassName("demo");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";  
}



//---------------------------------------------------------------------------------------//

function LoginFirst() {
    alert('Please Login first')
}

function AddToFavourite(missionId) {
    console.log(missionId)
    $.ajax({

        url: "/Pages/AddToFavourite",
        method: "POST",
        data: { 'missionId': missionId },
        success: function (data) {
            var newm = $($.parseHTML(data)).find("#favouriteBtn").html();
            $("#favouriteBtn").html(newm);
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

            console.log(data);

        },
        error: function (error) {
            console.log(error);
        }
    });
}

//----------------------------------------------------------------------------------------//

function sendMail(missionId) {

    var recUsersList = [];
    $('.modal-body input[type="checkbox"]:checked').each(function () {
        recUsersList.push($(this).attr("id"));
    });
    console.log(recUsersList);
    $.ajax({
        type: 'POST',
        url: '/Pages/RecommandToCoworkers',
        data: { "missionId": missionId, "userIds": recUsersList },
        success: function () {
            alert("Mission Recommended successfully!");
        },
        error: function () {
            console.log('error');
        },
    });
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
        success: function () {
            alert("Mission Applied!");
        },
        error: function () {
            console.log('error');
        },
    });
}

//----------------------------------------------------------------------------------------//

function pendingRating(x) {
    if (x == 0)
        alert("You're approval request is being pending!!!");
    else
        alert("You've not applied for this mission yet!!!");
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