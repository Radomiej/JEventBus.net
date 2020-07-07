using System;

namespace JEventBus.Exceptions
{
    public class RegisterObjectTwiceException : Exception
    {
        public RegisterObjectTwiceException() : base("You want register object twice")
        {
            
        }
    }
}