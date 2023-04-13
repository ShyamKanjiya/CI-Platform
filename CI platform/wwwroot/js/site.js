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