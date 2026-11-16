document.addEventListener('DOMContentLoaded', function () {
    const fileInput = document.getElementById('propertyImagesInput');
    const submitBtn = document.getElementById('submitBtn');
    const infoText = document.getElementById('imageCountInfo');

    if (fileInput) {
        fileInput.addEventListener('change', function () {
            if (fileInput.files.length > 4) {
                alert('Solo se permite registrar hasta 4 imágenes por propiedad.');
                fileInput.value = '';
                if (infoText) infoText.textContent = 'Seleccione archivos válidos (.jpg, .jpeg, .png).';
                if (submitBtn) submitBtn.disabled = false;
            } else if (fileInput.files.length > 0) {
                if (infoText) infoText.textContent = `${fileInput.files.length} archivo(s) seleccionado(s).`;
            }
        });
    }
});