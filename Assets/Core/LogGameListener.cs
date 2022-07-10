using System.Collections;
using UnityEngine;

namespace StationXYZ.Core
{
    public class LogGameListener : GameListener
    {
        public override IEnumerable Handle(GameAction action)
        {
            // Debug.Log("Game received: " + action.gameEvent);
            return base.Handle(action);
        }
    }
}