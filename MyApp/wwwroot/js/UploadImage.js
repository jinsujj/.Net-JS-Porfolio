function sendFile(img, i, file) {
    var formData = new FormData();
    debugger;
    formData.append("img", img);
    alert(img);
    $.ajax({
        data: formData,
        type: "POST",
        url: "/files/",
        cache: false,
        contentType: false,
        processData: false,
        success: function (url) {
            $('#Content').eq(i).summernote('insertImage', url);
        }
    });
}