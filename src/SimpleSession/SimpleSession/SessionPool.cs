using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SimpleSession
{
    /// <summary>
    /// 存放所有Session池子
    /// </summary>
    public class SessionPool
    {
        private readonly ConcurrentDictionary<Guid, SessionObject> _sessionContain = new ConcurrentDictionary<Guid, SessionObject>();

        public SessionObject this[Guid index]
        {
            get
            {
                if (_sessionContain.TryGetValue(index, out var obj))
                {
                    return obj;
                }
                else
                {

                    return _sessionContain.GetOrAdd(index, new SessionObject());
                }
            }
        }
    }
}