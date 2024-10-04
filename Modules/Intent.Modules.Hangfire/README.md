# Intent.Modules.Hangfire

This module provides patterns for time based, as well as fire-and-forget jobs, using Hangfire.

## What is Hangfire?

Hangfire is an open-source library for .NET that facilitates background job processing in a reliable and scalable manner - it allows developers to create, manage, and execute background tasks. Hangfire supports several different kinds of background tasks: short-running and long-running, CPU intensive and I/O intensive, one shot and recurrent

For more information on Hangfire, check out their [official docs](https://www.hangfire.io/).

## What's in this module?

This module creates the required infrastructure to run Hangfire in a .NET web application, as well as Windows service:-

* Hangfire host registration 
* Optional Hangfire Dashboard registration and configuration
* Job generation

## Service Designer

### Hangfire Configuration
In the `Service Designer`, right click on your service package and select the `Add Hangfire` context menu option:

![Add Hangfire](./docs/images/modeled-hangfire.png)

This will add the core Hangfire components to the application, with the following options:

![Hangfire options](./docs/images/hangfire-configuration.png)

#### Hangfire Configuration Options

##### Storage
The storage mechanism utilised by Hangfire. Available options are:
- In Memory: all job information, including job execution history is stored in memory and is lost if the service is restarted
- SQL Server: all job information is stored in SQL Server tables and is persisted even if the service is restarted
- None: no storage mechanism is configured by Intent Architect. The condiguration is entirely up to the developer.

##### Show Dashboard
If this condition is on (the default value), then the Hangfire Dashboard will be exposed using the _Dashboard Url_ 

##### Dashboard URL
The URL on which the Hangfire dashboard will be served. The default is `/hangfire`

##### Dashboard Title
The title which appears on the dashboard when browsing

##### Read Only Dashboard
If this condition is off (the default value), the dashboard can be used to trigger scheduled jobs, or reprocess completed jobs. If this condition is on, the dashboard is read-only and is used for monitoring purposes only.

##### Configure as Hangfire Server
If this condition is on (the default value) the application is configurated as a Hangfire server and will function as a servers, processing Hangfire jobs. If this condition is off, the application will only be able to create/schedule jobs, but not process any jobs.

##### Worker Count
This value indicates the number of paralell internal processors (workers) created to handle job processing (i.e. the maxiumum number of jobs which can be processed in parallel). If left blank (the recommendation) Hangfire will automatically calculate the optimal number of workers based on CPU's available.

##### Job Retention Hours
The time frame, in hours, that the history of completed jobs (successful or deleted) will be retained before expiring and being removed from Hangfire

### Hangfire Jobs and Queues

Once the core Hangfire configuration is completed, one or more jobs and/or queues can be added. In the `Service Designer`, right click on the `Hangfire Configuration` and select the `Add Job` or `Add Queue` context menu option:

![Add Hangfire](./docs/images/modeled-job-queue.png)

#### Queues

Zero or more queues can be added to the Hangfire configuration, which allows for different jobs to be prioritised, or split across different servers (for example):

![Add Hangfire](./docs/images/hangfire-queue-configuration.png)

A queue is not required to be added, and if none are added, a single _default_ queue to will be used for all job processing.

#### Jobs

Zero or more queues can be added to the Hangfire configuration. Adding a job will provide you with the folling job options:

![Add Hangfire](./docs/images/hangfire-job-configuration.png)

##### Name
The unique name of the job

##### Enabled
If this option is on (the default), the job processing handler code is included, otherwise it is excluded from the application. 

##### Job Type
The type of the job being added. The available options:
* Recurring (the default): These jobs are executed many times on a specific CRON schedule
* Delayed: These jobs are executed only once, but only after the specified time interval
* Fire and Forget: These jobs are executed only once, and almost immediately after they are fired.

##### Cron Schedule

Specifies the interval on which the job should executing, using the cron expression format.

> Only applicable when _Job Type_ is **Recurring** 

##### Disallow Concurrent Execution

If this condition is set, then only one instance of the recurring job can be executed at any given time.

##### Concurrent Execution Timeout

The amount of time, in seconds, a duplicate job will wait (if _Disallow Concurrent Execution_ is set) before it is cancelled.

##### Delay Time Frame

The unit of the _Delay Value_, the amount of time delayed before the job is executed.

> Only applicable when _Job Type_ is **Delayed** 

##### Delay Value

The value of the _Delay Time Frame_, the amount of time delayed before the job is executed.

> Only applicable when _Job Type_ is **Delayed** 

##### Retry Attempts

The number of times the processing of the job will be retried, in the case where the job did not complete successfully.

##### On Attempts Exceeded:

When the processing of a job has been retried more than _retry attempts_ number of times, this is the final state the job will be moved into. Options are _Fail_ and _Deleted_

##### Queue

The _optional_ queue on which the job should be queued when executed. If no queue is specified, the _default_ queue will be used.