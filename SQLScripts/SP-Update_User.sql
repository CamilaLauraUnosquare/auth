CREATE PROCEDURE Update_User
    @Email NVARCHAR(250),
    @RefreshToken NVARCHAR(MAX),
    @TokenCreated DATETIME2,
    @TokenExpires DATETIME2
AS
BEGIN
    UPDATE [User] SET 
    RefreshToken = @RefreshToken,
    TokenCreated = @TokenCreated,
    TokenExpires = @TokenExpires WHERE Email = @Email
END
GO