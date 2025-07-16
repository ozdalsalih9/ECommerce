$(document).ready(function () {
    // Anti-forgery token
    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': $('#RequestVerificationToken').val()
        }
    });

    $(document).on('click', '.remove-favorite', function () {
        const favoriteId = $(this).data('favorite-id');
        const cardElement = $(this).closest('.col-md-4');
        const productName = $(this).closest('.card').find('.card-title').text();

        if (!confirm(`${productName} ürününü favorilerden kaldırmak istiyor musunuz?`)) {
            return;
        }

        $.ajax({
            url: '/Favorite/Remove',
            type: 'POST',
            data: { favoriteId: favoriteId },
            success: function (response) {
                if (response.success) {
                    cardElement.fadeOut(300, function () {
                        $(this).remove();
                        checkEmptyFavorites();
                        updateFavoriteCount();
                    });
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                toastr.error('Hata oluştu: ' + xhr.responseText);
            }
        });
    });

    function checkEmptyFavorites() {
        if ($('#favorites-container .col-md-4').length === 0) {
            $('#favorites-container').html(`
                        <div class="empty-favorites">
                            <i class="fas fa-heart-broken"></i>
                            <div>Favori listeniz şu anda boş.</div>
                        </div>
                    `);
        }
    }

    function updateFavoriteCount() {
        $.get('/Favorite/GetFavoriteCount', function (data) {
            const badge = $('#favoriteCountBadge');
            if (data.count > 0) {
                badge.text(data.count).show();
            } else {
                badge.hide();
            }
        });
    }
});