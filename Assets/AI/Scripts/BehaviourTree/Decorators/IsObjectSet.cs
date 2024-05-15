namespace BehaviourTree
{
    public class IsObjectSet : Decorator
    {

        private dynamic setObject;
        private int setObjectRef;
        private bool inverted;

        public IsObjectSet(int blackboardObject, bool invert)
        {
            setObjectRef = blackboardObject;
            inverted = invert;
        }

        public override bool CheckCondition()
        {
            setObject = parentNode.ParentTree.Blackboard.Variables[setObjectRef];

            if (setObject != null)
            {
                if (inverted)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (inverted)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

    }
}