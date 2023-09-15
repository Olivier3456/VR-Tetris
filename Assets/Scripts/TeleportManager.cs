using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] private InputActionReference teleport;
    [Space(10)]
    [SerializeField] private GameObject player;

    private Transform[] teleportAnchors = new Transform[4];

    private int actualAnchorIndex = 0;


    private void OnEnable()
    {
        teleport.action.Enable();
        teleport.action.performed += Teleport;
    }
    private void OnDisable()
    {
        teleport.action.Disable();
        teleport.action.performed -= Teleport;
    }


    private void Start()
    {
        CreateTeleportationAnchors();
        PlaceAndRotateAnchors();
    }

    private void CreateTeleportationAnchors()
    {
        GameObject anchors = new GameObject("Teleportation Anchors");

        GameObject anchor1 = new GameObject("Anchor1");
        anchor1.transform.parent = anchors.transform;
        teleportAnchors[0] = anchor1.transform;

        GameObject anchor2 = new GameObject("Anchor2");
        anchor2.transform.parent = anchors.transform;
        teleportAnchors[1] = anchor2.transform;

        GameObject anchor3 = new GameObject("Anchor3");
        anchor3.transform.parent = anchors.transform;
        teleportAnchors[2] = anchor3.transform;

        GameObject anchor4 = new GameObject("Anchor4");
        anchor4.transform.parent = anchors.transform;
        teleportAnchors[3] = anchor4.transform;
    }

    private void PlaceAndRotateAnchors()
    {
        TetrisGrid grid = GridManager.instance.tetrisGrid;
        float gridCenterPosX = grid.worldPosition.x + (grid.sizeOfCells * grid.xNumberOfCells * 0.5f) + (grid.sizeOfCells * -0.5f);
        float gridBaseY = grid.worldPosition.y + (grid.sizeOfCells * -0.5f);
        float gridCenterPosZ = grid.worldPosition.z + (grid.sizeOfCells * grid.zNumberOfCells * 0.5f) + (grid.sizeOfCells * -0.5f);
        Vector3 XYgridCenter = new Vector3(gridCenterPosX, gridBaseY, gridCenterPosZ);

        float offset = 0.35f;
        float distanceToCenter = (grid.sizeOfCells * grid.xNumberOfCells * 0.5f) + offset;

        teleportAnchors[0].transform.position = new Vector3(XYgridCenter.x + distanceToCenter, XYgridCenter.y, XYgridCenter.z);
        teleportAnchors[1].transform.position = new Vector3(XYgridCenter.x, XYgridCenter.y, XYgridCenter.z + distanceToCenter);
        teleportAnchors[2].transform.position = new Vector3(XYgridCenter.x - distanceToCenter, XYgridCenter.y, XYgridCenter.z);
        teleportAnchors[3].transform.position = new Vector3(XYgridCenter.x, XYgridCenter.y, XYgridCenter.z - distanceToCenter);

        for (int i = 0; i < teleportAnchors.Length; i++)
        {
            teleportAnchors[i].LookAt(XYgridCenter);
        }
    }



    private void Teleport(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        if (Mathf.Abs(value.x) > 0.5f)
        {
            if (value.x > 0)
            {
                //DebugLog.Log("Teleport Right");

                if (actualAnchorIndex < teleportAnchors.Length - 1)
                {
                    actualAnchorIndex++;                    
                }
                else
                {
                    actualAnchorIndex = 0;
                }
            }
            else
            {
                //DebugLog.Log("Teleport Left");

                if (actualAnchorIndex > 0)
                {
                    actualAnchorIndex--;
                }
                else
                {
                    actualAnchorIndex = teleportAnchors.Length - 1;
                }
            }

            player.transform.position = teleportAnchors[actualAnchorIndex].position;
            player.transform.rotation = teleportAnchors[actualAnchorIndex].rotation;
        }
    }
}