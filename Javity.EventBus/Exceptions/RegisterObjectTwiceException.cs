using System;

namespace Javity.EventBus.Exceptions
{
    public class RegisterObjectTwiceException : Exception
    {
        public RegisterObjectTwiceException() : base("You want register object twice")
        {
            
        }
    }
}