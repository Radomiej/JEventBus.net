﻿namespace Javity.EventBus
{
    public interface IRawInterceptor
    {
        void SubscribeRaw(object incomingEvent);
        int GetPriority();
    }
}