using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        SaveManager.Load();
        Debug.Log("HighScore: " + SaveManager.data.highScore);
        Debug.Log("HintCount: " + SaveManager.data.hintCount);
    }
}
