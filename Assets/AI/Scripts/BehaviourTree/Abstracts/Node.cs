using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Node
    {
        public List<Decorator> decorators = new List<Decorator>();

        protected BehaviourTreeClass parentTree;

        public BehaviourTreeClass ParentTree { get => parentTree;}

        public abstract bool RunAction();

        public void SetParent(BehaviourTreeClass parent)
        {
            parentTree = parent;
        }

        public void AddDecorator(Decorator decorator)
        {
            decorators.Add(decorator);
        }
    }
}