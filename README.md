# rad-event-bus.net
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

bus.Post(fakeOneEvent);

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
