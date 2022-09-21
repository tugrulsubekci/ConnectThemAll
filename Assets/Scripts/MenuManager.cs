using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject soundOff;
    [SerializeField] private GameObject vibrationOff;
    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject vibrationOn;

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

        audioManager.Play("Click");
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

        audioManager.Play("Click");
    }

    public void InfoButton()
    {
        infoPopup.SetActive(true);

        audioManager.Play("Click");
    }

    public void SettingsButton()
    {
        audioManager.Play("Click");
        if (!settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
        }
    }

    public void MuteButton()
    {
        audioManager.Play("Click");
        if (DataManager.Instance.isMusicOn)
        {
            DataManager.Instance.isMusicOn = false;
            soundOff.SetActive(true);
            soundOn.SetActive(false);
        }
        else
        {
            DataManager.Instance.isMusicOn = true;
            soundOff.SetActive(false);
            soundOn.SetActive(true);
        }
    }

    public void VibrationButton()
    {
        audioManager.Play("Click");
        if (DataManager.Instance.isVibrationOn)
        {
            DataManager.Instance.isVibrationOn = false;
            vibrationOn.SetActive(false);
            vibrationOff.SetActive(true);
        }
        else
        {
            DataManager.Instance.isVibrationOn = true;
            vibrationOn.SetActive(true);
            vibrationOff.SetActive(false);
        }
    }

    public void UpdateSettings()
    {
        if (DataManager.Instance.isVibrationOn)
        {
            vibrationOn.SetActive(true);
            vibrationOff.SetActive(false);
        }
        else
        {
            vibrationOn.SetActive(false);
            vibrationOff.SetActive(true);
        }

        if (DataManager.Instance.isMusicOn)
        {
            soundOff.SetActive(false);
            soundOn.SetActive(true);
        }
        else
        {
            soundOff.SetActive(true);
            soundOn.SetActive(false);
        }
    }
}
