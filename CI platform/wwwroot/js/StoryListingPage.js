$(document).ready(function () {
    loadStory();
});

function loadStory() {
    $.ajax({
        url: "/Pages/bringStories",
        method: "POST",
        dataType: "html",
        data: {  },
        success: function (data) {
            $('#story-list').html("");
            $('#story-list').html(data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}