using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{

    public abstract class Decorator
    {
        protected Node parentNode;

        public abstract bool CheckCondition();

        public void setParent(Node parent)
        {
            parentNode = parent;
        }
    }
}