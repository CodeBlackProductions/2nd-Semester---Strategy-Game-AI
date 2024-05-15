using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourTreeClass : MonoBehaviour
    {
        protected Node startNode;
        protected Blackboard blackboard;

        public Blackboard Blackboard { get => blackboard; protected set => blackboard = value; }

        protected bool gameIsRunning = false;

        public void Awake()
        {
            blackboard = new Blackboard(this);
        }

        public void Start()
        {
            EventHandler.eventHandler.RTSStartBattle += OnStartGame;
        }

        private void OnDisable()
        {
            EventHandler.eventHandler.RTSStartBattle -= OnStartGame;
        }

        protected void OnStartGame()
        {
            gameIsRunning = true;
        }

        public abstract void Initialize();

        public abstract void SetupBlackboard();

        public abstract void BuildTree();

        public void Update()
        {
            if (gameIsRunning)
            {
                startNode.RunAction();
            }

            //"freeze" x and z rotation
            this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
        }

        public abstract void LateUpdate();

        public void AddChildren(Node NodeToAddTo, params Node[] childrenToAdd)
        {
            if (NodeToAddTo.GetType() == typeof(Sequence))
            {
                Sequence node = (Sequence)NodeToAddTo;
                foreach (Node n in childrenToAdd)
                {
                    node.childNodes.Add(n);
                    n.SetParent(this);
                }
            }
            if (NodeToAddTo.GetType() == typeof(Selector))
            {
                Selector node = (Selector)NodeToAddTo;
                foreach (Node n in childrenToAdd)
                {
                    node.childNodes.Add(n);
                    n.SetParent(this);
                }
            }
            if (NodeToAddTo.GetType() == typeof(Parallel))
            {
                Parallel node = (Parallel)NodeToAddTo;
                foreach (Node n in childrenToAdd)
                {
                    node.childNodes.Add(n);
                    n.SetParent(this);
                }
            }

            if (!(NodeToAddTo.GetType() == typeof(Sequence)) && !(NodeToAddTo.GetType() == typeof(Selector) && !(NodeToAddTo.GetType() == typeof(Parallel))))
            {
                Debug.LogError("Can't add Children to that type of Node!");
            }
        }
    }
}