using System;
using System.Linq;
using UnityEngine;

namespace StationXYZ.Core
{
    [Serializable]
    public struct GameState
    {
        public PlayerState[] players;


    }

    public static class GameStateExtensions
    {
        
        public static bool TryGetPlayerIndex(this ref GameState state, long id, out int index)
        {
            // TODO: shitty.
            
            for (var i = 0 ; i < state.players.Length; i ++)
            {
                if (state.players[i].playerId == id)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

    }
    

    [Serializable]
    public struct PlayerState
    {
        public Vector2Int position;
        public long playerId;
    }
}