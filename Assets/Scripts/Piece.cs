using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Piece : MonoBehaviour
{
    [HideInInspector] public List<Block> blocksList = new List<Block>();
    [HideInInspector] public int numberOfBlocks = 0;
    [HideInInspector] public PiecesManager piecesManager;
    [HideInInspector] public bool isHanded;
    private Vector3 handedPosition;
    private Quaternion handedRotation;

    public PiecesGrabber grabber = null;

    public bool pieceGrabbedAtLeastOnce = false;


    private void Start()
    {
        blocksList = GetComponentsInChildren<Block>().ToList();

        foreach (Block block in blocksList)
        {
            block.TryAndFindGridCellPosition();
            block.lastPositionOnGrid = block.positionOnGrid;
            block.piece = this;
        }

        if (!Check_If_All_Blocks_Cells_Are_Empty_At_Start(true))
        {
            return;
        }

        foreach (Block block in blocksList)
        {
            GridManager.instance.Fill_a_cell_with_a_block(block, false);
        }

        if (CheckIfGrounded())
        {
            DebugLog.Log("GAME OVER! New piece grounded at start.");
            GameManager.instance.gameOver = true;
            return;
        }
    }


    public bool Check_If_All_Blocks_Cells_Are_Empty_At_Start(bool firstCheck)
    {
        foreach (Block block in blocksList)
        {
            if (block.IsYourCellFull(true))
            {
                DebugLog.Log("GAME OVER! A dead piece already exists at a block position of the new piece.");
                GameManager.instance.gameOver = true;
                return false;
            }

            if (block.IsYourCellFull(false))
            {
                DebugLog.Log("A piece alive is already here. The new piece have to wait before spawning.");
                PiecesManager.instance.OnPieceCantSpawn(this, firstCheck);
                return false;
            }
        }

        return true;
    }


    private void Update()
    {
        if (!isHanded && !GameManager.instance.gameOver)
        {
            Fall();

            foreach (Block block in blocksList)
            {
                block.Find_Nearest_Grid_Cell_Position_Y_Only_With_Offset();

                if (block.lastPositionOnGrid != block.positionOnGrid)
                {
                    GridManager.instance.Empty_a_Cell(block.lastPositionOnGrid.x, block.lastPositionOnGrid.y, block.lastPositionOnGrid.z);
                    GridManager.instance.Fill_a_cell_with_a_block(block, false);
                    block.lastPositionOnGrid = block.positionOnGrid;
                }
            }

            if (CheckIfGrounded())
            {
                PiecesManager.instance.KillPiece(this);
            }
        }
    }


    private void Fall()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * PiecesManager.instance.piecesFallSpeed, transform.position.z);
    }

    /// <summary>
    /// Must be called when the player grabs the piece.
    /// </summary>    
    public void PieceGrabbed()
    {
        isHanded = true;

        foreach (Block block in blocksList)
        {
            GridManager.instance.Empty_a_Cell(block.positionOnGrid.x, block.positionOnGrid.y, block.positionOnGrid.z);
            block.lastPositionOnGrid = block.positionOnGrid;
        }

        if (!pieceGrabbedAtLeastOnce)
        {
            piecesManager.OnPieceGrabbedForTheFirstTime();
            pieceGrabbedAtLeastOnce = true;
        }
    }

    /// <summary>
    /// Must be called when the player drops the piece.
    /// </summary>
    public void TryAndReleaseThePiece()
    {
        handedPosition = transform.position;
        handedRotation = transform.rotation;

        FindAndRotateToNearestOrthogonalRotation();

        // Determine if each block is on the grid, and in an empty grid cell.     
        bool isNewPosValid = true;
        for (int i = 0; i < blocksList.Count; i++)
        {
            if (!blocksList[i].TryAndFindGridCellPosition())
            {
                isNewPosValid = false;
                break;
            }

            if (blocksList[i].IsYourCellFull(false))
            {
                isNewPosValid = false;
                break;
            }
        }

        if (!isNewPosValid)
        {
            // The piece stays in the hand of the player.            
            transform.position = handedPosition;
            transform.rotation = handedRotation;

            AudioManager.instance.Play_PieceDroppedError();
        }

        // The piece can be released. It must go to the world position corresponding to its blocks grid positions:
        // we ask to one of them its movement from its actual world position to its new grid cell world position, and we apply it to the entiere piece.
        else
        {
            Vector3 movementToApply = blocksList[0].Get_Movement_From_World_Position_To_Nearest_Grid_Cell_World_Position();
            MoveToWorldPosition(movementToApply);

            for (int i = 0; i < blocksList.Count; i++)
            {
                GridManager.instance.Fill_a_cell_with_a_block(blocksList[i], false);
                blocksList[i].lastPositionOnGrid = blocksList[i].positionOnGrid;
            }

            // The piece is effectively released from the hand which grabbed it.
            grabber.pieceGrabbed = null;
            grabber = null;
            transform.parent = null;
            isHanded = false;

            AudioManager.instance.Play_PieceDroppedGood();

            if (CheckIfGrounded())
            {
                PiecesManager.instance.KillPiece(this);
            }
        }
    }


    private void MoveToWorldPosition(Vector3 movementToApply)
    {
        transform.position -= movementToApply;
    }


    private void FindAndRotateToNearestOrthogonalRotation()
    {
        Quaternion initialRotation = transform.rotation;
        Vector3 initialRotationEuler = initialRotation.eulerAngles;

        // Find the nearest grid angle.
        float nearestX = Mathf.Round(initialRotationEuler.x / 90f) * 90f;
        float nearestY = Mathf.Round(initialRotationEuler.y / 90f) * 90f;
        float nearestZ = Mathf.Round(initialRotationEuler.z / 90f) * 90f;

        transform.rotation = Quaternion.Euler(nearestX, nearestY, nearestZ);
    }


    public bool CheckIfGrounded()
    {
        for (int i = 0; i < blocksList.Count; i++)
        {
            if (blocksList[i].IsGrounded)
            {
                return true;
            }
        }
        return false;
    }
}
