﻿namespace JEventBus
{
    public interface IPerformanceSubscriber
    {
        void SubscribeRaw(object incomingEvent);
    }
}