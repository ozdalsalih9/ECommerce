// Şifre göster/gizle
document.querySelectorAll('.toggle-password').forEach(button => {
    button.addEventListener('click', function () {
        const passwordInput = this.closest('.input-group').querySelector('input');
        const icon = this.querySelector('i');

        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            icon.classList.replace('fa-eye', 'fa-eye-slash');
        } else {
            passwordInput.type = 'password';
            icon.classList.replace('fa-eye-slash', 'fa-eye');
        }
    });
});

// Şifre güçlülük ve eşleşme kontrolü
const newPasswordInput = document.getElementById('newPassword');
const confirmPasswordInput = document.getElementById('confirmPassword');
const progressBar = document.querySelector('.progress-bar');
const strengthText = document.querySelector('.strength-text');
const matchText = document.createElement('small');
matchText.className = 'form-text text-danger d-block mt-1';
confirmPasswordInput.parentNode.appendChild(matchText);

function validatePasswords() {
    const password = newPasswordInput.value;
    const confirmPassword = confirmPasswordInput.value;

    // Şifre güçlülük kontrolü
    const strength = calculatePasswordStrength(password);
    progressBar.style.width = strength.percentage + '%';
    progressBar.className = 'progress-bar ' + strength.class;
    strengthText.textContent = 'Şifre gücü: ' + strength.text;
    strengthText.className = 'strength-text ' + strength.class;

    // Şifre eşleşme kontrolü
    if (confirmPassword.length > 0) {
        if (password !== confirmPassword) {
            matchText.textContent = 'Şifreler eşleşmiyor!';
            matchText.style.display = 'block';
            return false;
        } else {
            matchText.textContent = 'Şifreler eşleşiyor.';
            matchText.style.color = '#28a745';
            return true;
        }
    } else {
        matchText.textContent = '';
        return false;
    }
}

function calculatePasswordStrength(password) {
    let score = 0;

    // Uzunluk kontrolü
    if (password.length > 0) score += Math.min(password.length * 5, 25);

    // Karakter çeşitliliği
    if (/[A-Z]/.test(password)) score += 10;
    if (/[0-9]/.test(password)) score += 10;
    if (/[^A-Za-z0-9]/.test(password)) score += 15;

    // Sonuç değerlendirme
    if (score > 80) return { percentage: 100, class: 'bg-success', text: 'çok güçlü' };
    if (score > 60) return { percentage: 75, class: 'bg-info', text: 'güçlü' };
    if (score > 40) return { percentage: 50, class: 'bg-warning', text: 'orta' };
    return { percentage: 25, class: 'bg-danger', text: 'zayıf' };
}

// Input event listener'ları
newPasswordInput.addEventListener('input', validatePasswords);
confirmPasswordInput.addEventListener('input', validatePasswords);

// Form gönderim kontrolü
document.querySelector('form').addEventListener('submit', function (e) {
    if (!validatePasswords()) {
        e.preventDefault();
        if (newPasswordInput.value !== confirmPasswordInput.value) {
            matchText.textContent = 'Şifreler eşleşmiyor!';
            matchText.style.color = '#dc3545';
        }
    }
});