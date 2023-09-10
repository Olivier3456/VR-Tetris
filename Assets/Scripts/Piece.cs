using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public class Piece : MonoBehaviour
{
    [HideInInspector] public List<Block> blocksList = new List<Block>();
    [HideInInspector] public int numberOfBlocks = 0;
    [HideInInspector] public PiecesManager piecesManager;
    [HideInInspector] public bool isHanded;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;




    private void Update()
    {
        if (!isHanded)
        {
            Fall();

            foreach (Block block in blocksList)
            {
                block.FindNearestGridCellPosition_Y_Only_With_Offset();
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


    // Must be called when the player grabs the piece.
    public void PieceGrabbed()
    {
        isHanded = true;
        lastValidPosition = transform.position;
        lastValidRotation = transform.rotation;
    }


    // Must be called when the player drops the piece.
    public void PieceDropped()
    {
        FindAndRotateToNearestOrthogonalRotation();

        // Determine if each block is on the grid, and in an empty grid cell. If so, each grid block position is updated and the piece can be dropped.     
        bool isNewPosValid = true;
        for (int i = 0; i < blocksList.Count; i++)
        {
            if (!blocksList[i].TryAndFindGridCellPosition())
            {
                isNewPosValid = false;
                break;
            }

            if (blocksList[i].IsYourCellFull())
            {
                isNewPosValid = false;
                break;
            }
        }
        if (!isNewPosValid)
        {
            //Debug.Log("The piece is outside the bounds of the grid, or in full cell(s). You can't drop it here.");            
            transform.position = lastValidPosition;
            transform.rotation = lastValidRotation;

            for (int i = 0; i < blocksList.Count; i++)
            {
                blocksList[i].TryAndFindGridCellPosition();
            }

            AudioManager.instance.Play_PieceDroppedError();
        }
        else
        {
            // Piece can be dropped. It must go to the world position corresponding to its blocks grid positions:
            // we ask to one of them its movement from its actual world position to its new grid cell world position, and we apply it to the entiere piece.
            Vector3 movementToApply = blocksList[0].GetMovementFromWorldPositionToNearestGridCellWorldPosition();
            MoveToWorldPosition(movementToApply);

            if (CheckIfGrounded())
            {
                PiecesManager.instance.KillPiece(this);
            }

            AudioManager.instance.Play_PieceDroppedGood();
        }

        isHanded = false;
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
