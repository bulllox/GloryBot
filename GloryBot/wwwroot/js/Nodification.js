let timer = 5 // Time in seconds

if(document.getElementById("nody") != undefined) {
    ipcRenderer.on("window:nodifacation", (event, msg) => {
        // msg = {"title": "der title", "msg": "die msg"}
        var jsonMessage = JSON.parse(msg);
        $('.nodyTitle').text(jsonMessage["title"]);
        $('.nodyContent').text(jsonMessage["msg"]);
        $('#nody').show();
        setTimeout(() => {
            $("#nody").fadeOut(300);
        }, timer * 1000);
    });
} 