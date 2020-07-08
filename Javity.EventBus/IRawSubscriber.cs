﻿using System;

 namespace Javity.EventBus
{
    public interface IRawSubscriber : IRawInterceptor
    {
        Type GetEventType();

    }
}