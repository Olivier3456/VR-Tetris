using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public bool gameOver = false;

    [SerializeField] private int score = 0;

    [SerializeField] private TextMeshProUGUI ScoreText;

    [SerializeField] private ColorChange groundColorChange;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DebugLog.Log("An instance of GameManager already exists!");
        }
    }


    public void AddScoreForPieceGrounded()
    {
        score++;
        ScoreText.text = score.ToString();
    }


    public void AddScoreForLevelFull(int numberOfLevels)
    {
        switch (numberOfLevels)
        {
            case 1: score += 10; break;
            case 2: score += 15; break;
            case 3: score += 30; break;
            case 4: score += 50; break;
            default: DebugLog.Log("[GameManager] -- Number of levels must be between 1 and 4"); break;
        }

        ScoreText.text = score.ToString();

        StartCoroutine(ColorChangeAccelerator(numberOfLevels));
    }

    IEnumerator ColorChangeAccelerator(int numberOfLevels)
    {
        float multiplier = 10;
        groundColorChange.colorChangeSpeed *= multiplier;
        yield return new WaitForSeconds(numberOfLevels * 0.15f);
        groundColorChange.colorChangeSpeed /= multiplier;
    }
}
