using System;
using System.Collections.Generic;
using UnityEngine;

namespace StationXYZ.Core
{
    [Serializable]
    public class TurnClock
    {
        public int turnDurationSeconds = 10;
        public int actionPointsPerTurn = 6;

        public Turn currentTurn;

        public void StartNewTurn(float currentTime)
        {
            currentTurn = new Turn
            {
                startTime = currentTime + .3f,
                endTime = currentTime + turnDurationSeconds + .3f,
                events = new List<TurnEvent>()
            };
        }

        public float GetTurnRatio(float currentTime)
        {
            var duration = currentTurn.endTime - currentTurn.startTime;
            var ratio = (currentTime-currentTurn.startTime) / duration;
            return Mathf.Clamp(ratio, 0, 1);
        }

        public int GetUsedActionPointCount()
        {
            return currentTurn.events.Count;
        }
    }

    [Serializable]
    public class Turn
    {
        public float startTime;
        public List<TurnEvent> events;
        public float endTime;
    }

    [Serializable]
    public class TurnEvent
    {
        public float time;
        public int turnNumber;
        public string description;
    }

    [Serializable]
    public class MoveEvent : TurnEvent
    {
        public Vector3Int target;
    }
}