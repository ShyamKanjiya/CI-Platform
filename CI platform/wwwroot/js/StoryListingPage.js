$(document).ready(function () {
    loadStory();
});

function loadStory(pg) {
    $.ajax({
        url: "/Story/bringStories",
        method: "POST",
        dataType: "html",
        data: { 'pg': pg },
        success: function (data) {
            $('#story-list').html("");
            $('#story-list').html(data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}