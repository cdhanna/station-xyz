using System;
using System.Collections;
using System.Collections.Generic;


namespace StationXYZ.Core
{
    
    public class Game
    {

        public bool isGameOver;
        public long currentFrame;
        public IGameNetStream network;
        public List<IGameListener> listeners = new List<IGameListener>();

        public GameLogicListener _logic = new GameLogicListener();
        private Queue<IGameListener> _pendingListenerAdditions = new Queue<IGameListener>();
        
        
        public Game(IGameNetStream network)
        {
            this.network = network;
            AddListener(_logic);
        }

        public ref GameState State => ref _logic.state;

        public void AddEventToNextFrame(GameEvent evt)
        {
            network.Send(evt);
        }

        public void AddListener(IGameListener listener)
        {
            // listeners will always be added at the _start_ of the next frame.
            _pendingListenerAdditions.Enqueue(listener);
        }

        public IEnumerable Update()
        {
            currentFrame++;

            // add any listeners that were added in the last frame.
            var pendingListeners = new Queue<IGameListener>(_pendingListenerAdditions);
            foreach (var listener in pendingListeners)
            {
                listeners.Add(listener);
                listener.OnAdd(this);
            }
            _pendingListenerAdditions.Clear();
            
            // consume all available network actions and apply them.
            var actions = network.Consume();
            foreach (var evt in actions)
            {
                var outcome = Handle(evt);
                foreach (var progress in outcome)
                {
                    yield return progress;
                }
            }
        }

        private IEnumerable Handle(GameAction action)
        {
            List<IEnumerator> handlers = new List<IEnumerator>();
            foreach (var listener in listeners)
            {
                var generator = listener.Handle(action);
                handlers.Add(generator.GetEnumerator());
            }
            
            // run the handlers in parallel with eachother...
            var handlerIndex = 0;
            while (handlers.Count > 0)
            {
                var handler = handlers[handlerIndex];
                if (handler.MoveNext())
                {
                    yield return handler.Current;
                }
                else
                {
                    handlers.Remove(handler);
                }

                handlerIndex++;
                if (handlerIndex >= handlers.Count)
                {
                    handlerIndex = 0;
                }
            }
        }
        
    }

}
