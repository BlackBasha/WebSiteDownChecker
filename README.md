WebSite Down Checker
========================================
**Used technology and Libraries:**

- Asp MVC Core 5.0
- Entity FrameWrok 5.0
- Autofac 
- Fluent Scheduler
- Log4net
- Polly 
- RabbitMq
- Automapper.
- SQL Server

**How to start the application:**

Inside the application I am using EF code first so you have to change the connection string then create the database using the EF command.

The command will create the database with the name TestCase, inside it there are the authentication and authorization tables and the main two tables, TargetApp wich contains the information of the application that we would like to check, and Applog that contains the results of the check requests.

I used dockers image for the rabbetMq so to get the image you have to run this command:

**docker run -d --hostname my-rabbit --name my-rabbit -p 15672:15672 -p  5672:5672 rabbitmq:3-management**

this command will download the image and start the container , the default configuration shows that the amqp port is 5672 and the http port is 15672 so you can visit the app form the browser:

<http://localhost:15672/#/queues>

with the default user name and password: guest:guest. After login you have to create a new queue called “[**Message**](http://localhost:15672/#/queues/%2F/EmailMessage)”.

For more information please visit this link : <https://hub.docker.com/_/rabbitmq?tab=description>.

In this class EmailNotifierHandler you have to set your Email configurations.



**The structure of the app:**

**Entities Project:** Contains the main entities of the application including the identity user.

**DataAccess Project:** contains the repositories and the EF main migrations and context.

**Business Project:** contains the main Services of the entities an the autofac main objects initial config, also the jobs configuration classes.

**HeartBeatJobRegistry class:** get the jobs deninition for database and insilize them according to their checkinterval

**HeartBeatJob class:** start the executing process and checking the websites and raise events if the website that we check cannot be reach to message queue to be send as an email.

**Core Project:** contains the core classes of the app, which could be used as a core of any application.

Aspect: indicate the tracking of the logging and exception on the method level of the application.

CrossCuttingConcerns: the operations that could be used in all the projects like the logging and the notification. We are here using log4net, and you can find form the configuration that you can write to db or a file.

DataAccess: contains the base classes for the repository pattern.

Enitities: contains that base entity classes.

EventBus: contains the main structure for the Event Driven pattern and the RabbetMQ base classes.

HostServices: contains the main extension methods of the service Configuration of ASP MVC startup file. 

Utitilies: contains the main business rules class that we use inside the services to handle our own business rules inside code.

Interceptors: contains the main interceptors 

Results: contains the base classes that we user to return our data from the controllers, so all the return types will be in standard form.

**WebApplication project:** is a ASP MVC application with base login logout register operations and a base UI for CRUD operation of  the Target Applications and the log of each app.

Inside the TargetAppController you will notice that I control the information of the TargetApp object a nd according to the operation type if it is create or update or delete I change the job information inside the jobManager and the scheduler of the fluent Scheduler Dll.

//add job schedual

`            `IJob job = new HeartBeatJob(item, \_appLogService, \_emailNotifier);

`            `JobManager.AddJob(job, s => s.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.CheckInterval).Minutes());

**How the application works:**

When you start the app you have to register with a new user lese you can not access that TargetApp page to do the CURD operations.

After login you will see the main TargetApp List after adding the website information that you want immediately the job will start according to the check interval that you specified. After that the job we try to access the website if there is a problem to access that app a notification as a mail will be send to the RappitMq message queue, after the message received an event trigger happen and a consumer will handle it and in our case here it is a mail sender the mail sender will get the message and send a mail to a receiver.

**Used Patterns and structure:**

- Repository 
- Aspect oriented
- IOC
- Message queueing
- Event Driven
- Centralized logging 
- Code first EF

**Notes**:

- You can easily add new notifications type to the application, we are using dependency injection, so you can apply that by do inheritance  form the base classes and interfaces:
  - interface INotifier<T>: the main notification interface so for example you can implement SMS notification by using it for example I created EmailNotifier class
  - class Message: this class represents the main message that could be used to send content to receiver so you can inherit form it .
- As we are using Pub/Sub pattern and message queue we can define different receivers and different senders also different queues as we need to extend the performance for example and this make the structure more dynamic.
- All errors and exceptions are logged to local file and you can find that form the log4net configuration file


