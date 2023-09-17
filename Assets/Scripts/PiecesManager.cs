using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    public List<GameObject> piecesPrefabs = new List<GameObject>();
    public float piecesFallTimeStep = 1f;
    private Vector3Int piecesStartPosition;

    public static PiecesManager instance;

    [HideInInspector] public int maxNumberOfActivePieces = 3;

    // Needed to calculate the total number of pieces alive : not spawned yet + falling + hand-held.
    [HideInInspector] public int handHeldPieces = 0;

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

        StartCoroutine(PiecesFall());
    }

    public void Create_Random_Piece_If_Max_Number_Not_Reached()
    {
        if (!GameManager.instance.gameOver && (piecesWaitingToSpawn.Count + fallingPieces.Count + handHeldPieces) < maxNumberOfActivePieces)
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
    }


    private List<Piece> piecesWaitingToSpawn = new List<Piece>();
    /// <summary>
    /// This method is called by the piece, if another falling (= alive) piece is in its grid position.
    /// </summary>
    public void AddPieceToWaitingList(Piece piece)
    {
        piece.gameObject.SetActive(false);
        piecesWaitingToSpawn.Add(piece);
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

                if (piecesWaitingToSpawn[0].Check_If_All_Blocks_Cells_Are_Empty_At_Start())     // The piece can spawn.
                {
                    foreach (Block block in piecesWaitingToSpawn[0].blocksList)
                    {
                        GridManager.instance.Fill_a_cell_with_a_block(block, false);
                    }
                    fallingPieces.Add(piecesWaitingToSpawn[0]);
                    piecesWaitingToSpawn.Remove(piecesWaitingToSpawn[0]);
                }
                else    // The piece can't spawn.
                {
                    piecesWaitingToSpawn[0].gameObject.SetActive(false);
                }
            }
        }
    }



    public List<Piece> fallingPieces = new List<Piece>();    
    private IEnumerator PiecesFall()
    {
        while (true)
        {
            yield return new WaitForSeconds(piecesFallTimeStep);

            listOfFloors.Clear();
            int i;

            // Check whether any living pieces are on the ground, if so we kill them, and we check again as long as at least one piece has been detected on the ground in an iteration of the loop.
            bool isAPieceGroundedAtThisIteration;
            do
            {
                isAPieceGroundedAtThisIteration = false;
                i = fallingPieces.Count - 1;
                while (i >= 0)
                {
                    if (fallingPieces[i].CheckIfGrounded())
                    {
                        KillPiece(fallingPieces[i]);
                        fallingPieces.Remove(fallingPieces[i]);
                        isAPieceGroundedAtThisIteration = true;
                    }
                    i--;
                }
            } while (isAPieceGroundedAtThisIteration);


            // Living pieces fall in the cells at Y - 1.
            foreach (Piece piece in fallingPieces)
            {
                foreach (Block block in piece.blocksList)
                {
                    GridManager.instance.Empty_a_Cell(block.positionOnGrid.x, block.positionOnGrid.y, block.positionOnGrid.z);
                    block.positionOnGrid = new Vector3Int(block.positionOnGrid.x, block.positionOnGrid.y - 1, block.positionOnGrid.z);
                    GridManager.instance.Fill_a_cell_with_a_block(block, false);
                }

                Vector3 pieceWorldPosition = piece.transform.position;
                piece.transform.position = new Vector3(pieceWorldPosition.x, pieceWorldPosition.y - GridManager.instance.scaleOfCells, pieceWorldPosition.z);
            }


            // Check if the floors affected by the grounded pieces are full.
            if (listOfFloors.Count > 0)
            {
                GridManager.instance.CheckIfTheseFloorsAreFull(listOfFloors);
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

        for (int i = 0; i < pieceToDestroy.blocksList.Count; i++)
        {
            pieceToDestroy.blocksList[i].transform.parent = null;
            pieceToDestroy.blocksList[i].piece = null;

            GridManager.instance.Fill_a_cell_with_a_block(pieceToDestroy.blocksList[i], true);

            if (!listOfFloors.Contains(pieceToDestroy.blocksList[i].positionOnGrid.y))
            {
                listOfFloors.Add(pieceToDestroy.blocksList[i].positionOnGrid.y);
            }
        }

        Destroy(pieceToDestroy.gameObject);

        GameManager.instance.OnPieceGrounded();
    }
}
