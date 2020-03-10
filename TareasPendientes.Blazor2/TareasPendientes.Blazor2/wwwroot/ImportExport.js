
function FileSaveAs(filename, fileContent,mediaType="text/plain") {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:" + mediaType + ";base64," + encodeURIComponent(fileContent)
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}