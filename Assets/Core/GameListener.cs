using System;
using System.Collections;
using UnityEngine;

namespace StationXYZ.Core
{
    public partial interface IGameListener
    {
        void OnAdd(Game addedToGame);
    }

    public partial class GameListener : IGameListener
    {
        public Game game;
        
        public virtual void OnAdd(Game addedToGame)
        {
            game = addedToGame;
        }
        
    }
}