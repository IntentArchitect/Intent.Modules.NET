# Deployment

## Publish the application

```cmd
dotnet publish --output "C:\custom\publish\directory"
```

## Windows Service Installation

From a command console (Terminal) running as an administrator.

### Install Windows Service

```cmd
sc.exe create "BugSnagTest.ServiceHost" binpath="C:\custom\publish\directory\BugSnagTest.ServiceHost.exe"
```

### Remove Windows Service

```cmd
sc.exe delete "BugSnagTest.ServiceHost"
```

### Start Windows Service

```cmd
sc.exe start "BugSnagTest.ServiceHost"
```

### Stop Windows Service

```cmd
sc.exe stop "BugSnagTest.ServiceHost"
```

## Check you service in Windows

- Press `Windows Key` + 'R'
- Type `Services`
- Look for your service by name "BugSnagTest.ServiceHost"