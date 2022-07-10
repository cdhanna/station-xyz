using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StationXYZ.Core
{
    public class GameLogicListener : GameListener
    {

        public GameState state = new GameState();
        
        public override IEnumerable Handle(PlayerJoinGameEvent evt, GameEventArgs args)
        {
            Debug.Log("handling player join");
            
            // update the state with a new player
            var nextPlayers = new List<PlayerState>(state.players ?? new PlayerState[]{});
            nextPlayers.Add(new PlayerState
            {
                playerId = args.sourcedPlayerId,
                position = new Vector2Int(1, 5)
            });
            state.players = nextPlayers.ToArray();
            
            return base.Handle(evt, args);
        }

        public override IEnumerable Handle(PlayerMoveGameEvent evt, GameEventArgs args)
        {
            Debug.Log("handling player move");


            if (!state.TryGetPlayerIndex(args.sourcedPlayerId, out var index))
            {
                Debug.LogError("NO PLAYER :(*");
                return base.Handle(evt, args);

            }

            var plrState = state.players[index];
            var currPos = plrState.position;
            var delta = evt.position - currPos;
            delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));
            
            plrState.position += delta; // TODO: validation?

            state.players[index] = plrState;
            Debug.Log("player now has pos : " + plrState.position);
            return base.Handle(evt, args);
        }

        public override IEnumerable Handle(GameOverGameEvent evt, GameEventArgs args)
        {
            return base.Handle(evt, args);
        }
    }
}