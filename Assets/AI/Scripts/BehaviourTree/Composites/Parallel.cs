namespace BehaviourTree
{
    public class Parallel : Composite
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

            return RunParallel();
        }

        private bool RunParallel()
        {
            try
            {
                System.Threading.Tasks.Parallel.Invoke
                    (
                    () => childNodes[0].RunAction(),
                    () => childNodes[1].RunAction()
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}