using System;
using System.Collections.Generic;

namespace JEventBus
{
    public class SubscriptionStage
    {
        private readonly Dictionary<Type, List<PriorityDelegate>> _subscriptions =
            new Dictionary<Type, List<PriorityDelegate>>();
        
        readonly List<object> _receivers = new List<object>();


        public List<object> receivers => _receivers;

        public Dictionary<Type, List<PriorityDelegate>> subscriptions => _subscriptions;

        public void AddReceiver(object receiverToRegister)
        {
            receivers.Add(receiverToRegister);
        }

        public void AddSubscription(PriorityDelegate priorityDelegate, Type eventType)
        {
            if (!subscriptions.ContainsKey(eventType))
            {
                subscriptions.Add(eventType, new List<PriorityDelegate>());
            }
            subscriptions[eventType].Add(priorityDelegate);
        }
    }
}