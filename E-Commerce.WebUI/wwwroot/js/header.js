    document.addEventListener('DOMContentLoaded', function () {
        // Kullanıcının oturum açıp açmadığını Razor üzerinden kontrol et
        const isAuthenticated = @((User.Identity?.IsAuthenticated ?? false).ToString().ToLower());

    // Favori sayısını yükle
    if (isAuthenticated && document.getElementById('favoriteCountBadge')) {
        fetch('/Favorite/GetFavoriteCount')
            .then(response => {
                if (!response.ok) throw new Error("Favori sayısı alınamadı");
                return response.json();
            })
            .then(data => {
                const badge = document.getElementById('favoriteCountBadge');
                if (data.count > 0) {
                    badge.textContent = data.count;
                    badge.style.display = 'flex';
                }
            })
            .catch(error => console.error(error));
        }

    // Navbar scroll efekti
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) {
                navbar.style.boxShadow = '0 2px 10px rgba(0,0,0,0.1)';
                navbar.style.padding = '0.25rem 1rem';
            } else {
                navbar.style.boxShadow = '0 4px 12px rgba(0,0,0,0.08)';
                navbar.style.padding = '0.5rem 1rem';
            }
        });
        }
    });
