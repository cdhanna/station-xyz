using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BrewedInk.WFC;

namespace StationXYZ.LevelGeneration
{
    [CreateAssetMenu(fileName = "New GridWFCConfig", menuName = "BrewedInk WFC/GridWFCConfig")]
    public class GridWFCConfig : WCFConfigObject<GridWFCConfigModuleObject, GridWFCConfigModule>
    {
        public Vector2Int Size;

        protected override GenerationSpace CreateSpace()
        {
            var constraintGen = new GridConstraintProducer();
            var moduleSet = new ModuleSet(GetModules().ProduceConstraints(constraintGen));
            return GenerationSpace.From2DGrid(Size.x, Size.y, moduleSet);
        }
    }

    [System.Serializable]
    public class GridWFCConfigModule : Module
    {
        // data about your module goes here
        public string typeName;

        public Color debugColor;
        
        public override int GetHashCode()
        {
            return Display.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is GridWFCConfigModule other)
            {
                return other.Display.Equals(Display);
            }

            return false;
        }
    }

    [System.Serializable]
    public class GridWFCConfigModuleObject : ModuleObject<GridWFCConfigModule>
    {
        // visual layer metadata about the Module
    }

    public class GridConstraintProducer : ConstraintGenerator<GridWFCConfigModule>
    {
        public override GridWFCConfigModule Copy(GridWFCConfigModule module, List<ModuleConstraint> constraints)
        {
            return new GridWFCConfigModule
            {
                typeName = module.typeName,
                Constraints = constraints.ToList(),
                Display = module.Display,
                Weight = module.Weight,
                debugColor = module.debugColor
            };
        }

        public override List<ModuleConstraint> CreateConstraints(GridWFCConfigModule source)
        {
            var constraints = new List<ModuleConstraint>();
            
            // for now, every module must exist.
            constraints.Add(new MustExistConstraint(source));

            return constraints;
        }

        public override List<ModuleConstraint> CreateConstraints(GridWFCConfigModule source, GridWFCConfigModule target)
        {
            var constraints = new List<ModuleConstraint>();
            // constraints.Add(new MustExistConstraint(target));
        
            return constraints;
        }
    }

    public class MustExistConstraint : ModuleConstraint
    {
        private readonly Module _requiredModule;

        public MustExistConstraint(Module requiredModule)
        {
            _requiredModule = requiredModule;
        }

        public override bool ShouldRemoveModule(SlotEdge edge, GenerationSpace space, Module module,
            ModuleSet modulesToRemove)
        {
            
            // if this edge.source HAS a module that is any module
            /*
             * should you remove the module? Well, if it ISN'T the module that must be kept...
             * If no other slot has the module that needs to be kept, but this current one does, then we should remove every module exist the one to save.
             */
            var set = space.GetSlotOptions(edge.Source);
            var hasRequiredModuleInSource = set.Contains(_requiredModule);
            if (!hasRequiredModuleInSource) return false; // nothing to do here, we don't have the module anyway, so put our hand sup in the air. 
            
            // ah, we DO have the required module. That means that if the module isn't possible anywhere else, we should remove everything but the required. 
            if (module.Equals(_requiredModule)) return false;
            var foundElsewhere = false;
            foreach (var slot in space.Slots) // for every slot in the world...
            {
                if (slot.Equals(edge.Source)) continue; // except the one we are thinking about...
                var options = space.GetSlotOptions(slot);
                if (options.Contains(_requiredModule))
                {
                    return false; // okay, it exists somewhere else.
                }
            }

            return true;
        }
        
        
        public bool ShouldRemoveModule2(SlotEdge edge, GenerationSpace space, Module module, ModuleSet modulesToRemove)
        {

            
            
            
            
            
            /*
             * Should we remove the slot from the edge.Source set because the constraint has been broken?
             * The constraint is only broken if NO ONE else has the module in its possibility space. 
             */
            
            // check every slot in the space
            foreach (var slot in space.Slots) // for every slot in the world...
            {
                if (slot.Equals(edge.Source)) continue; // except the one we are thinking about...
                var options = space.GetSlotOptions(slot);
                var available = options.ModuleList;

                if (space.TryGetOnlyOption(slot, out var selected))
                {
                    // its been selected as the ONLY option somewhere else... 
                    // but we don't NEED to remove it from ourselves... 
                }
                
                if (options.Contains(_requiredModule)) // if the module exists in some other slot's possibility space...
                {
                    return false; // then we DONT need to remove this.
                }
            }

            // we didn't find the slot anywhere... so we CANNOT remove the module from ourselves
            return false;
        }
    }

}