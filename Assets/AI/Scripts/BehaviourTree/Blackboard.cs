using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System;

namespace BehaviourTree
{
    public class Blackboard
    {
        private BehaviourTreeClass parentTree;
        private Dictionary<int, dynamic> variables = new Dictionary<int, dynamic>();

        public Dictionary<int, dynamic> Variables { get => variables; set => variables = value; }

        public Blackboard(BehaviourTreeClass parent)
        {
            parentTree = parent;
        }
    }
}