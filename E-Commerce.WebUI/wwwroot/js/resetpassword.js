document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.auth-form');
    const pw1 = document.getElementById('newPassword');
    const pw2 = document.getElementById('confirmPassword');

    if (!form || !pw1 || !pw2) return; // güvenlik

    // --- Güç göstergesi öğeleri (opsiyonel yoksa hata vermesin)
    const strengthWrap = document.querySelector('.password-strength');
    const strengthBar = strengthWrap ? strengthWrap.querySelector('.progress-bar') : null;
    const strengthText = strengthWrap ? strengthWrap.querySelector('.strength-text') : null;

    // --- Eşleşme helper'ını input-group'tan HEMEN sonra ekle
    const confirmGroup = pw2.closest('.form-group');
    const inputGroup = confirmGroup ? confirmGroup.querySelector('.input-group') : null;
    let matchHelper = confirmGroup ? confirmGroup.querySelector('.match-helper') : null;

    if (confirmGroup && inputGroup && !matchHelper) {
        matchHelper = document.createElement('small');
        matchHelper.className = 'match-helper';
        inputGroup.insertAdjacentElement('afterend', matchHelper);
    }

    // --- Toggle password (göz)
    document.querySelectorAll('.toggle-password').forEach(btn => {
        btn.addEventListener('click', function () {
            const input = this.previousElementSibling && this.previousElementSibling.tagName === 'INPUT'
                ? this.previousElementSibling
                : this.parentElement.querySelector('input.form-control');

            const icon = this.querySelector('i');
            if (!input || !icon) return;

            const toType = input.type === 'password' ? 'text' : 'password';
            input.type = toType;

            icon.classList.toggle('fa-eye', toType === 'password');
            icon.classList.toggle('fa-eye-slash', toType === 'text');
        });
    });

    // --- Şifre gücü
    function calcStrength(pw) {
        let s = 0;
        if (pw.length >= 8) s++;
        if (/[A-Z]/.test(pw)) s++;
        if (/[a-z]/.test(pw)) s++;
        if (/\d/.test(pw)) s++;
        if (/[^A-Za-z0-9]/.test(pw)) s++;
        return Math.min(s, 4); // 0..4
    }

    function updateStrength() {
        if (!strengthBar || !strengthText) return;

        const s = calcStrength(pw1.value);
        const widths = ['0%', '25%', '50%', '75%', '100%'];
        const labels = ['zayıf', 'zayıf', 'orta', 'iyi', 'çok iyi'];
        const classes = ['bg-danger', 'bg-danger', 'bg-warning', 'bg-info', 'bg-success'];

        strengthBar.classList.remove('bg-danger', 'bg-warning', 'bg-info', 'bg-success');
        strengthBar.style.width = widths[s];
        strengthBar.classList.add(classes[s]);

        strengthText.textContent = 'Şifre gücü: ' + labels[s];
        strengthText.classList.remove('text-danger', 'text-warning', 'text-info', 'text-success');
        strengthText.classList.add(classes[s].replace('bg', 'text'));
    }

    // --- Şifre eşleşmesi
    function updateMatch() {
        if (!matchHelper) return;

        matchHelper.classList.remove('match-success', 'match-error');
        if (!pw2.value) { matchHelper.textContent = ''; return; }

        if (pw1.value === pw2.value) {
            matchHelper.textContent = 'Şifreler eşleşiyor.';
            matchHelper.classList.add('match-success');
        } else {
            matchHelper.textContent = 'Şifreler eşleşmiyor.';
            matchHelper.classList.add('match-error');
        }
    }

    // --- Eventler
    pw1.addEventListener('input', () => { updateStrength(); updateMatch(); });
    pw2.addEventListener('input', updateMatch);

    form.addEventListener('submit', (e) => {
        if (pw1.value !== pw2.value) {
            e.preventDefault();
            updateMatch();
            pw2.focus();
        }
    });

    // ilk durum
    updateStrength();
    updateMatch();
});