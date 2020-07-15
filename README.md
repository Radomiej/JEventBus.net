# JEventBus.net

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/29e6567aa96e424baf15ad086bbb4276)](https://app.codacy.com/manual/Radomiej/JEventBus.net?utm_source=github.com&utm_medium=referral&utm_content=Radomiej/JEventBus.net&utm_campaign=Badge_Grade_Settings)

Event Bus for .NET

## Fast start:

1. Get Default EventBus:

```csharp 

RadBus bus = RadBus.Default;

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
