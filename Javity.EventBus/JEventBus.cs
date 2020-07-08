using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Javity.EventBus.Exceptions;
using Javity.EventBus.Utils;

namespace Javity.EventBus
{
    public class JEventBus
    {
        public enum InterceptorType
        {
            Pre,
            Post,
            Unhandled,
            Aborted
        }

        private static Dictionary<string, JEventBus> _eventBuses = new Dictionary<string, JEventBus>();
        private static JEventBus _defaultInstance;

     

        public static JEventBus GetEventBusByName(string eventBusName)
        {
            return _eventBuses[eventBusName];
        }

        public static JEventBus GetDefault()
        {
            if (_defaultInstance == null)
            {
                _defaultInstance = new JEventBus();
                _defaultInstance.Init();
            }

            return _defaultInstance;
        }


        private readonly Dictionary<Type, SortedList<PriorityDelegate>> _subscriptions =
            new Dictionary<Type, SortedList<PriorityDelegate>>();

        private readonly Dictionary<object, List<PriorityDelegate>> _receivers =
            new Dictionary<object, List<PriorityDelegate>>();

        private readonly SortedList<IRawInterceptor> _preInterceptors =
            new SortedList<IRawInterceptor>(new RawInterceptorComparer());

        private readonly SortedList<IRawInterceptor> _postInterceptors =
            new SortedList<IRawInterceptor>(new RawInterceptorComparer());

        private readonly SortedList<IRawInterceptor> _unhandledInterceptors =
            new SortedList<IRawInterceptor>(new RawInterceptorComparer());

        private readonly SortedList<IRawInterceptor> _abortedInterceptors =
            new SortedList<IRawInterceptor>(new RawInterceptorComparer());

        private string _name;
        public bool PerformanceMode { get; set; }

        void Init(string eventBusName = "default")
        {
            _name = eventBusName;

            if (!_eventBuses.ContainsKey(_name))
            {
                _eventBuses.Add(_name, this);
            }
        }

        void OnDestroy()
        {
            _eventBuses.Remove(_name);
        }

        private SubscriptionStage _stage;

        public JEventBus()
        {
            PerformanceMode = false;
        }

        public void BeginStage()
        {
            if (_stage != null) throw new NotSupportedException("Before begin new stage you must close a previous one");
            _stage = new SubscriptionStage();
        }

        public void CloseStage()
        {
            foreach (var receiver in _stage.receivers)
            {
                Unregister(receiver);
            }

            foreach (var subscription in _stage.subscriptions)
            {
                foreach (var dDelegate in subscription.Value)
                {
                    _subscriptions[subscription.Key].Remove(dDelegate);
                }
            }

            _stage = null;
        }

        public void AddInterceptor(IRawInterceptor interceptor, InterceptorType interceptorType = InterceptorType.Pre)
        {
            if (interceptorType == InterceptorType.Pre) _preInterceptors.Add(interceptor);
            else if (interceptorType == InterceptorType.Post) _postInterceptors.Add(interceptor);
            else if (interceptorType == InterceptorType.Unhandled) _unhandledInterceptors.Add(interceptor);
            else if (interceptorType == InterceptorType.Aborted) _abortedInterceptors.Add(interceptor);
        }

        public void RemoveInterceptor(IRawInterceptor interceptor,
            InterceptorType interceptorType = InterceptorType.Pre)
        {
            if (interceptorType == InterceptorType.Pre) _preInterceptors.Remove(interceptor);
            else if (interceptorType == InterceptorType.Post) _postInterceptors.Remove(interceptor);
            else if (interceptorType == InterceptorType.Unhandled) _unhandledInterceptors.Remove(interceptor);
            else if (interceptorType == InterceptorType.Aborted) _abortedInterceptors.Remove(interceptor);
        }

        public void Post(object eventObject)
        {
            if (PerformanceMode)
            {
                PropagateEvent(eventObject);
                return;
            }
            
            if (!_subscriptions.ContainsKey(eventObject.GetType()))
            {
                ProcessEventInInterceptors(eventObject, _unhandledInterceptors);
                return;
            }
           
            
            try
            {
                ProcessEventInInterceptors(eventObject, _preInterceptors);
                PropagateEvent(eventObject);
                ProcessEventInInterceptors(eventObject, _postInterceptors);
            }
            catch (StopPropagationException)
            {
                ProcessEventInInterceptors(eventObject, _abortedInterceptors);
            }
        }

        private void PropagateEvent(object eventObject)
        {
            SortedList<PriorityDelegate> receiverDelegates = _subscriptions[eventObject.GetType()];
            if(receiverDelegates == null) return;
            
            int receiversCount = receiverDelegates.Count;
            for (int i = 0; i < receiversCount; i++)
            {
                if (receiverDelegates[i].PerformanceMode)
                {
                    ((IPerformanceSubscriber)receiverDelegates[i]).SubscribeRaw(eventObject);
                    continue;
                }
                
                Delegate delegateToInvoke = receiverDelegates[i].Handler;
                try
                {
                    delegateToInvoke?.DynamicInvoke(eventObject);
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    if (targetInvocationException.InnerException != null &&
                        targetInvocationException.InnerException is StopPropagationException stopPropagationException)
                        throw stopPropagationException;
                    throw;
                }
            }
        }

        private void ProcessEventInInterceptors(object eventObject, SortedList<IRawInterceptor> handlers)
        {
            foreach (var interceptor in handlers)
            {
                try
                {
                    interceptor.SubscribeRaw(eventObject);
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    if (targetInvocationException.InnerException != null &&
                        targetInvocationException.InnerException is StopPropagationException stopPropagationException)
                        throw stopPropagationException;
                    throw;
                }
            }
        }

        public void RegisterFast<T>(PerformanceSubscriber<T> subscriber)
        {
            AddReceiver(subscriber);
            AddPerformanceSubscription(subscriber);
            _receivers[subscriber].Add(subscriber);
        }

        private void AddPerformanceSubscription<T>(PerformanceSubscriber<T> subscriber)
        {
            Type type = subscriber.GetEventType();
            if (!_subscriptions.ContainsKey(type))
            {
                _subscriptions.Add(type, new SortedList<PriorityDelegate>());
            }

            _subscriptions[type].Add(subscriber);
            _stage?.AddSubscription(subscriber, type);
        }

        public delegate void RawSubscribe(object incomingEvent);

        public void Register(object objectToRegister, IRawSubscriber subscriber)
        {
            RawSubscribe singleDelegate = subscriber.SubscribeRaw;
            int priority = subscriber.GetPriority();

            AddReceiver(objectToRegister);
            PriorityDelegate priorityDelegate = AddSubscription(subscriber.GetEventType(), singleDelegate, priority);
            _receivers[objectToRegister].Add(priorityDelegate);
        }

        public void Register(object objectToRegister, bool silentMode = false)
        {
            bool alreadyRegistered = !AddReceiver(objectToRegister);
            if (alreadyRegistered)
            {
                if (!silentMode) throw new RegisterObjectTwiceException();
                return;
            }

            MethodInfo[] methods = objectToRegister.GetType().GetMethods(BindingFlags.NonPublic |
                                                                         BindingFlags.Instance | BindingFlags.Public);
            for (int m = 0; m < methods.Length; m++)
            {
                object[] attributes = methods[m].GetCustomAttributes(true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is Subscribe subscribe)
                    {
                        MethodInfo method = methods[m];
                        if (method.GetParameters().Length != 1) continue;

                        ParameterInfo firstArgument = method.GetParameters()[0];
                        List<Type> args = new List<Type>(
                            method.GetParameters().Select(p => p.ParameterType));
                        Type delegateType;
                        if (method.ReturnType == typeof(void))
                        {
                            delegateType = Expression.GetActionType(args.ToArray());
                        }
                        else
                        {
                            args.Add(method.ReturnType);
                            delegateType = Expression.GetFuncType(args.ToArray());
                        }

                        Delegate d = Delegate.CreateDelegate(delegateType, objectToRegister, method.Name);
                        PriorityDelegate priorityDelegate =
                            AddSubscription(firstArgument.ParameterType, d, subscribe.priority);
                        _receivers[objectToRegister].Add(priorityDelegate);
                    }
                }
            }
        }

        private bool AddReceiver(object receiverToRegister)
        {
            if (_receivers.ContainsKey(receiverToRegister)) return false;

            _receivers.Add(receiverToRegister, new List<PriorityDelegate>());
            _stage?.AddReceiver(receiverToRegister);
            return true;
        }

        private PriorityDelegate AddSubscription(Type eventType, Delegate d, int priority = 0)
        {
            PriorityDelegate priorityDelegate = new PriorityDelegate(priority, d);
            if (!_subscriptions.ContainsKey(eventType))
            {
                _subscriptions.Add(eventType, new SortedList<PriorityDelegate>());
            }

            _subscriptions[eventType].Add(priorityDelegate);
            _stage?.AddSubscription(priorityDelegate, eventType);
            return priorityDelegate;
        }

        public void Unregister(object objectToUnregister)
        {
            if (!_receivers.ContainsKey(objectToUnregister)) return;

            MethodInfo[] methods = objectToUnregister.GetType().GetMethods();
            for (int m = 0; m < methods.Length; m++)
            {
                object[] attributes = methods[m].GetCustomAttributes(true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is Subscribe)
                    {
                        MethodInfo method = methods[m];
                        if (method.GetParameters().Length != 1) continue;

                        ParameterInfo firstArgument = method.GetParameters()[0];
                        foreach (PriorityDelegate priorityDelegate in _receivers[objectToUnregister])
                        {
                            _subscriptions[firstArgument.ParameterType].Remove(priorityDelegate);
                        }
                    }
                }
            }

            _receivers.Remove(objectToUnregister);
        }

        public void ClearAll()
        {
            _subscriptions.Clear();
            _receivers.Clear();
            _abortedInterceptors.Clear();
            _unhandledInterceptors.Clear();
            _preInterceptors.Clear();
            _postInterceptors.Clear();
        }
    }
}