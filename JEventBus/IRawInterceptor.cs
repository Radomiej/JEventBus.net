﻿namespace JEventBus
{
    public interface IRawInterceptor
    {
        void SubscribeRaw(object incomingEvent);
        int GetPriority();
    }
}