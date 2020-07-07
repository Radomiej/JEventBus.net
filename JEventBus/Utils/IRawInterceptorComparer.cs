using System;
using System.Collections.Generic;

namespace JEventBus.Utils
{
    public class RawInterceptorComparer : Comparer<IRawInterceptor>
    {
        public override int Compare(IRawInterceptor x, IRawInterceptor y)
        {
            if(x == null || y == null) throw new NotSupportedException("Comparable objects cannot be null");
            return x.GetPriority() - y.GetPriority() ;
        }
    }
}