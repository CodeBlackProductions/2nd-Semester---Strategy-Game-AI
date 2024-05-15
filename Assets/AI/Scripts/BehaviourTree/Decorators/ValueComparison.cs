using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class ValueComparison : Decorator
    {
        public enum ComparisonType
        { smaller, equal, bigger , notEqual};

        private ComparisonType type;
        private dynamic aValue;
        private dynamic bValue;
        private int a;
        private int b;
        private float c;

        public ValueComparison(int blackboardValueA, int blackboardValueB, ComparisonType comparisonType)
        {
            a = blackboardValueA;
            b = blackboardValueB;
            c = float.NegativeInfinity;
            type = comparisonType;
        }

        public ValueComparison(int blackboardValueA, float compareValue, ComparisonType comparisonType)
        {
            a = blackboardValueA;
            c = compareValue;
            type = comparisonType;
        }

        public override bool CheckCondition()
        {
            if (c == float.NegativeInfinity)
            {
                aValue = parentNode.ParentTree.Blackboard.Variables[a];
                bValue = parentNode.ParentTree.Blackboard.Variables[b];
            }
            else
            {
                if (parentNode.ParentTree.Blackboard.Variables[a] is List<BT_Unit>)
                {
                    aValue = parentNode.ParentTree.Blackboard.Variables[a].Count;
                }
                else
                {
                    aValue = parentNode.ParentTree.Blackboard.Variables[a];
                }
                bValue = c;
            }
           

            switch (type)
            {
                case ComparisonType.smaller:
                    if (aValue < bValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case ComparisonType.equal:
                    if (aValue == bValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case ComparisonType.bigger:
                    if (aValue > bValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case ComparisonType.notEqual:
                    if (aValue != bValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }
    }
}