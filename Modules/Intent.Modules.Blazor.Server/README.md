# Intent.Blazor.Server

This module extends a ASP.Net Core application to include Blazor Server.

## What is Blazor Server?

Blazor Server is a web application framework developed by Microsoft that allows developers to build interactive and dynamic web applications using C# and .NET instead of traditional web technologies like JavaScript. In Blazor Server, the application's user interface is rendered on the server and then sent to the client's browser as HTML, with real-time updates and interactivity managed through SignalR, a real-time communication library. This approach enables developers to leverage their existing C# skills and libraries while building modern web applications, making it easier to maintain and secure applications while delivering a rich user experience.

## Whats in this module

This module does the following:

- Wires up Blazor Server hosting in and existing ASP.Net Core application.
- Adds Blazor Server usage example.

## Wires up Blazor Server hosting

Configures you ASP.NET core application as follows:

```csharp

public class Startup
{

    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRazorPages();
        services.AddServerSideBlazor();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseStaticFiles();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
        app.UseStaticFiles();
    }
}
```

## Blazor Server usage example

Modules create a sample working website, identical to the sample you would get through Visual Studio.

The application runs off of `https://localhost:{development port}/`.

> [!NOTE]
> For `FetchData` sample to work you need to register the `WeatherForecastService` service as follows:

```csharp
    public class Startup
    {
    
        ...

        public void ConfigureServices(IServiceCollection services)
        {
            ...
            //IntentIgnore
            services.AddSingleton<WeatherForecastService>();
            ...
        }
        
        ...
    }
```
