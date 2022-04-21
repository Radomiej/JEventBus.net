using System;

namespace Javity.EventBus.Exceptions
{
    public class JEventException : Exception
    {
        public JEventException(string printLog, Exception anyException) : base($"Exception throw: {anyException.Message} \n Events chain: { printLog }", anyException)
        {
        }
    }
}