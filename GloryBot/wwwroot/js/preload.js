const { ipcRenderer } = require("electron");
if (document.getElementById("CloseButton") != undefined && document.getElementById("MinButton") != undefined) {
    document.getElementById("CloseButton").addEventListener("click", () => {
        ipcRenderer.send("closeWindow");
    });
    document.getElementById("MinButton").addEventListener("click", () => {
        ipcRenderer.send("minimizeWindow");
    });
}
