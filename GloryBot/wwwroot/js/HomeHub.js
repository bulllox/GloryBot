"use strict"

$(document).ready(() => {
    var client = new signalR.HubConnectionBuilder().withAutomaticReconnect([0, 1000, 5000, null]).withUrl("/homeHub").build();

    client.on("UpdateStreamInfo", (data) => {
        var info = JSON.parse(data);

        $('#liveStaus').html = (Boolean(info.is_live)) ? '<span style="color:green;>Live</span>"' : '<span style="color: orange;">Offline</span>';
        setAgeLimit(info.audi_type);
        setCategory(info.category_id);

        $('#streamTitle').val(info.live_title);
        $('#follower').text(`Follower: ${info.followers}`);
        $('#subscriber').text(`Subscriber: ${info.subscriber_num}`);
        $('#viewer').text(`Viewer: ${info.current_viewers}`);
        console.log(info);
    });

    client.start().then(() => {

    }).catch((err) => {
        return console.error(err.toString());
    });
});

function setCategory(category) {
    var cat = $('#ageLimit').find(`option[value='${category}']`);
    cat.attr("selected", "selected");
}

function setAgeLimit(limit) {
    switch (limit) {
        case "CHANNEL_AUDIENCE_TYPE_FAMILYFRIENDLY":
            var limit = $('#ageLimit').find("option[value='CHANNEL_AUDIENCE_TYPE_FAMILYFRIENDLY']");
            limit.attr("selected", "selected");
            break;
        case "CHANNEL_AUDIENCE_TYPE_TEEN":
            var limit = $('#ageLimit').find("option[value='CHANNEL_AUDIENCE_TYPE_TEEN']");
            limit.attr("selected", "selected");
            break;
        case "CHANNEL_AUDIENCE_TYPE_EIGHTEENPLUS":
            var limit = $('#ageLimit').find("option[value='CHANNEL_AUDIENCE_TYPE_EIGHTEENPLUS']");
            limit.attr("selected", "selected");
            break;
    }
}