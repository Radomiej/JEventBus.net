# JEventBus.net

Event Bus for .NET

![Nuget](https://img.shields.io/nuget/v/Javity.EventBus)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/29e6567aa96e424baf15ad086bbb4276)](https://app.codacy.com/manual/Radomiej/JEventBus.net?utm_source=github.com&utm_medium=referral&utm_content=Radomiej/JEventBus.net&utm_campaign=Badge_Grade_Settings)
![.NET Core](https://github.com/Radomiej/JEventBus.net/workflows/.NET%20Core/badge.svg)

## Fast start:

1. Get Default EventBus:

```csharp 

JEventBus bus = JEventBus.GetDefault();


```

2. Register Subscriber:

```csharp 

bus.Register(mySubscriber);

```

3. Post Event:

```csharp 

bus.Post(myEvent);

```

4. Unregister Subscriber

```csharp 

bus.Unregister(mySubscriber);

```

5. Subscriber example:

```csharp 

class SubscriptionCounter
{
    public int myEventCount = 0;

    [Subscribe]
    public void counterMyEvents(MyEvent e)
    {
        myEventCount++;
    }
}

```
