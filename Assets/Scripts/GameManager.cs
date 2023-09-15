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
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private TextMeshProUGUI TimeText;

    [SerializeField] private GameObject ground;
    private ColorChange groundColorChange;

    public int timeBetweenLevels = 30;
    private int nextLevelTime;
    private int currentLevel = 1;
    private float time = 0;

    private float lastTimeDisplayed = 0;

    private float piecesSpeedFallIncreaseStep = 0.05f;

    public int maxNumberOfActivePieces = 3;
    public int actualNumnberOfActivePieces = 0;


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

        nextLevelTime = timeBetweenLevels;
    }

    private void Start()
    {
        CenterFloorOnGrid();

        groundColorChange = ground.GetComponent<ColorChange>();
    }

    private void Update()
    {
        if (!gameOver)
        {
            time += Time.deltaTime;
            IncreaseLevelIfTimeIsOver();
            DisplayTimeEverySeconds();
        }
    }

    private void IncreaseLevelIfTimeIsOver()
    {
        if (time > nextLevelTime)
        {
            nextLevelTime += timeBetweenLevels;
            currentLevel++;
            LevelText.text = "Level " + currentLevel;
            PiecesManager.instance.piecesFallSpeed += piecesSpeedFallIncreaseStep;
        }
    }

    private void DisplayTimeEverySeconds()
    {
        if (lastTimeDisplayed + 1 < time)
        {
            int minutes = (int)Mathf.Floor(time / 60);
            int seconds = (int)(time - (minutes / 60));
            TimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            lastTimeDisplayed = time;
        }
    }

    private void CenterFloorOnGrid()
    {
        TetrisGrid grid = GridManager.instance.tetrisGrid;
        float groundPosX = grid.worldPosition.x + (grid.sizeOfCells * grid.xNumberOfCells * 0.5f) + (grid.sizeOfCells * -0.5f);
        float groundPosY = grid.worldPosition.y + (grid.sizeOfCells * -0.5f);
        float groundPosZ = grid.worldPosition.z + (grid.sizeOfCells * grid.zNumberOfCells * 0.5f) + (grid.sizeOfCells * -0.5f);
        ground.transform.position = new Vector3(groundPosX, groundPosY, groundPosZ);
    }

    public void OnPieceGrounded()
    {
        ScoreText.text = score.ToString();

        actualNumnberOfActivePieces--;

        if (!gameOver && actualNumnberOfActivePieces < maxNumberOfActivePieces)
        {
            PiecesManager.instance.CreateRandomPiece();
        }
    }

    public void OnPieceGrabbed()
    {
        if (actualNumnberOfActivePieces < maxNumberOfActivePieces)
        {
            StartCoroutine(WaitBeforeSpawnNextPiece());
        }
    }

    private WaitForSeconds waitTwoSeconds = new WaitForSeconds(2);
    private IEnumerator WaitBeforeSpawnNextPiece()
    {
        yield return waitTwoSeconds;
        PiecesManager.instance.CreateRandomPiece();
    }


    public void AddScoreWhenLevelFull(int numberOfLevels)
    {
        switch (numberOfLevels)
        {
            case 1: score += 10; break;
            case 2: score += 15; break;
            case 3: score += 30; break;
            case 4: score += 50; break;
            default: DebugLog.Log("[GameManager] -- Error: number of levels must be between 1 and 4"); break;
        }

        ScoreText.text = score.ToString();

        StartCoroutine(ColorChangeAccelerator(numberOfLevels));
    }

    private IEnumerator ColorChangeAccelerator(int numberOfLevels)
    {
        float multiplier = 10;
        groundColorChange.colorChangeSpeed *= multiplier;
        yield return new WaitForSeconds(numberOfLevels * 0.15f);
        groundColorChange.colorChangeSpeed /= multiplier;
    }
}
