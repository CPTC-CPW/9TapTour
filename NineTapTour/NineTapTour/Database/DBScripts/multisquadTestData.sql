/* 
This is an insert script with 10 participants in multiple squads for 4 tournaments for testing bonus pins, cash payouts, and correct averages
This is designed to replace the randomized seed data when building a new database.
protected override void Seed(NineTapTour.Database.NineTapDb context) in configuration.cs in the migrations folder
needs to be commented out,
delete the database,
update database,
run this script
*/
USE [NineTapTour.NineTapDb]
GO
SET IDENTITY_INSERT [dbo].[Games] ON 
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (1, 103, 80, 95, 115, 125, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (2, 134, 114, 200, 147, 198, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), 9, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (3, 142, 98, 165, 231, 147, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (4, 141, 80, 115, 156, 195, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (5, 145, 145, 78, 159, 200, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 5, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (6, 136, 89, 155, 123, 142, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (7, 136, 123, 156, 189, 73, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (8, 158, 213, 156, 98, 165, 1, 1, 1, 1, N'', 13, 0, CAST(0.00 AS Decimal(18, 2)), 10, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (9, 146, 122, 146, 99, 165, 1, 1, 1, 1, N'', 13, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (10, 165, 155, 200, 219, 89, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 3, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (11, 219, 235, 255, 198, 189, 1, 1, 1, 1, N'', -27, 0, CAST(0.00 AS Decimal(18, 2)), 6, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (12, 160, 200, 187, 156, 132, 1, 1, 1, 1, N'', 19, 0, CAST(0.00 AS Decimal(18, 2)), 8, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (13, 152, 155, 165, 200, 89, 1, 1, 1, 1, N'', 19, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (14, 173, 215, 200, 230, 50, 1, 1, 1, 1, N'', 70, 0, CAST(35.00 AS Decimal(18, 2)), 1, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (15, 136, 68, 89, 121, 125, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (16, 192, 200, 204, 189, 178, 1, 1, 1, 1, N'', -1, 0, CAST(0.00 AS Decimal(18, 2)), 7, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (17, 145, 125, 156, 121, 178, 1, 1, 1, 1, N'', 67, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (18, 158, 198, 156, 184, 144, 1, 1, 1, 1, N'', 67, 0, CAST(15.00 AS Decimal(18, 2)), 2, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (19, 143, 200, 215, 111, 78, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 4, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (20, 135, 145, 189, 121, 85, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (21, 147, 155, 165, 175, 185, 1, 1, 1, 1, N'', 70, 0, CAST(7.50 AS Decimal(18, 2)), 2, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (22, 144, 145, 135, 125, 115, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (23, 144, 185, 175, 165, 155, 1, 1, 1, 1, N'', 70, 0, CAST(7.50 AS Decimal(18, 2)), 2, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (24, 140, 110, 120, 130, 140, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (25, 134, 90, 100, 110, 120, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (26, 138, 115, 120, 125, 130, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 8, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (27, 158, 200, 190, 180, 170, 1, 1, 1, 1, N'', 53, 0, CAST(0.00 AS Decimal(18, 2)), 4, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (28, 145, 110, 120, 130, 140, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (29, 217, 200, 210, 220, 230, 1, 1, 1, 1, N'', 4, 0, CAST(0.00 AS Decimal(18, 2)), 6, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (30, 139, 85, 95, 102, 110, 1, 1, 1, 1, N'', 57, 0, CAST(0.00 AS Decimal(18, 2)), 9, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (31, 137, 115, 125, 135, 145, 1, 1, 1, 1, N'', 19, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (32, 149, 160, 170, 180, 190, 1, 1, 1, 1, N'', 70, 0, CAST(35.00 AS Decimal(18, 2)), 1, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (33, 143, 110, 120, 130, 140, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (34, 144, 130, 140, 150, 160, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (35, 178, 185, 195, 205, 215, 1, 1, 1, 1, N'', 28, 0, CAST(0.00 AS Decimal(18, 2)), 5, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (36, 174, 140, 150, 160, 170, 1, 1, 1, 1, N'', -1, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (37, 176, 165, 175, 185, 195, 1, 1, 1, 1, N'', -1, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (38, 173, 150, 160, 170, 180, 1, 1, 1, 1, N'', -1, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (39, 141, 123, 133, 143, 153, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 7, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (40, 140, 100, 110, 120, 130, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 7, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (41, 140, 130, 140, 150, 160, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (42, 139, 150, 160, 170, 180, 1, 1, 1, 1, N'', 70, 0, CAST(35.00 AS Decimal(18, 2)), 1, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (43, 139, 110, 150, 0, 165, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (44, 138, 123, 134, 145, 156, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 5, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (45, 157, 134, 165, 189, 178, 1, 1, 1, 1, N'', 57, 0, CAST(0.00 AS Decimal(18, 2)), 3, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (46, 150, 110, 126, 114, 89, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (47, 156, 145, 156, 180, 123, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (48, 155, 145, 156, 0, 0, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (49, 199, 100, 200, 168, 189, 1, 1, 1, 1, N'', 4, 0, CAST(0.00 AS Decimal(18, 2)), 9, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (50, 139, 112, 156, 134, 187, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 4, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (51, 147, 115, 168, 189, 178, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 2, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (52, 175, 145, 168, 178, 145, 1, 1, 1, 1, N'', 40, 0, CAST(0.00 AS Decimal(18, 2)), 6, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (53, 153, 123, 145, 0, 167, 1, 1, 1, 1, N'', 59, 0, CAST(0.00 AS Decimal(18, 2)), 10, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (54, 135, 100, 110, 120, 130, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 7, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (55, 139, 112, 113, 156, 145, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 3, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (56, 140, 142, 146, 132, 178, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (57, 141, 142, 156, 178, 132, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (58, 140, 89, 145, 0, 123, 1, 1, 1, 1, N'', 18, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (59, 136, 110, 120, 130, 140, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 7, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (60, 137, 100, 98, 156, 143, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (61, 137, 123, 145, 156, 100, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 4, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (62, 154, 214, 200, 195, 100, 1, 1, 1, 1, N'', 70, 0, CAST(35.00 AS Decimal(18, 2)), 1, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (63, 151, 123, 145, 0, 0, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (64, 188, 123, 145, 156, 198, 1, 1, 1, 1, N'', 19, 0, CAST(0.00 AS Decimal(18, 2)), 10, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (65, 137, 110, 120, 130, 140, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 7, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (66, 145, 120, 130, 140, 150, 1, 1, 1, 1, N'', 66, 0, CAST(0.00 AS Decimal(18, 2)), 4, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (67, 171, 130, 140, 150, 160, 1, 1, 1, 1, N'', 41, 0, CAST(0.00 AS Decimal(18, 2)), 9, 1, 0, NULL)
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (68, 151, 142, 153, 123, 165, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 2, 1, 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Games] ([Id], [InputtedAvg], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [Notes], [Handicap], [Bonus], [MoneyWon], [PlaceStanding], [gameRegionID], [IsComp], [SidePot]) VALUES (69, 133, 135, 126, 148, 98, 1, 1, 1, 1, N'', 70, 0, CAST(0.00 AS Decimal(18, 2)), 6, 1, 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[Games] OFF
GO
SET IDENTITY_INSERT [dbo].[NineTapRegions] ON 
GO
INSERT [dbo].[NineTapRegions] ([NineTapRegionID], [NineTapRegionName]) VALUES (1, N'Washington')
GO
SET IDENTITY_INSERT [dbo].[NineTapRegions] OFF
GO
SET IDENTITY_INSERT [dbo].[Members] ON 
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (1, 1, 1, N'Jimothy', N'Bowler', N'', CAST(N'1900-06-06T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 200, 70, 0, CAST(N'1990-01-02T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:01:47.090' AS DateTime), NULL, 0, N'', 0, 1, CAST(7.50 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (2, 2, 1, N'Jane', N'Smith', N'', CAST(N'2000-02-03T00:00:00.000' AS DateTime), N'   -  -    ', 0, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 100, 70, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:04:08.363' AS DateTime), NULL, 0, N'', 0, 0, CAST(42.50 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (3, 3, 1, N'Steve', N'Hopper', N'', CAST(N'1978-03-04T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 205, 70, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:06:10.950' AS DateTime), NULL, 0, N'', 0, 0, CAST(0.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (4, 4, 1, N'Sarah', N'Person', N'', CAST(N'1980-09-12T00:00:00.000' AS DateTime), N'   -  -    ', 0, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 98, 70, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:11:11.680' AS DateTime), NULL, 0, N'', 0, 0, CAST(0.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (5, 5, 1, N'Victor', N'Hugo', N'', CAST(N'1945-10-15T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 250, 29, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:11:27.520' AS DateTime), NULL, 0, N'', 0, 1, CAST(0.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (6, 6, 1, N'Leslie', N'Kelp', N'', CAST(N'1930-05-17T00:00:00.000' AS DateTime), N'   -  -    ', 0, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 198, 70, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:15:58.750' AS DateTime), NULL, 0, N'', 0, 1, CAST(0.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (7, 7, 1, N'Ronald', N'Garp', N'', CAST(N'1996-12-01T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 123, 68, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:16:39.053' AS DateTime), NULL, 0, N'', 0, 0, CAST(70.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (8, 8, 1, N'Emily', N'Potash', N'', CAST(N'1956-03-04T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 222, 45, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:16:48.903' AS DateTime), NULL, 0, N'', 0, 1, CAST(0.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (9, 9, 1, N'Ben', N'Harper', N'', CAST(N'1988-05-06T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 145, 70, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:16:59.977' AS DateTime), NULL, 0, N'', 0, 0, CAST(15.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Members] ([Id], [Number], [IsActive], [FirstName], [LastName], [MiddleInitial], [DateOfBirth], [SSN], [Gender], [Street], [City], [State], [PostalCode], [Email], [PrimaryPhone], [SecondaryPhone], [Average], [StartAvg], [Handicap], [Bonus], [JoinDate], [RejoinDate], [LastBowled], [LastPayment], [IsLifetimeMember], [Notes], [Referrals], [IsSenior], [MoneyEarned], [NineTapRegionID]) VALUES (10, 10, 1, N'Been', N'Jelly', N'', CAST(N'1999-01-01T00:00:00.000' AS DateTime), N'   -  -    ', 1, N'', N'', N'', N'', N'', N'(   )    -', N'(   )    -', 0, 111, 70, 0, CAST(N'2019-08-14T00:00:00.000' AS DateTime), NULL, CAST(N'2019-08-21T15:17:13.653' AS DateTime), NULL, 0, N'', 0, 0, CAST(0.00 AS Decimal(18, 2)), 1)
GO
SET IDENTITY_INSERT [dbo].[Members] OFF
GO
SET IDENTITY_INSERT [dbo].[Tournaments] ON 
GO
INSERT [dbo].[Tournaments] ([Id], [Date], [Location], [Event], [Notes], [Sponsors], [Squads], [Doubles], [ThreeOutOf4], [TourneyRegion]) VALUES (1, CAST(N'2019-08-12T00:00:00.000' AS DateTime), N'Dudley 1', N'', N'', N'', 4, 0, 0, 1)
GO
INSERT [dbo].[Tournaments] ([Id], [Date], [Location], [Event], [Notes], [Sponsors], [Squads], [Doubles], [ThreeOutOf4], [TourneyRegion]) VALUES (2, CAST(N'2019-08-20T00:00:00.000' AS DateTime), N'Dudley 2', N'', N'', N'', 4, 0, 0, 1)
GO
INSERT [dbo].[Tournaments] ([Id], [Date], [Location], [Event], [Notes], [Sponsors], [Squads], [Doubles], [ThreeOutOf4], [TourneyRegion]) VALUES (3, CAST(N'2019-08-21T00:00:00.000' AS DateTime), N'dudley 3', N'', N'', N'', 4, 0, 0, 1)
GO
INSERT [dbo].[Tournaments] ([Id], [Date], [Location], [Event], [Notes], [Sponsors], [Squads], [Doubles], [ThreeOutOf4], [TourneyRegion]) VALUES (4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), N'dudley 4', N'', N'', N'', 4, 0, 0, 1)
GO
SET IDENTITY_INSERT [dbo].[Tournaments] OFF
GO
SET IDENTITY_INSERT [dbo].[Participants] ON 
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (1, 1, 1, 1, 1, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (2, 2, 1, 2, 1, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (3, 3, 1, 3, 1, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (4, 4, 1, 4, 1, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (5, 1, 1, 5, 2, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (6, 2, 1, 6, 2, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (7, 3, 1, 7, 2, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (8, 3, 1, 8, 3, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (9, 4, 1, 9, 3, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (10, 4, 1, 10, 4, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (11, 4, 1, 11, 5, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (12, 4, 1, 12, 6, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (13, 1, 1, 13, 6, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (14, 1, 1, 14, 7, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (15, 2, 1, 15, 7, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (16, 2, 1, 16, 8, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (17, 2, 1, 17, 9, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (18, 3, 1, 18, 9, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (19, 3, 1, 19, 10, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (20, 2, 1, 20, 10, 1)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (21, 1, 1, 21, 1, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (22, 2, 1, 22, 1, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (23, 2, 1, 23, 2, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (24, 3, 1, 24, 2, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (25, 4, 1, 25, 2, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (26, 4, 1, 26, 3, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (27, 4, 1, 27, 4, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (28, 1, 1, 28, 4, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (29, 1, 1, 29, 5, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (30, 1, 1, 30, 6, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (31, 2, 1, 31, 6, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (32, 2, 1, 32, 7, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (33, 3, 1, 33, 7, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (34, 4, 1, 34, 7, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (35, 4, 1, 35, 8, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (36, 1, 1, 36, 8, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (37, 2, 1, 37, 8, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (38, 3, 1, 38, 8, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (39, 3, 1, 39, 10, 2)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (40, 1, 1, 40, 1, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (41, 2, 1, 41, 1, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (42, 2, 1, 42, 2, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (43, 3, 1, 43, 2, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (44, 3, 1, 44, 3, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (45, 3, 1, 45, 4, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (46, 4, 1, 46, 4, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (47, 1, 1, 47, 4, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (48, 2, 1, 48, 4, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (49, 2, 1, 49, 5, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (50, 2, 1, 50, 6, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (51, 2, 1, 51, 7, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (52, 2, 1, 52, 8, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (53, 2, 1, 53, 9, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (54, 2, 1, 54, 10, 3)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (55, 1, 1, 55, 1, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (56, 2, 1, 56, 1, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (57, 3, 1, 57, 1, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (58, 4, 1, 58, 1, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (59, 4, 1, 59, 2, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (60, 1, 1, 60, 2, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (61, 1, 1, 61, 3, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (62, 1, 1, 62, 4, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (63, 2, 1, 63, 4, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (64, 1, 1, 64, 5, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (65, 1, 1, 65, 6, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (66, 1, 1, 66, 7, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (67, 1, 1, 67, 8, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (68, 1, 1, 68, 9, 4)
GO
INSERT [dbo].[Participants] ([Id], [SquadNumber], [ParticipantRegionID], [Game_Id], [Member_Id], [Tournament_Id]) VALUES (69, 1, 1, 69, 10, 4)
GO
SET IDENTITY_INSERT [dbo].[Participants] OFF
GO
SET IDENTITY_INSERT [dbo].[FinalizeTemps] ON 
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (1, 1, 14, 7, 7, N'Ronald', N'Garp', 1, 215, 200, 230, 50, 1, 1, 1, 1, 165, 173, NULL, 695, 0, 173, 70, 0, 975, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (2, 1, 7, 2, 2, N'Jane', N'Smith', 3, 123, 156, 189, 73, 1, 1, 1, 1, 136, 136, NULL, 541, 0, 135, 70, 0, 821, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (3, 1, 9, 3, 3, N'Steve', N'Hopper', 4, 122, 146, 99, 165, 1, 1, 1, 1, 146, 146, NULL, 532, 0, 133, 13, 0, 584, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (4, 1, 1, 1, 1, N'Jimothy', N'Bowler', 1, 80, 95, 115, 125, 1, 1, 1, 1, 103, 103, NULL, 415, 0, 103, 18, 0, 487, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (5, 1, 12, 6, 6, N'Leslie', N'Kelp', 4, 200, 187, 156, 132, 1, 1, 1, 1, 160, 160, NULL, 675, 0, 168, 19, 0, 751, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (6, 1, 5, 2, 2, N'Jane', N'Smith', 1, 145, 78, 159, 200, 1, 1, 1, 1, 145, 145, NULL, 582, 0, 145, 70, 0, 862, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (7, 1, 11, 5, 5, N'Victor', N'Hugo', 4, 235, 255, 198, 189, 1, 1, 1, 1, 219, 219, NULL, 877, 0, 219, -27, 0, 769, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (8, 1, 16, 8, 8, N'Emily', N'Potash', 2, 200, 204, 189, 178, 1, 1, 1, 1, 192, 192, NULL, 771, 0, 192, -1, 0, 767, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (9, 1, 17, 9, 9, N'Ben', N'Harper', 2, 125, 156, 121, 178, 1, 1, 1, 1, 145, 145, NULL, 580, 0, 145, 67, 0, 848, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (10, 1, 15, 7, 7, N'Ronald', N'Garp', 2, 68, 89, 121, 125, 1, 1, 1, 1, 132, 136, NULL, 403, 0, 100, 70, 0, 683, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (11, 1, 3, 1, 1, N'Jimothy', N'Bowler', 3, 98, 165, 231, 147, 1, 1, 1, 1, 142, 142, NULL, 641, 0, 160, 18, 0, 713, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (12, 1, 10, 4, 4, N'Sarah', N'Person', 4, 155, 200, 219, 89, 1, 1, 1, 1, 165, 165, NULL, 663, 0, 165, 70, 0, 943, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (13, 1, 8, 3, 3, N'Steve', N'Hopper', 3, 213, 156, 98, 165, 1, 1, 1, 1, 158, 158, NULL, 632, 0, 158, 13, 0, 684, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (14, 1, 2, 1, 1, N'Jimothy', N'Bowler', 2, 114, 200, 147, 198, 1, 1, 1, 1, 134, 134, NULL, 659, 0, 164, 18, 0, 731, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (15, 1, 6, 2, 2, N'Jane', N'Smith', 2, 89, 155, 123, 142, 1, 1, 1, 1, 136, 136, NULL, 509, 0, 127, 70, 0, 789, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (16, 1, 18, 9, 9, N'Ben', N'Harper', 3, 198, 156, 184, 144, 1, 1, 1, 1, 158, 158, NULL, 682, 0, 170, 67, 0, 950, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (17, 1, 13, 6, 6, N'Leslie', N'Kelp', 1, 155, 165, 200, 89, 1, 1, 1, 1, 152, 152, NULL, 609, 0, 152, 19, 0, 685, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (18, 1, 19, 10, 10, N'Been', N'Jelly', 3, 200, 215, 111, 78, 1, 1, 1, 1, 143, 143, NULL, 604, 0, 151, 70, 0, 884, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (19, 1, 4, 1, 1, N'Jimothy', N'Bowler', 4, 80, 115, 156, 195, 1, 1, 1, 1, 141, 141, NULL, 546, 0, 136, 18, 0, 618, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (20, 1, 20, 10, 10, N'Been', N'Jelly', 2, 145, 189, 121, 85, 1, 1, 1, 1, 135, 135, NULL, 540, 0, 135, 70, 0, 820, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (21, 2, 32, 7, 7, N'Ronald', N'Garp', 2, 160, 170, 180, 190, 1, 1, 1, 1, 149, 149, N'', 700, 0, 175, 70, 0, 980, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (22, 2, 24, 2, 2, N'Jane', N'Smith', 3, 110, 120, 130, 140, 1, 1, 1, 1, 140, 140, N'', 500, 0, 125, 70, 0, 780, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (23, 2, 29, 5, 5, N'Victor', N'Hugo', 1, 200, 210, 220, 230, 1, 1, 1, 1, 217, 217, N'', 860, 0, 215, 4, 0, 876, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (24, 2, 27, 4, 4, N'Sarah', N'Person', 4, 200, 190, 180, 170, 1, 1, 1, 1, 158, 158, N'', 740, 0, 185, 53, 0, 952, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (25, 2, 35, 8, 8, N'Emily', N'Potash', 4, 185, 195, 205, 215, 1, 1, 1, 1, 178, 178, N'', 800, 0, 200, 28, 0, 912, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (26, 2, 25, 2, 2, N'Jane', N'Smith', 4, 90, 100, 110, 120, 1, 1, 1, 1, 134, 134, N'', 420, 0, 105, 70, 0, 700, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (27, 2, 34, 7, 7, N'Ronald', N'Garp', 4, 130, 140, 150, 160, 1, 1, 1, 1, 144, 144, N'', 580, 0, 145, 70, 0, 860, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (28, 2, 33, 7, 7, N'Ronald', N'Garp', 3, 110, 120, 130, 140, 1, 1, 1, 1, 143, 143, N'', 500, 0, 125, 70, 0, 780, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (29, 2, 22, 1, 1, N'Jimothy', N'Bowler', 2, 145, 135, 125, 115, 1, 1, 1, 1, 144, 144, N'', 520, 0, 130, 18, 0, 592, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (30, 2, 37, 8, 8, N'Emily', N'Potash', 2, 165, 175, 185, 195, 1, 1, 1, 1, 176, 176, N'', 720, 0, 180, -1, 0, 716, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (31, 2, 31, 6, 6, N'Leslie', N'Kelp', 2, 115, 125, 135, 145, 1, 1, 1, 1, 137, 137, N'', 520, 0, 130, 19, 0, 596, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (32, 2, 28, 4, 4, N'Sarah', N'Person', 1, 110, 120, 130, 140, 1, 1, 1, 1, 145, 145, N'', 500, 0, 125, 70, 0, 780, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (33, 2, 38, 8, 8, N'Emily', N'Potash', 3, 150, 160, 170, 180, 1, 1, 1, 1, 173, 173, N'', 660, 0, 165, -1, 0, 656, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (34, 2, 36, 8, 8, N'Emily', N'Potash', 1, 140, 150, 160, 170, 1, 1, 1, 1, 174, 174, N'', 620, 0, 155, -1, 0, 616, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (35, 2, 26, 3, 3, N'Steve', N'Hopper', 4, 115, 120, 125, 130, 1, 1, 1, 1, 138, 138, N'', 490, 0, 122, 70, 0, 770, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (36, 2, 30, 6, 6, N'Leslie', N'Kelp', 1, 85, 95, 102, 110, 1, 1, 1, 1, 139, 139, N'', 392, 0, 98, 57, 0, 620, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (37, 2, 21, 1, 1, N'Jimothy', N'Bowler', 1, 155, 165, 175, 185, 1, 1, 1, 1, 147, 147, N'', 680, 0, 170, 70, 0, 960, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (38, 2, 23, 2, 2, N'Jane', N'Smith', 2, 185, 175, 165, 155, 1, 1, 1, 1, 144, 144, N'', 680, 0, 170, 70, 0, 960, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (39, 2, 39, 10, 10, N'Been', N'Jelly', 3, 123, 133, 143, 153, 1, 1, 1, 1, 141, 141, N'', 552, 0, 138, 70, 0, 832, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (40, 3, 45, 4, 4, N'Sarah', N'Person', 3, 134, 165, 189, 178, 1, 1, 1, 1, 144, 157, N'', 666, 0, 166, 57, 0, 894, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (41, 3, 47, 4, 4, N'Sarah', N'Person', 1, 145, 156, 180, 123, 1, 1, 1, 1, 156, 156, N'', 604, 0, 151, 70, 0, 884, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (42, 3, 50, 6, 6, N'Leslie', N'Kelp', 2, 112, 156, 134, 187, 1, 1, 1, 1, 139, 139, N'', 589, 0, 147, 70, 0, 869, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (43, 3, 51, 7, 7, N'Ronald', N'Garp', 2, 115, 168, 189, 178, 1, 1, 1, 1, 147, 147, N'', 650, 0, 162, 70, 0, 930, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (44, 3, 40, 1, 1, N'Jimothy', N'Bowler', 1, 100, 110, 120, 130, 1, 1, 1, 1, 140, 140, N'', 460, 0, 115, 70, 0, 740, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (45, 3, 49, 5, 5, N'Victor', N'Hugo', 2, 100, 200, 168, 189, 1, 1, 1, 1, 199, 199, N'', 657, 0, 164, 4, 0, 673, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (46, 3, 48, 4, 4, N'Sarah', N'Person', 2, 145, 156, 0, 0, 1, 1, 1, 1, 140, 155, N'', 301, 0, 150, 70, 0, 581, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (47, 3, 46, 4, 4, N'Sarah', N'Person', 4, 110, 126, 114, 89, 1, 1, 1, 1, 139, 150, N'', 439, 0, 109, 70, 0, 719, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (48, 3, 44, 3, 3, N'Steve', N'Hopper', 3, 123, 134, 145, 156, 1, 1, 1, 1, 138, 138, N'', 558, 0, 139, 70, 0, 838, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (49, 3, 42, 2, 2, N'Jane', N'Smith', 2, 150, 160, 170, 180, 1, 1, 1, 1, 139, 139, N'', 660, 0, 165, 70, 0, 940, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (50, 3, 41, 1, 1, N'Jimothy', N'Bowler', 2, 130, 140, 150, 160, 1, 1, 1, 1, 140, 140, N'', 580, 0, 145, 18, 0, 652, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (51, 3, 54, 10, 10, N'Been', N'Jelly', 2, 100, 110, 120, 130, 1, 1, 1, 1, 135, 135, N'', 460, 0, 115, 70, 0, 740, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (52, 3, 43, 2, 2, N'Jane', N'Smith', 3, 110, 150, 0, 165, 1, 1, 1, 1, 135, 139, N'', 425, 0, 141, 70, 0, 705, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (53, 3, 53, 9, 9, N'Ben', N'Harper', 2, 123, 145, 0, 167, 1, 1, 1, 1, 141, 153, N'', 435, 0, 145, 59, 0, 671, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (54, 3, 52, 8, 8, N'Emily', N'Potash', 2, 145, 168, 178, 145, 1, 1, 1, 1, 175, 175, N'', 636, 0, 159, 40, 0, 796, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (55, 4, 62, 4, 4, N'Sarah', N'Person', 1, 214, 200, 195, 100, 1, 1, 1, 1, 152, 154, N'', 709, 0, 177, 70, 0, 989, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (56, 4, 63, 4, 4, N'Sarah', N'Person', 2, 123, 145, 0, 0, 1, 1, 1, 1, 144, 151, N'', 268, 0, 134, 70, 0, 548, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (57, 4, 66, 7, 7, N'Ronald', N'Garp', 1, 120, 130, 140, 150, 1, 1, 1, 1, 145, 145, N'', 540, 0, 135, 66, 0, 804, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (58, 4, 64, 5, 5, N'Victor', N'Hugo', 1, 123, 145, 156, 198, 1, 1, 1, 1, 188, 188, N'', 622, 0, 155, 19, 0, 698, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (59, 4, 57, 1, 1, N'Jimothy', N'Bowler', 3, 142, 156, 178, 132, 1, 1, 1, 1, 141, 141, N'', 608, 0, 152, 18, 0, 680, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (60, 4, 55, 1, 1, N'Jimothy', N'Bowler', 1, 112, 113, 156, 145, 1, 1, 1, 1, 139, 139, N'', 526, 0, 131, 70, 0, 806, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (61, 4, 56, 1, 1, N'Jimothy', N'Bowler', 2, 142, 146, 132, 178, 1, 1, 1, 1, 140, 140, N'', 598, 0, 149, 18, 0, 670, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (62, 4, 61, 3, 3, N'Steve', N'Hopper', 1, 123, 145, 156, 100, 1, 1, 1, 1, 137, 137, N'', 524, 0, 131, 70, 0, 804, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (63, 4, 65, 6, 6, N'Leslie', N'Kelp', 1, 110, 120, 130, 140, 1, 1, 1, 1, 137, 137, N'', 500, 0, 125, 70, 0, 780, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (64, 4, 58, 1, 1, N'Jimothy', N'Bowler', 4, 89, 145, 0, 123, 1, 1, 1, 1, 137, 140, N'', 357, 0, 119, 18, 0, 429, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (65, 4, 60, 2, 2, N'Jane', N'Smith', 1, 100, 98, 156, 143, 1, 1, 1, 1, 137, 137, N'', 497, 0, 124, 70, 0, 777, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (66, 4, 59, 2, 2, N'Jane', N'Smith', 4, 110, 120, 130, 140, 1, 1, 1, 1, 136, 136, N'', 500, 0, 125, 70, 0, 780, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (67, 4, 69, 10, 10, N'Been', N'Jelly', 1, 135, 126, 148, 98, 1, 1, 1, 1, 133, 133, N'', 507, 0, 126, 70, 0, 787, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (68, 4, 67, 8, 8, N'Emily', N'Potash', 1, 130, 140, 150, 160, 1, 1, 1, 1, 171, 171, N'', 580, 0, 145, 41, 0, 744, 1)
GO
INSERT [dbo].[FinalizeTemps] ([FinalizeID], [TournamentID], [GameId], [MemberId], [memberNumber], [FirstName], [LastName], [Squad], [Game1], [Game2], [Game3], [Game4], [UseGame1], [UseGame2], [UseGame3], [UseGame4], [LeagueAverage], [AdjustedAvg], [Notes], [ScratchTotal], [KeepAdjustedAvg], [GameAvg], [Handicap], [Bonus], [HandicapTotal], [FinalizeRegionID]) VALUES (69, 4, 68, 9, 9, N'Ben', N'Harper', 1, 142, 153, 123, 165, 1, 1, 1, 1, 151, 151, N'', 583, 0, 145, 70, 0, 863, 1)
GO
SET IDENTITY_INSERT [dbo].[FinalizeTemps] OFF
GO
SET IDENTITY_INSERT [dbo].[PlayerHistories] ON 
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (1, 7, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 14, 215, 200, 230, 50, 0, 70, 0, CAST(35.00 AS Decimal(18, 2)), N'', 165, 173, N'', N'1', 1, 173)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (2, 4, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 10, 155, 200, 219, 89, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 165, 165, N'', N'3', 1, 165)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (3, 8, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 16, 200, 204, 189, 178, 0, -1, 0, CAST(0.00 AS Decimal(18, 2)), N'', 192, 192, N'', N'7', 1, 192)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (4, 9, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 18, 198, 156, 184, 144, 0, 67, 0, CAST(15.00 AS Decimal(18, 2)), N'', 158, 158, N'', N'2', 1, 170)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (5, 6, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 12, 200, 187, 156, 132, 0, 19, 0, CAST(0.00 AS Decimal(18, 2)), N'', 160, 160, N'', N'8', 1, 168)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (6, 1, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 2, 114, 200, 147, 198, 0, 18, 1, CAST(0.00 AS Decimal(18, 2)), N'', 134, 134, N'', N'9', 1, 164)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (7, 3, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 8, 213, 156, 98, 165, 0, 13, 0, CAST(0.00 AS Decimal(18, 2)), N'', 158, 158, N'', N'10', 1, 158)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (8, 2, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 5, 145, 78, 159, 200, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 145, 145, N'', N'5', 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (9, 10, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 19, 200, 215, 111, 78, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 143, 143, N'', N'4', 1, 151)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (10, 5, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 11, 235, 255, 198, 189, 0, -27, 0, CAST(0.00 AS Decimal(18, 2)), N'', 219, 219, N'', N'6', 1, 219)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (11, 10, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 20, 145, 189, 121, 85, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 135, 135, N'', NULL, 1, 135)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (12, 9, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 17, 125, 156, 121, 178, 0, 67, 0, CAST(0.00 AS Decimal(18, 2)), N'', 145, 145, N'', NULL, 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (13, 2, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 6, 89, 155, 123, 142, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 136, 136, N'', NULL, 1, 127)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (14, 6, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 13, 155, 165, 200, 89, 0, 19, 0, CAST(0.00 AS Decimal(18, 2)), N'', 152, 152, N'', NULL, 1, 152)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (15, 1, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 1, 80, 95, 115, 125, 0, 18, 1, CAST(0.00 AS Decimal(18, 2)), N'', 103, 103, N'', NULL, 1, 103)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (16, 1, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 3, 98, 165, 231, 147, 0, 18, 1, CAST(0.00 AS Decimal(18, 2)), N'', 142, 142, N'', NULL, 1, 160)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (17, 2, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 7, 123, 156, 189, 73, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 136, 136, N'', NULL, 1, 135)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (18, 1, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 4, 80, 115, 156, 195, 0, 18, 1, CAST(0.00 AS Decimal(18, 2)), N'', 141, 141, N'', NULL, 1, 136)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (19, 7, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 15, 68, 89, 121, 125, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 132, 136, N'', NULL, 1, 100)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (20, 3, 4, CAST(N'2019-08-12T00:00:00.000' AS DateTime), 9, 122, 146, 99, 165, 0, 13, 0, CAST(0.00 AS Decimal(18, 2)), N'', 146, 146, N'', NULL, 1, 133)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (21, 7, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 32, 160, 170, 180, 190, 0, 70, 0, CAST(35.00 AS Decimal(18, 2)), N'', 149, 149, N'', N'1', 1, 175)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (22, 1, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 21, 155, 165, 175, 185, 0, 70, 0, CAST(7.50 AS Decimal(18, 2)), N'', 147, 147, N'', N'2', 1, 170)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (23, 4, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 27, 200, 190, 180, 170, 0, 53, 1, CAST(0.00 AS Decimal(18, 2)), N'', 158, 158, N'', N'4', 1, 185)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (24, 8, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 35, 185, 195, 205, 215, 0, 28, 1, CAST(0.00 AS Decimal(18, 2)), N'', 178, 178, N'', N'5', 1, 200)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (25, 6, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 30, 85, 95, 102, 110, 0, 57, 1, CAST(0.00 AS Decimal(18, 2)), N'', 139, 139, N'', N'9', 1, 98)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (26, 10, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 39, 123, 133, 143, 153, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 141, 141, N'', N'7', 1, 138)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (27, 2, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 23, 185, 175, 165, 155, 0, 70, 0, CAST(7.50 AS Decimal(18, 2)), N'', 144, 144, N'', N'2', 1, 170)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (28, 3, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 26, 115, 120, 125, 130, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 138, 138, N'', N'8', 1, 122)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (29, 5, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 29, 200, 210, 220, 230, 0, 4, 0, CAST(0.00 AS Decimal(18, 2)), N'', 217, 217, N'', N'6', 1, 215)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (30, 2, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 24, 110, 120, 130, 140, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 140, 140, N'', NULL, 1, 125)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (31, 7, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 34, 130, 140, 150, 160, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 144, 144, N'', NULL, 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (32, 8, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 37, 165, 175, 185, 195, 0, -1, 1, CAST(0.00 AS Decimal(18, 2)), N'', 176, 176, N'', NULL, 1, 180)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (33, 4, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 28, 110, 120, 130, 140, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 145, 145, N'', NULL, 1, 125)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (34, 2, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 25, 90, 100, 110, 120, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 134, 134, N'', NULL, 1, 105)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (35, 7, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 33, 110, 120, 130, 140, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 143, 143, N'', NULL, 1, 125)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (36, 8, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 38, 150, 160, 170, 180, 0, -1, 1, CAST(0.00 AS Decimal(18, 2)), N'', 173, 173, N'', NULL, 1, 165)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (37, 1, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 22, 145, 135, 125, 115, 0, 18, 0, CAST(0.00 AS Decimal(18, 2)), N'', 144, 144, N'', NULL, 1, 130)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (38, 6, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 31, 115, 125, 135, 145, 0, 19, 1, CAST(0.00 AS Decimal(18, 2)), N'', 137, 137, N'', NULL, 1, 130)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (39, 8, 4, CAST(N'2019-08-20T00:00:00.000' AS DateTime), 36, 140, 150, 160, 170, 0, -1, 1, CAST(0.00 AS Decimal(18, 2)), N'', 174, 174, N'', NULL, 1, 155)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (40, 2, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 42, 150, 160, 170, 180, 0, 70, 0, CAST(35.00 AS Decimal(18, 2)), N'', 139, 139, N'', N'1', 1, 165)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (41, 9, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 53, 123, 145, 0, 167, 0, 59, 0, CAST(0.00 AS Decimal(18, 2)), N'', 141, 153, N'', N'10', 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (42, 8, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 52, 145, 168, 178, 145, 0, 40, 1, CAST(0.00 AS Decimal(18, 2)), N'', 175, 175, N'', N'6', 1, 159)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (43, 10, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 54, 100, 110, 120, 130, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 135, 135, N'', N'7', 1, 115)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (44, 4, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 45, 134, 165, 189, 178, 0, 57, 0, CAST(0.00 AS Decimal(18, 2)), N'', 144, 157, N'', N'3', 1, 166)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (45, 7, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 51, 115, 168, 189, 178, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 147, 147, N'', N'2', 1, 162)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (46, 6, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 50, 112, 156, 134, 187, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 139, 139, N'', N'4', 1, 147)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (47, 1, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 40, 100, 110, 120, 130, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 140, 140, N'', N'7', 1, 115)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (48, 5, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 49, 100, 200, 168, 189, 0, 4, 1, CAST(0.00 AS Decimal(18, 2)), N'', 199, 199, N'', N'9', 1, 164)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (49, 3, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 44, 123, 134, 145, 156, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 138, 138, N'', N'5', 1, 139)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (50, 2, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 43, 110, 150, 0, 165, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 135, 139, N'', NULL, 1, 141)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (51, 4, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 47, 145, 156, 180, 123, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 156, 156, N'', NULL, 1, 151)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (52, 4, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 48, 145, 156, 0, 0, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 140, 155, N'', NULL, 1, 150)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (53, 1, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 41, 130, 140, 150, 160, 0, 18, 0, CAST(0.00 AS Decimal(18, 2)), N'', 140, 140, N'', NULL, 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (54, 4, 4, CAST(N'2019-08-21T00:00:00.000' AS DateTime), 46, 110, 126, 114, 89, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 139, 150, N'', NULL, 1, 109)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (55, 1, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 55, 112, 113, 156, 145, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 139, 139, N'', N'3', 1, 131)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (56, 9, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 68, 142, 153, 123, 165, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 151, 151, N'', N'2', 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (57, 8, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 67, 130, 140, 150, 160, 0, 41, 0, CAST(0.00 AS Decimal(18, 2)), N'', 171, 171, N'', N'9', 1, 145)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (58, 10, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 69, 135, 126, 148, 98, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 133, 133, N'', N'6', 1, 126)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (59, 2, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 59, 110, 120, 130, 140, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 136, 136, N'', N'7', 1, 125)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (60, 6, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 65, 110, 120, 130, 140, 0, 70, 1, CAST(0.00 AS Decimal(18, 2)), N'', 137, 137, N'', N'7', 1, 125)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (61, 7, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 66, 120, 130, 140, 150, 0, 66, 0, CAST(0.00 AS Decimal(18, 2)), N'', 145, 145, N'', N'4', 1, 135)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (62, 4, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 62, 214, 200, 195, 100, 0, 70, 0, CAST(35.00 AS Decimal(18, 2)), N'', 152, 154, N'', N'1', 1, 177)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (63, 3, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 61, 123, 145, 156, 100, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 137, 137, N'', N'4', 1, 131)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (64, 5, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 64, 123, 145, 156, 198, 0, 19, 0, CAST(0.00 AS Decimal(18, 2)), N'', 188, 188, N'', N'10', 1, 155)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (65, 1, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 56, 142, 146, 132, 178, 0, 18, 0, CAST(0.00 AS Decimal(18, 2)), N'', 140, 140, N'', NULL, 1, 149)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (66, 2, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 60, 100, 98, 156, 143, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 137, 137, N'', NULL, 1, 124)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (67, 1, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 58, 89, 145, 0, 123, 0, 18, 0, CAST(0.00 AS Decimal(18, 2)), N'', 137, 140, N'', NULL, 1, 119)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (68, 1, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 57, 142, 156, 178, 132, 0, 18, 0, CAST(0.00 AS Decimal(18, 2)), N'', 141, 141, N'', NULL, 1, 152)
GO
INSERT [dbo].[PlayerHistories] ([hisID], [MemberNumber], [GamesPlayed], [TournamentDate], [GameID], [Game1], [Game2], [Game3], [Game4], [TotalScore], [HandiCap], [Bonus], [MoneyWon], [Notes], [trueAVG], [AVG], [ProPot], [PPHG], [regionID], [AverageForEntry]) VALUES (69, 4, 4, CAST(N'2019-08-23T00:00:00.000' AS DateTime), 63, 123, 145, 0, 0, 0, 70, 0, CAST(0.00 AS Decimal(18, 2)), N'', 144, 151, N'', NULL, 1, 134)
GO
SET IDENTITY_INSERT [dbo].[PlayerHistories] OFF
GO
