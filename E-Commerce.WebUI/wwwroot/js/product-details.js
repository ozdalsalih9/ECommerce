$(document).ready(function () {

    $('#favoriteToggle').click(function () {
        const button = $(this);
        const productId = button.data('product-id');
        const isCurrentlyFavorite = button.hasClass('btn-danger');

        const url = isCurrentlyFavorite ? '/Favorite/Remove' : '/Favorite/Add';

        const payload = isCurrentlyFavorite
            ? { productId: productId }   // FAVORİDEN ÇIKARMA - productId gönder
            : { productId: productId };  // FAVORİYE EKLEME - productId gönder

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (response) {
                if (response.success) {
                    button.toggleClass('btn-danger btn-outline-danger');
                    const iconClass = isCurrentlyFavorite ? 'fa-heart-o' : 'fa-heart';
                    const text = isCurrentlyFavorite ? 'Favorilere Ekle' : 'Favorilerden Çıkar';
                    button.html(`<i class="fas ${iconClass}"></i> ${text}`);
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('İşlem sırasında bir hata oluştu.');
            }
        });
    });


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

    if (typeof isAuthenticated !== 'undefined' && isAuthenticated) {
        fetchFavoriteCount();
    }

    // ========== BEDEN SEÇİMİ ==========
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

    // ========== SLICK SLIDER ==========
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

    // ========== OTOMATİK RESİM DÖNÜŞÜ ==========
    let currentIndex = 0;
    const thumbnails = $('.thumbnail');

    function rotateMainImage() {
        currentIndex = (currentIndex + 1) % thumbnails.length;
        const thumbnail = thumbnails[currentIndex];
        const imageUrl = $(thumbnail).attr('src');
        window.changeMainImage(thumbnail, imageUrl);
    }

    if (thumbnails.length > 1) {
        setInterval(rotateMainImage, 5000);
    }
});

// ========== GLOBAL FONKSİYONLAR ==========

window.changeMainImage = function (thumbnailElement, imageUrl) {
    document.getElementById('mainImage').src = imageUrl;
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });
    thumbnailElement.classList.add('active');
};

window.selectColor = function (element) {
    if (typeof productColors === 'undefined') {
        console.error('productColors JSON tanımlı değil.');
        return;
    }

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
                window.openModal(imgUrl);
            });
            colorImagesContainer.appendChild(img);
        });
    } else {
        colorImagesContainer.innerHTML = '<p class="text-muted">Bu renge ait resim bulunmamaktadır.</p>';
    }
};

window.openModal = function (imgUrl) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('modalImg');
    modal.style.display = 'block';
    modalImg.src = imgUrl;
};

window.addEventListener('DOMContentLoaded', () => {
    const modalClose = document.getElementById('modalClose');
    if (modalClose) {
        modalClose.onclick = function () {
            document.getElementById('imageModal').style.display = 'none';
        };
    }
});


// Renk resimlerini yükleme fonksiyonu
function loadColorImages(productId, colorId) {
    const colorImagesContainer = document.getElementById('colorImagesContainer');
    colorImagesContainer.innerHTML = '<div class="text-muted">Ürüne ait renkleri görmek için tıkla!</div>';

    // Tüm renk seçeneklerinden selected class'ını kaldır
    document.querySelectorAll('.color-option').forEach(option => {
        option.classList.remove('selected');
    });

    // Tıklanan renge selected class'ını ekle
    event.currentTarget.classList.add('selected');

    // API çağrısı yerine önceden yüklenmiş verileri kullanıyoruz
    const colorData = productColors.find(c => c.ColorId == colorId);

    if (colorData && colorData.Images && colorData.Images.length > 0) {
        let imagesHTML = '';

        colorData.Images.forEach((imageUrl, index) => {
            imagesHTML += `
                    <img src="${imageUrl}" 
                         alt="Renk ${colorId} - ${index + 1}" 
                         class="color-image"
                         onclick="changeMainImage(this, '${imageUrl}'); showModal('${imageUrl}')">
                `;
        });

        colorImagesContainer.innerHTML = imagesHTML;
    } else {
        colorImagesContainer.innerHTML = '<div class="text-muted">Bu renge ait resim bulunamadı</div>';
    }
}

// Ana resmi değiştirme fonksiyonu
function changeMainImage(element, newSrc) {
    const mainImage = document.getElementById('mainImage');

    // Fade out efekti
    mainImage.style.opacity = 0;

    setTimeout(() => {
        // Yeni resmi yükle
        const img = new Image();
        img.src = newSrc;

        img.onload = function () {
            mainImage.src = newSrc;
            mainImage.style.opacity = 1;

            // Tüm thumbnail'lerden active class'ını kaldır
            document.querySelectorAll('.thumb-item, .color-image').forEach(thumb => {
                thumb.classList.remove('active');
            });

            // Tıklanan thumbnail'e active class'ını ekle
            if (element) {
                element.classList.add('active');
            }
        };

        img.onerror = function () {
            mainImage.src = '/img/no-image.jpg';
            mainImage.style.opacity = 1;
        };
    }, 300);
}

// Beden seçme fonksiyonu
function selectSize(element) {
    if (element.classList.contains('out-of-stock')) return;

    // Tüm beden seçeneklerinden selected class'ını kaldır
    document.querySelectorAll('.size-option').forEach(option => {
        option.classList.remove('selected');
    });

    // Tıklanan bedene selected class'ını ekle
    element.classList.add('selected');

    // Stok bilgisini güncelle
    const stock = parseInt(element.getAttribute('data-stock'));
    const stockMessage = document.getElementById('stockMessage');

    if (stock === 0) {
        stockMessage.textContent = "Bu beden stokta bulunmamaktadır.";
        stockMessage.className = "stock-message out-of-stock";
    } else if (stock < 10) {
        stockMessage.textContent = `Son ${stock} ürün kaldı!`;
        stockMessage.className = "stock-message low-stock";
    } else {
        stockMessage.textContent = "Stokta mevcut";
        stockMessage.className = "stock-message in-stock";
    }
}

// Modal için fonksiyonlar
function showModal(imageUrl) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('modalImg');

    modal.style.display = "block";
    modalImg.src = imageUrl;
}

function closeModal() {
    document.getElementById('imageModal').style.display = "none";
}

// Modal kapatma işlevi
document.getElementById('modalClose').addEventListener('click', closeModal);

// Modal dışına tıklandığında kapat
window.addEventListener('click', function (event) {
    const modal = document.getElementById('imageModal');
    if (event.target == modal) {
        closeModal();
    }
});

// Sayfa yüklendiğinde ilk rengin resimlerini yükle
document.addEventListener('DOMContentLoaded', function () {
    const firstColor = document.querySelector('.color-option');
    if (firstColor) {
        const productId = '@Model.Id';
        const colorId = firstColor.getAttribute('data-color-id');
        loadColorImages(productId, colorId);
    }
});

favoriteBtn.addEventListener('click', function () {
    const button = this;
    const productId = button.getAttribute('data-product-id');
    const favoriteId = button.getAttribute('data-favorite-id');
    const isFavorite = favoriteId !== '0' && favoriteId !== null && favoriteId !== undefined;

    const url = isFavorite ? '/Favorite/Remove' : '/Favorite/Add';
    const payload = isFavorite
        ? { favoriteId: parseInt(favoriteId) }  // Kaldırırken favoriteId gönder
        : { productId: parseInt(productId) };    // Eklerken productId gönder

    fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    })
        .then(response => response.json())
        .then(data => {
            if (!data.success) return;

            const icon = button.querySelector('i');

            if (isFavorite) {
                // Favoriden çıkarıldı
                button.classList.remove('btn-danger');
                icon.className = 'fas fa-heart-o';
                button.innerHTML = '<i class="fas fa-heart-o"></i> Favorilere Ekle';
                button.setAttribute('data-favorite-id', '0');
            } else {
                // Favoriye eklendi
                button.classList.add('btn-danger');
                icon.className = 'fas fa-heart';
                button.innerHTML = '<i class="fas fa-heart"></i> Favorilerden Çıkar';
                // Backend'den gelen gerçek favoriteId'yi burada güncelle
                if (data.favoriteId) {
                    button.setAttribute('data-favorite-id', data.favoriteId.toString());
                } else {
                    button.setAttribute('data-favorite-id', '1'); // fallback
                }
            }
        })
        .catch(err => console.error('Favori hatası:', err));
});


