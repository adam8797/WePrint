# WePrint Web
A Drexel Senior Design Project

## Abstract
WePrint allows customers to find makers to print their 3D models. The site has two sides: makers and customers. For customers WePrint provides fair prices for services by implementing a blind bidding system. Customers can post a job and receive bids from makers, who can see all the details of the job but not other makers’ bids. This provides maximum value by finding customers fair prices and allowing makers to make effective use of their printers at prices they are comfortable with.

## Build Instructions

## Development Instructions

### Migrations

We're using Entity Framework with Migrations.

When you make a new data model, you'll need to add a new migration. You can either use `Add-Migration -o Data/Migrations [Migration Name]` or `ef migrations add -o Data/Migrations [Migration Name]`

Then run `Update Database` or `ef database update`. If you are updating the Staging or Production databases, run `$env:ASPNETCORE_ENVIRONMENT='[Environment]'` first