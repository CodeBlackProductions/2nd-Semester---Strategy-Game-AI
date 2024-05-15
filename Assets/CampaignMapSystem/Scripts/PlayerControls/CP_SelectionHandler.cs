//using UnityEngine;

//public class CP_SelectionHandler : MonoBehaviour
//{
//    #region Fields

//    private GameObject selectedTarget;
//    private float distance;

//    private CP_PlayerController playerController;

//    #endregion Fields

//    #region Internal Methods

//    private RaycastHit GetMouseTarget(Vector2 mousePosition)
//    {
//        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
//        Physics.Raycast(ray, out RaycastHit hitInfo);

//        return hitInfo;
//    }

//    #endregion Internal Methods

//    public void Start()
//    {
//        playerController = GetComponent<CP_PlayerController>();
//    }

//    private void OnDisable()
//    {
//    }

//    public dynamic TargetCheck(Vector2 mousePosition)
//    {
//        RaycastHit mouseTarget = GetMouseTarget(mousePosition);

//        if (mouseTarget.collider != null)
//        {
//            if (mouseTarget.collider.gameObject.TryGetComponent</*INSERT PLAYER ARMY HERE*/>(out /*INSERT PLAYER ARMY HERE*/ army))
//            {
//                return army;
//            }
//            else if (mouseTarget.collider.gameObject.TryGetComponent</*INSERT TILE HERE*/>(out /*INSERT TILE HERE*/ tile))
//            {
//                return tile;
//            }
//            else
//            {
//                return mouseTarget.point;
//            }
//        }
//        else
//        {
//            return null;
//        }
//    }

//    public void SelectTarget(GameObject target)
//    {
//        if (target != null && target is /*INSERT PLAYER ARMY HERE*/)
//        {
//            ClearSelection();
//           //SELECT ARMY AND SHOW PATH IF PLANNED
//        }
//        else if (target != null && target is /*INSERT TILE HERE*/)
//        {
//            ClearSelection();
//            //SELECT TILE AND SHOW STATS, IF OWNED: SHOW BUILD SLOTS
//        }
//        else
//        {
//            ClearSelection();
//        }
//    }

//    public void ClearSelection()
//    {
//        selectedTarget = null;
//    }
//}