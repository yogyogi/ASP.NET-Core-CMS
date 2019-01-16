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

Download the CMS files in your system and click the .sln file to open it with Visual Studio 2017 or newer version.

Then open the `appsettings.json` file given in the root of the CMS and change the connection string to your database. By default it is:

`
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=vaio;Database=CMSCore;Trusted_Connection=True;",
    "IdentityConnection": "Server=vaio;Database=CMSCoreIdentity;Trusted_Connection=True;"
 }
}
`




