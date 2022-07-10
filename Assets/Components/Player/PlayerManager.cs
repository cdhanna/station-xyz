using System.Collections;
using StationXYZ.Core;
using UnityEngine;

namespace StationXYZ.Player
{
    public class PlayerManager : GameListenerBehaviour
    {
        public WorldGridBehaviour grid;
        public PlayerBehaviour playerPrefab;
        
        public override void OnGameStart()
        {
            // game.AddEventToNextFrame(new PlayerJoinGameEvent());
        }

        public override IEnumerable Handle(PlayerJoinGameEvent evt, GameEventArgs args)
        {
            var player = Instantiate(playerPrefab);
            player.Setup(evt, args);
            // equip player with args?
            
            yield break;
        }
    }
}