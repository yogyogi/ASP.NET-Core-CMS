# Shree RAM CMS - the world's first ASP.NET-Core-CMS
![Shree RAM CMS](https://raw.githubusercontent.com/yogyogi/ASP.NET-Core-CMS/master/sree-ram-cms.png)

This is an Open-Source Content Management System(CMS) developed with [ASP.NET Core](http://www.yogihosting.com/category/aspnet-core/) framework by Microsoft. Use it to create any type of Responsive websites with unlimited pages and unlimited blogs.

## Programming Language, Web Technologies, Frameworks and Scripts used to Build this CMS.

I have build this CMS using the following:

1. ASP.NET Core 2.0
2. C#
3. Bootstrap 4
4. SQL Server
5. Identity Management
6. Entity Framework Core

## Installation

Download the CMS files in your system and click the .sln file to open it with Visual Studio 2017 or newer version. You need to follow the following Steps:

## Step 1: Change connection string

Then open the `appsettings.json` file given in the root of the CMS and change the connection string to your database. By default it is:

`
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=vaio;Database=CMSCore;Trusted_Connection=True;",
    "IdentityConnection": "Server=vaio;Database=CMSCoreIdentity;Trusted_Connection=True;"
 }
}
`
To run this CMS you will need 2 databases. One that will store pages, blogs, images, etc. The other one will be used by Identity Management to do authentication and authorization of Admin User.

You don't have to learn Identity for using this CMS but if you still like then visit [How to Setup and Configure Identity Membership System in ASP.NET Core](http://www.yogihosting.com/aspnet-core-identity-setup/)

## Step 2: Run EF Core Migration commands

The [EF Core Migrations[http://www.yogihosting.com/migrations-entity-framework-core/) commands will create both the databases for this CMS.

Open Package Manage Console and go to the directory of the Startup.cs class:

`PM> cd CMS`




