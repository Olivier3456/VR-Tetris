using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PiecesManager : MonoBehaviour
{
    public List<GameObject> piecesPrefabs = new List<GameObject>();
    public static float piecesFallSpeed = 0.125f;
    public static Vector3Int piecesStartPosition = new Vector3Int(1, 10, 1);
    

    private void Start()
    {
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

    public static void KillPiece(List<Block> pieceBlocks, Piece pieceToDestroy)
    {
        for (int i = 0; i < pieceBlocks.Count; i++)
        {
            pieceBlocks[i].transform.parent = null;

            // To avoid a slight position shift. Maybe not necessary.
            pieceBlocks[i].transform.position = GridManager.tetrisGrid.gridArray[pieceBlocks[i].blockPositionOnGrid.x,
                                                                                 pieceBlocks[i].blockPositionOnGrid.y,
                                                                                 pieceBlocks[i].blockPositionOnGrid.z].worldPosition;
            pieceBlocks[i].piece = null;
            pieceBlocks[i].isDead = true;

            GridManager.deadBlocks[pieceBlocks[i].blockPositionOnGrid.x,
                                   pieceBlocks[i].blockPositionOnGrid.y,
                                   pieceBlocks[i].blockPositionOnGrid.z] = pieceBlocks[i];
        }

        Destroy(pieceToDestroy.gameObject);
    }
}
