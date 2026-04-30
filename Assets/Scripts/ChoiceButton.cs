using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceButton : MonoBehaviour
{
    public Circuits circuit;
    private Button button;
    private Image img;
    private TextMeshProUGUI text;
    private GameManager gm;

    private void Awake()
    {
        button = GetComponent<Button>();
        img = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(GameManager manager, Circuits data)
    {
        gm = manager;
        circuit = data;
        text.text = data.circuitName;

        ResetColor();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => gm.OnChoose(this));
    }

    public void SetColor(Color c)
    {
        img.color = c;
    }

    public void ResetColor()
    {
        img.color = Color.white;
    }
}
