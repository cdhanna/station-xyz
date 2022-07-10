
using System;
using System.Collections.Generic;
using Beamable;
using Beamable.Common;
using Beamable.Experimental.Api.Sim;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StationXYZ.Core
{
    public class MultiplayerDriver : MonoBehaviour, IGameNetStream
    {
        private CustomSimClient _simClient;

        private BeamContext _ctx;

        private Queue<GameAction> _messageQueue;

        private long currentTick;

        public string roomName;
        public bool randoRoom;

        public static bool useStaticName;
        public static string staticRoomName;

        public async Promise Init(string roomId, int framesPerSecond)
        {
            if (randoRoom)
            {
                roomName = Random.Range(1, 1000000).ToString();
            }

            if (useStaticName)
            {
                roomName = staticRoomName;
            }
            _ctx = BeamContext.InParent(this);
            await _ctx.OnReady;
            _simClient = new CustomSimClient(new SimNetworkEventStream(roomName, _ctx.ServiceProvider), framesPerSecond, 4);

            _messageQueue = new Queue<GameAction>();
            
            _simClient.OnInit(OnInit);
            _simClient.OnConnect(HandleOnConnect);
            _simClient.OnDisconnect(HandleOnDisconnect);
            _simClient.OnTick(HandleOnTick);
        }

        private void FixedUpdate()
        {
            _simClient?.Update();
        }


        private void OnInit(string seed)
        {
            Debug.Log("MULTIPLAYER-ON_INIT-" + seed);
            _messageQueue.Enqueue(new GameInitGameEvent { seed = seed }.ToGameAction(new GameEventArgs
            {
                description = "The initial seed of the game",
                entityId = -1,
                receivedAt = currentTick,
                sourcedPlayerId = 0 // game event
            }));
        }
        
        private void HandleOnTick(long tick)
        {
            // Debug.Log("MULTIPLAYER-ON_TICK-" + tick);

            currentTick = tick;
            _messageQueue.Enqueue(new GameTickGameEvent
            {
                tick = tick
            }.ToGameAction(new GameEventArgs
            {
                description = $"the event for tick {tick}",
                entityId = -1,
                receivedAt = tick,
                sourcedPlayerId = 0
            }));
        }

        private void HandleOnConnect(string dbid)
        {
            Debug.Log("MULTIPLAYER-ON_CONNECT-" + dbid);

            _messageQueue.Enqueue(new PlayerJoinGameEvent
            {
                playerId = long.Parse(dbid)
            }.ToGameAction(new GameEventArgs
            {
                description = $"player {dbid} connected",
                entityId = -1,
                receivedAt = currentTick,
                sourcedPlayerId = long.Parse(dbid)
            }));
            
            _simClient.OnInternal("message", dbid, body =>
            {
                Debug.Log("MULTIPLAYER-ON_MSG-" + dbid + "/"+body);

                var action = body.DeserializeToAction();
                action.args.receivedAt = currentTick;
                action.args.sourcedPlayerId = long.Parse(dbid);
                _messageQueue.Enqueue(action);
            });
        }

        private void HandleOnDisconnect(string dbid)
        {
           // bleck, TODO.
        }

        public void Send(GameEvent evt)
        {
            var json = evt.Serialize();
            var origin = _simClient.Network.ClientId;
            Debug.Log($"Sending from {origin}: " +json);
            
            _simClient.Network.SendEvent(new SimEvent(origin, "message", json));
        }

        public Queue<GameAction> Consume()
        {
            var clone = new Queue<GameAction>(_messageQueue);
            _messageQueue.Clear();
            return clone;
        }

        public long ClientId => _ctx.PlayerId;
    }
}