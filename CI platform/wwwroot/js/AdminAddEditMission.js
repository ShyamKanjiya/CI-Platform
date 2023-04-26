$(document).ready(function () {
    tinymce.init({
        selector: 'textarea#missionDescription',
        plugins: 'preview importcss autosave save directionality fullscreen pagebreak nonbreaking anchor lists wordcount help emoticons',
        menubar: false,
        statusbar: false,
        toolbar: 'undo redo | bold italic strikethrough | alignleft aligncenter alignright alignjustify | superscript subscript removeformat',
        autosave_ask_before_unload: true,
        autosave_interval: "30s",
        autosave_prefix: "{path}{query}-{id}-",
        autosave_restore_when_empty: false,
        autosave_retention: "2m",
        content_css: '//www.tiny.cloud/css/codepen.min.css',
        importcss_append: true,
        height: 300,
        toolbar_mode: 'sliding',
        setup: function (ed) {
            //On change call
            ed.on('change', function (e) {
                //Validate tinyMCE on text change
                tinyMCE.triggerSave();

                $("#" + ed.id).valid();


            }
            );
        },
        setup: function (ed) {
            ed.on('keyUp', function (e) {
                var max = 40000;
                var count = CountCharacters();
                if (count >= max) {
                    if (e.keyCode != 8 && e.keyCode != 46)
                        tinymce.dom.Event.cancel(e);
                    document.getElementById("character_count").innerHTML = "Maximun allowed character is: 40000";

                } else {
                    document.getElementById("character_count").innerHTML = count;
                }
            });
        },
    });

    
});
function CountCharacters() {
    var body = tinymce.get("missionDescription").getBody();
    var content = tinymce.trim(body.innerText || body.textContent);
    return content.length;
};

//initializing the arrays
var missSkillList = [];
var missSkillId = [];
var missActives = '';

//preloaded skills
var preloadedMissSkills = [];
$('#mission-skills li').each(function () {
    preloadedMissSkills.push($(this).val());
});
for (var i = 0; i < preloadedMissSkills.length; i++) {
    $('#available li').each(function () {
        if (($(this).val()) == preloadedMissSkills[i]) {
            $(this).addClass('active-skill');
            return false;
        }
    });
}
missActives = $('.list-left ul li.active-skill');
missActives.clone().appendTo('.list-right ul');

missActives.remove();
if ($('.list-right ul li').hasClass('active-skill')) {
    $('.list-right ul li').removeClass('active-skill');
}

getMissSkillListAndIds();
fetchingMissSkillList();


//add skills

$(function () {
    $('body').on('click', '.list-group .list-group-item', function () {
        $(this).toggleClass('active-skill');
    });
    $('.list-arrows a').click(function () {
        var $a = $(this);
        if ($a.hasClass('move-left')) {
            missActives = $('.list-right ul li.active-skill');
            missActives.clone().appendTo('.list-left ul');
            missActives.remove();
            if ($('.list-left ul li').hasClass('active-skill')) {
                $('.list-left ul li').removeClass('active-skill');
            }
        } else if ($a.hasClass('move-right')) {
            missActives = $('.list-left ul li.active-skill');
            missActives.clone().appendTo('.list-right ul');

            missActives.remove();
            if ($('.list-right ul li').hasClass('active-skill')) {
                $('.list-right ul li').removeClass('active-skill');
            }
        }
        getMissSkillListAndIds();
    });

    $('#save-skills').on('click', function () {
        $('#addMissSkills').modal('toggle');
        $('#selMissSkills').html('');
        fetchingMissSkillList();
    });

    $('[name="SearchDualList"]').keyup(function (e) {
        var code = e.keyCode || e.which;
        if (code == '9') return;
        if (code == '27') $(this).val(null);
        var $rows = $(this).closest('.dual-list').find('.list-group li');
        var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase();
        $rows.show().filter(function () {
            var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
            return !~text.indexOf(val);
        }).hide();
    });
    $(window).on('resize', function () {
        var win = $(this);
        if (win.width() < 991) {
            $('.list-arrows .move-right i').removeClass('bi-caret-right-fill').addClass('bi-caret-down-fill');
            $('.list-arrows .move-left i').removeClass('bi-caret-left-fill').addClass('bi-caret-up-fill');
        } else {
            $('.list-arrows .move-right i').addClass('bi-caret-right-fill').removeClass('bi-caret-down-fill');
            $('.list-arrows .move-left i').addClass('bi-caret-left-fill').removeClass('bi-caret-up-fill');
        }
    });
});

function fetchingMissSkillList() {
    if (missSkillList.length > 0) {
        for (var i = 0; i < missSkillList.length; i++) {
            $('#selMissSkills').append('<small class="mb-2">' + missSkillList[i] + '</small>');
        }
    }
    else {
        $('#selMissSkills').append('<small class="mb-2 text-danger">No Skills Selected</small>');
    }
    if (missSkillId.length > 0) {
        for (var i = 0; i < missSkillId.length; i++) {
            let inputElement = $('<input>', {
                type: 'hidden',
                value: missSkillId[i],
                name: 'finalMissSkillList'
            });
            $('#selMissSkills').append(inputElement);
        }
    }
}

function getMissSkillListAndIds() {
    missSkillList = [];
    $('.list-right ul li').map(function () {
        missSkillList.push($(this).text());
    });
    missSkillId = [];
    $('.list-right ul li').map(function () {
        missSkillId.push($(this).val());
    });
}

/*! Image Uploader - v1.2.3 - 26/11/2019
Copyright (c) 2019 Christian Bayer; Licensed MIT */
!(function (e) {
    e.fn.imageUploader = function (t) {
        let n,
            i = {
                preloadedimage: [],
                imagesInputName: "MissionImageFiles",
                preloadedInputName: "preloadedmissimage",
                label: "Drag & Drop files here or click to browse",
                extensions: [".jpg", ".jpeg", ".png"],
                mimes: ["image/jpeg", "image/png", "image/jpeg"],
                maxSize: undefined,
                maxFiles: void 0,
            },
            a = this,
            s = new DataTransfer();
        (a.settings = {}),
            (a.init = function () {
                (a.settings = e.extend(a.settings, i, t)),
                    a.each(function (t, n) {
                        let i = o();
                        if (
                            (e(n).append(i),
                                i.on("dragover", r.bind(i)),
                                i.on("dragleave", r.bind(i)),
                                i.on("drop", p.bind(i)),
                                a.settings.preloadedimage.length)
                        ) {
                            i.addClass("has-files");
                            let e = i.find(".uploaded");
                            for (let t = 0; t < a.settings.preloadedimage.length; t++)
                                e.append(
                                    l(a.settings.preloadedimage[t].src, a.settings.preloadedimage[t].id, !0)
                                );
                        }
                    });
            });
        let o = function () {
            let t = e("<div>", { class: "image-uploader" });
            n = e("<input>", {
                type: "file",
                id: a.settings.imagesInputName + "-" + h(),
                name: a.settings.imagesInputName,
                accept: a.settings.extensions.join(","),
                multiple: "",
            }).appendTo(t);

            let i = e("<div>", { class: "upload-text" }).appendTo(t);
            e("<i>", { class: "bi bi-plus-lg" }).appendTo(i);
            e("<span>", { text: a.settings.label }).appendTo(i);
            e("<div>", { class: "uploaded" }).appendTo(t);
            return (
                i.on("click", function (e) {
                    d(e), n.trigger("click");
                }),
                n.on("click", function (e) {
                    e.stopPropagation();
                }),
                n.on("change", p.bind(t)),
                t
            );
        },
            d = function (e) {
                e.preventDefault(), e.stopPropagation();
            },
            l = function (t, i, o) {
                let l = e("<div>", { class: "uploaded-image" }),
                    r =
                        (e("<img>", { src: t }).appendTo(l),
                            e("<button>", { class: "delete-image" }).appendTo(l));
                e("<i>", { class: "bi bi-x" }).appendTo(r);
                if (o) {
                    l.attr("data-preloaded", !0);
                    e("<input>", {
                        type: "hidden",
                        name: a.settings.preloadedInputName + "[]",
                        value: t,
                    }).appendTo(l);
                } else l.attr("data-index", i);
                return (
                    l.on("click", function (e) {
                        d(e);
                    }),
                    r.on("click", function (t) {
                        d(t);
                        let o = l.parent();
                        if (!0 === l.data("preloadedimage"))
                            a.settings.preloadedimage = a.settings.preloadedimage.filter(function (e) {
                                return e.id !== i;
                            });
                        else {
                            let t = parseInt(l.data("index"));
                            o.find(".uploaded-image[data-index]").each(function (n, i) {
                                n > t && e(i).attr("data-index", n - 1);
                            }),
                                s.items.remove(t),
                                n.prop("files", s.files);
                        }
                        l.remove(),
                            o.children().length || o.parent().removeClass("has-files");
                    }),
                    l
                );
            },
            r = function (t) {
                d(t),
                    "dragover" === t.type
                        ? e(this).addClass("drag-over")
                        : e(this).removeClass("drag-over");
            },
            p = function (t) {
                d(t);
                let i = e(this),
                    o = Array.from(t.target.files || t.originalEvent.dataTransfer.files),
                    l = [];
                e(o).each(function (e, t) {
                    (a.settings.extensions && !g(t)) ||
                        (a.settings.mimes && !c(t)) ||
                        (a.settings.maxSize && !f(t)) ||
                        (a.settings.maxFiles && !m(l.length, t)) ||
                        l.push(t);
                }),
                    l.length
                        ? (i.removeClass("drag-over"), u(i, l))
                        : n.prop("files", s.files);
            },
            g = function (e) {
                return (
                    !(
                        a.settings.extensions.indexOf(
                            e.name.replace(new RegExp("^.*\\."), ".")
                        ) < 0
                    ) ||
                    (alert(
                        `The file "${e.name
                        }" does not match with the accepted file extensions: "${a.settings.extensions.join(
                            '", "'
                        )}"`
                    ),
                        !1)
                );
            },
            c = function (e) {
                return (
                    !(a.settings.mimes.indexOf(e.type) < 0) ||
                    (alert(
                        `The file "${e.name
                        }" does not match with the accepted mime types: "${a.settings.mimes.join(
                            '", "'
                        )}"`
                    ),
                        !1)
                );
            },
            f = function (e) {
                return (
                    !(e.size > a.settings.maxSize) ||
                    (alert(
                        `The file "${e.name}" exceeds the maximum size of ${a.settings.maxSize / 1024 / 1024
                        }Mb`
                    ),
                        !1)
                );
            },
            m = function (e, t) {
                return (
                    !(
                        e + s.items.length + a.settings.preloadedimage.length >=
                        a.settings.maxFiles
                    ) ||
                    (alert(
                        `The file "${t.name}" could not be added because the limit of ${a.settings.maxFiles} files was reached`
                    ),
                        !1)
                );
            },
            u = function (t, n) {
                t.addClass("has-files");
                let i = t.find(".uploaded"),
                    a = t.find('input[type="file"]');
                e(n).each(function (e, t) {
                    s.items.add(t),
                        i.append(l(URL.createObjectURL(t), s.items.length - 1), !1);
                }),
                    a.prop("files", s.files);
            },
            h = function () {
                return Date.now() + Math.floor(100 * Math.random() + 1);
            };
        return this.init(), this;
    };
})(jQuery);