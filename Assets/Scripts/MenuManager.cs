using DG.Tweening;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject gamePlayButtons;
    [SerializeField] private GameObject rateUsButton;
    [SerializeField] private GameObject noAdsButton;
    [SerializeField] private GameObject gameTitle;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject tapToPlay;
    [SerializeField] private GameObject infoPopup;

    private float rateUsPosX;
    private float noAdsPosX;
    private float settingsPosX;
    private float gameTitlePosY;
    private float tapToPlayPosY;

    private void Start()
    {
        rateUsPosX = rateUsButton.transform.position.x;
        noAdsPosX = noAdsButton.transform.position.x;
        settingsPosX = settingsButton.transform.position.x;
        gameTitlePosY = gameTitle.transform.position.y;
        tapToPlayPosY = tapToPlay.transform.position.y;
}
    public void PlayButton()
    {
        rateUsButton.transform.DOMoveX(rateUsPosX + 1.5f, 1);
        settingsButton.transform.DOMoveX(settingsPosX - 1.5f, 1);
        gameTitle.transform.DOMoveY(gameTitlePosY + 3f, 1);
        tapToPlay.transform.DOMoveY(tapToPlayPosY - 2f, 1);

        noAdsButton.transform.DOMoveX(noAdsPosX + 1.5f, 1).OnComplete(() =>
        {
            gameManager.ChangeState(GameState.WaitingInput);
            mainMenu.SetActive(false);
            gameMenu.SetActive(true);
            if (DataManager.Instance.tutorial)
            {
                gamePlayButtons.SetActive(true);
            }
            gameManager._time = Time.time;
        });
    }

    public void PauseButton()
    {
        gameManager.ChangeState(GameState.MainMenu);
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
        gamePlayButtons.SetActive(false);

        noAdsButton.transform.DOMoveX(noAdsPosX, 1);
        rateUsButton.transform.DOMoveX(rateUsPosX, 1);
        settingsButton.transform.DOMoveX(settingsPosX, 1);
        gameTitle.transform.DOMoveY(gameTitlePosY, 1);
        tapToPlay.transform.DOMoveY(tapToPlayPosY, 1);
    }

    public void InfoButton()
    {
        infoPopup.SetActive(true);
    }
}
