using UnityEngine;

namespace StationXYZ.Core
{
    public partial class GameListenerBehaviour : MonoBehaviour, IGameListener
    {
        public Game game;
        public GameBehaviour gameBehaviour;
        public void OnAdd(Game addedToGame)
        {
            game = addedToGame;
            OnGameStart();
        }

        public virtual void OnGameStart()
        {
            
        }

        public T Instantiate<T>(T prefab) where T : GameListenerBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.gameBehaviour = gameBehaviour;
            game.AddListener(instance);
            return instance;
        }
    }

}