// ----------------------------
// Универсальная функция запросов Ajax
// ----------------------------
function sendRequest(method, url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: method,
        contentType: "application/json",
        data: data ? JSON.stringify(data) : null,
        success: successCallback,
        error: function (xhr) {
            if (errorCallback) {
                errorCallback(xhr);
            } else {
                console.error(`Ошибка запроса ${method} ${url}:`, xhr.responseText);
                alert("Произошла ошибка при выполнении запроса.");
            }
        },
    });
}

// ----------------------------
// Универсальная функция для рендеринга таблиц
// ----------------------------
function renderTable(tableSelector, data, createRowCallback) {
    const tableBody = $(tableSelector).find("tbody");
    tableBody.empty();
    if (!Array.isArray(data) || data.length === 0) {
        alert("Данные отсутствуют или список пуст.");
        return;
    }
    data.forEach((item) => tableBody.append(createRowCallback(item)));
}

// ----------------------------
// Универсальная функция для валидации полей
// ----------------------------
function validateField(fieldValue, errorMessage) {
    if (!fieldValue || fieldValue.trim() === "") {
        alert(errorMessage);
        return false;
    }
    return true;
}

function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// ----------------------------
// Логика работы с клиентами
// ----------------------------
function loadClients() {
    sendRequest("GET", "/api/clients/get-all", null, function (data) {
        renderTable("#clients-table", data, createClientRow);
        addClientDeleteHandlers();
    });
}

function createClientRow(client) {
    return `
        <tr>
            <td>${client.id}</td>
            <td>${client.username}</td>
            <td>${client.email || "N/A"}</td>
            <td>${client.role || "Не указана"}</td>
            <td>
                <button class="btn btn-danger btn-sm delete-client" data-id="${client.id}">Удалить</button>
            </td>
        </tr>`;
}

function addClientDeleteHandlers() {
    $(".delete-client").on("click", function () {
        const clientId = $(this).data("id");
        sendRequest("DELETE", `/api/clients/delete/${clientId}`, null, function () {
            alert("Клиент удален!");
            loadClients();
        });
    });
}

$("#submit-client").on("click", function () {
    const clientName = $("#client-name").val().trim();
    const clientEmail = $("#client-email").val().trim();

    if (!validateField(clientName, "Имя клиента не может быть пустым!") ||
        !validateField(clientEmail, "Введите email клиента!") ||
        !validateEmail(clientEmail)) {
        return;
    }

    const clientData = { username: clientName, email: clientEmail };
    sendRequest("POST", "/api/clients/create", clientData, function () {
        alert("Клиент успешно добавлен!");
        $("#add-client-form").hide();
        loadClients();
    });
});

// ----------------------------
// Логика работы с книгами
// ----------------------------
function loadBooks() {
    sendRequest("GET", "/api/books/get-all-simple", null, function (data) {
        renderTable("#books-table", data, createBookRow);
        addBookUpdateHandlers();
        addViewContentHandlers();
        addBookDeleteHandlers();
    });
}

$("#search-books-btn").on("click", function () {
    const title = $("#search-title").val().trim();
    

    // Собираем параметры запроса
    const query = new URLSearchParams();
    if (title) query.append("title", title);
     

    // Отправляем запрос к API
    sendRequest("GET", `/api/books/search?${query.toString()}`, null, function (data) {
        // Рендер списка книг
        renderTable("#books-search-results", data, createBookRow);
    }, function (xhr) {
        // Если ответ не успешный
        console.error("Ошибка поиска:", xhr.responseText);
        alert(xhr.responseText || "Не удалось выполнить поиск.");
    });
});

// Функция рендера строки книги
function createBookRow(book) {
    return `
        <tr>
            <td>${book.id}</td>
            <td>${book.title}</td>
            <td>${book.author}</td>
            <td>${book.genre}</td>
            <td>${book.year}</td>
        </tr>`;
}

function createBookRow(book) {
    return `
        <tr>
            <td>${book.id}</td>
            <td>${book.title}</td>
            <td>${book.author}</td>
            <td>${book.genre}</td>
            <td>${book.description}</td>
            <td>${book.year}</td>
            <td>${book.amount || "Нет информации"}</td>
            <td>
                <button class="btn btn-warning btn-sm update-book" data-id="${book.id}">Изменить</button>
                <button class="btn btn-primary btn-sm view-content" data-id="${book.id}">Смотреть</button>
                <button class="btn btn-danger btn-sm delete-book" data-id="${book.id}">Удалить</button>
                <br><br>
                <input type="file" accept="application/pdf" class="upload-pdf" data-id="${book.id}" style="display: none;" />
                <button class="btn btn-success btn-sm upload-pdf-btn" data-id="${book.id}">Загрузить PDF</button>
            </td>
        </tr>`;
}
function addViewContentHandlers() {
    $(".view-content").on("click", function () {
        const bookId = $(this).data("id"); // Получаем ID книги

        // Перенаправление на страницу просмотра с передачей bookId через query параметр
        window.location.href = `/view-pdf.cshtml?bookId=${bookId}`;
    });
}

function addBookUpdateHandlers() {
    $(".update-book").on("click", function () {
        const bookId = $(this).data("id");
        const newDescription = prompt("Введите новое описание книги:");
        if (newDescription) {
            const updateData = { description: newDescription };
            sendRequest("PUT", `/api/books/update-book/${bookId}`, updateData, function () {
                alert("Описание книги обновлено!");
                loadBooks();
            });
        } else {
            alert("Описание не может быть пустым.");
        }
    });
}
function addBookDeleteHandlers() {
    $(".delete-book").on("click", function () {
        const bookId = $(this).data("id"); // Получаем ID книги

        // Подтверждение удаления
        const confirmDelete = confirm("Вы уверены, что хотите удалить эту книгу?");
        if (!confirmDelete) {
            return; // Пользователь отменил удаление
        }

        // Отправляем запрос на сервер
        sendRequest("DELETE", `/api/books/delete/${bookId}`, null, function () {
            alert("Книга успешно удалена!");
            loadBooks(); // Перезагружаем список книг после удаления
        });
    });
}
$("#submit-book").on("click",async function (event) {
    event.preventDefault(); // Останавливаем отправку формы

    const title = $("#book-title").val().trim();
    const author = $("#book-author").val().trim();
    const genre = $("#book-genre").val().trim();
    const description = $("#book-description").val().trim();
    const year = $("#book-year").val().trim();
    const amount = $("#book-amount").val().trim();

    // Валидация всех полей
    if (!validateField(title, "Название книги не может быть пустым!") ||
        !validateField(author, "Автор книги не может быть пустым!") ||
        !validateField(genre, "Жанр книги не может быть пустым!") ||
        !validateField(description, "Описание книги не может быть пустым!") ||
        !validateField(year, "Год издания книги не может быть пустым!") ||
        !validateField(amount, "Количество экземпляров книги должно быть указано!")) {
        return;
    }

    // Передача всех полей, включая amount (и преобразование типов, где нужно)
    const bookData = {
        title: title,
        author: author,
        genre: genre,
        description: description,
        year: parseInt(year, 10),
        amount: parseInt(amount, 10),
    };

    // Отправка данных на сервер (POST запрос)
    try {
        // Создаём книгу
        const response = await new Promise((resolve, reject) => {
            sendRequest("POST", "/api/books/create-book", bookData, resolve, reject);
        });

        // Если книга успешно создана
        alert("Книга успешно добавлена!");

        // Обновление интерфейса
        $("#add-book-form").hide(); // Закрываем форму добавления книги
        loadBooks(); // Обновляем список книг
    } catch (error) {
        console.error("Ошибка при добавлении книги:", error);
        alert("Ошибка при добавлении книги. Проверьте данные!");
    }
});

async function uploadPdf(bookId, pdfFile) {
    const formData = new FormData();
    formData.append("pdfFile", pdfFile);

    // Используем fetch для отправки данных на сервер
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/api/books/${bookId}/add-pdf`, // Укажите правильный маршрут API
            type: "POST",
            contentType: false, // Указываем, что данные отправляются как FormData
            processData: false, // Отключаем автоматическую сериализацию
            data: formData,
            success: resolve,
            error: reject
        });
    });
}

function addUploadPdfHandlers() {
    // Кнопка "Загрузить PDF" открывает выбор файла
    $(".upload-pdf-btn").on("click", function () {
        const bookId = $(this).data("id"); // ID книги
        $(`.upload-pdf[data-id="${bookId}"]`).trigger("click"); // Триггерим выбор файла
    });

    // Обработка выбора файла
    $(".upload-pdf").on("change", async function () {
        const bookId = $(this).data("id"); // ID книги
        const pdfFile = this.files[0]; // Загруженный файл

        if (!pdfFile) {
            alert("Выберите файл для загрузки.");
            return;
        }

        if (pdfFile.type !== "application/pdf") {
            alert("Можно загружать только PDF файлы.");
            return;
        }

        try {
            // Вызываем функцию загрузки файла
            await uploadPdf(bookId, pdfFile);
            alert("PDF успешно загружен!");
        } catch (error) {
            console.error("Ошибка загрузки PDF:", error);
            alert("Произошла ошибка при загрузке PDF.");
        }
    });
}

// Повторное подключение обработчиков после рендера таблицы
function loadBooks() {
    sendRequest("GET", "/api/books/get-all-simple", null, function (data) {
        renderTable("#books-table", data, createBookRow);
        addBookUpdateHandlers();
        addViewContentHandlers();
        addBookDeleteHandlers();
        addUploadPdfHandlers(); // Привязываем новый обработчик загрузки PDF
    });
}

// ----------------------------
// Логика работы с записями выдачи книг
// ----------------------------
function loadBorrowRecords() {
    sendRequest("GET", "/api/borrow-records/get-detailed", null, function (data) {
        console.log("Полученные записи выдачи:", data);
        renderTable("#borrow-records-table", data, createBorrowRecordRow);
        addBorrowRecordHandlers();
        addDeleteBorrowRecordHandlers();
    });
}
$("#search-records").on("click", function () {
    const bookTitle = $("#search-book-title").val().trim();
    const clientName = $("#search-client-name").val().trim();

     
    sendRequest("GET", `/api/borrow-records/search?bookTitle=${bookTitle}&clientName=${clientName}`, null, function (data) {
         
        renderTable("#search-results-table", data, createSearchResultRow);
    }, function (xhr) {
         
        console.error("Ошибка выполнения поиска:", xhr.responseText);
        alert("Произошла ошибка при выполнении поиска.");
    });
});
// Функция для форматирования даты в формате "день-месяц-год"
function formatDate(dateString) {
    if (!dateString) return "";
    const date = new Date(dateString);

    // Настраиваем формат в стиле "11 февраля 2025"
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return date.toLocaleDateString("ru-RU", options);
}
function createSearchResultRow(record) {
    return `
        <tr>
            <td>${record.clientName}</td>
            <td>${record.bookTitle}</td>
            <td>${formatDate(record.borrowDate)}</td>
            <td>${record.returnDate ? formatDate(record.returnDate) : "Не возвращено"}</td>
        </tr>`;
}



function loadClientsForBorrow() {
    sendRequest("GET", "/api/clients/get-all", null, function (data) {
        const clientSelect = $("#borrow-client");
        clientSelect.empty();
        clientSelect.append('<option value="">-- Выберите клиента --</option>');
        data.forEach(client => {
            clientSelect.append(`<option value="${client.id}">${client.username}</option>`);
        });
    });
}

function loadBooksForBorrow() {
    sendRequest("GET", "/api/books/get-all-simple", null, function (data) {
        const bookSelect = $("#borrow-book");
        bookSelect.empty();
        bookSelect.append('<option value="">-- Выберите книгу --</option>');
        data.forEach(book => {
            bookSelect.append(`<option value="${book.id}">${book.title}</option>`);
        });
    });
}
$("#add-borrow-record-btn").on("click", function () {
    $("#add-borrow-record-form").toggle(); // Показываем или скрываем форму
    loadClientsForBorrow(); // Загрузка списка клиентов
    loadBooksForBorrow();   // Загрузка списка книг
});

function createBorrowRecordRow(record) {
    console.log("Borrow record:", record);
    return `
        <tr>
            <td>${record.bookTitle}</td>
            <td>${record.clientName}</td>
            <td>${formatDate(record.borrowDate)}</td>
            <td>${record.returnDate ? formatDate(record.returnDate) : "Не возвращено"}</td>
            <td>
                <button class="btn btn-success btn-sm return-book" data-id="${record.id}">Вернуть</button>
                <button class="btn btn-danger btn-sm delete-record" data-id="${record.id}">Удалить</button>
            </td>
        </tr>`;
}
$("#submit-borrow-record").on("click", function (event) {
    event.preventDefault(); // Предотвращаем перезагрузку страницы

    const clientId = $("#borrow-client").val();
    const bookId = $("#borrow-book").val();
    const borrowDate = $("#borrow-date").val();

    // Валидация полей
    if (!validateField(clientId, "Выберите клиента!") ||
        !validateField(bookId, "Выберите книгу!") ||
        !validateField(borrowDate, "Укажите дату выдачи!")) {
        return;
    }

    const borrowData = {
        clientId: parseInt(clientId, 10),
        bookId: parseInt(bookId, 10),
        borrowDate: borrowDate
    };

    sendRequest("POST", "/api/borrow-records/create", borrowData, function () {
        alert("Запись выдачи успешно создана!");
        $("#add-borrow-record-form").hide(); // Скрываем форму
        loadBorrowRecords(); // Перезагружаем список записей выдачи
    });
});

function addBorrowRecordHandlers() {
    $(".return-book").on("click", function () {
        const recordId = $(this).data("id"); // Берем ID записи из кнопки
        console.log("Record ID:", recordId);
        if (!recordId) {
            console.error("Не удалось получить ID записи.");
            alert("Не удалось получить идентификатор записи.");
            return;
        }

        // Отправить PUT-запрос на сервер для возврата книги
        sendRequest("PUT", `/api/borrow-records/return/${recordId}`, null, function () {
            alert("Книга успешно возвращена!");
            loadBorrowRecords(); // Перезагружаем список записей
        }, function (xhr) {
            console.error("Ошибка при возврате книги:", xhr.responseText);
            alert("Произошла ошибка при возврате книги. Проверьте лог.");
        });

          
    });
    
}

$("#search-records").on("click", function () {
    const bookTitle = $("#search-book-title").val().trim();
    const clientName = $("#search-client-name").val().trim();

    // Выполняем AJAX-запрос к API для поиска записей
    sendRequest("GET", `/api/borrow-records/search?bookTitle=${bookTitle}&clientName=${clientName}`, null, function (data) {
        renderTable("#borrow-records-table", data, createBorrowRecordRow);
    });
});


function addDeleteBorrowRecordHandlers() {
    $(".delete-record").off("click").on("click", function () {
        const recordId = $(this).data("id");
        console.log("Удалить запись с ID:", recordId);

        if (!recordId || recordId === 0) {
            alert("Не удалось получить ID записи для удаления.");
            return;
        }

        if (confirm("Вы уверены, что хотите удалить эту запись?")) {
            // Отправляем DELETE-запрос на сервер
            sendRequest(
                "DELETE",
                `/api/borrow-records/delete/${recordId}`,
                null,
                function () {
                    alert("Запись успешно удалена!");

                    // Заново загружаем таблицу после успешного удаления
                    loadBorrowRecords();
                },
                function (xhr) {
                    console.error("Ошибка при удалении записи:", xhr.responseText);
                    alert("Не удалось удалить запись. Проверьте данные.");
                }
            );
        }
    });
}

// ----------------------------
// Централизация кнопок загрузки и переключения
// ----------------------------
function setupLoadButton(buttonId, loadFunction) {
    $(buttonId).on("click", function () {
        loadFunction();
    });
}

$(document).ready(function () {
    const apiBaseUrl = "/api";

    // Настройка кнопок загрузки
    setupLoadButton("#load-clients", loadClients);
    setupLoadButton("#load-books", loadBooks);
    setupLoadButton("#load-borrow-records", loadBorrowRecords);

    // Переключение отображения форм
    $("#add-client-btn").on("click", function () {
        $("#add-client-form").toggle();
    });

    $("#add-book-btn").on("click", function () {
        $("#add-book-form").toggle();
    });
});