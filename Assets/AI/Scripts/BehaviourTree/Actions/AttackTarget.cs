using System.Collections;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackTarget : Node
    {
        private int blackboardTargetUnit;
        private int blackboardAttackingUnit;

        public AttackTarget(int unitControllerKey, int targetKey)
        {
            blackboardTargetUnit = targetKey;
            blackboardAttackingUnit = unitControllerKey;
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
                parentTree.Blackboard.Variables[blackboardAttackingUnit].Attack(parentTree.Blackboard.Variables[blackboardTargetUnit]);
                parentTree.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.NavMeshAgent].isStopped = true;
                parentTree.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.NavMeshAgent].velocity = Vector3.zero;
                parentTree.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.NavMeshAgent].ResetPath();
                parentTree.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.IsInAnim] = true;
                WaitForAnimationEnd();
                parentTree.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.NavMeshAgent].isStopped = false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private IEnumerator WaitForAnimationEnd()
        {
            yield return new WaitUntil(() => parentTree.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.IsInAnim] = false);
        }
    }
}