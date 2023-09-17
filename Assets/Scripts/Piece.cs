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
    //[HideInInspector] public bool isHanded;
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
            block.piece = this;

            if (block.IsYourCellFull(true))     // We ignore all pieces alive: they are not taken into account to determine if the player has lost the game.
            {
                DebugLog.Log("GAME OVER! A dead piece already exists at a block position of the new piece.");
                GameManager.instance.gameOver = true;
                return;
            }
        }


        if (CheckIfGrounded())
        {
            DebugLog.Log("GAME OVER! New piece grounded at start.");
            GameManager.instance.gameOver = true;
            return;
        }

        PiecesManager.instance.AddPieceToWaitingList(this);
    }


    public bool Check_If_All_Blocks_Cells_Are_Empty_At_Start()
    {
        foreach (Block block in blocksList)
        {
            if (block.IsYourCellFull(false))    // This time we don't ignore all pieces alive, because we want to know if the piece can spawn here.
            {
                DebugLog.Log("A piece alive is already here. The new piece have to wait before spawning.");
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Must be called when the player grabs the piece.
    /// </summary>    
    public void PieceGrabbed()
    {
        //isHanded = true;
        piecesManager.fallingPieces.Remove(this);

        foreach (Block block in blocksList)
        {
            GridManager.instance.Empty_a_Cell(block.positionOnGrid.x, block.positionOnGrid.y, block.positionOnGrid.z);
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
            }

            // The piece is effectively released from the hand which grabbed it.
            grabber.pieceGrabbed = null;
            grabber = null;
            transform.parent = null;
            piecesManager.fallingPieces.Add(this);
            //isHanded = false;

            AudioManager.instance.Play_PieceDroppedGood();
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
