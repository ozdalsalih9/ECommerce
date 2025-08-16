<script>
    document.addEventListener('DOMContentLoaded', function () {
        'use strict';

    /* ---------- 1) Kullanıcı oturum durumu (Razor'dan JS bool) ---------- */
    const isAuthenticated = @((User.Identity?.IsAuthenticated ?? false).ToString().ToLower());

    /* ---------- 2) Favori sayısını yükle & rozeti göster ---------- */
    (function loadFavoriteCount() {
    const badge = document.getElementById('favoriteCountBadge');
    if (!isAuthenticated || !badge) return;

    fetch('/Favorite/GetFavoriteCount')
      .then(r => { if (!r.ok) throw new Error('Favori sayısı alınamadı'); return r.json(); })
      .then(data => {
        if (data && typeof data.count === 'number' && data.count > 0) {
        badge.textContent = data.count;
    badge.style.display = 'flex';
        }
      })
    .catch(console.error);
  })();

    /* ---------- 3) Navbar scroll gölgesi / padding efekti ---------- */
    (function navbarScrollEffect() {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;

    const onScroll = () => {
      if (window.scrollY > 50) {
        navbar.style.boxShadow = '0 2px 10px rgba(0,0,0,0.1)';
    navbar.style.padding = '0.25rem 1rem';
      } else {
        navbar.style.boxShadow = '0 4px 12px rgba(0,0,0,0.08)';
    navbar.style.padding = '0.5rem 1rem';
      }
    };
    onScroll();
    window.addEventListener('scroll', onScroll);
  })();

    /* ---------- 4) #contact'a yumuşak kaydırma + sticky offset + mobil menüyü kapat ---------- */
    function scrollToContact() {
    const target = document.getElementById('contact');
    if (!target) return;

    const header = document.querySelector('header.sticky-top');
    const offset = header ? header.offsetHeight : 0;
    const top = target.getBoundingClientRect().top + window.pageYOffset - offset - 8;

    window.scrollTo({top, behavior: 'smooth' });

    // Mobil: nav menüsü açıksa kapat
    const collapseEl = document.getElementById('navbarContent');
    if (collapseEl && collapseEl.classList.contains('show')) {
      try {
        const inst = (window.bootstrap && window.bootstrap.Collapse)
    ? window.bootstrap.Collapse.getOrCreateInstance(collapseEl)
    : null;
    inst ? inst.hide() : collapseEl.classList.remove('show');
      } catch (_) {collapseEl.classList.remove('show'); }
    }
  }

    // Sayfa #contact ile açılırsa offset'li kaydır
    if (location.hash.toLowerCase() === '#contact') {
        setTimeout(scrollToContact, 0);
  }

    // Header içindeki İletişim linklerine tıklandığında (aynı sayfadaki #contact ise) yakala
    document.querySelector('header')?.addEventListener('click', function (e) {
    const a = e.target.closest('a');
    if (!a) return;

    const href = a.getAttribute('href') || '';
    // "#contact" veya "...#contact" sonlanan linkleri hedefle
    const isContactAnchor = /#contact$/i.test(href);
    if (!isContactAnchor) return;

    // Aynı sayfadaysak default anchor davranışını engelle ve JS ile kaydır
    if (document.getElementById('contact')) {
        e.preventDefault();
    scrollToContact();
    }
  });
});
</script>
