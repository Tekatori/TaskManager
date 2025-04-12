function showLoader() {
    $('#loader-overlay').fadeIn(200);
}

function hideLoader() {
    $('#loader-overlay').fadeOut(200);
}

$(document).ajaxStart(function () {
    showLoader();
});

$(document).ajaxStop(function () {
    hideLoader();
});