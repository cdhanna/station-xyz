using System;
using System.Collections;
using StationXYZ.Core;
using UnityEngine;

namespace StationXYZ.Player
{

    public class PlayerBehaviour : GameListenerBehaviour
    {
        public WorldGridBehaviour grid;

        public long ownerPlayer;

        public override IEnumerable Handle(PlayerJoinGameEvent evt, GameEventArgs args)
        {
            Debug.Log("I shouldn't exist, because this message has already been processed.");
            return base.Handle(evt, args);
        }

        public override void OnGameStart()
        {
            grid = FindObjectOfType<WorldGridBehaviour>();
            base.OnGameStart();
        }

        // public override IEnumerable Handle(GameAction action)
        // {
        //     var baseEnumeration = base.Handle(action);
        //  
        //     return baseEnumeration;
        // }

        private void Update()
        {
            if (game == null) return;
            if (!game.State.TryGetPlayerIndex(ownerPlayer, out var index))
            {
                return;
            }
            var playerState = game.State.players[index];


            var pos = playerState.position;
            transform.position = grid.Grid.CellToWorld(new Vector3Int(pos.x, 0, pos.y));

            if (game.network.ClientId != ownerPlayer) return;
            if (Input.GetMouseButtonDown(1))
            {
                // send an event?
                Debug.Log("Sent event");
                game.AddEventToNextFrame(new PlayerMoveGameEvent
                {
                    position = grid.position
                });
            }
        }

        public void Setup(PlayerJoinGameEvent evt, GameEventArgs args)
        {
            ownerPlayer = args.sourcedPlayerId;
        }
    }
}