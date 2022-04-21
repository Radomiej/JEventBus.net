using System;
using System.Collections.Generic;
using System.Text;

namespace Javity.EventBus.DebugStage
{
    public class SubscriptionStage
    {
        private List<StageContext> _chainOfEvents = new List<StageContext>();
        private StageContext _last;

        public string PrintLog()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var stageContext in _chainOfEvents)
            {
                stageContext.Print(stringBuilder);
                stringBuilder.AppendLine("----------------------------------");
            }
            
            return stringBuilder.ToString();
        }

        public void AddEvent(object eventObject)
        {
            var stageContext = new StageContext(eventObject);
            _chainOfEvents.Add(stageContext);
            _last = stageContext;
        }

        public void AddDelegate(Delegate delegateToInvoke)
        {
            _last.AddDelegate(delegateToInvoke);
        }
    }
}