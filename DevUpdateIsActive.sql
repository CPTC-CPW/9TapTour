USE [NineTapTour.NineTapDb]
GO

UPDATE Members
SET IsActive=1
WHERE IsActive=0
GO