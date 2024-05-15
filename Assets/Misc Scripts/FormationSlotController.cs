using UnityEngine;

public class FormationSlotController : MonoBehaviour
{
    private UnitSelectionController parentController;

    public UnitSelectionController ParentController { get => parentController; set => parentController = value; }

    public float CurrentFormationSpacing { get; set; } = 1;
}