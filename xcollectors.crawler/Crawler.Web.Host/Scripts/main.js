document.addEventListener("DOMContentLoaded", function () {
    var textarea = document.querySelector("#MainContent_txtPatterns");
    textarea.style.display = "none";

    var editor = ace.edit("txtPatterns");

    editor.setOptions({
        selectionStyle: 'line',
        highlightActiveLine: true,
        highlightSelectedWord: true
    });
    editor.renderer.setOptions({
        showInvisibles: false,
        displayIndentGuides: true,
        fontSize: 13
    });

    editor.setTheme("ace/theme/sqlserver");
    editor.session.setMode("ace/mode/json");
    editor.session.setUseWrapMode(true);

    editor.session.setValue(JSON.stringify(JSON.parse(textarea.value), null, '\t'));
    editor.session.on('change', function () {
        textarea.value = editor.getSession().getValue();
    });

    var resultView = ace.edit("parseResult");
    resultView.setTheme("ace/theme/sqlserver");
    resultView.session.setMode("ace/mode/json");
    resultView.renderer.setOptions({
        fontSize: 13
    });
    var height = (resultView.session.getScreenLength() * resultView.renderer.lineHeight) + resultView.renderer.scrollBar.getWidth() + 4;
    document.querySelector("#parseResult").style.height = height.toString() + 'px';
    resultView.resize();
    resultView.setReadOnly(true);
});
