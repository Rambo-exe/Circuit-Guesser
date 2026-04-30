using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI hintsText;
    public TextMeshProUGUI highScoreEasyText;
    public TextMeshProUGUI highScoreMediumText;
    public Button SoundButton;
    public Sprite SoundOn;
    public Sprite SoundOff;
    
    void Start()
    {
        if (AddMob.Instance != null)
        {
            AddMob.Instance.LoadBanner();
        }
        highScoreText.text = SaveManager.data.highScore.ToString();
        hintsText.text =  SaveManager.data.hintCount.ToString();
        highScoreEasyText.text = "High Score: " + SaveManager.data.highscoreEasy.ToString();
        highScoreMediumText.text = "High Score: " + SaveManager.data.highscoreMedium.ToString();
        SetSoundButton();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void StartEasy()
    {
        SetEasyDifficulty();
        SceneManager.LoadScene("SampleScene");
        SoundManager.Instance.PlaySFX("button");
    }
    public void StartMedium()
    {
        SetMediumDifficulty();
        SceneManager.LoadScene("SampleScene");
        SoundManager.Instance.PlaySFX("button");
    }
    public void StartHard()
    {
        SetHardDifficulty();
        SceneManager.LoadScene("SampleScene");
        SoundManager.Instance.PlaySFX("button");
    }
    public void SetEasyDifficulty()
    {
        GameDifficulity.difficulty = 0;
    }
    public void SetMediumDifficulty()
    {
        GameDifficulity.difficulty = 1;
    }
    public void SetHardDifficulty()
    {
        GameDifficulity.difficulty = 2;
    }
    public void SoundButtonClick()
    {
        if (SoundManager.Instance.isMuted)
        {
            SoundManager.Instance.SetMute(false);
            SoundButton.image.sprite = SoundOn;
            SoundManager.Instance.PlaySFX("button");
        }
        else
        {
            SoundManager.Instance.SetMute(true);
            SoundButton.image.sprite = SoundOff;
        }
    }
    public void SetSoundButton()
    {
        if (SoundManager.Instance.isMuted)
        {
            SoundButton.image.sprite = SoundOff;
        }
        else
        {
            SoundButton.image.sprite = SoundOn;
        }
    }
    public static bool HasInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public void Reklamizle()
    {
        if (!HasInternet())
        {
            Debug.Log("Ýnternet baðlantýsý yok.\nReklam gösterilemedi.");
            return;
        }

        // 2) Reklam yüklü mü?
        if (AddMob.Instance.rewardedAd == null)
        {
            Debug.Log("Reklam þu an hazýr deðil.\nLütfen tekrar deneyin.");
            AddMob.Instance.LoadRewarded(); // yeniden yüklemeyi dene
            return;
        }

        // 3) Reklamý göster
        AddMob.Instance.rewardedAd.Show((Reward reward) =>
        {
            // Ödül kazandý

            SaveManager.data.hintCount++;
            SaveManager.Save();
            hintsText.text = SaveManager.data.hintCount.ToString();

            Debug.Log("Tebrikler!\n1 ipucu kazandýn.");

            // Reklamý tekrar yükle
            AddMob.Instance.LoadRewarded();
        });
    }
}
