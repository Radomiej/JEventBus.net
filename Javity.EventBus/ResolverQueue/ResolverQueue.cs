using System;
using System.Collections.Generic;
using Javity.EventBus.Exceptions;

namespace Javity.EventBus.ResolverQueue
{
    public class ResolverQueue : IRawInterceptor
    {
        private readonly JEventBus _eventBus;
        private readonly HashSet<Type> _storedEvents = new HashSet<Type>();
        private readonly Dictionary<Type, List<object>> _stored =
            new Dictionary<Type, List<object>>();

        private readonly StopPropagationException _stopPropagationException = new StopPropagationException();
        
        public ResolverQueue(JEventBus eventBus)
        {
            _eventBus = eventBus;
            eventBus.Register(this);
            _eventBus.AddInterceptor(this);
        }
        
        [Subscribe]
        public void AddEventToResolveListener(AddEventToResolve addEventToResolve)
        {
            Type eventType = addEventToResolve.EventTypeToResolve;
            if (_storedEvents.Contains(eventType))
            {
                return;
            }
            _storedEvents.Add(eventType);
            _stored.Add(eventType, new List<object>());
        }
        
        [Subscribe]
        public void ResolveEventListener(ResolveEvent resolveEvent)
        {
            Type eventType = resolveEvent.EventTypeToResolve;
            if(!_storedEvents.Contains(eventType)) return;
            _storedEvents.Remove(eventType);
            
            foreach (var eventToResolve in _stored[eventType])
            {
                _eventBus.Post(eventToResolve);
            }

            _stored.Remove(eventType);
        }
        
        
        public void SubscribeRaw(object incomingEvent)
        {
            Type eventType = incomingEvent.GetType();
            if(!_storedEvents.Contains(eventType)) return;
            
            _stored[eventType].Add(incomingEvent);
            throw _stopPropagationException;
        }

        public int GetPriority()
        {
            return 0;
        }
    }
}