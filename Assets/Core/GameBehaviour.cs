using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StationXYZ.Core
{
    public class GameBehaviour : MonoBehaviour
    {

        public TurnClock clock;
        public Vector3Int HoverCell;
        public Game game;

        public List<GameListenerBehaviour> listeners;

        public MultiplayerDriver multiplayerDriver;

        [SerializeField]
        private GameState _stateClone;

        [SerializeField]
        private long clientId;

        private async void Start()
        {
            //

            if (multiplayerDriver)
            {
                await multiplayerDriver.Init("abc", 4);
                game = new Game(multiplayerDriver);
            }
            else
            {
                game = new Game(new MockNetworkStream()); // LOCAL TESTING.
            }
            
            
            clock.StartNewTurn(Time.realtimeSinceStartup);

            foreach (var listener in listeners)
            {
                listener.gameBehaviour = this;
                game.AddListener(listener);
            }
            game.AddListener(new LogGameListener());
            
            StartCoroutine(RunGame());
        }

        IEnumerator RunGame()
        {
            yield return null; // always wait 1 frame to let other Unity object's initialize.
            
            
            while (!game.isGameOver)
            {
                var turnOutput = game.Update();
                foreach (var output in turnOutput)
                {
                    Debug.Log("turn output: " + output);
                    yield return output;
                }

                yield return null; // always wait 1 frame to let Unity render.
            }
            Debug.Log("Exiting game loop");
        }

        private void OnDestroy()
        {
            game.isGameOver = true;
        }


        private void Update()
        {

            _stateClone = game?.State ?? default;
            clientId = game?.network.ClientId ?? 0;
            // advance the clock?
            var time = Time.realtimeSinceStartup;

            if (time > clock.currentTurn.endTime + .3f)
            {
                clock.StartNewTurn(Time.realtimeSinceStartup);
            }

            // if (Input.GetMouseButtonDown(0))
            // {
            //     clock.currentTurn.events.Add(new MoveEvent()
            //     {
            //         time = Time.realtimeSinceStartup,
            //         target = HoverCell
            //     });
            // }
            
        }
    }
}