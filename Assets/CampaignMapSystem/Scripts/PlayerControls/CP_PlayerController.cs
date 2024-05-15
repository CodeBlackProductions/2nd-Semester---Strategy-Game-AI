using TileMap;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(CP_PathHandler))]
//[RequireComponent(typeof(CP_SelectionHandler))]
public class CP_PlayerController : MonoBehaviour
{
    #region Fields

    [SerializeField] [Range(50, 200)] private float cameraScrollSpeed = 150;
    [SerializeField] [Range(0.1f, 0.5f)] private float cameraZoomSpeed = 0.2f;
    [SerializeField] [Range(10, 200)] private float maxZoomIn = 20;
    [SerializeField] [Range(10, 200)] private float maxZoomOut = 200;
    [SerializeField] private Collider mapBounds;
    [SerializeField] private TileMapController map;

    //private CP_PathHandler pathHandler;
    //private CP_SelectionHandler selectionHandler;

    private Vector2 mousePos = new Vector3();
    private bool controlsActive = true;
    private int gamePhase = 0;

    private float cameraX = 0;
    private float cameraY = 0;

    private GameObject selectedTarget;
    public GameObject SelectedTarget { get => selectedTarget; set => selectedTarget = value; }

    #endregion Fields

    #region Input Methods

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }

    public void OnLeftMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hitInfo);

            

            int x = (int)(hitInfo.transform.position.x / map.TileSize);
            int y = (int)(hitInfo.transform.position.z / map.TileSize);

            map.tileGrid[x, y].visualRootObject.transform.position += new Vector3(0,100,0);
        }

        //if (controlsActive)
        //{
        //    if (context.performed)
        //    {
        //        dynamic temp = selectionHandler.TargetCheck(mousePos);
        //        if (GameManager.gameManager.CurrentgamePhase == GameManager.GamePhase.Campaign)
        //        {
        //            if (temp != null && temp is /*INSERT TILE CLASS HERE*/)
        //            {
        //                if (selectedTarget is  /*INSERT PLAYER ARMY CLASS HERE*/)
        //                {
        //                    pathHandler.CalculatePath(map.GetWalkableTiles(), /*INSERT PLAYER ARMY POSITION HERE*/, temp);
        //                }
        //                else
        //                {
        //                    selectedTarget = temp;
        //                    selectionHandler.SelectTarget(temp);
        //                }
        //            }
        //            else if (temp != null && temp is /*INSERT PLAYER ARMY CLASS HERE*/)
        //            {
        //                selectedTarget = temp;
        //                selectionHandler.SelectTarget(temp);
        //            }
        //            else
        //            {
        //                selectedTarget = null;
        //                selectionHandler.ClearSelection();
        //            }
        //        }
        //        else
        //        {
        //            //will there be another Phase? if not remove!
        //        }
        //    }
        //}
    }

    //public void OnRightMouseClick(InputAction.CallbackContext context)
    //{
    //    if (controlsActive)
    //    {
    //        if (context.performed)
    //        {
    //            if (GameManager.gameManager.CurrentgamePhase == GameManager.GamePhase.Campaign)
    //            {
    //                if (SelectedTarget != null && SelectedTarget is /*INSERT PLAYER ARMY CLASS HERE*/)
    //                {
    //                    pathHandler.ClearPath();
    //                }
    //            }
    //            else
    //            {
    //                //will there be another Phase? if not remove!
    //            }
    //        }
    //    }
    //}

    public void OnCameraX(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraX = context.ReadValue<float>();
        }

        if (context.canceled)
        {
            cameraX = context.ReadValue<float>();
        }
    }

    public void OnCameraY(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraY = context.ReadValue<float>();
        }

        if (context.canceled)
        {
            cameraY = context.ReadValue<float>();
        }
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.position += new Vector3(0, transform.position.y * cameraZoomSpeed * Mathf.Clamp(context.ReadValue<float>(), -1, 1), 0);

            if (transform.position.y > maxZoomOut)
            {
                transform.position = new Vector3(transform.position.x, maxZoomOut, transform.position.z);
            }

            if (transform.position.y < maxZoomIn)
            {
                transform.position = new Vector3(transform.position.x, maxZoomIn, transform.position.z);
            }
        }
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (UIHandler.uiHandler.UnitSelectionActive)
            {
                UIHandler.uiHandler.SwitchSelectionMenu(null);
            }
            else
            {
                controlsActive = !controlsActive;

                if (controlsActive)
                {
                    SceneLoadManager.sceneLoadManager.UnloadSceneAsync((int)Scenes.IngameMenu);

                    Time.timeScale = 1;
                    controlsActive = true;
                }
                else
                {
                    SceneLoadManager.sceneLoadManager.LoadSceneAdditive((int)Scenes.IngameMenu);

                    Time.timeScale = 0;
                    controlsActive = false;
                }
            }
        }
    }

    #endregion Input Methods

    #region Unity Methods

    private void Awake()
    {
        //EventHandler.eventHandler.FinishLoading += EnableInput;
        //EventHandler.eventHandler.FinishLoading += ChangeGamePhase;
        //selectionHandler = GetComponent<CP_SelectionHandler>();
        //pathHandler = GetComponent<CP_PathHandler>();
    }

    private void Start()
    {
        controlsActive = true;
        //EventHandler.eventHandler.CPSwitchControls += SwitchControls;
    }

    private void Update()
    {
        if (controlsActive)
        {
            if (mousePos.y >= Screen.height * 0.95f && cameraY != 1)
            {
                MovePlayer(0, 1);
            }

            if (mousePos.y <= Screen.height * 0.05f && cameraY != -1)
            {
                MovePlayer(0, -1);
            }

            if (mousePos.x >= Screen.width * 0.95f && cameraX != 1)
            {
                MovePlayer(1, 0);
            }

            if (mousePos.x <= Screen.width * 0.05f && cameraX != -1)
            {
                MovePlayer(-1, 0);
            }

            if (cameraX != 0 || cameraY != 0)
            {
                cameraX = Mathf.Clamp(cameraX, -1, 1);
                cameraY = Mathf.Clamp(cameraY, -1, 1);
                MovePlayer(cameraX, cameraY);
            }
        }
    }

    private void OnDisable()
    {
        EventHandler.eventHandler.FinishLoading -= EnableInput;
        EventHandler.eventHandler.FinishLoading -= ChangeGamePhase;
        EventHandler.eventHandler.CPSwitchControls -= SwitchControls;
    }

    #endregion Unity Methods

    private void EnableInput()
    {
        GetComponent<PlayerInput>().enabled = true;
    }

    private void SwitchControls(bool value)
    {
        controlsActive = value;
    }

    public void MovePlayer(float x, float z)
    {
        Vector3 newPos = new Vector3();
        if (Time.timeScale == 1)
        {
            newPos = new Vector3(x, 0, z).normalized * cameraScrollSpeed * Time.deltaTime;
        }
        else
        {
            newPos = new Vector3(x, 0, z).normalized * cameraScrollSpeed * 0.02f;
        }

        if (mapBounds.bounds.Contains(transform.position + newPos))
        {
            transform.position += newPos;
        }
    }

    private void ChangeGamePhase()
    {
        GameManager.gameManager.CurrentgamePhase = GameManager.GamePhase.Campaign;
    }
}