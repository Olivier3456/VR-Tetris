using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [HideInInspector] public Vector3Int blockPositionOnGrid;
    [HideInInspector] public Piece piece;
    [HideInInspector] public bool isDead = false;


    public bool IsGrounded
    {
        get
        {
            if (blockPositionOnGrid.y == 0)
            {
                // Block is at the lowest level of the grid, no need to verify the grid cell at y - 1.
                return true;
            }
            else if (GridManager.instance.IsThisCellFull(blockPositionOnGrid.x, blockPositionOnGrid.y - 1, blockPositionOnGrid.z))
            {
                // Block is right above a full grid cell.
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void Start()
    {
        piece = transform.parent.GetComponent<Piece>();
        piece.blocksList.Add(this);

        TryAndFindGridCellPosition();
    }


    public bool TryAndFindGridCellPosition()
    {
        Vector3 blockWorldPosition = transform.position;

        float blockXPositionFromGridXOriginPlusOffset = blockWorldPosition.x - GridManager.instance.tetrisGrid.worldPosition.x + (GridManager.instance.scaleOfCells * 0.5f);
        int blockXGridPosition = (int)Math.Floor(blockXPositionFromGridXOriginPlusOffset / GridManager.instance.tetrisGrid.sizeOfCells);

        if (blockXGridPosition > GridManager.instance.tetrisGrid.xNumberOfCells - 1 || blockXGridPosition < 0)
        {
            return false;
        }


        float blockYPositionFromGridYOriginPlusOffset = blockWorldPosition.y - GridManager.instance.tetrisGrid.worldPosition.y + (GridManager.instance.scaleOfCells * 0.5f);
        int blockYGridPosition = (int)Math.Floor(blockYPositionFromGridYOriginPlusOffset / GridManager.instance.tetrisGrid.sizeOfCells);

        if (blockYGridPosition > GridManager.instance.tetrisGrid.yNumberOfCells - 1 || blockYGridPosition < 0)
        {
            return false;
        }


        float blockZPositionFromGridZOriginPlusOffset = blockWorldPosition.z - GridManager.instance.tetrisGrid.worldPosition.z + (GridManager.instance.scaleOfCells * 0.5f);
        int blockZGridPosition = (int)Math.Floor(blockZPositionFromGridZOriginPlusOffset / GridManager.instance.tetrisGrid.sizeOfCells);

        if (blockZGridPosition > GridManager.instance.tetrisGrid.zNumberOfCells - 1 || blockZGridPosition < 0)
        {
            return false;
        }


        blockPositionOnGrid = new Vector3Int(blockXGridPosition, blockYGridPosition, blockZGridPosition);
        return true;
    }

    /// <summary>
    /// The verification adds an Y offset to prevent the block to be grounded in the middle of the grid cell height: the block is grounded at the bottom of the cell.
    /// </summary>
    public void FindNearestGridCellPosition_Y_Only_With_Offset()
    {
        float blockYPositionFromGridYOriginPlusOffset = transform.position.y - GridManager.instance.tetrisGrid.worldPosition.y + GridManager.instance.scaleOfCells;
        int blockYGridPosition = (int)Math.Floor(blockYPositionFromGridYOriginPlusOffset / GridManager.instance.tetrisGrid.sizeOfCells);
        blockPositionOnGrid = new Vector3Int(blockPositionOnGrid.x, blockYGridPosition, blockPositionOnGrid.z);
    }


    public bool IsYourCellFull()
    {
        return GridManager.instance.IsThisCellFull(blockPositionOnGrid.x, blockPositionOnGrid.y, blockPositionOnGrid.z);
    }


    public Vector3 GetMovementFromWorldPositionToNearestGridCellWorldPosition()
    {
        Vector3 res = transform.position - GridManager.instance.tetrisGrid.gridArray[blockPositionOnGrid.x, blockPositionOnGrid.y, blockPositionOnGrid.z].worldPosition;
        return res;
    }
}
