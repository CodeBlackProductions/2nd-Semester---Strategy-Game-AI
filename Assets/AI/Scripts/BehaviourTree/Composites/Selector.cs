namespace BehaviourTree
{
    public class Selector : Composite
    {
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

            return RunSelector();
        }

        private bool RunSelector()
        {
            foreach (Node node in childNodes)
            {
                if (node.RunAction())
                {
                    return true;
                }
            }
            return false;
        }
    }
}