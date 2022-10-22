// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



if (document.getElementById("btnSaveStream") != undefined) {
    if (document.getElementById("lastCat") != undefined) {
        document.getElementById("lastCat").addEventListener("click", (e) => {
            var target = $(e.target).text();
            $(`#gameCategorys option:contains(${target})`).attr("selected", "selected");
        });
    }
    

// if (document.getElementById("followerTable") != undefined) {

//     $(document).ready(() => {
//         $('#followerTable').DataTable({
//             language: {
//                 url: '/jsLang/de.json'
//             }
//         });
//     });

// }

function createTable(lang, table) {
    $(`#${table}`).DataTable({
        language: {
            url: `/DataTables/${lang}.json`
        }
    });
}
