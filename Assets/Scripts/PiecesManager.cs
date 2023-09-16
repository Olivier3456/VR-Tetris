using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    public List<GameObject> piecesPrefabs = new List<GameObject>();
    public float piecesFallSpeed = 0.15f;
    private Vector3Int piecesStartPosition;

    public static PiecesManager instance;

    public int maxNumberOfActivePieces = 3;
    public int actualNumberOfActivePieces = 0;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("An instance of PiecesManager already exists!");
        }
    }

    private void Start()
    {
        Vector3Int gridSize = new Vector3Int(GridManager.instance.tetrisGrid.gridArray.GetLength(0),
                                             GridManager.instance.tetrisGrid.gridArray.GetLength(1),
                                             GridManager.instance.tetrisGrid.gridArray.GetLength(2));

        // New pieces will spawn in the middle of the floor, and a little lower than highest floor to avoid "3 blocks high" pieces to have blocks above the grid highest floor.
        piecesStartPosition = new Vector3Int((int)Mathf.Ceil((gridSize.x - 1) * 0.5f), gridSize.y - 3, (int)Mathf.Ceil((gridSize.z - 1) * 0.5f));

        Create_Random_Piece_If_Max_Number_Not_Reached();

        StartCoroutine(Spawn_Waiting_Pieces_If_Possible_Repeatedly());
    }

    public void Create_Random_Piece_If_Max_Number_Not_Reached()
    {
        if (!GameManager.instance.gameOver && actualNumberOfActivePieces < maxNumberOfActivePieces)
        {
            int randomIndex = UnityEngine.Random.Range(0, piecesPrefabs.Count);
            CreatePiece(piecesPrefabs[randomIndex]);
        }
    }


    private void CreatePiece(GameObject pieceToCreate)
    {
        Vector3 pieceWorldPosition = GridManager.instance.gridOriginPosition + piecesStartPosition.ConvertTo<Vector3>() * GridManager.instance.scaleOfCells;

        GameObject pieceGameObject = Instantiate(pieceToCreate, pieceWorldPosition, Quaternion.identity);

        Piece piece = pieceGameObject.GetComponent<Piece>();
        piece.transform.localScale = new Vector3(GridManager.instance.scaleOfCells, GridManager.instance.scaleOfCells, GridManager.instance.scaleOfCells);
        piece.piecesManager = instance;

        actualNumberOfActivePieces++;
    }


    private List<Piece> piecesWaitingToSpawn = new List<Piece>();
    /// <summary>
    /// This method is called by the piece, if another falling (= alive) piece is in its grid position.
    /// </summary>
    public void OnPieceCantSpawn(Piece piece, bool firstCheck)
    {
        piece.gameObject.SetActive(false);

        if (firstCheck)
        {
            piecesWaitingToSpawn.Add(piece);
        }
    }


    private WaitForSeconds waitOneSecond = new WaitForSeconds(1);
    private IEnumerator Spawn_Waiting_Pieces_If_Possible_Repeatedly()
    {
        while (true)
        {
            yield return waitOneSecond;

            if (!GameManager.instance.gameOver && piecesWaitingToSpawn.Count > 0)
            {
                piecesWaitingToSpawn[0].gameObject.SetActive(true);

                if (piecesWaitingToSpawn[0].Check_If_All_Blocks_Cells_Are_Empty_At_Start(false))   // If no, OnPieceCantSpawn() is called again and deactivate the piece again.
                {
                    foreach (Block block in piecesWaitingToSpawn[0].blocksList)
                    {
                        GridManager.instance.Fill_a_cell_with_a_block(block, false);
                    }
                    piecesWaitingToSpawn.Remove(piecesWaitingToSpawn[0]);               // The piece spawns, thus can be removed from the list.
                }
            }
        }
    }


    public void OnPieceGrabbedForTheFirstTime()
    {
        Create_Random_Piece_If_Max_Number_Not_Reached();
    }


    private static List<int> listOfFloors = new List<int>();
    public void KillPiece(Piece pieceToDestroy)
    {
        AudioManager.instance.Play_PieceGrounded();

        listOfFloors.Clear();

        for (int i = 0; i < pieceToDestroy.blocksList.Count; i++)
        {
            pieceToDestroy.blocksList[i].transform.parent = null;
            pieceToDestroy.blocksList[i].piece = null;

            // To avoid a slight Y position shift. Maybe not necessary.
            pieceToDestroy.blocksList[i].transform.position = GridManager.instance.tetrisGrid.gridArray[pieceToDestroy.blocksList[i].positionOnGrid.x,
                                                                                                        pieceToDestroy.blocksList[i].positionOnGrid.y,
                                                                                                        pieceToDestroy.blocksList[i].positionOnGrid.z].worldPosition;
            GridManager.instance.Fill_a_cell_with_a_block(pieceToDestroy.blocksList[i], true);

            if (!listOfFloors.Contains(pieceToDestroy.blocksList[i].positionOnGrid.y))
            {
                listOfFloors.Add(pieceToDestroy.blocksList[i].positionOnGrid.y);
            }
        }

        GridManager.instance.CheckIfTheseFloorsAreFull(listOfFloors);

        Destroy(pieceToDestroy.gameObject);

        GameManager.instance.OnPieceGrounded();
    }
}
