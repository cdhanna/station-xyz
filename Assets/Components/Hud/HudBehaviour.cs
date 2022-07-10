using System;
using System.Collections.Generic;
using StationXYZ.Core;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Components.Hud
{
    public class HudBehaviour : MonoBehaviour
    {
        public GameBehaviour Game;
        public Slider TurnTimerSlider;

        public float sliderVelocity;

        public RectTransform actionPointContainer;

        public ActionPointBehaviour actionPointPrefab;
        public ObjectPool<ActionPointBehaviour> actionPointPool;

        public List<ActionPointBehaviour> actionPoints;

        private void Start()
        {
            actionPointPool = new ObjectPool<ActionPointBehaviour>(
                () => Instantiate(actionPointPrefab, actionPointContainer),
                ap => ap.gameObject.SetActive(true),
                ap => ap.gameObject.SetActive(false));


            for (var i = 0; i < actionPointContainer.childCount; i++)
            {
                Destroy(actionPointContainer.GetChild(i).gameObject);
            }
            for (var i = 0; i < Game.clock.actionPointsPerTurn; i++)
            {
                var ap = actionPointPool.Get();
                actionPoints.Add(ap);
            }
        }

        private void Update()
        {
            var turnTime = Game.clock.GetTurnRatio(Time.realtimeSinceStartup);
            turnTime = Mathf.SmoothDamp(TurnTimerSlider.value, turnTime, ref sliderVelocity, .06f);
            TurnTimerSlider.SetValueWithoutNotify(turnTime);

            // TODO: Make sure there are enough prefabs for the action points per turn.

            for (var i = 0; i < Game.clock.actionPointsPerTurn; i++)
            {
                var pointsRemaining = Game.clock.actionPointsPerTurn - Game.clock.GetUsedActionPointCount();
                var isSpent = pointsRemaining > i;
                var point = actionPoints[i];
                
                point.SetSpent(isSpent);
                if (isSpent)
                {
                }
            }
            
        }
    }
}