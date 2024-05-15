using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RTS_SelectionHandler))]
[RequireComponent(typeof(RTS_PathHandler))]
public class RTS_PlayerController : MonoBehaviour
{
    #region Fields

    [SerializeField] private bool groupCommandsEnabled;
    [SerializeField] [Range(50, 200)] private float cameraScrollSpeed = 150;
    [SerializeField] [Range(0.1f, 0.5f)] private float cameraZoomSpeed = 0.2f;
    [SerializeField] [Range(10, 200)] private float maxZoomIn = 20;
    [SerializeField] [Range(10, 200)] private float maxZoomOut = 200;
    [SerializeField] private Collider mapBounds;
    [SerializeField] private GameObject unitSelectionPrefab;

    private RTS_SelectionHandler selectionHandler;
    private RTS_PathHandler pathHandler;

    private bool leftIsDragging = false;
    private bool rightIsDragging = false;
    private Vector2 mousePos = new Vector3();
    private Vector2 dragStartPos = new Vector3();
    private Vector2 dragEndPos = new Vector3();
    private bool controlsActive = true;
    private int gamePhase = 0;

    private float cameraX = 0;
    private float cameraY = 0;

    private List<RTS_UnitController> selectedUnits = new List<RTS_UnitController>();
    public List<RTS_UnitController> SelectedUnits { get => selectedUnits; set => selectedUnits = value; }

    #endregion Fields

    #region Input Methods

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }

    public void OnLeftMouseClick(InputAction.CallbackContext context)
    {
        if (controlsActive)
        {
            if (leftIsDragging == false && rightIsDragging == false && context.performed)
            {
                dynamic temp = selectionHandler.TargetCheck(mousePos);
                if (gamePhase == 0)
                {
                    if (temp != null && temp is FormationSlotController)
                    {
                        EventHandler.eventHandler.RTSSlotClicked?.Invoke(temp);
                    }
                    else if (temp != null && temp is Vector3)
                    {
                        GameObject.Instantiate<GameObject>(unitSelectionPrefab, temp, Quaternion.identity);
                    }
                }
                else
                {
                    if (temp != null && temp is UnitController)
                    {
                        pathHandler.ResetCommands(false);

                        EventHandler.eventHandler.RTSUnitClicked?.Invoke(temp);
                    }
                    else
                    {
                        EventHandler.eventHandler.RTSUnitClicked?.Invoke(null);
                        pathHandler.ResetCommands(false);
                    }
                }
            }
        }
    }

    public void OnLeftMouseDrag(InputAction.CallbackContext context)
    {
        if (controlsActive && gamePhase != 0)
        {
            if (context.started)
            {
                dragStartPos = mousePos;
            }

            if (rightIsDragging == false && context.performed)
            {
                leftIsDragging = true;
            }

            if (leftIsDragging == true && context.canceled)
            {
                pathHandler.ResetCommands(false);

                selectedUnits = selectionHandler.SelectUnitsInSelection();

                leftIsDragging = false;

                dragStartPos = Vector3.zero;
                dragEndPos = Vector3.zero;
            }
        }
    }

    public void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (controlsActive && gamePhase != 0)
        {
            if (leftIsDragging == false && rightIsDragging == false && context.performed)
            {
                if (selectedUnits != null && selectedUnits.Count > 0)
                {
                    pathHandler.ClearPath(selectedUnits[0]);
                    pathHandler.ResetCommands(true);
                }
            }
        }
    }

    public void OnRightMouseDrag(InputAction.CallbackContext context)
    {
        if (controlsActive && gamePhase != 0)
        {
            if (leftIsDragging == false && context.performed)
            {
                pathHandler.ResetCommands(true);
                rightIsDragging = true;

                if (groupCommandsEnabled)
                {
                    if (selectedUnits != null && selectedUnits.Count != 0)
                    {
                        for (int i = 0; i < selectedUnits.Count; i++)
                        {
                            pathHandler.ClearPath(selectedUnits[i]);
                        }
                    }
                }
                else
                {
                    pathHandler.ClearPath(selectedUnits[0]);
                }
            }

            if (rightIsDragging == true && context.canceled)
            {
                rightIsDragging = false;
            }
        }
    }

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
        EventHandler.eventHandler.FinishLoading += EnableInput;
        selectionHandler = GetComponent<RTS_SelectionHandler>();
        pathHandler = GetComponent<RTS_PathHandler>();
    }

    private void Start()
    {
        EventHandler.eventHandler.RTSSwitchControls += SwitchControls;
        EventHandler.eventHandler.RTSFinishDeployment += ChangeGamePhase;
        EventHandler.eventHandler.RTSStartBattle += ChangeGamePhase;
    }

    private void Update()
    {
        if (leftIsDragging == true)
        {
            dragEndPos = mousePos;
            selectionHandler.DrawSelection(dragStartPos, dragEndPos);
        }

        if (rightIsDragging == true)
        {
            if (groupCommandsEnabled)
            {
                if (selectedUnits != null && selectedUnits.Count != 0)
                {
                    for (int i = 0; i < selectedUnits.Count; i++)
                    {
                        pathHandler.AddPathPoint(mousePos, selectedUnits[i]);
                    }
                }
            }
            else
            {
                pathHandler.AddPathPoint(mousePos, selectedUnits[0]);
            }
        }

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
        EventHandler.eventHandler.RTSSwitchControls -= SwitchControls;
        EventHandler.eventHandler.RTSFinishDeployment -= ChangeGamePhase;
        EventHandler.eventHandler.RTSStartBattle -= ChangeGamePhase;
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
        gamePhase++;
    }
}