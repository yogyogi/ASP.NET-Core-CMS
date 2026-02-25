using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Infrastructure
{
    public class CreateStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"GO
				/****** Object:  StoredProcedure [dbo].[sp_DeleteMedia]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_DeleteMedia]
					@Id	VARCHAR(50)
				AS  
				BEGIN
					DELETE FROM Media WHERE id in (select s from dbo.split(',',@Id))	
				END



				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetBlogCategoryWithPaging]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetBlogCategoryWithPaging] --1,'yogyogi',1,null
				@PageNo INT,
				@Name VARCHAR(100),
				@PageSize	INT,
				@Status	INT
				AS  
				BEGIN
					DECLARE @qryMain  NVARCHAR(1000);
					DECLARE @qry  NVARCHAR(1000);   
					SET @qryMain='SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY AddedON DESC) AS ''RowNum'',* FROM BlogCategory WHERE 1=1' 
					SET @qry=''

					IF @Status IS NOT NULL
						SET @qry=@qry+' AND Status='''+CAST(@Status AS NVARCHAR(5))+''''     
					IF @Name IS NOT NULL
						SET @qry=@qry+' AND Name like''%'+@Name+'%'''  
		   
					SET @qryMain=@qryMain+@qry+') a WHERE a.RowNum BETWEEN ('+CAST(@pageNo AS NVARCHAR(5))+'-1)*'+CAST(@pageSize AS NVARCHAR(5))+'+1 AND ('+CAST(@pageNo AS NVARCHAR(5))+'*'+CAST(@pageSize AS NVARCHAR(5))+')'

					DECLARE @qryTotal  NVARCHAR(1000); 
					DECLARE @Total INT;
					SET @qryTotal='' 
					SET @qryTotal='SELECT @Total=COUNT(*) FROM BlogCategory WHERE 1=1'+@qry
					EXEC Sp_executesql @qryTotal, N'@Total INT OUTPUT', @Total OUTPUT

					DECLARE	@AllTotalPage	INT
					DECLARE @ActiveTotalPage	INT
					DECLARE	@InActiveTotalPage	INT
					SELECT @AllTotalPage= COUNT(*) FROM BlogCategory
					SELECT @ActiveTotalPage= COUNT(*) FROM BlogCategory WHERE Status=1
					SELECT @InActiveTotalPage= COUNT(*) FROM BlogCategory WHERE Status=0

					SET @qryMain=@qryMain+'; SELECT '+CAST(@Total AS NVARCHAR(5))+' AS ''Total'','+CAST(@AllTotalPage AS NVARCHAR(5))+' AS ''AllTotalPage'','+CAST(@ActiveTotalPage AS NVARCHAR(5))+' AS ''ActiveTotalPage'','+CAST(@InActiveTotalPage AS NVARCHAR(5))+' AS ''InActiveTotalPage'''
					EXEC sp_executesql @qryMain 		
				END


				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetMediaById]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetMediaById]  

					@Id	INT

				AS            

				BEGIN

				  SELECT * FROM Media WHERE id=@Id

				END


				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetMediaDate]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetMediaDate]  
				AS            
				BEGIN
				  --SELECT CAST(YEAR(AddedOn) as VARCHAR(4))+'/'+CAST(MONTH(AddedOn) as VARCHAR(2)) AS 'Date' FROM Media GROUP BY YEAR(AddedOn),MONTH(AddedOn)
				  SELECT * FROM (SELECT DATENAME(MONTH, DATE) +' '+ DATENAME(YEAR,DATE) AS 'DateText',CAST(YEAR(DATE) as VARCHAR(4))+'/'+FORMAT(CAST(DATE AS DATE),'MM')  AS 'DateValue' FROM (SELECT CONVERT(VARCHAR(10), AddedOn, 111) AS 'DATE' FROM Media GROUP BY CONVERT(VARCHAR(10),AddedOn, 111))a)b GROUP BY DateValue,DateText
				END






				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetMediaWithPaging]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetMediaWithPaging]  --1,35,null,'all','all'
				@PageNo INT,
				@PageSize	INT,
				@Name VARCHAR(100),
				@FileType VARCHAR(10),
				@MediaDateSearch VARCHAR(10)
				AS  
				BEGIN
					DECLARE @qryMain  NVARCHAR(1000);
					DECLARE @qry  NVARCHAR(1000);   
					SET @qryMain='SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY AddedON DESC) AS ''RowNum'',* FROM Media WHERE 1=1 ' 
					SET @qry=''
					IF @Name IS NOT NULL
						SET @qry=@qry+' AND Name like''%'+@Name+'%'''
	
					IF @FileType='image'
						SET @qry=@qry+' AND ((Name like''%.png%'') OR (Name like''%.jpg%'') OR (Name like''%.jpeg%'') OR (Name like''%.gif%'') OR (Name like''%.bmp%''))'	  
					ELSE IF @FileType='audio'
						SET @qry=@qry+' AND ((Name like''%.mp3%'') OR (Name like''%.wav%'') OR (Name like''%.wma%''))'
					ELSE IF @FileType='video'
						SET @qry=@qry+' AND ((Name like''%.mp4%'') OR (Name like''%.wav%'') OR (Name like''%.wma%'') OR (Name like''%.3gp%'') OR (Name like''%.avi%'') OR (Name like''%.webm%'') OR (Name like''%.mpeg%'') OR (Name like''%.mpg%''))'	  
					ELSE IF @FileType='compress'
						SET @qry=@qry+' AND ((Name like''%.zip%'') OR (Name like''%.rar%''))'
					ELSE IF @FileType='text'
						SET @qry=@qry+' AND ((Name like''%.txt%''))'
					ELSE IF @FileType='word'
						SET @qry=@qry+' AND ((Name like''%.docx%''))'

					IF @MediaDateSearch!='all'
					BEGIN
						DECLARE @mediaMonth VARCHAR(2)
						DECLARE @mediaYear  VARCHAR(4)
						SELECT @mediaMonth=RIGHT(@MediaDateSearch,2),@mediaYear=LEFT(@MediaDateSearch,4)
						SET @qry=@qry+ ' AND MONTH(AddedOn) = '''+@mediaMonth+''' AND YEAR(AddedOn) = '''+@mediaYear+''''
					END 

					SET @qryMain=@qryMain+@qry+') a WHERE a.RowNum BETWEEN ('+CAST(@pageNo AS NVARCHAR(5))+'-1)*'+CAST(@pageSize AS NVARCHAR(5))+'+1 AND ('+CAST(@pageNo AS NVARCHAR(5))+'*'+CAST(@pageSize AS NVARCHAR(5))+')'
					SET @qryMain='SELECT t1.*,t2.url AS ''ThumbUrl'' FROM ('+@qryMain+')t1 LEFT OUTER JOIN (SELECT * FROM Media Where id in (SELECT MIN(id) FROM MEDIA WHERE Parentid IS NOT NULL GROUP BY ParentId))t2 ON t1.id=t2.ParentId'
					EXEC sp_executesql @qryMain
					--SELECT * FROM Media Where id in (SELECT MIN(id) as 'b' FROM MEDIA WHERE Parentid IS NOT NULL GROUP BY ParentId) Order by id desc
					--SELECT t1.*,t2.id,t2.url FROM (SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY AddedON DESC) AS 'RowNum',* FROM Media WHERE 1=1 AND ParentId IS NULL) a WHERE a.RowNum BETWEEN (1-1)*35+1 AND (1*35))t1 LEFT OUTER JOIN (SELECT * FROM Media Where id in (SELECT MIN(id) as 'b' FROM MEDIA WHERE Parentid IS NOT NULL GROUP BY ParentId))t2 ON t1.id=t2.ParentId
					--print @qryMain
				END


				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetPageById]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetPageById]  
					@Id	INT
				AS            
				BEGIN
				  SELECT * FROM Page WHERE id=@Id
				END


				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetPageWithPaging]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetPageWithPaging] --1,'yogyogi',1,null
				@PageNo INT,
				@Name VARCHAR(100),
				@PageSize	INT,
				@Status	INT
				AS  
				BEGIN
					DECLARE @qryMain  NVARCHAR(1000);
					DECLARE @qry  NVARCHAR(1000);   
					SET @qryMain='SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY AddedON DESC) AS ''RowNum'',* FROM Page WHERE 1=1' 
					SET @qry=''

					IF @Status IS NOT NULL
						SET @qry=@qry+' AND Status='''+CAST(@Status AS NVARCHAR(5))+''''     
					IF @Name IS NOT NULL
						SET @qry=@qry+' AND Name like''%'+@Name+'%'''  
		   
					SET @qryMain=@qryMain+@qry+') a WHERE a.RowNum BETWEEN ('+CAST(@pageNo AS NVARCHAR(5))+'-1)*'+CAST(@pageSize AS NVARCHAR(5))+'+1 AND ('+CAST(@pageNo AS NVARCHAR(5))+'*'+CAST(@pageSize AS NVARCHAR(5))+')'

					DECLARE @qryTotal  NVARCHAR(1000); 
					DECLARE @Total INT;
					SET @qryTotal='' 
					SET @qryTotal='SELECT @Total=COUNT(*) FROM Page WHERE 1=1'+@qry
					EXEC Sp_executesql @qryTotal, N'@Total INT OUTPUT', @Total OUTPUT

					DECLARE	@AllTotalPage	INT
					DECLARE @ActiveTotalPage	INT
					DECLARE	@InActiveTotalPage	INT
					SELECT @AllTotalPage= COUNT(*) FROM Page
					SELECT @ActiveTotalPage= COUNT(*) FROM Page WHERE Status=1
					SELECT @InActiveTotalPage= COUNT(*) FROM Page WHERE Status=0

					SET @qryMain=@qryMain+'; SELECT '+CAST(@Total AS NVARCHAR(5))+' AS ''Total'','+CAST(@AllTotalPage AS NVARCHAR(5))+' AS ''AllTotalPage'','+CAST(@ActiveTotalPage AS NVARCHAR(5))+' AS ''ActiveTotalPage'','+CAST(@InActiveTotalPage AS NVARCHAR(5))+' AS ''InActiveTotalPage'''
					EXEC sp_executesql @qryMain 		
				END


				GO
				/****** Object:  StoredProcedure [dbo].[sp_GetURL]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_GetURL] --'my-pipin-1','-',null
					@Url		VARCHAR(100),
					@Sep		VARCHAR(1),
					@TableName	VARCHAR(25),
					@Id			INT,   
					@TempUrl	VARCHAR(100) OUTPUT   
				AS            
				BEGIN
					DECLARE @LastUrlPart VARCHAR(5), @FirstUrlPart VARCHAR(100), @Counter INT = 0, @UrlWithoutEndNo VARCHAR(100)
					SELECT @LastUrlPart=REVERSE(SUBSTRING(REVERSE(@Url),0,CHARINDEX(@sep,REVERSE(@Url))))
					SELECT @FirstUrlPart=LEFT(@Url,LEN(@Url)-CHARINDEX('-',REVERSE(@Url)))

					create table #Temp
					(
						URL VARCHAR(100) 
					)
	
					IF(ISNUMERIC(@LastUrlPart) = 1)
					  BEGIN
						SET @UrlWithoutEndNo=@FirstUrlPart
					  END
					ELSE
					  BEGIN
						SET @UrlWithoutEndNo=@Url
					  END 
    
					IF(@TableName is null)
					  BEGIN 
						INSERT INTO #Temp SELECT Url FROM Page WHERE Url LIKE @UrlWithoutEndNo+'%'
     					INSERT INTO #Temp SELECT Url FROM Blog WHERE Url LIKE @UrlWithoutEndNo+'%'
						INSERT INTO #Temp SELECT Url FROM BlogCategory WHERE Url LIKE @UrlWithoutEndNo+'%'
					  END
					ELSE IF(@TableName='Page')
					  BEGIN
						INSERT INTO #Temp SELECT Url FROM Page WHERE Url LIKE @UrlWithoutEndNo+'%' AND Id<>@Id
     					INSERT INTO #Temp SELECT Url FROM Blog WHERE Url LIKE @UrlWithoutEndNo+'%'
						INSERT INTO #Temp SELECT Url FROM BlogCategory WHERE Url LIKE @UrlWithoutEndNo+'%'
					  END
					ELSE IF(@TableName='Blog')
					  BEGIN
						INSERT INTO #Temp SELECT Url FROM Page WHERE Url LIKE @UrlWithoutEndNo+'%' 
     					INSERT INTO #Temp SELECT Url FROM Blog WHERE Url LIKE @UrlWithoutEndNo+'%' AND Id<>@Id
						INSERT INTO #Temp SELECT Url FROM BlogCategory WHERE Url LIKE @UrlWithoutEndNo+'%'
					  END
					ELSE IF(@TableName='BlogCategory')
					  BEGIN
						INSERT INTO #Temp SELECT Url FROM Page WHERE Url LIKE @UrlWithoutEndNo+'%'
     					INSERT INTO #Temp SELECT Url FROM Blog WHERE Url LIKE @UrlWithoutEndNo+'%'
						INSERT INTO #Temp SELECT Url FROM BlogCategory WHERE Url LIKE @UrlWithoutEndNo+'%' AND Id<>@Id
					  END

					SET @TempUrl=@Url
					WHILE EXISTS (SELECT * FROM #Temp WHERE url=@TempUrl)
					  BEGIN
						SET @Counter=@Counter+1
						SET @TempUrl=@UrlWithoutEndNo+'-'+CAST(@Counter AS VARCHAR(100))
					  END
	
					print @tempurl
				END	

				--[sp_GetURL] 'my-pipin-6','-',null

				GO
				/****** Object:  StoredProcedure [dbo].[sp_InsertMedia]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_InsertMedia]   
					@Name	VARCHAR(100), 
					@Url	VARCHAR(250),   
					@Title varchar(100),
					@Alt varchar(100),
					@Description varchar(100),
					@ParentId INT, 
					@Result	VARCHAR(50) OUTPUT,
					@CreatedFileName	VARCHAR(100) OUTPUT,   
					@CreatedId	INT OUTPUT      
				AS            
				BEGIN
					DECLARE @MainFileNameWithoutExtension VARCHAR(100)
					DECLARE @FileNameWithExtension VARCHAR(100)
					DECLARE @FileNameWithoutExtension VARCHAR(100)
					DECLARE @FileExtension VARCHAR(100)
					DECLARE @TempUrl	VARCHAR(250)
					SELECT @TempUrl=@Url
					DECLARE @Counter INT = 0;

					SET @MainFileNameWithoutExtension=LEFT(@Name,CHARINDEX('.',@Name)-1)
					SET @CreatedFileName=@Name
					WHILE EXISTS (SELECT * FROM Media WHERE url=@TempUrl)
					BEGIN
						SET @Counter=@Counter+1
						SELECT @FileNameWithExtension=REVERSE(SUBSTRING(REVERSE(@TempUrl),0,CHARINDEX('/',REVERSE(@TempUrl))))
						SELECT @FileNameWithoutExtension=LEFT(@FileNameWithExtension,CHARINDEX('.',@FileNameWithExtension)-1)
						SELECT @FileExtension=Right(@FileNameWithExtension,LEN(@FileNameWithExtension)-CHARINDEX('.',@FileNameWithExtension))
						SET @CreatedFileName=(@MainFileNameWithoutExtension+CAST(@Counter AS VARCHAR(100))+'.'+@FileExtension)
						SET @TempUrl=REPLACE(@TempUrl,@FileNameWithExtension,@CreatedFileName)
					END

					INSERT INTO Media(Name,Url,Title,Alt,Description,ParentId) VALUES (@Name,@TempUrl,@Title,@Alt,@Description,@ParentId) 
					SET @Result='Success'	
					set @CreatedId=@@IDENTITY
				END	

				--  DECLARE @MainFileNameWithoutExtension VARCHAR(100)
				--  DECLARE @FileNameWithExtension VARCHAR(100)
				--	DECLARE @FileNameWithoutExtension VARCHAR(100)
				--	DECLARE @FileExtension VARCHAR(100)
				--	DECLARE @TempUrl	VARCHAR(250)
				--	SELECT @TempUrl='Upload/2015/7/1small.png'
				--	DECLARE @Counter INT = 0;

				--	SET @MainFileNameWithoutExtension=LEFT('1small.png',CHARINDEX('.','1small.png')-1)

				--	WHILE EXISTS (SELECT * FROM Media WHERE url=@TempUrl)
				--	BEGIN
				--	    SET @Counter=@Counter+1
				--		SELECT @FileNameWithExtension=REVERSE(SUBSTRING(REVERSE(@TempUrl),0,CHARINDEX('/',REVERSE(@TempUrl))))
				--		SELECT @FileNameWithoutExtension=LEFT(@FileNameWithExtension,CHARINDEX('.',@FileNameWithExtension)-1)
				--		SELECT @FileExtension=Right(@FileNameWithExtension,LEN(@FileNameWithExtension)-CHARINDEX('.',@FileNameWithExtension))
				--		SET @TempUrl=REPLACE(@TempUrl,@FileNameWithExtension,@MainFileNameWithoutExtension+CAST(@Counter AS VARCHAR(100))+'.'+@FileExtension)
				--	END
				--PRINT @TempUrl

				--PRINT @TempUrl
				--PRINT @TempUrl
				--PRINT @TempUrl

				GO
				/****** Object:  StoredProcedure [dbo].[sp_InsertPage]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_InsertPage]   
					@Name	VARCHAR(100), 
					@Url	VARCHAR(100),   
					@MetaTitle varchar(250),
					@MetaKeyword varchar(250),
					@MetaDescription varchar(250),
					@Description varchar(max),            
					@Status bit, 
					@Result	VARCHAR(50) OUTPUT,
					@CreatedId	INT OUTPUT      
				AS            
				BEGIN
					IF NOT EXISTS(SELECT * FROM Page WHERE Name=@Name)
					BEGIN
						--Finding URL--
						DECLARE @Query				NVARCHAR(250)
						DECLARE @ParmDefinition		NVARCHAR(250)
						DECLARE @TempUrl			VARCHAR(100)

						Set @Query = 'Exec sp_GetURL @Url, @Sep, @TableName, @Id, @TempUrl OUTPUT'
						SET @ParmDefinition = '@Url VARCHAR(100), @Sep VARCHAR(1), @TableName VARCHAR(25), @Id INT, @TempUrl VARCHAR(100) OUTPUT'
						EXEC sp_executesql @query,   
							 @ParmDefinition,
							 @Url=@Url,
							 @Sep='-',
							 @TableName=null,
							 @Id=null, 
							 @TempUrl=@TempUrl OUTPUT
						--END--

						INSERT INTO Page(Name,Url,MetaTitle,MetaKeyword,MetaDescription,Description,Status) VALUES (@Name,@TempUrl,@MetaTitle,@MetaKeyword,@MetaDescription,@Description,@Status) 
						SET @Result='Insert Successful'	
						set @CreatedId=@@IDENTITY
					END
					ELSE
						SET @Result='Already Present'
					END	

				GO
				/****** Object:  StoredProcedure [dbo].[sp_LoginUser]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_LoginUser]  
					@UserName	Varchar(50),
					@Password	Varchar(50),
					@Result		Varchar(50) OUTPUT
				AS            
				BEGIN
				  IF EXISTS(SELECT * FROM [User] WHERE UserName=@UserName AND Password=@Password AND Status=1)
					SET @Result='Success'
				  ELSE
					SET @Result='Failed'
				END
 

				GO
				/****** Object:  StoredProcedure [dbo].[sp_UpdateBulkBlogCategoryStatus]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				Create PROCEDURE [dbo].[sp_UpdateBulkBlogCategoryStatus] -- '24,25',1,null 
					@Id	Varchar(50),
					@Status bit,            
					@Result	VARCHAR(50) OUTPUT    
				AS            
				BEGIN
					--UPDATE Page SET Status=@Status WHERE Id in (select s from dbo.split(',',@Id)) 	
					--SET @Result='Success'	
					BEGIN TRY
						DECLARE @dynamicUpdateQuery nvarchar(max)
						SET @dynamicUpdateQuery='UPDATE BlogCategory SET Status='+CAST(@Status AS Varchar(1))+ ' WHERE Id in (' +@id + ')';
						EXECUTE sp_executesql @dynamicUpdateQuery
						SELECT @Result='Success'
					END TRY
					BEGIN CATCH
						SELECT @Result=ERROR_MESSAGE()
					END CATCH
				END

				GO
				/****** Object:  StoredProcedure [dbo].[sp_UpdateBulkPageStatus]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_UpdateBulkPageStatus] -- '24,25',1,null 
					@Id	Varchar(50),
					@Status bit,            
					@Result	VARCHAR(50) OUTPUT    
				AS            
				BEGIN
					--UPDATE Page SET Status=@Status WHERE Id in (select s from dbo.split(',',@Id)) 	
					--SET @Result='Success'	
					BEGIN TRY
						DECLARE @dynamicUpdateQuery nvarchar(max)
						SET @dynamicUpdateQuery='UPDATE Page SET Status='+CAST(@Status AS Varchar(1))+ ' WHERE Id in (' +@id + ')';
						EXECUTE sp_executesql @dynamicUpdateQuery
						SELECT @Result='Success'
					END TRY
					BEGIN CATCH
						SELECT @Result=ERROR_MESSAGE()
					END CATCH
				END

				GO
				/****** Object:  StoredProcedure [dbo].[sp_UpdateMedia]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_UpdateMedia]   
					@Id	INT,
					@Name	VARCHAR(100),   
					@Url	VARCHAR(250),   
					@Title varchar(100),
					@Alt varchar(100),
					@Description varchar(100),           
					@Result	VARCHAR(50) OUTPUT    
				AS            
				BEGIN
					UPDATE Media SET Name=@Name,Url=@Url,Title=@Title,Alt=@Alt,Description=@Description WHERE Id=@Id 	
					SET @Result='Update Successful'	
				END


				GO
				/****** Object:  StoredProcedure [dbo].[sp_UpdatePage]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE [dbo].[sp_UpdatePage]   
					@Id	INT,
					@Name	VARCHAR(100),   
					@Url	VARCHAR(100),   
					@MetaTitle varchar(250),
					@MetaKeyword varchar(250),
					@MetaDescription varchar(250),
					@Description varchar(max),            
					@Status bit,            
					@Result	VARCHAR(50) OUTPUT,
					@CreatedUrl VARCHAR(100) OUTPUT    
				AS            
				BEGIN
					IF NOT EXISTS(SELECT * FROM Page WHERE Name=@Name AND Id<>@Id)
					BEGIN

						--Finding URL--
						DECLARE @Query				NVARCHAR(250)
						DECLARE @ParmDefinition		NVARCHAR(250)
						DECLARE @TempUrl			VARCHAR(100)

						Set @Query = 'Exec sp_GetURL @Url, @Sep, @TableName, @Id, @TempUrl OUTPUT'
						SET @ParmDefinition = '@Url VARCHAR(100), @Sep VARCHAR(1), @TableName VARCHAR(25), @Id INT, @TempUrl VARCHAR(100) OUTPUT'
						EXEC sp_executesql @query,   
							 @ParmDefinition,
							 @Url=@Url,
							 @Sep='-',
							 @TableName='Page',
							 @Id=@Id, 
							 @TempUrl=@TempUrl OUTPUT
						--END--

						UPDATE Page SET Name=@Name,Url=@TempUrl,MetaTitle=@MetaTitle,MetaKeyword=@MetaKeyword,MetaDescription=@MetaDescription,Description=@Description,Status=@Status WHERE Id=@Id 	
						SET @Result='Update Successful'
						SET @CreatedUrl=@TempUrl	
					END
					ELSE
						SET @Result='Already Present'
					END


				GO
				/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 01-16-2019 17:59:42 ******/
				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE FUNCTION [dbo].[Split] (@sep char(1), @s varchar(8000))
				RETURNS table
				AS
				RETURN (
					WITH Pieces(pn, start, stop) AS (
					  SELECT 1, 1, CHARINDEX(@sep, @s)
					  UNION ALL
					  SELECT pn + 1, stop + 1, CHARINDEX(@sep, @s, stop + 1)
					  FROM Pieces
					  WHERE stop > 0
					)
					SELECT 
					  SUBSTRING(@s, start, CASE WHEN stop > 0 THEN stop-start ELSE 512 END) AS s
					FROM Pieces
				  )

				GO
				";
            migrationBuilder.Sql(sp);
        }
    }
}
