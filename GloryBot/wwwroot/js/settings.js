if (document.getElementById("languageSelector") != undefined) {

    document.getElementById("languageSelector").addEventListener("change", (e) => {
        if (window.jQuery) {
            
            var selectedLang = $('#languageSelector option:selected').text();
         
            window.location = "/settings/changelanguage?lang=" + selectedLang;

        }
    });
    /*
    
    
        ipcRenderer.on("LanguageChanged", (event, args) => {
            if (args == "true") {
                location.reload();
            }
        })
    */
}