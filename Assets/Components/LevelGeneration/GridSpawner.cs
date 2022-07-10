using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrewedInk.WFC;

namespace StationXYZ.LevelGeneration
{
    public class GridSpawner : MonoBehaviour
    {
        public GridWFCConfig Config;

        public SpriteRenderer cellPrefab;
        
        public GenerationSpace space;
        
        // Start is called before the first frame update
        void Start()
        {
            Generate();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        [ContextMenu("Validation")]
        public void Validate()
        {
            var validation = space.Validate();
            Debug.Log("valid: " + validation);
        }
        
        
        /*
         *
         * Make this validate at each step and see where the error happens.
         * The constraints should be invalidated when it picks random slots 
         * 
         */

        public void Generate()
        {
            space = Config.Create();
            space.Collapse().RunAsCoroutine(this).OnCompleted(() =>
            {
                Debug.Log("complete!");

                foreach (var slot in space.Slots)
                {
                    if (!space.TryGetOnlyOption(slot, out var module))
                    {
                        Debug.LogWarning("Slot had no module!");
                        continue;
                    }

                    var sprite = GameObject.Instantiate(cellPrefab, slot.Coordinate, Quaternion.identity, transform);
                    if (!Config.TryGetObject(module, out var gridModule))
                    {
                        Debug.LogWarning("module doesn't exist");
                        continue;
                    }

                    sprite.color = gridModule.Module.debugColor;
                }
                
            });
        }
    }
}
