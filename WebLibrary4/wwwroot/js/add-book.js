$(document).ready(function () {
    // "Добавить книгу" по нажатию кнопки
    $("#add-book").on("click", function () {
        // Открываем модальное окно с формой для добавления книги
        const bookFormHtml = `
            <div id="addBookModal" style="display: block; position: fixed; z-index: 10; 
                background: rgba(0,0,0,0.7); top: 0; left: 0; width: 100%; height: 100%;" >
                <div style="background: white; padding: 20px; width: 400px; 
                    margin: 100px auto; border-radius: 8px;">
                    <h4>Добавить книгу</h4>
                    <form id="add-book-form">
                        <div class="mb-3">
                            <label for="title" class="form-label">Название</label>
                            <input type="text" class="form-control" id="title" required>
                        </div>
                        <div class="mb-3">
                            <label for="author" class="form-label">Автор</label>
                            <input type="text" class="form-control" id="author" required>
                        </div>
                        <div class="mb-3">
                            <label for="genre" class="form-label">Жанр</label>
                            <input type="text" class="form-control" id="genre" required>
                        </div>
                        <div class="mb-3">
                            <label for="description" class="form-label">Описание</label>
                            <textarea class="form-control" id="description" rows="3" required></textarea>
                        </div>
                        <div class="mb-3">
                            <label for="year" class="form-label">Год</label>
                            <input type="number" class="form-control" id="year" required>
                        </div>
                        <div class="mb-3">
                            <label for="amount" class="form-label">Количество</label>
                            <input type="number" class="form-control" id="amount" required>
                        </div>
                        <button type="button" id="submit-add-book" class="btn btn-success">Добавить</button>
                        <button type="button" id="cancel-add-book" class="btn btn-secondary">Отмена</button>
                    </form>
                </div>
            </div>
        `;

        $("body").append(bookFormHtml); // Добавляем форму в DOM

        // Событие на отмену (закрытие формы)
        $("#cancel-add-book").on("click", function () {
            $("#addBookModal").remove(); // Убираем модальное окно
        });

        // Событие на отправку формы добавления книги
        $("#submit-add-book").click(function () {
            // Данные для отправки на сервер
            const bookData = {
                title: $("#title").val().trim(),
                author: $("#author").val().trim(),
                genre: $("#genre").val().trim(),
                description: $("#description").val().trim(),
                year: parseInt($("#year").val()),
                amount: parseInt($("#amount").val()),
            };

            // Простая валидация
            if (!bookData.title || !bookData.author || !bookData.genre || !bookData.description || !bookData.year || !bookData.amount) {
                alert("Заполните все поля формы!");
                return;
            }

            // AJAX-запрос на сервер для добавления книги
            $.ajax({
                url: "/api/books/create-book", // Укажите правильный путь к API
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(bookData),
                success: function (response) {
                    alert("Книга успешно добавлена!");
                    $("#addBookModal").remove(); // Закрываем модальное окно после успешного добавления книги
                    // Возможно, вы захотите обновить список книг, чтобы новая книга отображалась
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.error("Ошибка при добавлении книги:", textStatus, errorThrown);
                    alert("Не удалось добавить книгу. Попробуйте ещё раз.");
                },
            });
        });
    });
});