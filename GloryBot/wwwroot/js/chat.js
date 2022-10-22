"use strict"
$('#defaultCommands').css("display", "none");
var msgCounter = 0;
var hit = 0;


const commandList = [
    "/mods",
    "/banned",
    "/ban [@username]",
    "/ban [@username] [duration]",
    "/unban [@username]",
    "/unmod [@username]",
    "/clear",
    "/slow [seconds]",
    "/slowoff",
    "/haton",
    "/hatoff",
    "/followers [duration]",
    "/followers",
    "/followersoff",
    "/subscribers",
    "/subscribersoff",
    "/raid [@username]",
    "/unraid",
    "/host [@username]",
    "/unhost",
    "/settitle [title]",
    "/setcategory [category name]",
    "/bulletscreenon",
    "/bulletscreenoff",
    "/scheduleon",
    "/scheduleoff",
    "/setlanguage [language name]",
    "/addrole [rolename] [@username]",
    "/addvibetag [vibetag title]",
    "/removevibetag",
    "/fastclip"

];
$(document).ready(() => {
        hit = 0;
    
    var client = new signalR.HubConnectionBuilder().withAutomaticReconnect([0, 1000, 5000, null]).withUrl("/chatHub").build();

    

    client.on("ReceiveMessage", (avatar, color, nickname, message) => {
        
            var list = document.createElement("li");


            list.id = `list_${msgCounter}`;
            $('.ChatBox').append(list);
            $(`#list_${msgCounter}`).css("border-bottom", "1px solid silver");
            $(`#list_${msgCounter}`).css("padding", "5px");
            $(`#list_${msgCounter}`).css("box-sizing", "border-box");
            $(`#list_${msgCounter}`).css("-moz-box-sizing", "border-box");
            $(`#list_${msgCounter}`).css("-webkit-box-sizing", "border-box");

            var avatarImage = document.createElement("img");
            avatarImage.id = "img_" + msgCounter;
            avatarImage.src = avatar;
            avatarImage.alt = "no pic";
            avatarImage.width = 26;
            avatarImage.height = 26;
            $(`#list_${msgCounter}`).append(avatarImage);
            $(`#img_` + msgCounter).css("margin-right", "4px");

            var userName = document.createElement("span");
            userName.id = "user_" + msgCounter;
            userName.innerText = `${nickname}: `;
            $(`#list_${msgCounter}`).append(userName);
            $(`#user_` + msgCounter).css("font-size", "16px");
            $(`#user_` + msgCounter).css("color", color);

            var messageSpan = document.createElement("span");
            messageSpan.id = "msg_" + msgCounter;
            messageSpan.innerHTML = message;
            $('#list_' + msgCounter).append(messageSpan);

            $(".ChatBox").scrollTop($(".ChatBox")[0].scrollHeight);
            msgCounter = msgCounter + 1;
        
    });


    client.on("ClearChat", () => {
        $('.ChatBox').empty();
        var list = document.createElement("li");
        list.id = `list_${msgCounter}`;
        $('.ChatBox').append(list);
        $(`#list_${msgCounter}`).css("border-bottom", "1px solid silver");
        $(`#list_${msgCounter}`).css("padding", "5px");
        $(`#list_${msgCounter}`).css("box-sizing", "border-box");
        $(`#list_${msgCounter}`).css("-moz-box-sizing", "border-box");
        $(`#list_${msgCounter}`).css("-webkit-box-sizing", "border-box");
        list.textContent = "[System]: Cleared Chat";
        msgCounter = msgCounter + 1;
    });

    client.start().then(() => {
        client.invoke("GetOldMessage");
    }).catch((err) => { return console.err(err.toString()); });

    let commandCounter = 0;
    document.getElementById("chatInput").addEventListener("input", (e) => {
        var target = e.target;
        $('#defaultCommands').empty();
        if ($(`#${target.id}`).val().includes("/") && $(`#${target.id}`).val().length > 0 && $(`#${target.id}`).val().length < 2) {
            var ul = document.createElement("ul");

            commandList.forEach((element) => {
                var li = document.createElement("li");
                li.id = `command_${commandCounter}`;
                li.onclick = (e) => {
                    var msg = e.target.innerText;
                    console.log(msg);
                    $('#chatInput').val(msg);
                    $('#defaultCommands').empty();
                    $('#defaultCommands').hide();
                };
                li.textContent = element;
                ul.appendChild(li);
                commandCounter = commandCounter + 1;
            });

            $('#defaultCommands').append(ul);
            $('#defaultCommands').show();
        } else {
            $("#defaultCommands").hide();
        }
        

    });

    
 
    document.getElementById("chatInput").addEventListener("keydown", (e) => {
        if (e.key == "Enter") {
            var user = $("#sendAsUser option:selected").val();
            var msg = $('#chatInput').val();
            if ($("#defaultCommands").css("display") == "block") {
                $("#defaultCommands").hide();    
            }
            client.invoke("Send", user, msg);

            $('#chatInput').val("");
        }
    });
    document.getElementById("btnSendChat").addEventListener("click", (e) => {
        var user = $("#sendAsUser option:selected").val();
        var msg = $('#chatInput').val();

        client.invoke("Send", user, msg);

        $('#chatInput').val(""); 
    })
});
