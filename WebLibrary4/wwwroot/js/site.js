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
$(document).ready(function () {
    // Обработчик кнопки "Список клиентов"
    $("#load-clients").on("click", function () {
        loadClients();
    });

    // Привязка кнопок пагинации
    $(document).on("click", "#pagination-controls-clients .page-link", function (e) {
        e.preventDefault();
        const page = $(this).data("page");
        loadClients(page); // Загружаем выбранную страницу
    });
});

function loadClients(page = 1, pageSize = 10) {
    // Отправляем запрос с указанием страницы и размера страницы
    sendRequest("GET", `/api/clients/paginated?page=${page}&pageSize=${pageSize}`, null, function (data) {
        console.log("Ответ от API /api/clients/paginated", data);

        if (data && data.clients && data.clients.length > 0) {
            renderTable("#clients-table", data.clients, createClientRow);
            console.log(
                "Перед рендерингом пагинации: currentPage =", data.currentPage,
                "totalPages =", data.totalPages
            );

            renderPaginationClients(data.currentPage, data.totalPages);
        } else {
            const tableBody = $("#clients-table").find("tbody");
            tableBody.empty();
            tableBody.append("<tr><td colspan='5'>Клиенты не найдены</td></tr>");
            $("#pagination-controls").empty(); 
        }
        $("#clients-search-results").hide();
        addClientDeleteHandlers(); 
    });
}
function renderPaginationClients(currentPage, totalPages) {
    const paginationControls = $("#pagination-controls-clients");
    paginationControls.empty(); // Очистить предыдущие кнопки

    if (totalPages <= 1) {
        return; // Если всего одна страница, не создаем пагинацию
    }

    // Кнопка "Предыдущая"
    if (currentPage > 1) {
        paginationControls.append(`
            <li class="page-item">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Предыдущая</a>
            </li>
        `);
    }

    // Генерация кнопок для всех страниц
    for (let i = 1; i <= totalPages; i++) {
        const activeClass = (i === currentPage) ? "active" : "";
        paginationControls.append(`
            <li class="page-item ${activeClass}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>
        `);
    }

    // Кнопка "Следующая"
    if (currentPage < totalPages) {
        paginationControls.append(`
            <li class="page-item">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Следующая</a>
            </li>
        `);
    }
}

 

// Обработчик кликов на кнопках пагинации
$(document).on("click", "#pagination-controls .page-link", function (e) {
    e.preventDefault();
    const page = $(this).data("page");
    loadClients(page); // Загружаем выбранную страницу
});


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

$("#clients-table").on("click", ".delete-client", function () {
    const clientId = $(this).data("id");
    sendRequest("DELETE", `/api/clients/delete/${clientId}`, null, function () {
        alert("Клиент удален!");
        loadClients();
    });
});

// Логика поиска клиентов по имени
let searchParams = {
    query: "",
    page: 1,
    pageSize: 10
};

$("#search-clients-btn").on("click", function () {
    const clientName = $("#search-client-name").val().trim();

    if (!clientName) {
        alert("Введите имя клиента для поиска.");
        return;
    }

    searchParams.query = clientName;
    searchParams.page = 1; // При новом запросе начнем с первой страницы

    loadSearchResults();
});

function loadSearchResults() {
    const { query, page, pageSize } = searchParams;

    sendRequest("GET", `/api/clients/search?name=${query}&page=${page}&pageSize=${pageSize}`, null, function (data) {
        if (!data.clients || data.clients.length === 0) {
            alert("Клиенты не найдены.");
            $("#clients-search-results").hide();
            $("#pagination-controls-search").empty();
            return;
        }

        renderTable("#clients-search-results", data.clients, createClientSearchRow);
        $("#clients-search-results").show();
        renderSearchPagination(data.currentPage, data.totalPages);
    }, function (xhr) {
        console.error("Ошибка при поиске клиентов:", xhr.responseText);
        alert(xhr.responseText || "Не удалось выполнить поиск.");
    });
}

function renderSearchPagination(currentPage, totalPages) {
    const paginationControls = $("#pagination-controls-search");
    paginationControls.empty();

    if (totalPages <= 1) {
        return;
    }

    if (currentPage > 1) {
        paginationControls.append(`
            <li class="page-item">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Предыдущая</a>
            </li>
        `);
    }

    for (let i = 1; i <= totalPages; i++) {
        const activeClass = i === currentPage ? "active" : "";
        paginationControls.append(`
            <li class="page-item ${activeClass}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>
        `);
    }

    if (currentPage < totalPages) {
        paginationControls.append(`
            <li class="page-item">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Следующая</a>
            </li>
        `);
    }
}

$(document).on("click", "#pagination-controls-search .page-link", function (e) {
    e.preventDefault();
    const page = $(this).data("page");

    if (!page) return;

    searchParams.page = page;
    loadSearchResults();
});

function createClientSearchRow(client) {
    return `
        <tr>
            <td>${client.id}</td>
            <td>${client.username}</td>
            <td>${client.email || "N/A"}</td>
            <td>${client.role || "Не указана"}</td>
        </tr>`;
}

function addClientDeleteHandlers() {
    $("#clients-table").on("click", ".delete-client", function () {
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
function loadBooks(page = 1, pageSize = 10, title = "") {
    // Формируем URL-запрос с поддержкой фильтрации
    const url = `/api/books/paginated?page=${page}&pageSize=${pageSize}&title=${encodeURIComponent(title)}`;

    // Отправляем запрос на сервер для получения данных
    sendRequest("GET", url, null, function (data) {
        if (data.books && data.books.length > 0) {
            // Рендерим книги в таблицу
            renderTable("#books-search-results", data.books, createBookRow);

            // Рендерим кнопки пагинации
            renderPagination(data.currentPage, data.totalPages);
        } else {
            // Очищаем таблицу и добавляем сообщение
            $("#books-search-results tbody").empty().append("<tr><td colspan='8'>Книги не найдены.</td></tr>");
            $("#pagination-controls").empty(); // Очищаем пагинацию
        }
    }, function (xhr) {
        console.error("Ошибка при загрузке книг:", xhr.responseText);
        alert("Не удалось загрузить список книг.");
    });
}
function renderPagination(currentPage, totalPages) {
    const paginationControls = $("#pagination-controls");
    paginationControls.empty(); // Очищаем старые кнопки

    // Кнопка "Предыдущая"
    if (currentPage > 1) {
        paginationControls.append(`
            <li class="page-item">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Предыдущая</a>
            </li>
        `);
    }

    // Генерация кнопок для всех страниц
    for (let i = 1; i <= totalPages; i++) {
        const activeClass = i === currentPage ? "active" : "";
        paginationControls.append(`
            <li class="page-item ${activeClass}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>
        `);
    }

    // Кнопка "Следующая"
    if (currentPage < totalPages) {
        paginationControls.append(`
            <li class="page-item">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Следующая</a>
            </li>
        `);
    }
}

// Добавляем обработчики событий для пагинации
$(document).on("click", "#pagination-controls .page-link", function (e) {
    e.preventDefault(); // Отменяем действие по умолчанию ссылки
    const page = $(this).data("page"); // Получаем номер страницы
    loadBooks(page); // Загружаем выбранную страницу
});


// Обработка кнопки поиска
// Обработка кнопки поиска (без пагинации)
$("#search-books-btn").on("click", function () {
    const title = $("#search-title").val().trim();
    searchBooks(title); // вызываем функцию поиска без параметров страницы
});

// Функция для поиска книг по title (обращается к /api/books/search)
function searchBooks(title) {
    const url = `/api/books/search?title=${encodeURIComponent(title)}`;

    sendRequest("GET", url, null, function (books) {
        if (books && books.length > 0) {
            renderTable("#books-search-results", books, createBookRow);
        } else {
            $("#books-search-results tbody")
                .empty()
                .append("<tr><td colspan='8'>Книги не найдены.</td></tr>");
        }
    }, function (xhr) {
        console.error("Ошибка при поиске книг:", xhr.responseText);
        alert("Не удалось найти книги.");
    });
}
// Функция рендера строки книги
// function createBookRow(book) {
//     return `
//         <tr>
//             <td>${book.id}</td>
//             <td>${book.title}</td>
//             <td>${book.author}</td>
//             <td>${book.genre}</td>
//             <td>${book.year}</td>
//         </tr>`;
// }

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
                <button class="btn btn-danger btn-sm delete-book" data-id="${book.id}">Удалить</button>
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
$("#submit-book").off("click").on("click", async function (event) {
 
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
        // loadBooks(); // Обновляем список книг
    } catch (error) {
        console.error("Ошибка при добавлении книги:", error);
        alert("Ошибка при добавлении книги. Проверьте данные!");
    }
});


// Повторное подключение обработчиков после рендера таблицы
function loadBooks(page = 1, pageSize = 10, title = "") {
    const url = `/api/books/paginated?page=${page}&pageSize=${pageSize}&title=${encodeURIComponent(title)}`;

    sendRequest("GET", url, null, function (data) {
        if (data.books && data.books.length > 0) {
            renderTable("#books-table", data.books, createBookRow);
            renderPagination(data.currentPage, data.totalPages);

            // После загрузки данных нужно переподключить обработчики
            addBookUpdateHandlers();
            addBookDeleteHandlers();
        } else {
            $("#books-table tbody").empty().append("<tr><td colspan='8'>Книги не найдены.</td></tr>");
            $("#pagination-controls").empty(); // Очищаем пагинацию
        }
    }, function (xhr) {
        console.error("Ошибка загрузки книг:", xhr.responseText);
        alert("Не удалось загрузить список книг.");
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
function searchRecordsByBookTitle() {
    const bookTitle = $("#search-book-title").val().trim();

    if (!bookTitle) {
        alert("Введите название книги для поиска.");
        return;
    }

    // Выполняем AJAX-запрос для поиска по книге
    sendRequest("GET", `/api/borrow-records/search-by-book?bookTitle=${bookTitle}`, null, function (data) {
        if (!data.length) {
            alert("Записи не найдены по указанной книге.");
            return;
        }

        // Рендерим результаты поиска в таблицу
        renderTable("#search-results-table", data, createBorrowRecordRow);
    }, function (xhr) {
        console.error("Ошибка поиска записей по книге:", xhr.responseText);
        alert("Произошла ошибка при выполнении поиска по книге.");
    });
}

function searchRecordsByClientName() {
    const clientName = $("#search-client-names").val().trim();

    if (!clientName) {
        alert("Введите имя клиента для поиска.");
        return;
    }

    // Выполняем AJAX-запрос для поиска по клиенту
    sendRequest("GET", `/api/borrow-records/search-by-client?clientName=${clientName}`, null, function (data) {
        if (!data.length) {
            alert("Записи не найдены по указанному клиенту.");
            return;
        }

        // Рендерим результаты поиска в таблицу
        renderTable("#search-results-table", data, createBorrowRecordRow);
    }, function (xhr) {
        console.error("Ошибка поиска записей по клиенту:", xhr.responseText);
        alert("Произошла ошибка при выполнении поиска по клиенту.");
    });
}

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


function filterOptions(searchInputId, selectId) {
    $(searchInputId).on("input", function() {
        const searchTerm = $(this).val().toLowerCase();
        $(selectId + " option").each(function() {
            const optionText = $(this).text().toLowerCase();
            $(this).toggle(optionText.includes(searchTerm));
        });
    });
}

// Привязка обработчиков событий к полям ввода
filterOptions("#client-search", "#borrow-client");
filterOptions("#book-search", "#borrow-book");

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
    // Поиск записей по названию книги
    $("#search-records-by-book").on("click", function () {
        searchRecordsByBookTitle();
    });

    // Поиск записей по имени клиента
    $("#search-records-by-client").on("click", function () {
        searchRecordsByClientName();
    });
    
});