using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject gamePlayButtons;

    public void TapToPlay()
    {
        gameManager.ChangeState(GameState.WaitingInput);
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        gameManager._time = Time.time;
        if(DataManager.Instance.tutorial)
        {
            gamePlayButtons.SetActive(true);
        }
    }

    public void PauseButton()
    {
        gameManager.ChangeState(GameState.MainMenu);
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
    }
}
