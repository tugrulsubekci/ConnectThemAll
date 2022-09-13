using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject gameMenu;

    public void TapToPlay()
    {
        gameManager.ChangeState(GameState.WaitingInput);
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        gameManager._time = Time.time;
    }

    public void PauseButton()
    {
        gameManager.ChangeState(GameState.MainMenu);
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
    }
}
