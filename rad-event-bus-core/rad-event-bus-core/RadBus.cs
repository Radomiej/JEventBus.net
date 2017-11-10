using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rad.EventBus
{
    public class RadBus
    {
        private static Dictionary<string, RadBus> eventBuses = new Dictionary<string, RadBus>();
        public static RadBus GetEventBusByName(string eventBusName)
        {
            return eventBuses[eventBusName];
        }

        private static RadBus defaultInstance;
        public static RadBus GetDefault()
        {
            if (defaultInstance == null) defaultInstance = new RadBus("default");
            return defaultInstance;
        }
        public static RadBus Default
        {
            get { return GetDefault(); }
        }

        private Dictionary<Type, List<Delegate>> subscribtions = new Dictionary<Type, List<Delegate>>();
        private Dictionary<object, List<Delegate>> recivers = new Dictionary<object, List<Delegate>>();

        public string eventBusName = "default";
        private string _name;

        public RadBus(string name)
        {
            _name = name;
            if (eventBuses.ContainsKey(_name))
            {
                Console.WriteLine("Overriding exist EventBus with name: " + _name);
            }
            eventBuses.Add(_name, this);
        }

        public void Dispose()
        {
            if (defaultInstance == this)
            {
                Console.WriteLine("Cannot dispose default event bus");
            }

            eventBuses.Remove(_name);
            recivers.Clear();
            subscribtions.Clear();
        }

        public void Post(object eventObject)
        {
            if (!subscribtions.ContainsKey(eventObject.GetType()))
            {
                Console.WriteLine("Does not exist reciver for event: " + eventObject.GetType());
                return;
            }

            List<Delegate> reciverDelegates = subscribtions[eventObject.GetType()];
            foreach(Delegate delegateToInvoke in reciverDelegates)
            {
                delegateToInvoke.DynamicInvoke(eventObject);
            }
        }

        public void Register(object objectToRegister)
        {
            recivers.Add(objectToRegister, new List<Delegate>());

            MethodInfo[] methods = objectToRegister.GetType().GetMethods(BindingFlags.NonPublic |
                             BindingFlags.Instance | BindingFlags.Public);
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
                        if (!subscribtions.ContainsKey(firstArgument.ParameterType))
                        {
                            subscribtions.Add(firstArgument.ParameterType, new List<Delegate>());
                        }
                        subscribtions[firstArgument.ParameterType].Add(d);
                        recivers[objectToRegister].Add(d);
                    }
                }
            }
        }

        public void Unregister(object objectToUnregister)
        {
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
                        foreach (Delegate d in recivers[objectToUnregister])
                        {
                            subscribtions[firstArgument.ParameterType].Remove(d);
                        }
                    }
                }

                recivers.Remove(objectToUnregister);
            }
        }
    }
}
