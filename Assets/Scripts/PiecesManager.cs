using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PiecesManager : MonoBehaviour
{
    public List<GameObject> piecesPrefabs = new List<GameObject>();
    public static float piecesFallSpeed = 0.125f;

    private void Start()
    {
        CreateRandomPiece(new Vector3Int(2, 8, 2), GridManager.scaleOfCells);
    }


    public void CreateRandomPiece(Vector3Int positionOnGrid, float size)
    {
        int randomIndex = Random.Range(0, piecesPrefabs.Count);
        CreatePiece(piecesPrefabs[randomIndex], positionOnGrid, size);
    }

    public void CreatePiece(GameObject pieceToCreate, Vector3Int positionOnGrid, float scale)
    {
        Vector3 pieceWorldPosition = GridManager.gridOriginPosition + positionOnGrid.ConvertTo<Vector3>() * scale;

        GameObject pieceGameObject = Instantiate(pieceToCreate, pieceWorldPosition, Quaternion.identity);
        Piece piece = pieceGameObject.GetComponent<Piece>();
        piece.scale = scale;
        piece.transform.localScale = new Vector3(scale, scale, scale);
        piece.piecesManager = this;
    }

    public void KillPiece(List<Block> pieceBlocks, Piece pieceToDestroy)
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
