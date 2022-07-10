using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace StationXYZ.Core
{
    public interface IGameNetStream
    {
        void Send(GameEvent evt);
        Queue<GameAction> Consume();
        long ClientId { get; }
    }

    public static class IGameNetStreamExtensions
    {
            
        [Serializable]
        public class NetworkMessage
        {
            public string gameEventPayload;
            public string gameEventType;
            public GameEventArgs args;
        }

        public static string Serialize(this GameEvent evt)
        {
            var action = new NetworkMessage
            {
                gameEventType = evt.GetTypeName(),
                gameEventPayload = JsonConvert.SerializeObject(evt),
                args = new GameEventArgs
                {
                    description = "from mock network",
                    entityId = 1,
                    receivedAt = 1,
                    sourcedPlayerId = 1
                }
            };
            var json = JsonConvert.SerializeObject(action);
            return json;
        }

        public static GameAction DeserializeToAction(this string message)
        {
            var msg = JsonConvert.DeserializeObject<NetworkMessage>(message);
            var type = msg.gameEventType.GetGameEventType();
            var evt = (GameEvent)JsonConvert.DeserializeObject(msg.gameEventPayload, type);
            return new GameAction
            {
                args = msg.args,
                gameEvent = evt
            };
        }

        public static GameAction ToGameAction(this GameEvent evt, GameEventArgs args)
        {
            return new GameAction(evt, args);
        }
    }

    
    
    public class MockNetworkStream : IGameNetStream
    {


        
        private Queue<string> _events = new Queue<string>();
        
        public void Send(GameEvent evt)
        {
            // TODO: send this on a network stream... We could add some delay to fake it.

            var json = evt.Serialize();
            _events.Enqueue(json); // TODO: this is a SEND on a network...
        }

        public Queue<GameAction> Consume()
        {
            // TODO: the results should be coming async off a network queue. 
            // TODO: decorate the game event results.
            var results = new Queue<string>(_events);
            _events.Clear();

            var actionQueue = new Queue<GameAction>();
            foreach (var result in results)
            {
                var action = result.DeserializeToAction();
                actionQueue.Enqueue(action);
            }

            return new Queue<GameAction>(actionQueue.Reverse());
        }

        public long ClientId => 1;
    }
}