using UnityEngine.AI;

namespace BehaviourTree
{
    public class MoveToPoint : Node
    {
        private int blackboardVector;
        private int oldBlackboardVector;
        private int navMeshAgent;

        public MoveToPoint(int navMeshAgentKey, int moveToTargetKey, int oldMoveToKey)
        {
            blackboardVector = moveToTargetKey;
            navMeshAgent = navMeshAgentKey;
            oldBlackboardVector = oldMoveToKey;
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
                NavMeshAgent temp = parentTree.Blackboard.Variables[navMeshAgent];
                if (!(temp.destination == parentTree.Blackboard.Variables[blackboardVector]))
                {
                    temp.ResetPath();
                    parentTree.Blackboard.Variables[oldBlackboardVector] = parentTree.Blackboard.Variables[blackboardVector];

                    return temp.SetDestination(parentTree.Blackboard.Variables[blackboardVector]);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}