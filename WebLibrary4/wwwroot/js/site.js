



$(document).ready(function () {
    const apiBaseUrl = "/api"; // Базовый URL API (здесь локальный)

    // Загрузка списка клиентов
    $("#load-clients").on("click", function () {
        $.get("/api/clients/get-all", function (data) {
            console.log("Полученные клиенты:", data); // Лог текущих данных, возвращаемых API

            const tableBody = $("#clients-table tbody");
            tableBody.empty(); // Очищаем таблицу перед обновлением

            // Отображаем данные клиентов в таблице
            data.forEach((client) => {
                const row = `<tr>
                <td>${client.id}</td> <!-- ID клиента -->
                <td>${client.username}</td> <!-- Имя клиента -->
                <td>${client.email || "N/A"}</td> <!-- Email клиента -->
                <td>${client.role || "Не указана"}</td> <!-- Роль клиента -->
                <td>
                    <button class="btn btn-danger btn-sm delete-client" data-id="${client.id}">Удалить</button>
                </td>
            </tr>`;
                tableBody.append(row);
            });

            // Кнопка удаления клиента
            $(".delete-client").on("click", function () {
                const clientId = $(this).data("id");
                $.ajax({
                    url: `${apiBaseUrl}/clients/delete/${clientId}`,
                    type: "DELETE",
                    success: function () {
                        alert("Клиент удален!");
                        $(`#clients-table tbody tr:has(button[data-id='${clientId}'])`).remove();
                    },
                    error: function () {
                        alert("Ошибка удаления клиента");
                    },
                });
            });
        });
    });
});

$(document).ready(function () {
    const apiBaseUrl = "/api"; // Базовый URL API (здесь локальный)

    // Показ формы добавления клиента
    $("#add-client-btn").on("click", function () {
        $("#add-client-form").toggle(); // Переключение отображения формы
    });

    // Обработка нажатия на кнопку "Добавить клиента"
    $("#submit-client").on("click", function () {
        const clientName = $("#client-name").val(); // Получение имени клиента
        const clientEmail = $("#client-email").val(); // Получение email клиента

        // Проверяем, что поля не пустые
        if (!clientName.trim()) {
            alert("Имя клиента не может быть пустым!");
            return;
        }

        if (!clientEmail.trim() || !validateEmail(clientEmail)) {
            alert("Введите корректный Email!");
            return;
        }

        // AJAX-запрос на добавление клиента
        $.ajax({
            url: `${apiBaseUrl}/clients/create`, // URL для добавления клиента
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                username: clientName, // Имя клиента
                email: clientEmail,   // Email клиента
            }),
            success: function (response) {
                alert("Клиент успешно добавлен!");
                $("#add-client-form").hide(); // Скрыть форму
                $("#client-name").val(""); // Очистить поле ввода
                $("#client-email").val(""); // Очистить поле ввода email
                $("#load-clients").click(); // Обновить список клиентов
            },
            error: function (xhr, status, error) {
                console.error("Ошибка добавления клиента:", error);
                alert("Не удалось добавить клиента. Проверьте правильность данных.");
            },
        });
    });

    // Функция для валидации email
    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }
});

// Загрузка списка книг
$("#load-books").on("click", function () {
    $.get(`${apiBaseUrl}/books/get-all-sample`, function (data) {
        $.get(`${apiBaseUrl}/books/get-all-sample`)
            .done(function (data) {
                console.log("Данные от API (книги)", data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.error("Ошибка получения данных (книги):", textStatus, errorThrown);
                alert("Ошибка загрузки данных. Проверьте API.");
            });
        console.log("Полученные книги:", data); // Логируем данные, чтобы убедиться, что они корректно приходят

        const tableBody = $("#books-table tbody");
        tableBody.empty(); // Очищаем таблицу

        // Проверяем, вернулись ли данные
        if (!Array.isArray(data) || data.length === 0) {
            alert("Данные о книгах недоступны или список пуст.");
            return;
        }

        // Перебираем список книг и добавляем их в таблицу
        data.forEach((book) => {
            const row = `<tr>
                <td>${book.id}</td> <!-- ID книги -->
                <td>${book.title}</td> <!-- Название книги -->
                <td>${book.author}</td> <!-- Автор -->
                <td>${book.genre}</td> <!-- Жанр -->
                <td>${book.description}</td> <!-- Описание -->
                <td>${book.year}</td> <!-- Год издания -->
                <td>${book.amount || "Нет информации"}</td> <!-- Количество -->
                <td><button class="btn btn-warning btn-sm update-book" data-id="${book.id}">Изменить</button></td>
            </tr>`;
            tableBody.append(row);
        });
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error("Ошибка загрузки книг:", textStatus, errorThrown);
        alert("Не удалось загрузить книги. Проверьте, доступен ли API.");
    });
});

        // Кнопка изменения книги
        $(".update-book").on("click", function () {
            const bookId = $(this).data("id");
            const newDescription = prompt("Введите новое описание книги:");
            if (newDescription) {
                $.ajax({
                    url: `${apiBaseUrl}/books/update-book/${bookId}`,
                    type: "PUT",
                    contentType: "application/json",
                    data: JSON.stringify({ id: bookId, description: newDescription }),
                    success: function () {
                        alert("Описание обновлено!");
                        $(`#books-table tbody tr:has(button[data-id='${bookId}']) td:nth-child(5)`).text(
                            newDescription
                        );
                    },
                    error: function () {
                        alert("Ошибка обновления описания");
                    },
                });
            }
        });
 


// Загрузка записей о выдаче книг
$("#load-borrow-records").on("click", function () {
    $.get(`${apiBaseUrl}/borrow-records/get-all`, function (data) {
        const tableBody = $("#borrow-records-table tbody");
        tableBody.empty(); // Очистка таблицы
        data.forEach((record) => {
            const row = `<tr>
                <td>${record.id}</td>
                <td>${record.bookTitle}</td>
                <td>${record.clientName}</td>
                <td>${record.borrowDate}</td>
                <td>${record.returnDate || "Не возвращено"}</td>
                <td><button class="btn btn-success btn-sm return-book" data-id="${record.id}">Вернуть</button></td>
            </tr>`;
            tableBody.append(row);
        });

        // Обработка возврата книги
        $(".return-book").on("click", function () {
            const recordId = $(this).data("id");
            $.ajax({
                url: `${apiBaseUrl}/borrow-records/update`,
                type: "PUT",
                contentType: "application/json",
                data: JSON.stringify({ id: recordId, returnDate: new Date().toISOString() }),
                success: function () {
                    alert("Книга возвращена!");
                    const returnDate = new Date().toLocaleDateString();
                    $(`#borrow-records-table tbody tr:has(button[data-id='${recordId}']) td:nth-child(5)`).text(
                        returnDate
                    );
                },
                error: function () {
                    alert("Ошибка возврата книги");
                },
            });
        });
    });
});

