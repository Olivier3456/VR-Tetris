using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    public List<GameObject> piecesPrefabs = new List<GameObject>();
    public static float piecesFallSpeed = 0; //0.125f;
    private static Vector3Int piecesStartPosition = new Vector3Int(1, 10, 1);


    private void Start()
    {
        Vector3Int gridSize = new Vector3Int(GridManager.tetrisGrid.gridArray.GetLength(0), GridManager.tetrisGrid.gridArray.GetLength(1), GridManager.tetrisGrid.gridArray.GetLength(2));

        // New pieces will spawn in the middle of the floor, and at second highest floor to avoid large pieces to have blocks above the grid highest floor.
        piecesStartPosition = new Vector3Int((int)Mathf.Ceil((gridSize.x - 1) * 0.5f), gridSize.y - 2, (int)Mathf.Ceil((gridSize.z - 1) * 0.5f));

        CreateRandomPiece();
    }


    public void CreateRandomPiece()
    {
        int randomIndex = Random.Range(0, piecesPrefabs.Count);
        CreatePiece(piecesPrefabs[randomIndex]);
    }

    public void CreatePiece(GameObject pieceToCreate)
    {
        Vector3 pieceWorldPosition = GridManager.gridOriginPosition + piecesStartPosition.ConvertTo<Vector3>() * GridManager.scaleOfCells;

        GameObject pieceGameObject = Instantiate(pieceToCreate, pieceWorldPosition, Quaternion.identity);
        Piece piece = pieceGameObject.GetComponent<Piece>();
        piece.transform.localScale = new Vector3(GridManager.scaleOfCells, GridManager.scaleOfCells, GridManager.scaleOfCells);
        piece.piecesManager = this;
    }


    private static List<int> listOfFloors = new List<int>();
    public static void KillPiece(Piece pieceToDestroy)
    {
        listOfFloors.Clear();

        for (int i = 0; i < pieceToDestroy.blocksList.Count; i++)
        {
            pieceToDestroy.blocksList[i].transform.parent = null;

            // To avoid a slight position shift. Maybe not necessary.
            pieceToDestroy.blocksList[i].transform.position = GridManager.tetrisGrid.gridArray[pieceToDestroy.blocksList[i].blockPositionOnGrid.x,
                                                                                               pieceToDestroy.blocksList[i].blockPositionOnGrid.y,
                                                                                               pieceToDestroy.blocksList[i].blockPositionOnGrid.z].worldPosition;
            pieceToDestroy.blocksList[i].piece = null;
            pieceToDestroy.blocksList[i].isDead = true;

            GridManager.Fill_a_cell_with_a_block(pieceToDestroy.blocksList[i]);

            if (!listOfFloors.Contains(pieceToDestroy.blocksList[i].blockPositionOnGrid.y))
            {
                listOfFloors.Add(pieceToDestroy.blocksList[i].blockPositionOnGrid.y);
            }
        }

        GridManager.CheckIfTheseFloorsAreFull(listOfFloors);

        Destroy(pieceToDestroy.gameObject);
    }
}
