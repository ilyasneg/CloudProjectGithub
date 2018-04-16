# CloudProjectGithub
Test project

Перед запуском приложения убедиться, что в директории App_Data имеется папка uploads (для локального хранения файлов). Если её нет, то создать.

Также, в папке App_Data/DBBackUp находится бэкап файл базы данных.

Хранимая процедура dbo.AddDocument:

USE [CloudDB]
GO
/****** Object:  StoredProcedure [dbo].[AddDocument]    Script Date: 16.04.2018 22:37:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[AddDocument]
(@name char(32), @date datetime, @author char(32), @link char(32), @contentPath nvarchar(MAX))
AS
BEGIN
	INSERT INTO CloudDB.dbo.SingleDocument([Name], [Date], [Author], [Link], [ContentPath]) 
    VALUES (@name, @date, @author, @link, @contentPath);
END
