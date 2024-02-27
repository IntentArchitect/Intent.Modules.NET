# Deployment

## Publish the application

```cmd
dotnet publish --output "C:\custom\publish\directory"
```

## Windows Service Installation

From a command console (Terminal) running as an administrator.

### Install Windows Service

```cmd
sc.exe create "WindowsServiceHost.Tests" binpath="C:\custom\publish\directory\WindowsServiceHost.Tests.exe"
```

### Remove Windows Service

```cmd
sc.exe delete "WindowsServiceHost.Tests"
```

### Start Windows Service

```cmd
sc.exe start "WindowsServiceHost.Tests"
```

### Stop Windows Service

```cmd
sc.exe stop "WindowsServiceHost.Tests"
```

## Check you service in Windows

- Press `Windows Key` + 'R'
- Type `Services`
- Look for your service by name "WindowsServiceHost.Tests"