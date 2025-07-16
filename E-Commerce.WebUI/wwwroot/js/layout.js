// Initialize AOS animation
AOS.init({
    duration: 800,
    easing: 'ease-in-out',
    once: true
});

// Toastr configuration
toastr.options = {
    "closeButton": true,
    "progressBar": true,
    "positionClass": "toast-bottom-right",
    "timeOut": "3000",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// Enhanced Navbar Scroll Behavior
document.addEventListener('DOMContentLoaded', function () {
    const navbar = document.querySelector('.navbar');
    let lastScroll = 0;
    const navbarHeight = navbar.offsetHeight;

    window.addEventListener('scroll', function () {
        const currentScroll = window.pageYOffset;

        if (currentScroll <= 100) {
            navbar.classList.remove('scrolled');
        } else if (currentScroll > lastScroll && currentScroll > navbarHeight) {
            // Scroll down - hide navbar
            navbar.style.transform = `translateY(-${navbarHeight}px)`;
            navbar.style.opacity = '0';
        } else {
            // Scroll up - show navbar
            navbar.style.transform = 'translateY(0)';
            navbar.style.opacity = '1';
            navbar.classList.add('scrolled');
        }
        lastScroll = currentScroll;
    });

    // Smooth Back to Top Button
    const backToTopButton = document.getElementById('backToTop');
    window.addEventListener('scroll', function () {
        if (window.pageYOffset > 300) {
            backToTopButton.style.opacity = '1';
            backToTopButton.style.visibility = 'visible';
        } else {
            backToTopButton.style.opacity = '0';
            backToTopButton.style.visibility = 'hidden';
        }
    });

    backToTopButton.addEventListener('click', function (e) {
        e.preventDefault();
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });

    // Smooth scrolling for all links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                window.scrollTo({
                    top: target.offsetTop - 100,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Add pulse animation to CTA buttons
    document.querySelectorAll('.btn-primary').forEach(btn => {
        btn.classList.add('pulse-animation');
    });
});