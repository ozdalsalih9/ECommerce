$(document).ready(function () {
    // Sepete Ekle Butonu
    $('.add-to-cart').click(function (e) {
        e.preventDefault();
        const productId = $(this).data('product-id');

        // Sepet animasyonu
        $(this).html('<i class="fas fa-check me-2"></i> Eklendi');
        $(this).addClass('bg-success').removeClass('bg-primary');

        setTimeout(() => {
            $(this).html('<i class="fas fa-shopping-cart me-2"></i> Sepete Ekle');
            $(this).removeClass('bg-success').addClass('bg-primary');
        }, 2000);

        // Sepet API'si entegre edilecekse buraya yaz
        console.log('Ürün ID:', productId);

        // Başarı bildirimi
        toastr.success('Ürün sepetinize eklendi!', 'Başarılı', {
            timeOut: 2000,
            progressBar: true,
            position: 'bottom-right'
        });
    });

    // Favori Butonu
    $('.favorite-btn').click(function () {
        const icon = $(this).find('i');

        if (icon.hasClass('far')) {
            icon.removeClass('far').addClass('fas');
            $(this).addClass('bg-purple').removeClass('bg-white');
            toastr.info('Ürün favorilere eklendi!');
        } else {
            icon.removeClass('fas').addClass('far');
            $(this).removeClass('bg-purple').addClass('bg-white');
            toastr.info('Ürün favorilerden çıkarıldı!');
        }
    });
});