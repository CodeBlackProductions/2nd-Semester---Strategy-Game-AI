using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Composite
    {

        public override bool RunAction()
        {
            if (decorators.Count > 0)
            {
                foreach (Decorator decorator in decorators)
                {
                    if (!decorator.CheckCondition())
                    {
                        return false;
                    }
                    
                }
            }

            return RunSequence();

        }

        public bool RunSequence()
        {
            foreach (Node node in childNodes)
            {
                if (!node.RunAction())
                {
                    return false;
                }

            }
            return true;
        }
    }
}