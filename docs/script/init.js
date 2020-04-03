function getDownloads() {
    const url = "http://47.95.218.243/statistics";
    const xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.send();
    xhr.onload = () => $('#total-downloads').attr('src', `https://img.shields.io/static/v1?label=releases-downloads&message=${xhr.responseText}&color=green&style=flat-square`);
}

$(function () {
    $('.parallax').parallax();
    getDownloads()
});
