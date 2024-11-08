ALTER PROCEDURE Get_User 
    @Email NVARCHAR(250)
AS
BEGIN
    SELECT UserId, FirstName, LastName, Email,PasswordHash, PasswordSalt FROM [User] WHERE  Email = @Email
END
GO