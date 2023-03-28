--Author: Daniel
--Date: 10th March, 2023
/*
    Description:Creates database and tables for PH124&125 (Telegram Bot data bundles)
*/
USE MASTER
GO
DROP DATABASE IF EXISTS [Bundles]
GO
CREATE DATABASE [Bundles]
GO

ALTER DATABASE [Bundles] SET AUTO_CLOSE OFF 
GO

CREATE TABLE [dbo].[Bundles](
    [bundleId][int] IDENTITY(1,1) NOT NULL,
    [name][varchar](100) NOT NULL,
    CONSTRAINT[Pk_Bundles] PRIMARY KEY CLUSTERED(
        [bundleId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[BundlePackages](
    [bundlePackageId][int] IDENTITY(1,1) NOT NULL,
    [bundleId][int] NOT NULL,
    [amount][decimal](8,2) NOT NULL,
    [size][varchar](20) NOT NULL,
    [unit][varchar](10)NOT NULL,
    CONSTRAINT [PK_BundlePackages] PRIMARY KEY CLUSTERED (
        [bundlePackageId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


GO

CREATE TABLE [dbo].[User](
    [userId][int] IDENTITY(1,1) NOT NULL,
    [telegramUserId]varchar(30)NOT NULL,
    [phoneNumber]varchar(30) NOT NULL,
    [pin][int] NOT NULL,
    [vfCashBalance][decimal](8,2) NOT NULL,
    [dataBalance][varchar](100) NOT NULL,
    [airtimeBalance][decimal](8,2) NOT NULL,
    [isSuspended][bit]NOT NULL,
    [isActive][bit]NOT NULL
    CONSTRAINT[PK_UsersId] PRIMARY KEY CLUSTERED(
        [userId] ASC
    ) 
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[DataPurchases](
    [purchaseId][int] IDENTITY(1,1) NOT NULL,
    [userId][int] NOT NULL,
    [bundlePackageId][int]NOT NULL,
    [bundlePackagePrice][decimal](8,2)NOT NULL,
    [purchaseMode][varchar](30) NOT NULL,
    [purchaseDate][datetime] NOT NULL,
    [transactionReference][varchar](50)NOT NULL,
    CONSTRAINT [Pk_DataPurchases] PRIMARY KEY CLUSTERED (
        [purchaseId] ASC
    )  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO 

CREATE TABLE [dbo].[ApiCustomers](
    [customerId][int]IDENTITY(1,1) NOT NULL,
    [name][varchar](100) NOT NULL,
    [username][varchar](100) NOT NULL,
    [password][varchar](128) NOT NULL,
    [email][varchar](100)NOT NULL,
    [phoneNumber][varchar](30) NOT NULL,
    CONSTRAINT [Pk_ApiCustomers] PRIMARY KEY CLUSTERED(
        [customerId] ASC
    )  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BundlePackages]  WITH CHECK ADD FOREIGN KEY([bundleId])
REFERENCES [dbo].[Bundles] ([bundleId])

GO

ALTER TABLE [dbo].[DataPurchases]  WITH CHECK ADD FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([userId])

GO

ALTER TABLE [dbo].[DataPurchases] WITH CHECK ADD FOREIGN KEY (bundlePackageId)
REFERENCES [dbo].[BundlePackages] ([bundlePackageId])

ALTER TABLE [dbo].[DataPurchases] ADD DEFAULT (getdate()) FOR [purchaseDate]

ALTER TABLE [dbo].[User] ADD DEFAULT (0) FOR [isSuspended]
GO 

---Author:Daniel
---Date: 16th March, 2023
/*
    Description: Verifies username and password OF ApiCustomer
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_ApiCustomers_GetByUsernameAndPassword]
@username varchar(100),
@password varchar(128)
AS
BEGIN 
    BEGIN TRY
        SELECT TOP 1 username,email FROM [dbo].[ApiCustomers]  WHERE [username] = @username AND [password] = @password 
    END TRY
    BEGIN CATCH
       PRINT 'Error Number' + STR(ERROR_NUMBER())
       PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH 
END

GO  

---Author: Daniel
---Date: 16th March,2023
/*
    Description: Verifies if user exists within Users table
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_User_CheckExistence]
@telegramUserId varchar(30)
AS
BEGIN
    BEGIN TRY
        IF EXISTS(SELECT 1 FROM [dbo].[User] WHERE telegramUserId = @telegramUserId)
            SELECT 1 AS [existence]
        ELSE
            SELECT 0 AS [existence]
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END

GO

---Author: Daniel
---Date: 16th March, 2023
/*
    Description: Adds a new user to User table
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_User_AddNewUser]
@telegramUserId varchar(30),
@phoneNumber varchar(30),
@pin varchar(8)
AS 
BEGIN 
    BEGIN TRY
        INSERT INTO [dbo].[User](
            telegramUserId,
            phoneNumber,
            pin
        )
        VALUES
        (
            @telegramUserId,
            @phoneNumber,
            @pin
        )
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END
 


GO
--Author: Daniel
--Date: 9th March, 2023
/*
    Description:Returns bundle packages for a given bundle id
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_BundlePackages_Get]
    @bundleId int
AS
BEGIN 
    BEGIN TRY
        IF EXISTS(SELECT 1 FROM [dbo].[Bundles] WHERE bundleId = @bundleId)
        BEGIN 
            SELECT size,unit,amount,bundlePackageId FROM [dbo].[BundlePackages]
            WHERE bundleId = @bundleId
            -- ORDER BY [amount] ASC
        END
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END

GO

--Author: Daniel
--Date: 13th March, 2023
/*
    Description:Generates transaction reference based on transaction ID
*/
CREATE OR ALTER FUNCTION [dbo].[PCES_GenerateTransactionId](@transactionId int) 
RETURNS varchar(50)
AS
BEGIN

   DECLARE @transactionLength int = LEN(@transactionId);
   DECLARE @zeroCount int = 10 - @transactionLength;
   DECLARE @date date = GETDATE();
   DECLARE @year int = YEAR(@date);

   RETURN  'BT'+ REPLICATE(0,@zeroCount)+CONVERT(varchar(10),@transactionId)+CONVERT(varchar(10), @year)
    
END

GO
--Author: Daniel
--Date: 13th March, 2023
/*
    Description:Inserts new purchase into the data purchase table
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_DataPurchase_Insert]
@purchaseId int OUTPUT,
@userId int,
@bundlePackageId int,
@bundlePackagePrice decimal(8,2),
@purchaseMode varchar(30)
AS
BEGIN
INSERT INTO [dbo].[DataPurchases](
                userId,
                bundlePackageId,
                bundlePackagePrice,
                purchaseMode
            )
            VALUES (
                @userId,
                @bundlePackageId,
                @bundlePackagePrice,
                @purchaseMode
            )
            SET @purchaseId = SCOPE_IDENTITY()
            RETURN @purchaseId
END

GO
--Author: Daniel
--Date: 9th March, 2023
/*
    Description:Bundle purchase transaction. Creates a record when user purchases a bundle.
    Debits the user account.
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_DataPurchases_InsTransaction]
    @userId int,
    @bundlePackageId int,
    @purchaseMode varchar(30)
AS
BEGIN 
    BEGIN TRY
        BEGIN TRANSACTION
        
        --Get price of package from BundlePackages table
        DECLARE @price AS decimal(8,2)
        DECLARE @size AS int

        SELECT 
            @price=[amount], 
            @size=[size] 
        FROM [dbo].[BundlePackages]
        WHERE bundlePackageId = @bundlePackageId

         --Get userbalance from db
        DECLARE @balance AS decimal(8,2)
        DECLARE @userData AS int

        SELECT 
            @balance=vfCashBalance, 
            @userData=dataBalance 
        FROM [dbo].[User]
        WHERE userId = @userId

        --Validate if transaction is feasible
        IF (@balance<@price)
            RAISERROR('Insufficient Funds', 16, 1)
        ELSE 
            --Reduce user account balance
            --Add to user data balance
            UPDATE [dbo].[User]
            SET vfCashBalance =  @balance-@price,
                dataBalance = @userData+@size
            WHERE userId = @userId

            --Insert new record into  datapurchases table
            DECLARE @Id int;
            EXEC PCES_DataPurchase_Insert
                @purchaseId = @Id OUTPUT,
                @userId=@userId,
                @bundlePackageId=@bundlePackageId,
                @bundlePackagePrice=@price,
                @purchaseMode=@purchaseMode
            
            --Generate transaction reference
            DECLARE @finalId varchar(100);
            EXEC @finalId = [dbo].[PCES_GenerateTransactionId] @transactionId=@Id


            --update record in table with id with transaction reference
            UPDATE dbo.DataPurchases
            SET 
               transactionReference=  @finalId
            WHERE purchaseId = @Id            

        COMMIT TRANSACTION

    END TRY
    BEGIN CATCH
       PRINT 'Error Number' + STR(ERROR_NUMBER())
       PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END


GO

--Author: Daniel
--Date: 9th March, 2023
/*
    Description:Returns latest transaction by user
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_DataPurchases_GetLatest]
@userId int
AS
BEGIN
    IF EXISTS (SELECT 1  FROM dbo.[User] WHERE userId = @userId)
        SELECT TOP 1 size,unit, transactionReference, purchaseDate, amount
        FROM dbo.DataPurchases AS a
        INNER JOIN dbo.BundlePackages AS b
        ON b.bundlePackageId = a.bundlePackageId
        WHERE userId = @userId 
        ORDER BY purchaseId DESC
END
GO

--Author: Daniel
--Date: 10th March, 2023
/*
    Description:Validates a users pin
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_User_ValidatePin]
@userId int,
@pin int
AS
BEGIN
    BEGIN TRY 
        IF EXISTS (SELECT 1 FROM [dbo].[User] WHERE userId = @userId)
        BEGIN
            SELECT pin
            FROM [dbo].[User]
            WHERE userId = @userId  AND  pin = @pin
        END
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH

END

GO

--Author: Daniel
--Date: 13th March, 2023
/*
    Description:Changes isSuspended column on table to 1 for a given user
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_User_Suspend]
@userId int
AS
BEGIN
    BEGIN TRY
    IF EXISTS(SELECT 1 FROM dbo.[User] WHERE userId=@userId)
        BEGIN 
            UPDATE [dbo].[User]
            SET 
            [IsSuspended]=1
            WHERE userId = @userId
        END
    ELSE
         RAISERROR('Email not found',15,1)
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END
GO

--Author: Daniel
--Date: 21st March, 2023
/*
    Description:Changes isActive column on table to 1 for a given user
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_User_ChangeActiveStatus]
@userId int
AS
BEGIN 
    BEGIN TRY
        IF EXISTS(SELECT 1 FROM dbo.[User] WHERE userId=@userId)
            BEGIN 
                UPDATE [dbo].[User]
                SET 
                [IsActive]=1
                WHERE userId = @userId
            END
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END

GO

--Author: Daniel
--Date: 22nd March, 2023
/*
    Description:Changes isSuspended column on table to 1 for a given user
*/
CREATE OR ALTER PROCEDURE [dbo].[PCES_User_ChangeActiveStatus]
@userId int
AS
BEGIN 
    BEGIN TRY
        IF EXISTS(SELECT 1 FROM dbo.[User] WHERE userId=@userId)
            BEGIN 
                SELECT
            END
    END TRY
    BEGIN CATCH
        PRINT 'Error Number' + STR(ERROR_NUMBER())
        PRINT 'Error Message: '+ ERROR_MESSAGE()
    END CATCH
END

GO

SELECT isSuspended, userId, telegramUserId
FROM [User]
WHERE telegramUserId = '123456'

---Seed Data
/************Bundles*************/
INSERT INTO dbo.Bundles (
    [name]
)
VALUES
('2Moorch'),
('Daily/Bossu'),
('Weekly'),
('Monthly'),
('Hour'),
('Night'),
('Bossu')

GO 

/************Bundle Packages*************/
INSERT INTO dbo.BundlePackages (
    [size],
    [unit],
    [amount],
    [bundlePackageId]
)
VALUES
('60','GB',250,1 ),
('55','MB',1,1),
('130','MB',2,1),
('655','MB',5,1),
('1.2','GB',10,1),
('2.2','GB',20,1),
('5.5','GB',50,1),
('12','GB',100,1)

GO

/************Users*************/
INSERT INTO dbo.[User](
    [dataBalance],
    [airtimeBalance],
    [vfCashBalance],
    [pin]
)
VALUES
(200,50,700,1234),
(1000,50,10,1234),
(0,50,200,1234)

GO

/************Api customers*************/
INSERT INTO [dbo].[ApiCustomers](
    [name],
    [username],
    [password],
    [email],
    [phoneNumber]
)
VALUES
('TestOrg','test','GHANA@50','test@test.com', '2334560340302'),
('PH','phtest', 'HELLO@PH', 'ph@ph.test', '2335435449009')


/************Tests*************/
-- EXEC  [dbo].[PCES_User_ValidatePin] @userId=2, @pin=1234
EXEC [dbo].[PCES_User_CheckExistence] @telegramUserId= '532015284'
-- EXEC [dbo].[PCES_DataPurchases_GetLatest] @userId =1

-- EXEC [dbo].[PCES_BundlePackages_Get] @bundleId=1 
-- EXEC [dbo].[PCES_User_Suspend]@userId=6
SELECT * FROM DataPurchases
-- SELECT * FROM Bundles
EXEC PCES_ApiCustomers_GetByUsernameAndPassword @username='test', @password='GHANA@50'
SELECT * FROM ApiCustomers
SELECT * FROM [User] 
--  EXEC PCES_User_AddNewUser @telegramUserId='3333333', @phoneNumber='23345467965', @pin='1800'
-- EXEC [dbo].[PCES_DataPurchases_Ins] 
--     @userId =1,
--     @bundlePackageId = 4,
--     @purchaseMode = "vfcash"
-- DECLARE @output int;
-- EXEC [dbo].[PCES_DataPurchase_TestInsert]  
--     @userId =1,
--     @bundlePackageId = 4,
--     @purchaseMode = "vfcash",
--     @purchaseId  = @output OUTPUT
-- SELECT @output as 'Count'

-- GO

-- EXEC [dbo].[PCES_DataPurchases_Test]
--      @userId =1,
--     @bundlePackageId = 4,
--     @purchaseMode = "vfcash"



