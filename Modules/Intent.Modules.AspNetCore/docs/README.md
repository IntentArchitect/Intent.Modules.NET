# Intent.AspNetCore

This module generates base code for hosting Web services on the ASP.NET Core infrastructure.

## How to disable HTTPS Redirect

By default, the `UseHttpsRedirection` statement is **included** in the application's HTTP request pipeline. This can be disabled using the `Enable HTTPS Redirect` setting, found on the Application Settings screen:

![HTTPS enabled](images/https-enable.png)