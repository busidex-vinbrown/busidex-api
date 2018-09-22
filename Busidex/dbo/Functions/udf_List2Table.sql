CREATE FUNCTION dbo.udf_List2Table
(
	@List VARCHAR(MAX),
	@Delimiter CHAR
)
RETURNS
@ParsedList TABLE
(
	item VARCHAR(MAX)
)
AS
BEGIN
	DECLARE @item VARCHAR(MAX), @Pos INT
	SET @List = LTRIM(RTRIM(@List))+ @Delimiter
	SET @Pos = CHARINDEX(@Delimiter, @List, 1)
	WHILE @Pos > 0
		BEGIN
		
		SET @item = LTRIM(RTRIM(LEFT(@List, @Pos - 1)))
		IF @item <> ''
		BEGIN
			INSERT INTO @ParsedList (item)
			VALUES (CAST(@item AS VARCHAR(MAX)))
		END
		SET @List = RIGHT(@List, LEN(@List) - @Pos)
		SET @Pos = CHARINDEX(@Delimiter, @List, 1)

	END
RETURN
END