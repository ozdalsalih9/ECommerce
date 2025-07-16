$(document).ready(function () {
    // Favori butonu click event
    $('#favoriteToggle').click(function () {
        const productId = $(this).data('product-id');
        const favoriteId = $(this).data('favorite-id');
        const isCurrentlyFavorite = $(this).hasClass('btn-danger');
        const url = isCurrentlyFavorite ?
            '/Favorite/Remove' : '/Favorite/Add';
        const data = isCurrentlyFavorite ?
            { productId: productId } : { productId: productId };
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            success: function (response) {
                if (response.success) {
                    // Buton görünümünü güncelle
                    const button = $('#favoriteToggle');
                    button.toggleClass('btn-danger btn-outline-danger');
                    // Favori ID'sini güncelle (ekleme durumunda)
                    if (response.favoriteId) {
                        button.data('favorite-id', response.favoriteId);
                    }
                    // İkon ve metni güncelle
                    const iconClass = isCurrentlyFavorite ? 'fa-heart-o' : 'fa-heart';
                    const text = isCurrentlyFavorite ? 'Favorilere Ekle' : 'Favorilerden Çıkar';
                    button.html(`<i class="fas ${iconClass}"></i> ${text}`);
                    // Bildirim göster
                    toastr.success(response.message);
                    // Favori sayısını güncelle
                    fetchFavoriteCount();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('İşlem sırasında bir hata oluştu. Lütfen tekrar deneyin.');
            }
        });
    });
    // Favori sayısını güncelle
    function fetchFavoriteCount() {
        fetch('/Favorite/GetFavoriteCount')
            .then(response => response.json())
            .then(data => {
                const badge = document.getElementById('favoriteCountBadge');
                if (data.count > 0) {
                    badge.textContent = data.count;
                    badge.style.display = 'flex';
                } else {
                    badge.style.display = 'none';
                }
            });
    }
    // Sayfa yüklendiğinde favori durumunu kontrol et
    if ('@User.Identity.IsAuthenticated' === 'True') {
        fetchFavoriteCount();
    }
});
function changeMainImage(thumbnailElement, imageUrl) {
    document.getElementById('mainImage').src = imageUrl;
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });
    thumbnailElement.classList.add('active');
}
function selectColor(element) {
    // Renk seçiminde aktif sınıfı değiştir
    document.querySelectorAll('.color-circle').forEach(c => c.style.borderColor = 'transparent');
    element.style.borderColor = '#000'; // aktif rengi siyah yap
    const colorId = element.getAttribute('data-color-id');
    const colorImagesContainer = document.getElementById('colorImagesContainer');
    colorImagesContainer.innerHTML = ''; // önce temizle
    // Model'den ürün renklerine göre resimleri alıyoruz:
    // Sayfada Model.ProductColors olduğu için JS'de göremiyoruz, Razor ile JSON gönderelim:
}
$(document).ready(function () {
    // JSON olarak renk resimlerini alalım
    const productColors = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.ProductColors.Select(pc => new
        {
            ColorId = pc.ColorId,
            Images = pc.ProductColorImages.Select(img => $"/img/Products/ColorImages/P-{Model.Id}/C-{pc.ColorId}/" + img.ImageUrl).ToList()
        })));

    window.selectColor = function (element) {
        document.querySelectorAll('.color-circle').forEach(c => c.style.borderColor = 'transparent');
        element.style.borderColor = '#000';
        const colorId = element.getAttribute('data-color-id');
        const colorImagesContainer = document.getElementById('colorImagesContainer');
        colorImagesContainer.innerHTML = '';
        const selectedColor = productColors.find(c => c.ColorId == colorId);
        if (selectedColor && selectedColor.Images.length > 0) {
            selectedColor.Images.forEach(imgUrl => {
                const img = document.createElement('img');
                img.src = imgUrl;
                img.alt = 'Renk resmi';
                img.style.width = '80px';
                img.style.height = '80px';
                img.style.objectFit = 'cover';
                img.style.marginRight = '10px';
                img.style.cursor = 'pointer';
                img.style.border = '1px solid #ddd';
                img.style.borderRadius = '4px';
                img.addEventListener('click', function () {
                    openModal(imgUrl);
                });
                colorImagesContainer.appendChild(img);
            });
        }
        else {
            colorImagesContainer.innerHTML = '<p class="text-muted">Bu renge ait resim bulunmamaktadır.</p>';
        }
    };
    // Modal açma fonksiyonu
    window.openModal = function (imgUrl) {
        const modal = document.getElementById('imageModal');
        const modalImg = document.getElementById('modalImg');
        modal.style.display = 'block';
        modalImg.src = imgUrl;
    };
    // Modal kapatma
    document.getElementById('modalClose').onclick = function () {
        document.getElementById('imageModal').style.display = 'none';
    };
    // Diğer mevcut kodlar (slick, stock vb.)
    // ... (mevcut jQuery kodların kalabilir)
});
function changeMainImage(thumbnailElement, imageUrl) {
    document.getElementById('mainImage').src = imageUrl;
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });
    thumbnailElement.classList.add('active');
}
$(document).ready(function () {
    $('#sizeSelect').change(function () {
        const selectedOption = $(this).find('option:selected');
        const stock = selectedOption.data('stock');
        const sizeName = selectedOption.text().split('(')[0].trim();
        if (selectedOption.val()) {
            if (stock > 0) {
                const stockText = stock < 10 ? `Son ${stock} adet!` : `${stock} adet stokta`;
                $('#stockMessage').html(`${sizeName} beden: <span class="text-success">${stockText}</span>`);
            } else {
                $('#stockMessage').html(`${sizeName} beden: <span class="text-danger">Stokta yok</span>`);
            }
        } else {
            $('#stockMessage').html('');
        }
    });
    $('#thumbnailSlider').slick({
        dots: false,
        arrows: true,
        infinite: false,
        speed: 300,
        slidesToShow: 5,
        slidesToScroll: 1,
        prevArrow: '<button type="button" class="slick-prev"><i class="fas fa-chevron-left"></i></button>',
        nextArrow: '<button type="button" class="slick-next"><i class="fas fa-chevron-right"></i></button>',
        responsive: [
            { breakpoint: 992, settings: { slidesToShow: 4 } },
            { breakpoint: 768, settings: { slidesToShow: 3 } },
            { breakpoint: 576, settings: { slidesToShow: 2 } }
        ]
    });
    let currentIndex = 0;
    const thumbnails = $('.thumbnail');
    function rotateMainImage() {
        currentIndex = (currentIndex + 1) % thumbnails.length;
        const thumbnail = thumbnails[currentIndex];
        const imageUrl = $(thumbnail).attr('src');
        changeMainImage(thumbnail, imageUrl);
    }
    if (thumbnails.length > 1) {
        setInterval(rotateMainImage, 5000);
    }
});