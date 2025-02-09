// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// site-scripts.js

$(document).ready(function () {
    // Скрипт для добавления клиента
    $('#addClientForm').on('submit', function (e) {
        e.preventDefault(); // Отключаем стандартное поведение формы

        // Скрыть сообщения успеха и ошибки
        $('#successMessage').addClass('d-none');
        $('#errorMessage').addClass('d-none');

        // Собираем данные из формы
        const formData = new FormData(this);
        const token = $('input[name="__RequestVerificationToken"]').val(); // Anti-CSRF токен

        // Отправляем данные через AJAX
        $.ajax({
            url: 'api/clients/create', // URL метода контроллера
            type: 'POST',
            data: formData,
            processData: false, // Не преобразовывать объект FormData в строку
            contentType: false, // Отключить content-type
            headers: {
                "RequestVerificationToken": token // Передаём CSRF-токен
            },
            success: function () {
                // Успех — показать сообщение и очистить форму
                $('#successMessage').removeClass('d-none');
                $('#addClientForm')[0].reset(); // Сброс формы
            },
            error: function (xhr, status, error) {
                // Ошибка — показать сообщение об ошибке
                const errorMessage = xhr.responseText || "Произошла ошибка. Попробуйте снова.";
                $('#errorMessage').text(errorMessage).removeClass('d-none');
                console.error("Ошибка:", errorMessage);
            }
        });
    });

    // Скрипт для добавления книги
    $('#addBookForm').on('submit', function (e) {
        e.preventDefault(); // Отключение стандартной отправки формы

        // Очистка сообщений об успехе/ошибке
        $('#addBookSuccessMessage').addClass('d-none');
        $('#addBookErrorMessage').addClass('d-none');

        // Формируем данные для отправки
        const formData = new FormData(this);
        const token = $('input[name="__RequestVerificationToken"]').val(); // Анти-CSRF токен

        $.ajax({
            url: 'api/books/create-book', // URL для отправки данных
            type: 'POST',
            data: formData,
            processData: false, // Не преобразовывать `FormData` в строку
            contentType: false, // Убираем `content-type`, чтобы браузер сам установил
            headers: {
                "RequestVerificationToken": token // Отправляем CSRF-токен
            },
            success: function (response) {
                // Успешное добавление книги
                $('#addBookSuccessMessage').removeClass('d-none'); // Показываем сообщение об успехе
                $('#addBookForm')[0].reset(); // Очищаем форму
            },
            error: function (xhr, status, error) {
                // Ошибка при добавлении книги
                console.error("Ошибка:", error);
                $('#addBookErrorMessage').removeClass('d-none'); // Показываем сообщение об ошибке
            }
        });
    });

    // Скрипт для добавления записи
    $('#addBorrowRecordForm').on('submit', function (e) {
        e.preventDefault(); // Отключить стандартную отправку формы

        // Скрыть сообщения об успехе и ошибке
        $('#addBorrowRecordSuccess').addClass('d-none');
        $('#addBorrowRecordError').addClass('d-none');

        // Собираем данные формы
        const formData = new FormData(this);
        const token = $('input[name="__RequestVerificationToken"]').val(); // Анти-CSRF токен

        // Отправляем AJAX-запрос
        $.ajax({
            url: 'api/borrow-records/create', // URL метода контроллера
            type: 'POST',
            data: formData,
            processData: false, // Не обрабатывать данные формы в строку
            contentType: false, // Убираем вручную тип содержимого (браузер добавит сам)
            headers: {
                "RequestVerificationToken": token // Передаём анти-CSRF токен
            },
            success: function () {
                // Успех: Покажем сообщение об успехе
                $('#addBorrowRecordSuccess').removeClass('d-none');
                $('#addBorrowRecordForm')[0].reset(); // Очистка формы
            },
            error: function (xhr, status, error) {
                // Ошибка: Покажем сообщение об ошибке
                console.error("Ошибка при отправке формы:", error);
                $('#addBorrowRecordError').removeClass('d-none'); // Сообщение об ошибке
            }
        });
    });


});