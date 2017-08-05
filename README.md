# XamList

An iOS and Android app showcasing Xamarin.Forms, Azure Functions, Azure SQL Database, Azure API and Mobile Center.

### Visual Studio for Mac Requirements

This solution requires Visual Studio for Mac Version 7.1 Build 1294 or later. 

Earlier versions of VSMac don't support Shared Project references from .NET Standard libraries.

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

[![Build status](https://build.mobile.azure.com/v0.1/apps/e53e8e26-f6c5-4bb0-823b-355acf4d6100/branches/master/badge)](https://mobile.azure.com)
