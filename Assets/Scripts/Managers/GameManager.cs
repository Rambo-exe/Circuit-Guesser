using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Difficulty Lists")]
    public List<Circuits> easyCircuits;
    public List<Circuits> mediumCircuits;
    public List<Circuits> hardCircuits;

    [Header("UI")]
    public Image circuitImage;
    public ChoiceButton[] choiceButtons;
    public TextMeshProUGUI scoreText;
    public GameObject CircuitPanel;
    public TextMeshProUGUI HintValueText;
    public TextMeshProUGUI infoText;
    public GameObject HintPanel;
    public GameObject ReklamPanel;
    public GameObject ExitGamePanel;
    public GameObject ResultPanel;
    public Button Reklamizlebutton;
    public Button geriButton;
    public Button SoundButton;
    public Button HomeButton;
    public Button ExitGameButton;
    public Button ResumeGameButton;
    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI correctAnswersText;
    public TextMeshProUGUI wrongAnswersText;
    public TextMeshProUGUI longestStreakText;
    public TextMeshProUGUI usedHintsText;
    public TextMeshProUGUI hintsRemainingText;
    public TextMeshProUGUI messageText;

    [Header("SuggestionObjects")]
    public TMP_InputField guessInput;
    public GameObject suggestionPanel;
    public Transform suggestionContent;
    public GameObject suggestionButtonPrefab;

    [Header("Animator")]
    public Animator BorderAC;

    [Header("Others")]
    public Image Can1, Can2, Can3;
    public int canCounter = 0;
    public Color Red;
    public Color HintColor;
    public Color winColor;
    public Color loseColor;
    public Sprite SoundOn;
    public Sprite SoundOff;
    private List<Circuits> workingList;
    private List<Circuits> wrongPool;
    private List<Circuits> allCircuits;
    private Circuits correctCircuit;
    public string winMessage;
    public string loseMessage;

    public GameObject ButtonsPanel, InputPanel;
    // Dışarıdan ayarlayacağın zorluk
    [Header("InGameStats")]
    public int difficulty = 0; // 0=easy,1=medium,2=hard
    public int score = 0;
    public int hintCount;
    public string info;
    private bool isRewarded = false;
    private bool isHintUsed = false;
    private int correctStreak = 0;
    private int longestStreak = 0;
    private int currentLongestStreak = 0;
    private int CorrectAnswers = 0;
    private int WrongAnswers = 0;
    private int usedHints = 0;

    void Start()
    {
        if (AddMob.Instance != null)
        {
            AddMob.Instance.HideBanner();
        }

        SetDifficulity();

        guessInput.onValueChanged.AddListener(delegate { UpdateSuggestions(); });

        BorderAC = CircuitPanel.GetComponent<Animator>();

        hintCount = SaveManager.data.hintCount;
        HintValueText.text = hintCount.ToString();

        longestStreak = SaveManager.data.longestStreak;

        SetSoundButton();
        LoadNextCircuit();
    }

    public static bool HasInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public void Reklamizle()
    {
        if (!HasInternet())
        {
            Debug.Log("İnternet bağlantısı yok.\nReklam gösterilemedi.");
            return;
        }

        // 2) Reklam yüklü mü?
        if (AddMob.Instance.rewardedAd == null)
        {
            Debug.Log("Reklam şu an hazır değil.\nLütfen tekrar deneyin.");
            AddMob.Instance.LoadRewarded(); // yeniden yüklemeyi dene
            return;
        }

        // 3) Reklamı göster
        AddMob.Instance.rewardedAd.Show((Reward reward) =>
        {
            // Ödül kazandı
            /*
            hintCount++;
            SaveManager.data.hintCount = hintCount;
            SaveManager.Save();
            HintValueText.text = hintCount.ToString();
            */

            ReklamPanel.SetActive(false);
            infoText.text = info;
            HintPanel.GetComponent<Image>().color = HintColor;
            isHintUsed = true;
            usedHints++;
            Debug.Log("Tebrikler!\n1 ipucu kazandın.");

            // Reklamı tekrar yükle
            AddMob.Instance.LoadRewarded();
        });
    }
    public void CloseReklamPanel()
    {
        ReklamPanel.SetActive(false);
    }
    public void UseHint()
    {
        if(isHintUsed)
            return;
        SoundManager.Instance.PlaySFX("hint");
        if (hintCount <= 0)
        {
            ReklamPanel.SetActive(true);
        }
        else
        {
            hintCount--;
            HintValueText.text = hintCount.ToString();
            SaveManager.data.hintCount = hintCount;
            SaveManager.Save();
            // Doğru cevabı göster
            infoText.text = info;
            HintPanel.GetComponent<Image>().color = HintColor;
            isHintUsed = true;
            usedHints++;
        }   
    }

    void UpdateSuggestions()
    {
        string rawInput = guessInput.text.ToLower().Trim();

        if (string.IsNullOrEmpty(rawInput))
        {
            suggestionPanel.SetActive(false);
            return;
        }

        // inputu kelimelere ayır
        string[] inputWords = rawInput.Split(' ');

        // önce eski butonları sil
        foreach (Transform child in suggestionContent)
            Destroy(child.gameObject);

        List<Circuits> filtered = new List<Circuits>();

        foreach (var circuit in allCircuits)
        {
            // pist adını kelimelere ayır
            string[] circuitWords = circuit.circuitName.ToLower().Split(' ', '-', '_');

            bool allMatched = true;

            // oyuncunun yazdığı HER kelime eşleşmeli
            foreach (string inputWord in inputWords)
            {
                if (string.IsNullOrEmpty(inputWord)) continue;

                bool matchedThisWord = false;

                // pist kelimelerinden biri input kelimesiyle başlıyorsa ⇒ eşleşti
                foreach (string cw in circuitWords)
                {
                    if (cw.StartsWith(inputWord))
                    {
                        matchedThisWord = true;
                        break;
                    }
                }

                if (!matchedThisWord)
                {
                    allMatched = false;
                    break;
                }
            }

            if (allMatched)
                filtered.Add(circuit);
        }

        if (filtered.Count == 0)
        {
            suggestionPanel.SetActive(false);
            return;
        }

        suggestionPanel.SetActive(true);

        // önerileri oluştur
        foreach (var circuit in filtered)
        {
            GameObject b = Instantiate(suggestionButtonPrefab, suggestionContent);
            b.GetComponentInChildren<TextMeshProUGUI>().text = circuit.circuitName;

            b.GetComponent<Button>().onClick.AddListener(() =>
            {
                guessInput.text = circuit.circuitName;
                suggestionPanel.SetActive(false);
                CheckGuess(circuit);
            });
        }
    }

    void CorrectStreakHolder()
    {
        correctStreak++;
        CorrectAnswers++;
        if (correctStreak > currentLongestStreak)
        {
            currentLongestStreak = correctStreak;
        }

        if (correctStreak > longestStreak)
        {
            longestStreak = correctStreak;
            SaveManager.data.longestStreak = longestStreak;
        }
    }

    void CheckGuess(Circuits chosen)
    {
        // önce butonları gizle
        suggestionPanel.SetActive(false);

        // Doğru mu?
        if (chosen == correctCircuit)
        {
            Debug.Log("Doğru!");
            BorderAC.SetTrigger("True");
            score += 2;
            scoreText.text = score.ToString();
            CorrectStreakHolder();
            SoundManager.Instance.PlaySFX("correct");
            // can / diğer mekanikler
        }
        else
        {
            Debug.Log("Yanlış!");
            BorderAC.SetTrigger("False");
            canCounter++;
            score -= 1;
            scoreText.text = score.ToString();
            correctStreak = 0;
            SoundManager.Instance.PlaySFX("wrong");
            // can sistemi aynen kalır
        }

        guessInput.text = ""; // temizle
        LoadNextCircuit();     // yeni soru
    }

    void LoadNextCircuit()
    {
        // Tüm sorular bitti
        if (workingList.Count == 0)
        {
            EndGame(true);
            return;
        }

        isHintUsed = false;
        infoText.text = "Hint panel"; // ipucu temizle
        HintPanel.GetComponent<Image>().color = Color.white;

        // Rastgele bir soru seç
        int rnd = Random.Range(0, workingList.Count);
        correctCircuit = workingList[rnd];
        workingList.RemoveAt(rnd);

        // Resmi göster
        circuitImage.sprite = correctCircuit.circuitImage;
        info = correctCircuit.hint;

        if (score >= 30 && isRewarded == false)
        {
            hintCount += 1;
            HintValueText.text = hintCount.ToString();
            SaveManager.data.hintCount = hintCount;
            SaveManager.Save();
            isRewarded = true;
        }

        // Şıkları oluştur
        if (difficulty <= 1)
            GenerateChoices();
    }

    void GenerateChoices()
    {
        // 3 yanlış için kullanılacak havuz
        List<Circuits> tempWrong = new List<Circuits>(wrongPool);
        tempWrong.Remove(correctCircuit); // doğru olanı çıkar

        List<Circuits> wrongList = new List<Circuits>();

        // 3 yanlış bul
        for (int i = 0; i < 3; i++)
        {
            int rnd = Random.Range(0, tempWrong.Count);
            wrongList.Add(tempWrong[rnd]);
            tempWrong.RemoveAt(rnd);
        }

        // Doğru + Yanlışları birleştir
        List<Circuits> merged = new List<Circuits>();
        merged.Add(correctCircuit);
        merged.AddRange(wrongList);

        // Karıştır
        for (int i = 0; i < merged.Count; i++)
        {
            Circuits temp = merged[i];
            int rndIndex = Random.Range(i, merged.Count);
            merged[i] = merged[rndIndex];
            merged[rndIndex] = temp;
        }

        // Butonlara ata
        for (int i = 0; i < 4; i++)
        {
            choiceButtons[i].Init(this, merged[i]);
        }
    }

    public void OnChoose(ChoiceButton btn)
    {
        // Önce tüm butonları kilitleyelim
        foreach (var b in choiceButtons)
            b.GetComponent<Button>().interactable = false;

        if (btn.circuit == correctCircuit)
        {
            btn.SetColor(Color.green);
            BorderAC.SetTrigger("True");
            score += 2;
            scoreText.text = score.ToString();
            CorrectStreakHolder();
            SoundManager.Instance.PlaySFX("correct");
            if (canCounter == 1)
            {
                Can1.color = Red;
                canCounter--;
            }
            else if (canCounter == 2)
            {
                Can2.color = Red;
                canCounter--;
            }
        }
        else
        {
            btn.SetColor(Color.red);
            BorderAC.SetTrigger("False");
            canCounter++;
            score -= 1;
            scoreText.text = score.ToString();
            correctStreak = 0;
            WrongAnswers++;
            SoundManager.Instance.PlaySFX("wrong");
            if (canCounter == 1)
            {
                Can1.color = Color.black;
            }
            else if (canCounter == 2)
            {
                Can2.color = Color.black;
            }
            else if (canCounter >= 3)
            {
                Can3.color = Color.black;
                // Oyun bitti
                EndGame(false);
                return;
            }
            // Doğru butonu bul ve belirt
            foreach (var c in choiceButtons)
            {
                if (c.circuit == correctCircuit)
                {
                    c.SetColor(Color.yellow);
                }
            }
        }

        StartCoroutine(NextAfterDelay());
    }

    IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(0.75f);

        // Renkleri sıfırla + butonları aktif et
        foreach (var b in choiceButtons)
        {
            b.ResetColor();
            b.GetComponent<Button>().interactable = true;
        }

        LoadNextCircuit();
    }

    void EndGame(bool victory)
    {
        string sound;
        if (victory)
        {
            sound = "victory";
            messageText.text = winMessage;
            messageText.color = winColor;
        }
        else
        {
            sound = "endgame";
            messageText.text = loseMessage;
            messageText.color = loseColor;
        }

        SoundManager.Instance.PlaySFX(sound);
        Debug.Log("GAME OVER - All circuits used.");
        if (difficulty == 0)
        {
            if (score > SaveManager.data.highscoreEasy)
            {
                SaveManager.data.highscoreEasy = score;
                SaveManager.Save();
            }
        }
        else if(difficulty == 1)
        {
            if (score > SaveManager.data.highscoreMedium)
            {
                SaveManager.data.highscoreMedium = score;
                SaveManager.Save();
            }
        }

        if (score > SaveManager.data.highScore)
        {
            SaveManager.data.highScore = score;
            SaveManager.Save();
        }

        if (AddMob.Instance != null)
        {
            AddMob.Instance.ShowInterstitial();
        }
        //StartCoroutine(ReturnToMenuAfterDelay());
        // Buraya sonuç ekranı gelecek
        ResultPanel.SetActive(true);
        resultScoreText.text = score.ToString();
        correctAnswersText.text = CorrectAnswers.ToString();
        wrongAnswersText.text = WrongAnswers.ToString();
        longestStreakText.text = currentLongestStreak.ToString();
        usedHintsText.text = usedHints.ToString();
        hintsRemainingText.text = hintCount.ToString();
    }

    IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }

    public void SoundButtonClick()
    {
        if(SoundManager.Instance.isMuted)
        {
            SoundManager.Instance.SetMute(false);
            SoundButton.image.sprite = SoundOn;
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

    public void SetDifficulity()
    {
        difficulty = GameDifficulity.difficulty;

        if (difficulty <= 1)
        {
            InputPanel.SetActive(false);
            ButtonsPanel.SetActive(true);
        }
        else if (difficulty == 2)
        {
            InputPanel.SetActive(true);
            ButtonsPanel.SetActive(false);
        }
        switch (difficulty)
        {
            case 0: workingList = new List<Circuits>(easyCircuits); wrongPool = new List<Circuits>(easyCircuits); break;
            case 1: workingList = new List<Circuits>(mediumCircuits); wrongPool = new List<Circuits>(mediumCircuits); break;
            case 2: workingList = new List<Circuits>(mediumCircuits); wrongPool = new List<Circuits>(mediumCircuits); break;
        }
        allCircuits = new List<Circuits>(workingList);
    }

    public void HomeButtonClick()
    {
        ExitGamePanel.SetActive(true);
        SoundManager.Instance.PlaySFX("button");
    }
    public void CloseExitGamePanel()
    {
        ExitGamePanel.SetActive(false);
        SoundManager.Instance.PlaySFX("button");
    }
    public void ExitGameClick()
    {
        
        SceneManager.LoadScene("MainMenu");
        SoundManager.Instance.PlaySFX("button");
    }
    public void RestartGameClick()
    {
        
        SceneManager.LoadScene("SampleScene");
        SoundManager.Instance.PlaySFX("button");
    }
}
