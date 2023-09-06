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
            else if (GridManager.IsThisCellFull(blockPositionOnGrid.x, blockPositionOnGrid.y - 1, blockPositionOnGrid.z))
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
        TryAndFindGridCellPosition();
    }


    public bool TryAndFindGridCellPosition()
    {
        Vector3 blockWorldPosition = transform.position;

        float blockXPositionFromGridXOriginPlusOffset = blockWorldPosition.x - GridManager.tetrisGrid.worldPosition.x + (GridManager.scaleOfCells * 0.5f);
        int blockXGridPosition = (int)Math.Floor(blockXPositionFromGridXOriginPlusOffset / GridManager.tetrisGrid.sizeOfCells);

        if (blockXGridPosition > GridManager.tetrisGrid.xNumberOfCells - 1 || blockXGridPosition < 0)
        {            
            return false;
        }


        float blockYPositionFromGridYOriginPlusOffset = blockWorldPosition.y - GridManager.tetrisGrid.worldPosition.y + (GridManager.scaleOfCells * 0.5f);
        int blockYGridPosition = (int)Math.Floor(blockYPositionFromGridYOriginPlusOffset / GridManager.tetrisGrid.sizeOfCells);

        if (blockYGridPosition > GridManager.tetrisGrid.yNumberOfCells - 1 || blockYGridPosition < 0)
        {            
            return false;
        }


        float blockZPositionFromGridZOriginPlusOffset = blockWorldPosition.z - GridManager.tetrisGrid.worldPosition.z + (GridManager.scaleOfCells * 0.5f);
        int blockZGridPosition = (int)Math.Floor(blockZPositionFromGridZOriginPlusOffset / GridManager.tetrisGrid.sizeOfCells);

        if (blockZGridPosition > GridManager.tetrisGrid.zNumberOfCells - 1 || blockZGridPosition < 0)
        {            
            return false;
        }


        blockPositionOnGrid = new Vector3Int(blockXGridPosition, blockYGridPosition, blockZGridPosition);
        return true;
    }

    /// <summary>
    /// The verification adds an Y offset to prevent the block to be grounded in the middle of the grid cell height: the block is grounded at the bottom of the cell.
    /// </summary>
    public void FindNearestGridCellPosition_Y_Only_Plus_Offset()
    {
        float blockYPositionFromGridYOriginPlusOffset = transform.position.y - GridManager.tetrisGrid.worldPosition.y + GridManager.scaleOfCells;
        int blockYGridPosition = (int)Math.Floor(blockYPositionFromGridYOriginPlusOffset / GridManager.tetrisGrid.sizeOfCells);

        blockPositionOnGrid = new Vector3Int(blockPositionOnGrid.x, blockYGridPosition, blockPositionOnGrid.z);
    }


    public bool IsYourCellFull()
    {
        return GridManager.IsThisCellFull(blockPositionOnGrid.x, blockPositionOnGrid.y, blockPositionOnGrid.z);
    }


    public Vector3 GetMovementFromWorldPositionToNearestGridCellWorldPosition()
    {
        Vector3 res = transform.position - GridManager.tetrisGrid.gridArray[blockPositionOnGrid.x, blockPositionOnGrid.y, blockPositionOnGrid.z].worldPosition;
        return res;
    }
}
