namespace BehaviourTree
{
    public class BoolComparison : Decorator
    {
        private dynamic aValue;

        private int a;
        private bool b;

        public BoolComparison(int blackboardBool, bool inverted)
        {
            a = blackboardBool;
            b = inverted;
        }

        public override bool CheckCondition()
        {
            aValue = parentNode.ParentTree.Blackboard.Variables[a];

            if (aValue)
            {
                if (!b)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!b)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}