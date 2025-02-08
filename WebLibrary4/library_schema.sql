-- Удаление таблицы BorrowRecord
DROP TABLE IF EXISTS BorrowRecord;

-- Удаление таблицы PdfDocument
DROP TABLE IF EXISTS PdfDocument;

-- Удаление таблицы Books
DROP TABLE IF EXISTS Books;

-- Удаление таблицы Users
DROP TABLE IF EXISTS Clients;


CREATE TABLE Clients (
                       Id SERIAL PRIMARY KEY,              -- Уникальный идентификатор
                       Username VARCHAR(100) NOT NULL,     -- Имя пользователя
                       Email VARCHAR(255) NOT NULL UNIQUE, -- Email (уникальный)
                       Role VARCHAR(50) DEFAULT 'Client'   -- Роль пользователя (по умолчанию Client)
);

INSERT INTO Clients (Username, Email, Role)
VALUES ('TestUser', 'testuser@example.com','Admin');

SELECT * FROM Clients;


CREATE TABLE Books (
                       Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,  -- Уникальный идентификатор
                       Title VARCHAR(100) NOT NULL,        -- Название книги
                       Description VARCHAR(500),           -- Описание книги
                       Author VARCHAR(100) NOT NULL,       -- Автор книги
                       Genre VARCHAR(50) NOT NULL,         -- Жанр книги
                       Year INT CHECK (Year >= 1000 AND Year <= 2026), -- Год выпуска (проверка диапазона)
                       Amount INT NOT NULL CHECK (Amount > 0) -- Количество экземпляров книги
);
INSERT INTO Books (Title, Description, Author, Genre, Year, Amount)
VALUES ('Example Book', 'A book description', 'Author Name', 'Fiction', 2020, 10);

SELECT * FROM Books;

CREATE TABLE PdfDocument (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,         -- Уникальный идентификатор PDF
    FileName VARCHAR(255) NOT NULL,            -- Имя файла
    ContentType VARCHAR(50) DEFAULT 'application/pdf', -- Тип контента (по умолчанию PDF)
    Content BYTEA NULL,                  -- Содержимое файла
    BookId INT UNIQUE,  -- Ссылка на книгу (Id) из таблицы Books (уникальна для ограничения "один-на-одного")
    CONSTRAINT fk_Book FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE
);
INSERT INTO PdfDocument (FileName, Content)
VALUES ('example.pdf', NULL);

SELECT * FROM PdfDocument;

CREATE TABLE BorrowRecord (
                              Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,        -- Уникальный идентификатор записи
                              BookId INT NOT NULL,                      -- Идентификатор книги
                              UserId INT NOT NULL,                      -- Идентификатор пользователя
                              BorrowDate DATE NOT NULL,                 -- Дата аренды
                              ReturnDate DATE,                          -- Дата возврата (может быть NULL)
                              FOREIGN KEY (BookId) REFERENCES Books(Id) -- Связь с таблицей Books
                                  ON DELETE CASCADE,
                              FOREIGN KEY (UserId) REFERENCES Clients(Id) -- Связь с таблицей Users
                                  ON DELETE CASCADE
                              
);

INSERT INTO BorrowRecord (BookId, UserId, BorrowDate, ReturnDate)
VALUES (1, 1, '2023-10-20', NULL);

SELECT * FROM BorrowRecord;