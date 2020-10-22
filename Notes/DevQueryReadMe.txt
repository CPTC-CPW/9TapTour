1. Open NineTapTour.sln in Visual Stuido
2. Click View -> SQL Server Object Explorer
3. Find NineTapTour.NineTapDb under (localdb)\MSSQLLocalDB -> Databases
4. Right-click NineTapTour.NineTapDb and select New Query
5. Copy and Paste this code into the query form :

UPDATE Members
SET IsActive=1
WHERE IsActive=0

6. Click the green play-button to Execute the query.