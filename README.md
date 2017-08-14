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

### 1. Create Azure API App

1. Open a browser, navigate to the [Azure Portal](portal.azure.com) and log in.

![](https://user-images.githubusercontent.com/13558917/29183032-048619ee-7db6-11e7-86bd-d8aa56e8b579.png)

2. After logging in, click on New -> Enter `API App` into the Search Bar -> Selected `API App` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29196930-7af3d342-7dec-11e7-9b40-3bf92f4ee5f0.png)

3. Name the API App
    - I named mine XamListAPIApp
4. Select the Subscription
    - I selected my Visual Studio Enterprise subscription
    - If you do not have a VS Enterprise Subscription, you will need to select a different option
5. Create a new Resource Group
    - I named my resource group XamList
6. Create a new App Service Plan
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

### 2. Create Azure Function App

![](https://user-images.githubusercontent.com/13558917/29196481-756d88bc-7de9-11e7-9d81-33c14d1077b0.png)

1. In the Azure portal, click on New -> Enter `Function App` into the Search Bar -> Selected `Function App` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29196973-ea5fb796-7dec-11e7-92d3-fda7ba5a6f6b.png)

2. Name the Function App
    - I named mine XamListFunctionApp
3. Select Consumption for the Hosting Plan
4. Select the XamList Resource Group
    - We created this resource group when we made our API App, above
5. Select the Location closest to you
6. Under Storage, Select Create New
    - I named my storage "xamlistfunctionapp"
7. Click Create

### 3. Create Azure SQL Database

![](https://user-images.githubusercontent.com/13558917/29196780-9324ac1c-7deb-11e7-9d87-8a95ab62b0c5.png)

1. In the Azure portal, click on New -> Enter `SQL Database` into the Search Bar -> Selected `SQL Database` from the search results -> Click Create

![](https://user-images.githubusercontent.com/13558917/29197883-2b850292-7df4-11e7-8bfd-8016d72f799a.png)

2. Name the SQL Database
    - I named mine XamListDatabase
3. Select the Subscription
    - I selected my Visual Studio Enterprise subscription
    - If you do not have a VS Enterprise Subscription, you will need to select a different option
4. Select the XamList Resource Group
    - We created this resource group when we made our API App, above
5. Select Blank Database

![](https://user-images.githubusercontent.com/13558917/29198124-efa3b08c-7df5-11e7-87f4-42cf0dc95862.png)

6. Select Server
7. Select Create New Server
8. Enter the Server Name
    - I named mine xamlistdatabaseserver
9. Create a Server admin login
    - Store this password somewhere safe, because we will need to use it for our database connection later!
10. Create a password
11. Select the closest location
12. Click "Select"

![](https://user-images.githubusercontent.com/13558917/29198240-f8b25cae-7df6-11e7-8f76-b8977645a712.png)

13. Select "Not Now" for the SQL Elastic Pool option
14. Select Pricing Tier
    1. Select Basic
    2. Move the slider to maximum, 2GB
        - Don't worry, it's the same price for 2GB as it is for 100MB. No clue why!
    3. Click Apply
15. Click Create

### 4. Get SQL Database Connection String

![](https://user-images.githubusercontent.com/13558917/29198409-9d0dcab2-7df8-11e7-8c41-4797228ee4ab.png)

1. On the Azure Portal, navigate to the SQL Database we created, above
2. Click on "Connection Strings" -> "ADO.NET"
3. Copy the entire Connection String into a text editor

 ![](https://user-images.githubusercontent.com/13558917/29198528-b26f19f0-7df9-11e7-82c2-b4d46f60389a.png)

4. In the text editor, change "{your_username}" and "{your_password}" to match the SQL Database Username / Password created above
    - Don't use my username / password because it won't work ;-)

### 5. Connect SQL Database to the API App

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

### 6. Connect SQL Database to the Azure Function App

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

### 7. Create the ContactModel Table in the SQL Database

![](https://user-images.githubusercontent.com/13558917/29254139-a74c1360-8043-11e7-93a1-1d6610d803bc.png)
1. In the Azure Portal, navigate to the SQL Database Server we created in a previous step
2. Enter your public IP Address
    - Your public IP address is listed on this page as "Client IP Address"
    - Alternatively, you can discover your public IP Address by navigating to http://whatismyipaddress.com
3. Click Save 

![](https://user-images.githubusercontent.com/13558917/29254113-fd4605b0-8042-11e7-97ea-0600bf3f948c.png)

4. On the Azure Portal, retrieve the URL of the database we created in the previous steps.

![](https://user-images.githubusercontent.com/13558917/29254220-5e23dea0-8045-11e7-9c50-0901b03f8efb.PNG)

5. Open SQL Server Management Studio and enter the following connection information:
   - Server Type: Database Engine
   - Server Name: [your database server url]
     - Mine is xamlistdatabaseserver.database.windows.net
   - Authentication: SQL Server Authentication
   - User: [your database admin username]
   - Password: [your database admin password]
6. Click Connect

![](https://user-images.githubusercontent.com/13558917/29254236-b457c0d4-8045-11e7-9b25-20c75cf0a22d.png)

7. In SSMS, create a new Table
    - In the Object Explorer, navigate to the database we created in a previous step
    - Right-click Tables -> New -> Table...

![](https://user-images.githubusercontent.com/13558917/29254263-8ad418e2-8046-11e7-8916-0ba84ed45b53.PNG)

8. Create New Columns
    - Id -> nvarchar(128) -> False
    - FirstName -> nvarchar(128) -> False
    - LastName -> nvarchar(128) -> False
    - PhoneNumber -> nvarchar(128) -> False
    - CreatedAt -> datetimeoffset(7) -> False
    - UpdatedAt -> datetimeoffset(7) -> False
    - Deleted -> bit -> False

![](https://user-images.githubusercontent.com/13558917/29254294-4c1ab682-8047-11e7-9053-458b5d744ba2.png)

9. Create Table Name
    - In the Properties window, enter "ContactModels" for the Name
    - Click SaveAll

### 8. Install Visual Studio Azure Functions Extension

1. Open [XamList.sln](https://github.com/brminnick/XamList/blob/master/XamList.sln) using Visual Studio for PC (Version 15.3 or later)

![](https://user-images.githubusercontent.com/13558917/29254393-8a1b69e8-8049-11e7-8426-5e1d3ccb3193.png)

2. Ensure the following Extensions are installed
    - Visual Studio 2017 Tools for Azure Functions
    - Azure Functions and Web Jobs Tools


### 8. Publish API App to Azure

![](https://user-images.githubusercontent.com/13558917/29254418-0a1be884-804a-11e7-8c8d-ff13d8b124ff.png)

1. In Visual Studio, right-click on XamList.API and select Publish

![](https://user-images.githubusercontent.com/13558917/29254459-98227cf6-804a-11e7-8fc6-e404d551159e.png)

2. In the Publish window, choose "Select Existing" -> "Publish"

![](https://user-images.githubusercontent.com/13558917/29254501-266009d4-804b-11e7-8046-1607506cfaea.png)

3. In the App Service window, select the API App we created earlier, and click OK

4. Visual Studio is now publishing the XamList.API code to your Azure API App

### 9. Publish Function App to Azure

![](https://user-images.githubusercontent.com/13558917/29254534-ad6861a6-804b-11e7-886a-bfb267546afe.png)

1. In Visual Studio, right-click on XamList.Functions and select Publish

![](https://user-images.githubusercontent.com/13558917/29254603-9d52477c-804c-11e7-8da5-7a8d1389e1d5.png)

2. Choose AzureFunctionApp -> Select Existing -> Publish

![](https://user-images.githubusercontent.com/13558917/29254618-fd69c400-804c-11e7-9ba6-97f1da527b94.png)

3. In the App Service window, select the Functions App we created earlier, and click OK 

4. Visual Studio is now publishing the XamList.API code to your Azure API App

### 10. Configure API URL for Mobile App

![](https://user-images.githubusercontent.com/13558917/29290868-7651182e-80f6-11e7-826f-d6b293ff53e9.png)

1. In [BackendConstants.cs](https://github.com/brminnick/XamList/blob/master/XamList.Mobile.Common/Constants/BackendConstants.cs), change the value of `AzureAPIUrl` to match your Azure API App URL
   - The address for the one I created in this tutorial is http://xamlistapiapp.azurewebsites.net/
   
![](https://user-images.githubusercontent.com/13558917/29292989-28b1d6c8-80fe-11e7-8493-663c5b53a25d.png)   
   
2. In [BackendConstants.cs](https://github.com/brminnick/XamList/blob/master/XamList.Mobile.Common/Constants/BackendConstants.cs), change the value of `AzureFunctionUrl` to match your Azure Function URL
   - The address for the one I created in this tutorial is https://xamlistfunctionapp.azurewebsites.net
   
   
### 11. Configure Azure Function
