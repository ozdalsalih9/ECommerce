document.addEventListener("DOMContentLoaded", function () {
    const favoriteButtons = document.querySelectorAll(".product-card__favorite");

    favoriteButtons.forEach(button => {
        button.addEventListener("click", function (e) {
            e.preventDefault();

            const productId = parseInt(this.dataset.productId);
            const favoriteId = parseInt(this.dataset.favoriteId) || 0;

            const isFavorite = this.classList.contains("active");

            if (!isFavorite) {
                // ADD
                fetch("/Favorite/Add", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        productId: productId,
                        favoriteId: favoriteId
                    })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            const icon = this.querySelector("i");
                            icon.classList.remove("far");
                            icon.classList.add("fas");
                            this.classList.add("active");
                            this.dataset.favoriteId = data.favoriteId;

                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'success',
                                title: 'Ürün favorilere eklendi!',
                                showConfirmButton: false,
                                timer: 2000,
                                timerProgressBar: true
                            });
                        } else {
                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'info',
                                title: data.message,
                                showConfirmButton: false,
                                timer: 2000,
                                timerProgressBar: true
                            });
                        }
                    })
                    .catch(error => {
                        console.error("Hata:", error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Bir hata oluştu.'
                        });
                    });

            } else {
                // REMOVE
                fetch("/Favorite/Remove", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        productId: productId,
                        favoriteId: favoriteId
                    })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            const icon = this.querySelector("i");
                            icon.classList.remove("fas");
                            icon.classList.add("far");
                            this.classList.remove("active");
                            this.dataset.favoriteId = "";

                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'success',
                                title: 'Ürün favorilerden kaldırıldı!',
                                showConfirmButton: false,
                                timer: 2000,
                                timerProgressBar: true
                            });
                        } else {
                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'info',
                                title: data.message,
                                showConfirmButton: false,
                                timer: 2000,
                                timerProgressBar: true
                            });
                        }
                    })
                    .catch(error => {
                        console.error("Hata:", error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Bir hata oluştu.'
                        });
                    });
            }
        });
    });
});
