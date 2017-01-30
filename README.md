# Shift
Shift background or long running jobs into reliable and durable workers out of the main client app process. 

**Features:**
- Reliable and durable background / long running jobs.
- Out of band processing of long running jobs. 
- Ability to stop, reset, and restart long running jobs.
- Optional detailed progress tracking for each running jobs.
- Scale out with multiple shift managers to run large number of jobs.
- Optional encryption for serialized data.
- Run Shift Server in your own .NET apps, Azure WebJobs, or Windows services. 

**Shift Client**
The client component allows clieat apps to add jobs and send commands to Shift server to stop, delete, reset, and run jobs.


**Shift Server**
The server component gathers available jobs and executes commands from clients. The server is a simple .NET library and needs to run inside a container app, Azure WebJob, or Windows service. 

Job status:
- None, ready to run
- Running
- Stopped
- Completed
- Error

Two runnable server apps projects are included as quick start templates. The Shift.WinService is the standalone Windows service server component, multiple services can be installed in the same server. The Shift.WebJob is the Azure WebJob component that can be easily deployed to Azure cloud environment, multiple web jobs can also be deployed. If you're using Azure, it is highly recommended to locate the Azure SQL and Azure Redis within the same region as the Shift web jobs.
