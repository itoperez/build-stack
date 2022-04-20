using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private int score = -1;
    private TextMeshProUGUI text;  

    private void Start()
    {   
        // ResetHighscore();
        text = GetComponent<TMPro.TextMeshProUGUI>();
        GameManager.OnCubeSpawned += GameManager_OnCubeSpawned;
    }

    private void OnDestroy() {
        TrySetNewHighscore(score);
        GameManager.OnCubeSpawned -= GameManager_OnCubeSpawned;
    }

    private void GameManager_OnCubeSpawned() {
        score++;
        text.text = "SCORE: " + (score) + "\nHighscore: " + (GetHighscore());
    }    

    public static int GetHighscore() {
        return PlayerPrefs.GetInt("highscoreBuildStack");
    }

    public static bool TrySetNewHighscore(int score) {
        int currentHighscore = GetHighscore();
        if (score > currentHighscore) {
            PlayerPrefs.SetInt("highscoreBuildStack", score);
            PlayerPrefs.Save();
            return true;
        } else {
            return false;
        }
    }

    public static void ResetHighscore() {
        PlayerPrefs.SetInt("highscoreBuildStack", 0);
        PlayerPrefs.Save();
    }

    public int GetScore() {
        return score;
    }
}
