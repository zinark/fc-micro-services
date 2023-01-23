# Creating New Microservice

The following service package should be connected to a newly created service. First create a brand new web-api application in your ide. Then install this nuget.

```
nuget install fc.micro.services
```

## Program.cs

A minimum start can be made to the Program.cs file as follows. The dbcontext needed by the microservice is given and it is run with Run() method. By default, the connection string is tried to be retrieved by searching for a key called "DB".

```cs
Microservice
    .Create(args)
    .UseDbContext<HelloWorldDbContext>()
    .Run();


```

## Events / Subcriptions

If we want the service to listen to an event, a code like the one below will suffice. It listens for the ``CalculationFinished`` event and executes ``CalculationFinishedSubscription`` when it receives the message.

```cs
Microservice
    .Create(args)
    .WithSubscription<CalculationFinished>()
    .Run();

```

Sample Event:

```cs
using fc.micro.services.Core.Microservice.Components.BUS.Events;

namespace fc.micro.services.Hello.Microservice.Messages.Events
{
    [Event]
    public class CalculationFinished
    {
        public int Result { get; set; }
    }
}

```

Sample Event Subscription:

```cs
using fc.micro.services.Core.Microservice.Components.BUS.Events;
using fc.micro.services.Core.Microservice.Extensions;

namespace fc.micro.services.Hello.Microservice.Messages.Events
{
    public class CalculationFinishedSubscription : EventSubscription<CalculationFinished>
    {
        public override void OnEvent(CalculationFinished input)
        {
            Console.WriteLine("Message received!" + input.ToJson(true));
        }
    }
}

```



## Probe'lar

We understand the status of a service with probes. The following are the default values. Here is an example of the code that says I am healthy in any case. Probes are used during virtualization. [Configure Liveness, Readiness and Startup Probes | Kubernetes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/)
 

* Liveness http://localhost:5000/liveness
  
  * There may be cases where a pod opens very slowly. Killing with Liveness is prevented. If Liveness is false, it will be deleted and restarted.

* Readiness http://localhost:5000/readiness
  
  * This service allows you to decide whether to receive a request or not. It cannot be done. Shall I provide request traffic to the pod? says kubernetes

* Startup http://localhost:5000/startup
  
  * It allows to query whether the application is ready or not. Liveness and readiness are treated as false in the startup = false case. If everything is ok, Kubernetes deploys the pod to the network.

```cs
Microservice
    .Create(args)
    .WithProbes(x => x
        .SetLiveness<DefaultLivenessHealthCheck>()
        .SetReadiness<DefaultReadinessHealthCheck>()
        .SetStartup<DefaultStartupHealthCheck>()
    )
    .Run();

```

## IConfigLoader

With ConfigLoader, it is provided to get settings first from environment and then from appsettings.

```cs
var a = IConfigloader.Load ("a", "1");
```

In the above case, if the a variable is passed in appsettings, it takes the value there. It is useful for us when debugging while working in local.

```json
{
    "a" : 200
}
```

The variable a is overwritten to 200. If it is not in appsettings, it may come from environment. This happens mostly with docker run.

```
docker run microservice -e a 100
```

The variable a will be crushed to 100 in the environment it is running.



The default value of 1 is the value to be assigned if nothing is found. It is optional, if it is not given, it means no default value. And we force it to be in env and appsettings.


## Settings for api

* use_pubsub : "yes" / "no"

* use_tracer : "yes" / "no"

Components to be used in appsettings.json can be determined. If Event/Subscription is to be used, use_pubsub must be marked yes. If trace is to be done, use_tracer must be yes in the same way.

```

    "use_pubsub": "yes",
    "use_tracer": "yes"
```



## Kestrel Endpoints

To change the endpoints, the Kestrel part in appsettings.json can be given as follows. Which ports are required for GRPC and HTTP.

```
 "Kestrel": {
        "Endpoints": {
            "http": {
                "Url": "http://0.0.0.0:5000",
                "Protocols": "Http1"
            },
            "grpc": {
                "Url": "http://0.0.0.0:5001",
                "Protocols": "Http2"
            }
        }
    },
```


When the Console application runs, it will give an output like the one below.

```
PROBES
=========================
{
  "liveness": null,
  "readiness": null,
  "startup": null
}
subscribing... HesaplamaBitti

SUBSCRIPTIONS
=========================
[
  "HesaplamaBitti"
]

ConfigLoader
=========================
{
  "DB": "Host=localhost;Database=hello-microservice;Port=5432;Username=postgres;Password=1234",
  "use_pubsub": "yes",
  "use_tracer": "yes",
  "PUBSUB_URL": "http://localhost:4222",
  "LOG_COLLECTOR": "http://localhost:14268",
  "LOG_AGENT": "localhost",
  "LOG_AGENT_PORT": "6831"
}
warn: Microsoft.AspNetCore.Server.Kestrel[0]
      Overriding address(es) 'http://localhost:5000'. Binding to endpoints defined via IConfiguration and/or UseKestrel() instead.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\repos\microservice\hello.microservice\
```

# Enterprise Service BUS

We run commands and queries with the bus. When we need to throw an event, we can broadcast or look at the result of another service with BUSClient.

### Command:

### Query:

### Event:

### Dependency injection in Handlers

### service inspection with api/info

### BUS usage

### Publisher usage on BUS



# Connecting other micro-services to the microservice

In order to include and call messages from other microservices in the project, the following package must be installed.

```
nuget install [TODO:MicroMessages]
```

### BUS and BUSClient example


# Docker microservice and send to harbor

# Using one (more than one) dockerized service on the same machine