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

const dropZone = document.getElementById("drop-zone");
const fileInput = document.getElementById("file-input");
const imagePreview = document.getElementById("image-preview");
const uploadedFiles = new Set();

dropZone.addEventListener("click", () => {
    fileInput.click();
});

dropZone.addEventListener("dragover", (event) => {
    event.preventDefault();
    dropZone.classList.add("dragover");
});

dropZone.addEventListener("dragleave", () => {
    dropZone.classList.remove("dragover");
});

dropZone.addEventListener("drop", (event) => {
    event.preventDefault();
    dropZone.classList.remove("dragover");
    const files = event.dataTransfer.files;
    handleFiles(files);
});

fileInput.addEventListener("change", () => {
    const files = fileInput.files;
    handleFiles(files);
});

function handleFiles(files) {
    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        console.log(file.name);
        if (!file.type.startsWith("image/") && !file.type.startsWith("video/")) continue;
        if (uploadedFiles.has(file.name)) {
            alert(`File "${file.name}" has already been uploaded.`);
            continue;
        }
        uploadedFiles.add(file.name);
        const image = document.createElement("img");
        image.classList.add("image-preview");
        const imageContainer = document.createElement("div");
        imageContainer.classList.add("image-container");
        const removeImage = document.createElement("div");
        removeImage.innerHTML = "&#10006;";
        removeImage.classList.add("remove-image");
        removeImage.addEventListener("click", () => {
            uploadedFiles.delete(file.name);
            imageContainer.remove();
        });
        const reader = new FileReader();
        reader.readAsDataURL(file);
        console.log(reader.result);
        reader.onload = () => {
            image.src = reader.result;
            imageContainer.appendChild(image);
            imageContainer.appendChild(removeImage);
            imagePreview.appendChild(imageContainer);
        };
        console.log(reader.result);
    }
}

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
                $("#image-preview").html("");

            }
            else {

                $("#story_title").val(data[0].title);
                var dt = data[0].publishedAt;
                dt = dt.split('T')[0];
                $("#story_date").val(dt);
                var txt = data[0].description;

                tinyMCE.activeEditor.setContent(txt);
                $("#image-preview").html("");
                //set images
                for (let x in data) {
                    let imgPath = data[x].path.substring(51);

                    var element = `<div class="image-container"><img class="image-preview" src="${imgPath}" alt="image"> <span onclick="deleteImage()">&times;</span></div >`
                    $("#image-preview").append(element);
                }

                if ($("#submitStoryBtn").hasClass('disabled')) {
                    $("#submitStoryBtn").removeClass('disabled');
                }
            }


        },
        error: function (error) {
            console.log(error);
        }
    });
}