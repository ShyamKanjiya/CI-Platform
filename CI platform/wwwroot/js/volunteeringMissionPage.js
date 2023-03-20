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
            location.reload()
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
        data: { 'missionId': missionId, "comment_area": comment_area },
        success: function (data) {
            location.reload()
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