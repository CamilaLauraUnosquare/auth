SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Save_User]
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(250),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
BEGIN
    IF NOT EXISTS (SELECT *
    FROM [User]
    WHERE Email = @Email)
BEGIN
        INSERT INTO [User]
            (FirstName, LastName, Email, PasswordHash, PasswordSalt)
        VALUES
            (@FirstName, @LastName, @Email, @PasswordHash, @PasswordSalt);
        PRINT 'Data inserted successfully.'
    END
ELSE
BEGIN
        PRINT 'Table does not exist.'
    END
END 
GO
