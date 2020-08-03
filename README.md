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
## Advenced usage

### Performance mode

Default false. When enabled it disable additional logic in EventBus:

* pre- and post- interceptors are not longer work

How to enable:

```csharp

JEventBus.GetDefault().PerformanceMode = true;

```

### Register subscriber to custom handler object key

```csharp

 var subscriber = new RawSubscriber<TestEvent>(myEvent => MyHandler(incoming));
 JEventBus.GetDefault().Register(this, subscriber);
 JEventBus.GetDefault().PerformanceMode = performanceMode;

```

### Maximum performance boost | Single handler

```csharp

var subscriber = new PerformanceSubscriber<TestEvent>(incoming => MyHandler(incoming));
JEventBus.GetDefault().RegisterFast(subscriber);
JEventBus.GetDefault().PerformanceMode = performanceMode;

```

## Benchmark

Benchmark source you find in: https://github.com/Radomiej/JEventBus.net/blob/master/Javity.EventBusTest/JEventBusTest.cs
look at methods start with *PerformanceTest*

*The test are just for general perfomance insight.*

**Each test based on sent 100k events**

Testing platform:
Laptop with 
* CPU: Intel(R) Core(TM) i7-7700HQ CPU @ 2.80GHz
* RAM: 24 GB


**PerformanceTest #1** (Classic, annotation-based)

Execution time: 869 ms | 869874500 ns | 869 ns/event & performance mode: False

Execution time: 695 ms | 695731800 ns | 695 ns/event & performance mode: True


**PerformanceTest #2** (Special cases, action-based)

Execution time: 556 ms | 556563000 ns | 556 ns/event & performance mode: False

Execution time: 454 ms | 454930900 ns | 454 ns/event & performance mode: True


**PerformanceTest #3** (Need for speed, action-based)

Execution time: 236 ms | 236253800 ns | 236 ns/event & performance mode: False

Execution time: 119 ms | 119565700 ns | 119 ns/event & performance mode: True
