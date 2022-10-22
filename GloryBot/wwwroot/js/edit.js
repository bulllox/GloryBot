const { ipcRenderer } = require("electron");

document.getElementById("btnSaveCommand").addEventListener("click", () => {
    var oldName = $('#oldCommandName').val();
    var oldCommandDesc = $("#oldCommandDesc").val();
    var oldCommandText = $("#oldCommandText").val();
    var oldRoles = $("#oldRoles").val();
    var newName = $('#commandName').val();
    var newCommandDec = $("#commandDesc").val();
    var newCommandText = $("#commandText").val();
    var newRoles = $("#roles").val();

    var data = {oldCommandName: oldName, oldCommandDesc, oldCommandText, oldRoles, commandName: newName, commandDesc: newCommandDec, commandText: newCommandText, roles: newRoles};
    ipcRenderer.send("UpdateCommand", data);
});

ipcRenderer.on("closewindow", (event, args) => {
    window.close();
})