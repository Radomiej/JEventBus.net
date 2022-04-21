using System;
using System.Collections.Generic;
using System.Text;

namespace Javity.EventBus.DebugStage
{
    public class StageContext
    {
        private readonly object _eventObject;
        private readonly List<Delegate> _delegates = new List<Delegate>();

        public StageContext(object eventObject)
        {
            _eventObject = eventObject;
        }

        public void AddDelegate(Delegate delegateToInvoke)
        {
            _delegates.Add(delegateToInvoke);
        }

        public void Print(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"Event: {_eventObject.GetType().FullName}");
            foreach (var dDelegate in _delegates)
            {
                stringBuilder.AppendLine($"| -> Listener: {dDelegate.Target.GetType().Name} . {dDelegate.Method.Name}"); 
            }
        }
    }
}