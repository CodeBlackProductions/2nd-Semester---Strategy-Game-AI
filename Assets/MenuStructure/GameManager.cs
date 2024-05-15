using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields

    public static GameManager gameManager;

    public enum GamePhase
    { MainMenu, Campaign, BattlePreparation, BattlePlanning, Battle }

    private GamePhase currentgamePhase = GamePhase.MainMenu;

    public GamePhase CurrentgamePhase { get => currentgamePhase; set => currentgamePhase = value; }

    #endregion Fields

    public void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(this.transform);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}