# WePrint Web
A Drexel Senior Design Project

## Public Release Note
We've included the entire development tree, and as such there are some secrets exposed in the config files. All relevant services have been deactivated or deleted, so if you want some broken keys, go searching. But I've replaced them all with `(removed)` in the most recent commit

This project was built to functional completion, but was never released to the public

## Abstract
WePrint allows customers to find makers to print their 3D models. The site has two sides: makers and customers. For customers WePrint provides fair prices for services by implementing a blind bidding system. Customers can post a job and receive bids from makers, who can see all the details of the job but not other makers’ bids. This provides maximum value by finding customers fair prices and allowing makers to make effective use of their printers at prices they are comfortable with.

## Post-Pivot Abstract

With the COVID-19 Pandemic, there was an outcry by hospitals for 3D printed face shields to be donated. While WePrint was not originally intended to meet this need, we changed course in order to try and facilitate the process of managing donations, communicating to the printing community, and increasing the volume of donations. 

Our primary purpose is to allow an Organization (a hospital or community group) to start a Project (e.g., 3D printed face shields) and then accept and manage donations from the 3D printing community. These donations can be tracked through the entire process, then ultimately marked as received by the managing organization. 

## Build Instructions

Run `npm install` in the `src/WePrint/ClientApp` directory, then build with Visual Studio


## Migrations

We're using Entity Framework with Migrations.

When you make a new data model, you'll need to add a new migration. You can either use `Add-Migration -o Data/Migrations [Migration Name]` or `ef migrations add -o Data/Migrations [Migration Name]`

Then run `Update Database` or `ef database update`. If you are updating the Staging or Production databases, run `$env:ASPNETCORE_ENVIRONMENT='[Environment]'` first
