document.addEventListener("DOMContentLoaded", function () {
    const phoneInput = document.getElementById("Phone");
    if (!phoneInput) return;

    phoneInput.addEventListener("input", function () {
        let phone = this.value.replace(/\D/g, "").slice(0, 10);

        if (phone.length > 6) {
            phone = phone.substring(0, 3) + "-" +
                phone.substring(3, 6) + "-" +
                phone.substring(6);
        } else if (phone.length > 3) {
            phone = phone.substring(0, 3) + "-" +
                phone.substring(3);
        }

        this.value = phone;
    });
});