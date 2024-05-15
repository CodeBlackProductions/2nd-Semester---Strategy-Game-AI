//using UnityEngine;

//[RequireComponent(typeof(Pathfinder))]
//public class CP_PathHandler : MonoBehaviour
//{
//    [SerializeField] private float maxMovementPerTurn = 3;

//    private Pathfinder pathfinder;

//    #region Internal Methods

//    private RaycastHit GetMouseTarget(Vector2 mousePosition)
//    {
//        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
//        Physics.Raycast(ray, out RaycastHit hitInfo);

//        return hitInfo;
//    }

//    private float GetDistance(Vector3 startPos, Vector3 endPos)
//    {
//        float distance = Vector3.Distance(endPos, startPos);
//        return distance;
//    }

//    #endregion Internal Methods

//    #region UnityMethods

//    private void Awake()
//    {
//        pathfinder = GetComponent<Pathfinder>();
//    }

//    #endregion

//    public void CalculatePath(/*INSERT ARMY POSITION TILE HERE, INSERT TARGET TILE HERE*/)
//    {
//        pathfinder.FindPath(/*INSERT LIST OF WALKABLE TILES HERE,INSERT ARMY POSITION TILE HERE, INSERT TARGET TILE HERE,*/ true);
//        /*INSERT ARMY HERE*/.BezierCurve.DrawBezier(/*INSERT FOUND TILE COORDS HERE*/);
//    }

//    public void ClearPath()
//    {
//        /*INSERT ARMY HERE*/.BezierCurve.ClearBezier();
//    }
//}