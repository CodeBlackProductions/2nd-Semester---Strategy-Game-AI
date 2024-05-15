using System.Collections.Generic;

namespace BehaviourTree
{
    public abstract class Composite : Node
    {
        public List<Node> childNodes = new List<Node>();
    }
}