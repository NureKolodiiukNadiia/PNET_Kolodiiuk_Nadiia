-- Розробити процедуру додавання оцінки користувача для фільму.
-- Вхідні параметри – ім’я користувача або email, назва контенту, значення оцінки.
-- Якщо існує декілька записів контенту з однаковою назвою, обрати перший за датою релізу.
-- Якщо користувач уже оцінював цей контент, оновити існуючу оцінку,
-- інакше створити новий запис у таблиці Votes.

CREATE OR ALTER PROCEDURE Add_Vote(
    @TitleName VARCHAR(100),
    @Email VARCHAR(200),
    @VoteValue SMALLINT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @VoteValue < 0 OR @VoteValue > 10
    BEGIN
        RAISERROR('Vote value must be within range 0-10', 16, 1);
        RETURN;
    END;

    DECLARE @UserId UNIQUEIDENTIFIER;

    SELECT @UserId = u.Id
    FROM AspNetUsers u
    WHERE u.Email = @Email;

    IF @UserId IS NULL
    BEGIN
        RAISERROR('User not found', 16, 1);
        RETURN;
    END;

    DECLARE @TitleId BIGINT;

    SELECT TOP 1 @TitleId = t.Id
    FROM Titles t
    WHERE t.Name = @TitleName
    ORDER BY t.ReleaseDate ASC, t.Id ASC;

    IF @TitleId IS NULL
    BEGIN
        RAISERROR('Title not found', 16, 1, @TitleName);
        RETURN;
    END;

    DECLARE @VoteId BIGINT;

    SELECT @VoteId = v.Id
    FROM Votes v
    WHERE v.TitleId = @TitleId AND v.UserId = @UserId;

    IF @VoteId IS NULL
    BEGIN
        INSERT INTO Votes (TitleId, UserId, [Value], UpdatedAt)
        VALUES (@TitleId, @UserId, @VoteValue, GETDATE());
        RETURN;
    END;

    UPDATE Votes
    SET [Value] = @VoteValue, UpdatedAt = GETDATE()
    WHERE Id = @VoteId;
END
GO

-- =====================================================================================
-- Створити функцію, яка виводить інформацію про кожен i-й фільм із заданим жанром,
-- де i — остання цифра номера за списком; якщо i = 0 або i = 1, обрати i = 10.
-- Функція має повертати назву, дату релізу, рейтинг TMDB, режисера та жанри.

CREATE OR ALTER FUNCTION Get_Title_Info_By_Genre
(
    @Genre NVARCHAR(30),
    @I INT
)
RETURNS TABLE
AS
RETURN
(
    WITH OrderedTitles AS
    (
        SELECT
            t.Name,
            t.ReleaseDate,
            t.AvgTmdbRating AS Rating,
            t.Director,
            t.Genres,
            ROW_NUMBER() OVER (ORDER BY t.Name ASC, t.Id ASC) AS RowNum
        FROM Titles t
        WHERE t.Genres LIKE N'%' + @Genre + N'%'
    )
    SELECT
        ot.Name,
        ot.ReleaseDate,
        ot.Rating,
        ot.Director,
        ot.Genres
    FROM OrderedTitles ot
    WHERE ot.RowNum %
        CASE
            WHEN @I = 0 OR @I = 1 THEN 10
            ELSE @I
        END = 0
);
GO

-- ======================================================================

-- Розробити функцію, яка визначає назву фільму з найвищим середнім рейтингом, оціненого у вказану дату.
-- Вхідний параметр — дата.
-- Якщо існує декілька таких записів, вивести один із них.
-- Якщо у вказану дату оцінок не було, повернути повідомлення: “Контент не був оцінений у [Дата]”

create or alter function Get_Title_With_Highest_Rating_By_Date(@VoteDate date)
returns nvarchar(300)
as
begin
    declare @Name nvarchar(300);

    with VotedAtDate as (
        select
            t.Id,
            t.Name,
            v.[Value]
        from Titles t
        inner join Votes v on t.Id = v.TitleId
        where CAST(v.UpdatedAt as date) = @VoteDate
    ),
    Grouped as (
        select
          vad.Name,
          avg(cast(vad.[Value] as float)) AvgRating
        from VotedAtDate vad
        group by vad.Id, vad.Name
    )

    select top 1 @Name = g.Name
    from Grouped g
    order by g.AvgRating desc;

    if (@Name is null)
    begin
        set @Name = concat('Контент не був оцінений у ', cast(@VoteDate as nvarchar(20)));
    end;

    return @Name;
end
go

-- ========================================================================================
-- Розробити тригер, який після додавання нового елемента до списку перегляду перевіряє кількість контенту в цьому списку.
-- Якщо у списку стало більше 10 різних фільмів або серіалів,
-- додати до назви списку або службового поля інформацію на кшталт “Список містить більше 10 найменувань”,
-- якщо така інформація ще не була додана.

CREATE OR ALTER TRIGGER trg_WatchListItems_AfterInsert
ON WatchListItems
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @WatchListId BIGINT;
    DECLARE @CurrentName NVARCHAR(300);
    DECLARE @TitlesCount INT;

    DECLARE watchlist_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT DISTINCT WatchListId
        FROM inserted;

    OPEN watchlist_cursor;

    FETCH NEXT FROM watchlist_cursor INTO @WatchListId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @TitlesCount = COUNT(DISTINCT TitleId)
        FROM WatchListItems
        WHERE WatchListId = @WatchListId;

        SELECT @CurrentName = Name
        FROM WatchLists
        WHERE Id = @WatchListId;

        IF @TitlesCount > 10
           AND ISNULL(@CurrentName, N'') NOT LIKE N'%Список містить більше 10 найменувань%'
        BEGIN
            UPDATE WatchLists
            SET Name = CONCAT(Name, N' — Список містить більше 10 найменувань')
            WHERE Id = @WatchListId;
        END;

        FETCH NEXT FROM watchlist_cursor INTO @WatchListId;
    END;

    CLOSE watchlist_cursor;
    DEALLOCATE watchlist_cursor;
END;
GO
