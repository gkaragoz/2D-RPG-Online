using ShiftServer.Base.Auth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ShiftServer.Base.Core
{
    /// <summary>
    /// Management of server-client messages
    /// </summary>
    public class ServerDataHandler
    {

        public List<ServerEventCallback> serverEvents = null;
        public List<PlayerEventCallback> playerEvents = null;

        public ServerDataHandler()
        {
            serverEvents = new List<ServerEventCallback>();
            playerEvents = new List<PlayerEventCallback>();
        }

        private static ShiftServerData data = null;

        public void HandleMessage(byte[] bb, ShiftClient client, log4net.ILog log)
        {
            data = ShiftServerData.Parser.ParseFrom(bb);

            switch (data.Basevtid)
            {
                case MSBaseEventId.PlayerEvent:
                    ServerEventInvoker.Fire(playerEvents, data, client, log);
                    break;
                case MSBaseEventId.ServerEvent:
                    ServerEventInvoker.Fire(serverEvents, data, client, log);
                    break;
                default:
                 
                    break;

            }
        }

        public class ObjectPool<T>
        {
            private ConcurrentBag<T> _objects;
            private Func<T> _objectGenerator;

            public ObjectPool(Func<T> objectGenerator)
            {
                if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
                _objects = new ConcurrentBag<T>();
                _objectGenerator = objectGenerator;
            }

            public T GetObject()
            {
                T item;
                if (_objects.TryTake(out item)) return item;
                return _objectGenerator();
            }

            public void PutObject(T item)
            {
                _objects.Add(item);
            }
        }
    }
}
