using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PiecesGrabber : MonoBehaviour
{
    public InputActionReference grabInputActionReference;

    private GameObject pieceHovering;
    public Piece pieceGrabbed;


    private void OnEnable()
    {
        grabInputActionReference.action.Enable();
    }

    private void OnDisable()
    {
        grabInputActionReference.action.Disable();
    }

    private void Update()
    {
        ReceiveInput();
    }

    private void ReceiveInput()
    {
        if (grabInputActionReference.action.WasPressedThisFrame())
        {
            Grab_Or_Release_Piece();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Piece") && pieceGrabbed == null)
        {
            pieceHovering = other.gameObject;
            //Debug.Log("Piece hovered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == pieceHovering)
        {
            pieceHovering = null;
            //Debug.Log("End of the piece hovering");
        }
    }

    private void Grab_Or_Release_Piece()
    {
        // Grab the piece hovered, if the hand doesnt have a grabbed piece.
        if (pieceGrabbed == null)
        {
            if (pieceHovering != null)
            {
                pieceGrabbed = pieceHovering.GetComponent<Piece>();                

                if (pieceGrabbed.grabber != null)
                {
                    pieceGrabbed.grabber.pieceGrabbed = null;         // Empty the other hand...                                      
                }
                else
                {
                    pieceGrabbed.PieceGrabbed();
                }

                pieceGrabbed.grabber = this;                          // ...before switching hand.                

                pieceGrabbed.transform.parent = transform;
            }
        }        
        else
        {
            pieceGrabbed.TryAndReleaseThePiece();
        }
    }    
}
