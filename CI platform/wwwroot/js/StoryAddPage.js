tinymce.init({
    selector: 'textarea#myStory',
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

//----------------------------------------------------------------------------------------------------------------------//

function GetDraftedStory() {
    var missionId = $("select").val();

    $.ajax({
        url: "/Story/GetMissionDetails",
        method: "POST",
        dataType: "json",
        data: { 'missionId': missionId },
        success: function (data) {
            if ($("#saveStoryBtn").hasClass('disabled')) {
                $("#saveStoryBtn").removeClass('disabled');
            }

            if (data == 0) {
                $("#story_title").val(null);
                $("#story_date").val(null);
                tinyMCE.activeEditor.setContent("");
                $("#video_url").val(null);
                $(".input-images").html("");
                $('.input-images').imageUploader({});
            }
            else {

                $("#story_title").val(data[0].title);
                var dt = data[0].publishedAt;
                console.log(data[0].publishedAt);
                dt = dt.split('T')[0];
                $("#story_date").val(dt);
                var txt = data[0].description;



                tinyMCE.activeEditor.setContent(txt);


                $(".input-images").html("");
                var i = 1;
                let preloaded = [];
                if (data[0].path != null) {
                    for (let x in data) {
                        if (data[x].type != 'video') {
                            let imgPath = data[x].path;
                            var content = {
                                id: i, src: "/StoryImages/" + imgPath
                            };
                            i++;
                            preloaded.push(content);
                        }
                        else {
                            $("#video_url").val(data[x].path);
                        }
                    }
                }

                $('.input-images').imageUploader({
                    preloaded: preloaded,
                    maxSize: 0.5 * 1024 * 1024,
                });

                if($("#submitStoryBtn").hasClass('disabled')) {
                    $("#submitStoryBtn").removeClass('disabled');
                }
            }
        },
        error: function (error) {
            console.log(error);
        }       
    });
}

//----------------------------------------------------------------------------------------------------------------------//

function submitStory() {
    var missionId = $("select").val();
    $.ajax({
        url: "/Story/SubmitStory",
        method: 'POST',
        data: { 'missionId': missionId },
        success: function (data) {
            location.reload();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//----------------------------------------------------------------------------------------------------------------------//

function validateYouTubeUrl() {
    var url = document.getElementById("video_url").value;
    if (url != null) {
        var regExp = /^(?:https?:\/\/)?(?:m\.|www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=|watch\?.+&v=))((\w|-){11})(?:\S+)?$/;
        if (url.match(regExp)) {
            alert("YoutubeURL");
        }
        else {
            alert("WrongURL")
        }
    }
    return false;
}