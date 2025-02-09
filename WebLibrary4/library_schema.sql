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
VALUES
    ('TestUser1', 'testuser1@example.com', 'Admin'),
    ('TestUser2', 'testuser2@example.com', 'Client'),
    ('TestUser3', 'testuser3@example.com', 'Client'),
    ('TestUser4', 'testuser4@example.com', 'Client'),
    ('TestUser5', 'testuser5@example.com', 'Client');
 

 

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
SELECT * FROM Books;

INSERT INTO Books (Title, Description, Author, Genre, Year, Amount)
VALUES ('Example Book', 'A book description', 'Author Name', 'Fiction', 2020, 10),
                ('  Book2', 'A book description2', 'Author Name2', 'Fiction2', 2025, 10),
                ('  Book3', 'A book  4', 'Author  32', 'Fiction2', 2022, 10),
                ('  Book4', 'A book description2', '  Name2', ' Piro', 2022, 10),
                ('E e Book5', 'A book description2', 'AutName42', 'iction2', 2022, 10);


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
VALUES (1, 1, '2023-10-20', NULL),
       (2, 3, '2023-10-20', NULL),
       (5, 4, '2023-10-20', NULL),
       (1, 2, '2023-10-20', NULL),
       (3, 4, '2023-10-20', NULL);


SELECT * FROM BorrowRecord;

SELECT
    c.Username AS ClientName,      -- Имя клиента
    b.Title AS BookTitle,          -- Название книги
    br.BorrowDate AS BorrowDate,   -- Дата взятия книги
    br.ReturnDate AS ReturnDate    -- Дата возврата книги (если есть)
FROM
    BorrowRecord br
        JOIN
    Clients c ON br.UserId = c.Id
        JOIN
    Books b ON br.BookId = b.Id;  