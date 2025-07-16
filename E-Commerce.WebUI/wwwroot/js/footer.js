document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('footerContactForm');
    const msgDiv = document.getElementById('footerContactMsg');

    if (!form) return;

    // Önceki hata mesajlarını temizle
    function clearErrors() {
        form.querySelectorAll('.error-text').forEach(el => el.remove());
        msgDiv.innerHTML = '';
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        clearErrors();

        const formData = new FormData(form);
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token,
                    'Accept': 'application/json'
                },
                body: formData
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Sunucu hatası');
            }

            if (data.success) {
                msgDiv.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
                form.reset();
            } else {
                if (data.errors) {
                    for (const [field, messages] of Object.entries(data.errors)) {
                        const input = form.querySelector(`[name="${field}"]`);
                        if (input) {
                            const errorDiv = document.createElement('div');
                            errorDiv.className = 'error-text text-danger';
                            errorDiv.textContent = messages.join(', ');
                            input.parentNode.appendChild(errorDiv);
                        }
                    }
                }
                msgDiv.innerHTML = `<div class="alert alert-danger">${data.message}</div>`;
            }
        } catch (error) {
            console.error('Hata:', error);
            msgDiv.innerHTML = `<div class="alert alert-danger">Bir hata oluştu: ${error.message}</div>`;
        }
    });
});