# XamList

[![Build status](https://build.mobile.azure.com/v0.1/apps/e53e8e26-f6c5-4bb0-823b-355acf4d6100/branches/master/badge)](https://mobile.azure.com)

An iOS and Android app showcasing Xamarin.Forms, Azure Functions, Azure SQL Database, Azure API and Mobile Center.

## Visual Studio Requirements

### Visual Studio for Mac

This solution requires Visual Studio for Mac Version 7.1 Build 1294 or later.

Earlier versions of VSMac don't support Shared Project references from .NET Standard libraries.

### Visual Studio for PC

This solution requires Visual Studio for PC Version 15.3 or later.

Earlier versions of VS don't support Azure Functions.

## Getting Started

### Create Azure API App

Open a browser, navigate to the [Azure Portal](portal.azure.com) and log in.

![](https://user-images.githubusercontent.com/13558917/29183032-048619ee-7db6-11e7-86bd-d8aa56e8b579.png)

After logging in, click on New -> Enter `API App` into the Search Bar -> Selected `API App` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29196930-7af3d342-7dec-11e7-9b40-3bf92f4ee5f0.png)

1. Name the API App
    - I named mine XamListAPIApp
2. Select the Subscription
    - I selected my Visual Studio Enterprise subscription
    - If you do not have a VS Enterprise Subscription, you will need to select a different option
3. Create a new Resource Group
    - I named my resource group XamList
4. Create a new App Service Plan
    1. Click App Service Plam
    2. Click Create New
    3. Name the App Service Plan
       - I named my App Service Plan XamListAppService
    4. Select the location closest to you
       - I selected South Central US
    5. Select the Free Pricing Tier
       1. Click "Pricing Tier"
       2. Select "Free"
       3. Click "Select"
    6. Click "OK"
    7. Click "Create"

### Create Azure Function App

![](https://user-images.githubusercontent.com/13558917/29196481-756d88bc-7de9-11e7-9d81-33c14d1077b0.png)

In the Azure portal, click on New -> Enter `Function App` into the Search Bar -> Selected `Function App` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29196973-ea5fb796-7dec-11e7-92d3-fda7ba5a6f6b.png)

1. Name the Function App
    - I named mine XamListFunctionApp
2. Select Consumption for the Hosting Plan
3. Select the XamList Resource Group
    - We created this resource group when we made our API App, above
4. Select the Location closest to you
5. Under Storage, Select Create New
    - I named my storage "xamlistfunctionapp"
6. Click Create

### Create Azure SQL Database

![](https://user-images.githubusercontent.com/13558917/29196780-9324ac1c-7deb-11e7-9d87-8a95ab62b0c5.png)

In the Azure portal, click on New -> Enter `SQL Database` into the Search Bar -> Selected `SQL Database` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29197883-2b850292-7df4-11e7-8bfd-8016d72f799a.png)

1. Name the SQL Database
    - I named mine XamListDatabase
2. Select the Subscription
    - I selected my Visual Studio Enterprise subscription
    - If you do not have a VS Enterprise Subscription, you will need to select a different option
3. Select the XamList Resource Group
    - We created this resource group when we made our API App, above
4. Select Blank Database

![](https://user-images.githubusercontent.com/13558917/29198124-efa3b08c-7df5-11e7-87f4-42cf0dc95862.png)

5. Select Server
6. Select Create New Server
7. Enter the Server Name
    - I named mine xamlistdatabaseserver
8. Create a Server admin login
    - Store this password somewhere safe, because we will need to use it for our database connection later!
9. Create a password
10. Select the closest location
11. Click "Select"

![](https://user-images.githubusercontent.com/13558917/29198240-f8b25cae-7df6-11e7-8f76-b8977645a712.png)

12. Select "Not Now" for the SQL Elastic Pool option
13. Select Pricing Tier
    1. Select Basic
    2. Move the slider to maximum, 2GB
        - Don't worry, it's the same price for 2GB as it is for 100MB. No clue why!
    3. Click Apply
14. Click Create

### Get SQL Database Connection String

![](https://user-images.githubusercontent.com/13558917/29198409-9d0dcab2-7df8-11e7-8c41-4797228ee4ab.png)

1. On the Azure Portal, navigate to the SQL Database we created, above
2. Click on "Connection Strings" -> "ADO.NET"
3. Copy the entire Connection String into a text editor

 ![](https://user-images.githubusercontent.com/13558917/29198528-b26f19f0-7df9-11e7-82c2-b4d46f60389a.png)

4. In the text editor, change "{your_username}" and "{your_password}" to match the SQL Database Username / Password created above
    - Don't use my username / password because it won't work ;-)

### Connect SQL Database to the API App

![](https://user-images.githubusercontent.com/13558917/29198731-7d63f31e-7dfb-11e7-9985-35f6f3ed971b.png)

1. In the Azure Portal, navigate to the API App
    - I named mine XamListAPIApp
2. Click Application Settings
3. In the Application Settings, scroll down to "Connection Strings"
4. Create a new connection string
    - Set the name as `XamListDatabaseConnectionString`
      - Make sure to use this _exact_ name, otherwise the source code will not work
    - Copy/paste the connection string from the text editor
    - From the drop-down menu, Select SQLDatabase
5. Click Save

### Connect SQL Database to the Azure Function App

![](https://user-images.githubusercontent.com/13558917/29198794-f3673e5e-7dfb-11e7-89fc-ee042fe34704.png)

1. On the Azure Portal, navigate to the Functions App we create above
2. Select "Application Settings"

![](https://user-images.githubusercontent.com/13558917/29198881-aaebd0c6-7dfc-11e7-870b-2f4d358139cb.png)

3. In the Application Settings, scroll down to "Connection Strings"
4. Create a new connection string
    - Set the name as `XamListDatabaseConnectionString`
      - Make sure to use this _exact_ name, otherwise the source code will not work
    - Copy/paste the connection string from the text editor
    - From the drop-down menu, Select SQLDatabase
5. Click Save

### Create the ContactModel Table in the SQL Database


## Azure SQL Database

All data for this mobile app is stored in the cloud using an [Azure SQL Databse](https://azure.microsoft.com/en-us/services/sql-database/). Azure SQL Database allows you to store your data securely in the cloud and scale performance in real-time.

## Azure Functions

It is rare that servers run at 100% CPU & Memory capacity 24/7. Why pay for an idle server when it's not in use? Azure Functions allows you to do just this!

[Azure Functions](https://azure.microsoft.com/en-us/services/functions/) allows you to run code on a cloud server on-demand, giving you the ability to save cost and pay only for the server time you need.

This app utilizes Azure Functions to access the Azure SQL Database and restore deleted contacts.

[The code for the Azure Functions](./XamList.Functions/) was built and published using Visual Studio 2017 Preview 15.3 Build 6.

## Azure API

[Azure API Apps](https://azure.microsoft.com/en-us/services/app-service/api/?cdn=disable) allow you to quickly stand up a REST API in the cloud.

For mobile apps communicating with a backend database, REST APIs are critical. Mobile apps can't rely on a direct connection to a backend database because mobile internet connections are unreliable and we risk corrupting the database.

A REST API acts as the perfect intermediary to interpret and process CRUD requests between a mobile app and its backend SQL Database.

[The code for the API](./XamList.API/) was built and published using ASP.NET in [Visual Studio 2017 Preview 15.3 Build 6](https://www.visualstudio.com/vs/preview/).

## Mobile Center

[Mobile Center](https://mobile.azure.com) is the next generation of mobile app DevOps:

- Continuous Integration
- Mobile UI Testing
- Mobile App Distribution
- Mobile App Push Notifications
- Gather Realtime Crash Reports
- Monitor Mobile App Usage and Analytics