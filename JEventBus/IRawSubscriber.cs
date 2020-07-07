﻿using System;

 namespace JEventBus
{
    public interface IRawSubscriber : IRawInterceptor
    {
        Type GetEventType();

    }
}