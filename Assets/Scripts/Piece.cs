using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public class Piece : MonoBehaviour
{
    public Block[] blocksArray;
    [HideInInspector] public float scale;
    [HideInInspector] public PiecesManager piecesManager;
    [HideInInspector] public bool isHanded;
    [HideInInspector] public bool isLerpingToNewWorldPosition;
    //[HideInInspector] public bool isLerpingToGridCellWorldRotation;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    

    private void Update()
    {
        if (!isHanded && !isLerpingToNewWorldPosition) // && !isLerpingToGridCellWorldRotation)
        {
            Fall();

            foreach (Block block in blocksArray)
            {
                block.FindNearestGridCellPosition_Y_Only_Plus_Offset();
            }


            if (CheckIfGrounded())
            {
                MarkGridCellsAsFull();

                //piecesManager.CreateRandomPiece(new Vector3Int(2, 8, 2), GridManager.scaleOfCells);                
                piecesManager.KillPiece(blocksArray, this);
            }
        }
    }


    private void Fall()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * PiecesManager.piecesFallSpeed, transform.position.z);
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
        for (int i = 0; i < blocksArray.Length; i++)
        {
            if (!blocksArray[i].TryAndFindGridCellPosition())
            {
                isNewPosValid = false;
                break;
            }

            if (blocksArray[i].IsYourCellFull())
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

            for (int i = 0; i < blocksArray.Length; i++)
            {
                blocksArray[i].TryAndFindGridCellPosition();
            }
        }
        else
        {
            //Debug.Log("Piece dropped");

            isHanded = false;

            // Piece can be dropped. It must go to the world position corresponding to its blocks grid positions:
            // we ask to one of them its movement from its actual world position to its new grid cell world position, and we apply it to the entiere piece.
            Vector3 movementToApply = blocksArray[0].GetMovementFromWorldPositionToNearestGridCellWorldPosition();
            MoveToWorldPosition(movementToApply, false);

            if (CheckIfGrounded())
            {
                MarkGridCellsAsFull();
                piecesManager.KillPiece(blocksArray, this);
            }
        }
    }

    private void MarkGridCellsAsFull()
    {
        for (int i = 0; i < blocksArray.Length; i++)
        {
            GridManager.SetThisCellAsFull(blocksArray[i].blockPositionOnGrid.x,
                                          blocksArray[i].blockPositionOnGrid.y,
                                          blocksArray[i].blockPositionOnGrid.z, true, false);
        }
    }

    private void MoveToWorldPosition(Vector3 movementToApply, bool lerp = true)
    {
        if (!lerp)
        {
            transform.position -= movementToApply;
        }
        else
        {
            StartCoroutine(CoroutineLerpToGridCellWorldPosition(movementToApply));
        }
    }

    private IEnumerator CoroutineLerpToGridCellWorldPosition(Vector3 movementToApply)
    {
        isLerpingToNewWorldPosition = true;
        Vector3 newPosition = transform.position - movementToApply;

        while (Vector3.Distance(transform.position, newPosition) < scale * 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
            yield return null;
        }

        transform.position = newPosition;
        isLerpingToNewWorldPosition = false;
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
        for (int i = 0; i < blocksArray.Length; i++)
        {
            if (blocksArray[i].IsGrounded)
            {
                Debug.Log("Piece grounded.");
                return true;
            }
        }
        return false;
    }
}
