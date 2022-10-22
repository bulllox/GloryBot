"use strict";
var running = false;
var alertsWaiting = [];
var alertPlaying = undefined;
$(document).ready(() => {

    var client = new signalR.HubConnectionBuilder().withAutomaticReconnect([0, 1000, 5000, null]).withUrl("/alertHub").build();

    client.on("ShowAlert", (data) => {
        var info = JSON.parse(data);
        doAlert(info.text, info.sourceImage, info.sourceSound, parseInt(info.duration), parseFloat(info.Volume), info.Animation, info.TextColor);
    });



    client.start().then(() => {

    }).catch((err) => {
        return console.error(err.toString());
    });
});

function doAlert(text, sourceImage, sourceSound, duration, volume, animation, textColor) {

    // showAlert(info.text, info.sourceImage, info.sourceSound, parseInt(info.duration), parseFloat(info.Volume), parseInt(info.Fontsize));
    $('#alertText').css("color", textColor);
    $('#alertText').text(text);
    $('#alertImage').html(`<img src="${sourceImage}" width="400" height="400" alt="no Pic">`);
    //$('#alertSound').attr("src", audioFile);
    switch (animation) {

        case "Slide In":
            $('#alertBox').attr("class", "slideIn");
            break;
        case "Fade In":
            $('#alertBox').attr("class", "fadeIn");
            break;
        case "Role In":
            $('#alertBox').attr("class", "roleIn");
            break;
        case "Pulse":
            $('#alertBox').attr("class", "pulse");
            break;
    }
    var audio = document.getElementById("alertSound");
    $('#alertSound').attr("src", sourceSound);
    audio.volume = volume;
    setTimeout(() => {
        $('#alertBox').show();
    }, 500); 
    
    audio.play();

    setTimeout(() => {
        $('#alertBox').fadeOut(200);
        var audio = document.getElementById("alertSound");
        audio.pause();
        audio.currentTime = 0;
    }, duration * 1000);




}

// document.getElementById("playSound").addEventListener("click", () => {
//     var audio = document.getElementById("alertSound");
//     audio.pause();
//     audio.currentTime = 0;
// })