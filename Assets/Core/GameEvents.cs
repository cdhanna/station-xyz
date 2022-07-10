using System;
using UnityEngine;

namespace StationXYZ.Core
{

    public class GameEvent
    {
    }

    public struct GameEventArgs
    {
        public long sourcedPlayerId;
        public long entityId;
        public long receivedAt;
        public string description;
    }

    public class GameAction
    {
        public GameEvent gameEvent;
        public GameEventArgs args;

        public GameAction()
        {
            
        }

        public GameAction(GameEvent evt, GameEventArgs args)
        {
            gameEvent = evt;
            this.args = args;
        }
    }


    public partial class PlayerJoinGameEvent : GameEvent
    {
        public long playerId;
    }

    public partial class PlayerMoveGameEvent : GameEvent
    {
        public Vector2Int position;
    }
    
    public partial class GameOverGameEvent : GameEvent
    {
        
    }

    public partial class GameInitGameEvent : GameEvent
    {
        public string seed;
    }

    public partial class GameTickGameEvent : GameEvent
    {
        public long tick;
    }
    
    
}