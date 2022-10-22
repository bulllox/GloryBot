const { ipcRenderer } = require("electron");

document.getElementById("CloseButton").addEventListener("click", () => {
    ipcRenderer.send("closeWindow");
});
document.getElementById("MinButton").addEventListener("click", () => {
    ipcRenderer.send("minimizeWindow");
});


$(document).ready(() => {
    if($('input[type="checkbox"]').is(":checked")) {
        $('.switch').css("background-color", "green");
        $('input[type="checkbox"]').attr("checked", "checked");
    } else {
        $('.switch').css("background-color", "gray");
        $('input[type="checkbox"]').removeAttr("checked");
    }
    $('.switch').on("click", (e) => {
        ToggleSwitch(e);
    });
});

function ToggleSwitch(e) {
    var target = e.target;
    var plugin = $(target).data("plugin");
    var command = $(target).data("command");
    $('.switch .round').toggleClass("on");
        if (!$('input[type="checkbox"]').is(":checked")) {
            var data = {"state": "on", "command": command, "plugin": plugin};
            $('.switch').css("background-color", "green");
            $('input[type="checkbox"]').attr("checked", "checked");
            ipcRenderer.send("plugin:state", data);
        } else {
            $('.switch').css("background-color", "gray");
            var data = {"state": "off", "command": command, "plugin": plugin};
            ipcRenderer.send("plugin:state", data);
            $('input[type="checkbox"]').removeAttr("checked");

        }
}