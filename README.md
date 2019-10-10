[![Build Status](https://dev.azure.com/CPTC-Programming/9Tap/_apis/build/status/9Tap-.NET%20Desktop-CI?branchName=master)](https://dev.azure.com/CPTC-Programming/9Tap/_build/latest?definitionId=2&branchName=master)

# 9-Tap Tour
This app is the 9-Tap Tour Replacement Application, that keeps track of the 9 Tap Tour. This information includes member data, tournament information, games, monies earned, player stats etc. The app will replace the current program being used by the client to run future 9-Tap tournaments.

## Getting Started
To get started with this project please review the following and reference the [project Wiki](https://github.com/CPTC-CPW/9TapTour/wiki) for things not listed here.

To get started with using 9TapTour and successfully running the database and getting test data, refer to this youtube video. Please refer to the description when you get to using SQL Server Management Studio

* [9TapTour - Installing Database and Test Data](https://www.youtube.com/watch?v=CgwC94uQqxA)

## Getting Started With 9TapTour Step-By-Step Insructions
- **Note** This is for those who have added the project to a computer without an exsisting copy of 9TapTour and/or the database
- We are using Visual Studio 2019 and SQL Server Management Studio in this Example. This would Work Using Visual Studio 2017
1. In VS2019 Go to Tools -> NuGet Package Manager -> Package Manager Console
![Opening Package Manager Console](https://github.com/CPTC-CPW/9TapTour/tree/TannerDisney-patch-1/9TT-Doc/PackageMC.PNG)
2. In the Package Manager Console Enter `Update-Database` and Run it.
![Entering Update-Database in Package Manager Console](https://github.com/CPTC-CPW/9TapTour/tree/TannerDisney-patch-1/9TT-Doc/Update-DB.PNG)
3. Once you updated your database go to your Solution Explorer - > Database -> DBScripts (Right Click Folder) -> Open Folder in File Explorer. Then open the multisquadTestData.sql file, which will open on SQL Server Management Studio.
![Opening Folder in File Explorer](https://github.com/CPTC-CPW/9TapTour/tree/TannerDisney-patch-1/9TT-Doc/OpenFolderinFE.PNG)
4. In SQL Server Management Studio, It will ask you to Login Make Sure your Settings are as Follows:
    * Server Type: Database Engine
    * Server Name: (localdb)\msSQLlocaldb
    * Authentication: Windows Authentication
![Opening SQL Server and Logging In](https://github.com/CPTC-CPW/9TapTour/tree/TannerDisney-patch-1/9TT-Doc/SSMS-Login.PNG)
5. Then you can connect to SQL Server Management Studio and Execute the script adding Test data to 9TapTour

### Prerequisites
The current build is being built on Windows machines through Visual Studio 2017 v.15 or greater.

https://visualstudio.microsoft.com/downloads/ 

### Coding Style Requirements
Reference the [code style requirements Wiki](https://github.com/CPTC-CPW/9TapTour/wiki/Coding-Style-Requirements) for more information.

## Built With
* [Visual Studio](https://visualstudio.microsoft.com/) - Program Used for Design

## Authors 
Reference the list of [contributors](https://github.com/CPTC-CPW/9TapTour/graphs/contributors) who participated in this project.

## License 
9-Tap Tour's [License Agreement](https://github.com/CPTC-CPW/9TapTour/wiki/License-Agreement) Wiki.
