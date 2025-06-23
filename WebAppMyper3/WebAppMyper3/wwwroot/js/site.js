// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Cargar provincias cuando cambia el departamento
    $("#IdDepartamento").change(function () {
        var departamentoId = $(this).val();
        $("#IdProvincia").empty().append('<option value="">-- Cargando... --</option>');
        $("#IdDistrito").empty().append('<option value="">-- Seleccione Provincia --</option>');

        if (departamentoId) {
            $.getJSON('/Trabajadores/GetProvinciasByDepartamento', { departamentoId: departamentoId }, function (data) {
                $("#IdProvincia").empty().append('<option value="">-- Seleccione --</option>');
                $.each(data, function (index, item) {
                    $("#IdProvincia").append($('<option></option>').val(item.id).text(item.nombreProvincia));
                });
            });
        }
    });

    // Cargar distritos cuando cambia la provincia
    $("#IdProvincia").change(function () {
        var provinciaId = $(this).val();
        $("#IdDistrito").empty().append('<option value="">-- Cargando... --</option>');

        if (provinciaId) {
            $.getJSON('/Trabajadores/GetDistritosByProvincia', { provinciaId: provinciaId }, function (data) {
                $("#IdDistrito").empty().append('<option value="">-- Seleccione --</option>');
                $.each(data, function (index, item) {
                    $("#IdDistrito").append($('<option></option>').val(item.id).text(item.nombreDistrito));
                });
            });
        }
    });

    // Cierra automáticamente las alertas después de 3 segundos
    setTimeout(function () {
        $('#successAlert').alert('close');
    }, 3000);

    // Busqueda directa en el combo con Select2
    $('.select2').select2();
    $(document).on('select2:open', () => {
        setTimeout(() => {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });
});
