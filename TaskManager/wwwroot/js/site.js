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


function showAlert(message, type = 'success', delay = 5000) {
    const icons = {
        success: 'bi bi-check-circle',
        info: 'bi bi-info-circle',
        warning: 'bi bi-exclamation-triangle',
        error: 'bi bi-x-circle'
    };

    const colors = {
        success: 'green',
        info: 'blue',
        warning: 'orange',
        error: 'red'
    };

    const iconClass = icons[type] || icons['success'];
    const colorClass = colors[type] || colors['success'];

    const alert = `
        <div class="alert alert-${type} alert-dismissible fade show d-flex align-items-center" role="alert" style="border-left: 5px solid ${colorClass};">
            <i class="${iconClass} fs-4 me-2"></i>
            <div class="alert-message">
                <strong>${type === 'success' ? 'Thành công!' : type === 'info' ? 'Thông tin' : type === 'warning' ? 'Cảnh báo' : 'Lỗi'}</strong> 
                <div>${message}</div>
            </div>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;

    $('#alert-container').append(alert);

    setTimeout(() => {
        $('#alert-container .alert').fadeOut('slow', function () {
            $(this).remove();
        });
    }, delay);
}
