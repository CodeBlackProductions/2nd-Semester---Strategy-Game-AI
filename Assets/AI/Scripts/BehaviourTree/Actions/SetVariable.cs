using UnityEngine;

namespace BehaviourTree
{
    public class SetVariable<T> : Node
    {
        private int targetVariable;
        private T value;
        private int targetValue;
        private int targetIndex;

        public SetVariable(T inputValue, int blackboardVariable)
        {
            targetVariable = blackboardVariable;
            value = inputValue;
            targetValue = int.MinValue;
            targetIndex = int.MinValue;
        }

        public SetVariable(int inputBlackboardValue, int blackboardVariable)
        {
            targetVariable = blackboardVariable;
            targetValue = inputBlackboardValue;
            targetIndex = int.MinValue;
        }

        public SetVariable(int inputBlackboardValue, int index, int blackboardVariable)
        {
            targetVariable = blackboardVariable;
            targetValue = inputBlackboardValue;
            targetIndex = index;
        }

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

            try
            {
                if (targetValue == int.MinValue)
                {
                    parentTree.Blackboard.Variables[targetVariable] = value;

                    return true;
                }
                else
                {
                    if (targetIndex == int.MinValue)
                    {
                        parentTree.Blackboard.Variables[targetVariable] = parentTree.Blackboard.Variables[targetValue];

                        return true;
                    }
                    else if (targetIndex != -1)
                    {
                        parentTree.Blackboard.Variables[targetVariable] = parentTree.Blackboard.Variables[targetValue][targetIndex].Blackboard.Variables[targetVariable];

                        return true;
                    }
                    else
                    {
                        parentTree.Blackboard.Variables[targetVariable] = parentTree.Blackboard.Variables[targetValue][Random.Range(0, parentTree.Blackboard.Variables[targetValue].Count)].Blackboard.Variables[targetVariable];

                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}