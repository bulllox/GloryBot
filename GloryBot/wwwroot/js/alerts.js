
function SetData(alert, text, soundfile, imagefile, duration, volume, animation) {
    var splittedImage;
    var splittedSound;
    if (imagefile.includes("\\")) {
        splittedImage = imagefile.split("\\");
    } else {
        splittedImage = imagefile.split("/");
    };
    if (soundfile.includes("\\")) {
        splittedSound = soundfile.split("\\");
    } else {
        splittedSound = soundfile.split("/");
    };
    LoadImageUrlToInput(alert, imagefile, splittedImage[splittedImage.length - 1]);
    var img = document.createElement("img");
    img.src = imagefile;
    img.width = 200;
    img.height = 200;
    document.getElementById(`alert${alert}ImagePreview`).append(img);

    LoadSoundUrlToInput(alert, soundfile, splittedSound[splittedSound.length - 1]);
    console.log(volume);
    document.getElementById(`alert${alert}AudioPreview`).src = soundfile;
    document.getElementById(`alert${alert}AudioPreview`).style.display = "block";
    document.getElementById(`alert${alert}TextPreview`).innerHTML = text;
    document.getElementById(`alert${alert}TextPreview`).style.fontSize = "30px";
    $(`#${alert}VolumeSlider`).slider("value", parseFloat(volume));
    $(`#${alert}VolumeHandler`).text(volume);
    $(`#${alert}VolumeValue`).val(volume);
    var audio = document.getElementById(`alert${alert}AudioPreview`);
    audio.volume = parseFloat(volume);
    $(`#${alert}DurationValue`).val(duration);
    $(`#${alert}DurationSlider`).slider("value", parseInt(duration));
    $(`#${alert}DurationHandler`).text(duration);
    if (animation != "") {
        $(`#${alert}Animation`).find(`option[value='${animation}']`).attr("selected", "selected");
    }


}

function LoadImageUrlToInput(alert, url, filename) {
    getImgURL(url, (imgBlob) => {
        // Load img blob to input
        // WIP: UTF8 character error
        let fileName = filename;
        let file = new File([imgBlob], fileName, { type: imgBlob["type"], lastModified: new Date().getTime() }, 'utf-8');
        let container = new DataTransfer();
        container.items.add(file);
        document.querySelector(`#${alert}ImageFile`).files = container.files;

    });
}

function LoadSoundUrlToInput(alert, url, filename) {
    getImgURL(url, (imgBlob) => {
        // Load img blob to input
        // WIP: UTF8 character error
        let fileName = filename;
        let file = new File([imgBlob], fileName, { type: imgBlob["type"], lastModified: new Date().getTime() }, 'utf-8');
        let container = new DataTransfer();
        container.items.add(file);
        document.querySelector(`#${alert}Sound`).files = container.files;

    });
}

function getImgURL(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function () {
        callback(xhr.response);
    };
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
}
$(document).ready(() => {
    $("#followVolumeSlider").slider({
        min: 0,
        max: 1,
        step: 0.01,
        value: 0.2,
        create: function () {
            $('#followVolumeValue').val($(this).slider("value"));
            setHandleText("followVolumeHandler", $(this).slider("value"))
        },
        slide: function (event, ui) {
            $('#followVolumeValue').val(ui.value);
            changeHandleText(event, ui, "followVolumeHandler");
            var audio = document.getElementById("alertfollowAudioPreview");
            audio.volume = parseFloat(ui.value);
        }
    });
    $("#followDurationSlider").slider({
        min: 8,
        max: 60,
        create: function () {
            $('#followDurationValue').val($(this).slider("value"));
            setHandleText("followDurationHandler", $(this).slider("value"));
        },
        slide: function (event, ui) {
            $('#followDurationValue').val(ui.value);
            changeHandleText(event, ui, "followDurationHandler");
        }
    });

    $("#subVolumeSlider").slider({
        min: 0,
        max: 1,
        step: 0.01,
        value: 0.2,
        create: function () {
            $('#subVolumeValue').val($(this).slider("value"));
            setHandleText("subVolumeHandler", $(this).slider("value"))
        },
        slide: function (event, ui) {
            $('#subVolumeValue').val(ui.value);
            changeHandleText(event, ui, "subVolumeHandler");
            var audio = document.getElementById("alertsubAudioPreview");
            audio.volume = parseFloat(ui.value);
        }
    });
    $("#subDurationSlider").slider({
        min: 8,
        max: 60,
        create: function () {
            $('#subDurationValue').val($(this).slider("value"));
            setHandleText("subDurationHandler", $(this).slider("value"));
        },
        slide: function (event, ui) {
            $('#subDurationValue').val(ui.value);
            changeHandleText(event, ui, "subDurationHandler");
        }
    });

    $("#spellVolumeSlider").slider({
        min: 0,
        max: 1,
        step: 0.01,
        value: 0.2,
        create: function () {
            $('#spellVolumeValue').val($(this).slider("value"));
            setHandleText("spellVolumeHandler", $(this).slider("value"))
        },
        slide: function (event, ui) {
            $('#spellVolumeValue').val(ui.value);
            changeHandleText(event, ui, "spellVolumeHandler");
            var audio = document.getElementById("alertspellAudioPreview");
            audio.volume = parseFloat(ui.value);
        }
    });
    $("#spellDurationSlider").slider({
        min: 8,
        max: 60,
        create: function () {
            $('#spellDurationValue').val($(this).slider("value"));
            setHandleText("spellDurationHandler", $(this).slider("value"));
        },
        slide: function (event, ui) {
            $('#spellDurationValue').val(ui.value);
            changeHandleText(event, ui, "spellDurationHandler");
        }
    });

    $("#giftVolumeSlider").slider({
        min: 0,
        max: 1,
        step: 0.01,
        value: 0.2,
        create: function () {
            $('#giftVolumeValue').val($(this).slider("value"));
            setHandleText("giftVolumeHandler", $(this).slider("value"))
        },
        slide: function (event, ui) {
            $('#giftVolumeValue').val(ui.value);
            changeHandleText(event, ui, "giftVolumeHandler");
            var audio = document.getElementById("alertgiftAudioPreview");
            audio.volume = parseFloat(ui.value);
        }
    });
    $("#giftDurationSlider").slider({
        min: 8,
        max: 60,
        create: function () {
            $('#giftDurationValue').val($(this).slider("value"));
            setHandleText("giftDurationHandler", $(this).slider("value"));
        },
        slide: function (event, ui) {
            $('#giftDurationValue').val(ui.value);
            changeHandleText(event, ui, "giftDurationHandler");
        }
    });


    





    $('#followColorValue').on("click", (e) => {
        $('#followColor').show();
    });

    $("#followColorOkbtn").on("click", (e) => {
        e.preventDefault();
        $('#followColor').hide();
    });

    $('#subColorValue').on("click", (e) => {
        $('#subColor').show();
    });

    $("#subColorOkbtn").on("click", (e) => {
        e.preventDefault();
        $('#subColor').hide();
    });

    $('#spellColorValue').on("click", (e) => {
        $('#spellColor').show();
    });

    $("#spellColorOkbtn").on("click", (e) => {
        e.preventDefault();
        $('#spellColor').hide();
    });

    $('#giftColorValue').on("click", (e) => {
        $('#giftColor').show();
    });

    $("#giftColorOkbtn").on("click", (e) => {
        e.preventDefault();
        $('#giftColor').hide();
    });
 
});
    function setHandleText(what, value) {
        $(`#${what}`).text(value);


    }
    function changeHandleText(event, ui, what) {
        var handle = $(`#${what}`);
        handle.text(ui.value);
    }
    var activTab = "alertFollow";
    function ChangeTab(tab) {
        if ($(`#${tab}`).css("display") == "none") {
            $(`#${tab}`).css("display", "block");
            $(`#${activTab}`).hide();
            activTab = tab;
        }
    }
    function setTextPreview(alert, textbox) {
        var text = $(`#${textbox}`).val();
        $(`#alert${alert}TextPreview`).css("font-size", "30px");
        $(`#alert${alert}TextPreview`).text(text);
}



    
    
