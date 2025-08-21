// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
    // Cerrar sidebar al hacer clic fuera de él en móviles
    $(document).on('click', function (e) {
        if ($(window).width() <= 768) {
            if (!$(e.target).closest('#sidebar, #sidebarCollapse').length) {
                $('#sidebar').removeClass('active');
            }
        }
    });

    // Manejar redimensionamiento de ventana
    $(window).on('resize', function () {
        if ($(window).width() > 768) {
            $('#sidebar').removeClass('active');
        }
    });

    // Mejorar accesibilidad - cerrar con tecla Escape
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && $('#sidebar').hasClass('active')) {
            $('#sidebar').removeClass('active');
            $('#sidebarCollapse').focus();
        }
    });
});
