using UnityEngine;

namespace BehaviourTree
{
    public class RotateTowards : Node
    {
        private int blackboardVector;
        private int blackboardRotSpd;

        public RotateTowards(int blackboardTargetKey, int blackboardSpeedKey)
        {
            blackboardVector = blackboardTargetKey;
            blackboardRotSpd = blackboardSpeedKey;
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
                Quaternion q = Quaternion.LookRotation(parentTree.Blackboard.Variables[blackboardVector] - parentTree.transform.position);
                parentTree.transform.rotation = Quaternion.RotateTowards(parentTree.transform.rotation, q, parentTree.Blackboard.Variables[blackboardRotSpd] * Time.deltaTime);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}