using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PiecesManager : MonoBehaviour
{
    public List<GameObject> piecesPrefabs = new List<GameObject>();
    public float piecesFallSpeed = 0.05f;
    private Vector3Int piecesStartPosition;

    public static PiecesManager instance;


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

        CreateRandomPiece();
    }


    public void CreateRandomPiece()
    {
        int randomIndex = UnityEngine.Random.Range(0, piecesPrefabs.Count);
        CreatePiece(piecesPrefabs[randomIndex]);
    }


    // VRAIE FONCTION
    public void CreatePiece(GameObject pieceToCreate)
    {
        Vector3 pieceWorldPosition = GridManager.instance.gridOriginPosition + piecesStartPosition.ConvertTo<Vector3>() * GridManager.instance.scaleOfCells;

        GameObject pieceGameObject = Instantiate(pieceToCreate, pieceWorldPosition, Quaternion.identity);

        Piece piece = pieceGameObject.GetComponent<Piece>();
        piece.transform.localScale = new Vector3(GridManager.instance.scaleOfCells, GridManager.instance.scaleOfCells, GridManager.instance.scaleOfCells);
        piece.piecesManager = instance;
        GameManager.instance.actualNumnberOfActivePieces++;
    }


    // VERSION DEBUG : endroit d'apparition des pi�ces al�atoire.
    //public void CreatePiece(GameObject pieceToCreate)
    //{
    //    Vector3 pieceWorldPosition;
    //    int index = UnityEngine.Random.Range(0, 3);
    //    switch (index)
    //    {
    //        case 0:
    //            pieceWorldPosition = GridManager.gridOriginPosition + new Vector3(1, 12, 1) * GridManager.scaleOfCells;     // Milieu
    //            break;
    //        case 1:
    //            pieceWorldPosition = GridManager.gridOriginPosition + new Vector3(1, 12, 0) * GridManager.scaleOfCells;     // Droite
    //            break;
    //        default:
    //            pieceWorldPosition = GridManager.gridOriginPosition + new Vector3(1, 12, 2) * GridManager.scaleOfCells;     // Gauche
    //            break;
    //    }

    //    // Pour r�f�rence :
    //    //pieceWorldPosition = GridManager.gridOriginPosition + piecesStartPosition.ConvertTo<Vector3>() * GridManager.scaleOfCells;  // Milieu
    //    //pieceWorldPosition = GridManager.gridOriginPosition + new Vector3(1, 12, 0) * GridManager.scaleOfCells;     // Droite
    //    //pieceWorldPosition = GridManager.gridOriginPosition + new Vector3(1, 12, 2) * GridManager.scaleOfCells;     // Gauche

    //    GameObject pieceGameObject = Instantiate(pieceToCreate, pieceWorldPosition, Quaternion.identity);
    //    Piece piece = pieceGameObject.GetComponent<Piece>();
    //    piece.transform.localScale = new Vector3(GridManager.scaleOfCells, GridManager.scaleOfCells, GridManager.scaleOfCells);
    //    piece.piecesManager = this;
    //}





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
