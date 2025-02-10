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
    });
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
            </td>
        </tr>`;
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

$("#submit-book").on("click", function (event) {
    event.preventDefault();
    const title = $("#book-title").val().trim();
    const author = $("#book-author").val().trim();
    const genre = $("#book-genre").val().trim();
    const description = $("#book-description").val().trim();
    const year = $("#book-year").val().trim();

    if (!validateField(title, "Название книги не может быть пустым!") ||
        !validateField(author, "Автор книги не может быть пустым!") ||
        !validateField(genre, "Жанр книги не может быть пустым!") ||
        !validateField(description, "Описание книги не может быть пустым!") ||
        !validateField(year, "Год издания книги не может быть пустым!")) {
        return;
    }

    const bookData = {
        title: title,
        author: author,
        genre: genre,
        description: description,
        year: parseInt(year, 10),
    };

    sendRequest("POST", "/api/books/create-book", bookData, function () {
        alert("Книга успешно добавлена!");
        $("#add-book-form").hide();
        loadBooks();
    });
});

// ----------------------------
// Логика работы с записями выдачи книг
// ----------------------------
function loadBorrowRecords() {
    sendRequest("GET", "/api/borrow-records/get-detailed", null, function (data) {
        renderTable("#borrow-records-table", data, createBorrowRecordRow);
        addBorrowRecordHandlers();
    });
}

function createBorrowRecordRow(record) {
    return `
        <tr>
            <td>${record.bookTitle}</td>
            <td>${record.clientName}</td>
            <td>${record.borrowDate}</td>
            <td>${record.returnDate || "Не возвращено"}</td>
            <td>
                <button class="btn btn-success btn-sm return-book" data-id="${record.id}">Вернуть</button>
            </td>
        </tr>`;
}

function addBorrowRecordHandlers() {
    $(".return-book").on("click", function () {
        const recordId = $(this).data("id");
        const updateData = { id: recordId, returnDate: new Date().toISOString() };
        sendRequest("PUT", "/api/borrow-records/update", updateData, function () {
            alert("Книга возвращена!");
            loadBorrowRecords();
        });
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