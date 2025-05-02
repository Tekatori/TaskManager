function showLoader() {
    $('#loader-overlay').fadeIn(200);
}

function hideLoader() {
    $('#loader-overlay').fadeOut(200);
}

$(document).ajaxStart(function () {
    showLoading();
});

$(document).ajaxStop(function () {
    hideLoading();
});


function showLoading() {
    $("#loading").stop(true, true).fadeIn(150);
}

function hideLoading() {
    $("#loading").stop(true, true).fadeOut(200);
}

$(document).ready(function () {
    $('a').on('click', function (e) {
        const href = $(this).attr('href');

        if (!href || href.startsWith('#') || href.startsWith('javascript:')) return;

        e.preventDefault();
        showLoading();

        setTimeout(() => {
            window.location.href = href;
        }, 200);
    });
});


function ShowPoppupChangePassword() {
    $('#changePasswordModal').modal('show');
}

$('#changePasswordForm').on('submit', function (e) {
    e.preventDefault();
    const currentPassword = $('#currentPassword').val().trim();
    const newPassword = $('#newPassword').val().trim();
    const confirmNewPassword = $('#confirmNewPassword').val().trim();

    if (!currentPassword || !newPassword || !confirmNewPassword) {
        showAlert('Vui lòng điền đầy đủ thông tin.', 'error');
        return;
    }

    if (newPassword !== confirmNewPassword) {
        showAlert('Mật khẩu mới không khớp.', 'error');
        return;
    }

    $.ajax({
        url: '/User/ChangePassword',
        type: 'POST',
        data: {
            currentPassword,
            newPassword
        },
        success: function (res) {
            if (res.success) {
                showAlert('Đổi mật khẩu thành công.', 'success');
                $('#changePasswordModal').modal('hide');
                $('#changePasswordForm')[0].reset();
            } else {
                showAlert(res.error || 'Không thể đổi mật khẩu.', 'error');
            }
        },
        error: function () {
            showAlert('Lỗi máy chủ.', 'error');
        }
    });
});

function showAlert(message, type = 'success', delay = 3000) {
    const swalOptions = {
        icon: type,
        title: type === 'success' ? 'Thành công' : type === 'info' ? 'Thông tin' : type === 'warning' ? 'Cảnh báo' : 'Lỗi',
        text: message,
        timer: delay,
        showConfirmButton: false,
        toast: true,
        position: 'bottom-end' 
    };

    Swal.fire(swalOptions);
}

