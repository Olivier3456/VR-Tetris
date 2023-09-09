using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static TetrisGrid tetrisGrid;

    public GameObject cubePrefabToSeeEmptyCells;
    [Space(5)]
    [Range(3, 10)] public int sizeX = 5;
    [Range(8, 20)] public int sizeY = 12;
    [Range(3, 10)] public int sizeZ = 5;
    [Space(5)]
    public static Vector3 gridOriginPosition = Vector3.zero;
    [Space(5)]
    public static float scaleOfCells = 0.1f;

    private static int totalNumberOfCellsInEachFloor;

    //public static Block[,,] deadBlocks;


    private void Awake()
    {
        CreateGrid();
    }


    private void Start()
    {
        DisplayGridWithSeparateBlocks();
        //DisplayGridWithOneBlock(cubePrefabToSeeEmptyCells);
        totalNumberOfCellsInEachFloor = sizeX * sizeZ;
    }



    private void CreateGrid()
    {
        tetrisGrid = new TetrisGrid(sizeX, sizeY, sizeZ, gridOriginPosition, scaleOfCells);
    }


    private void DisplayGridWithSeparateBlocks()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    GameObject obj = Instantiate(cubePrefabToSeeEmptyCells, tetrisGrid.gridArray[x, y, z].worldPosition, Quaternion.identity);
                    float sizeOfVisual = scaleOfCells - scaleOfCells * 0.075f;
                    obj.transform.localScale = new Vector3(sizeOfVisual, sizeOfVisual, sizeOfVisual);

                    tetrisGrid.gridArray[x, y, z].visualMarker = obj;
                }
            }
        }
    }


    public void DisplayGridWithOneBlock(GameObject objectToDisplay)
    {
        Vector3 scale = new Vector3(sizeX * scaleOfCells, sizeY * scaleOfCells, sizeZ * scaleOfCells);

        Vector3 position = new Vector3(gridOriginPosition.x + scale.x * 0.5f,
                                       gridOriginPosition.y + scale.y * 0.5f,
                                       gridOriginPosition.z + scale.z * 0.5f);

        GameObject cube = Instantiate(objectToDisplay, position, Quaternion.identity);
        cube.transform.localScale = scale;
    }


    public static bool IsThisCellFull(int posX, int posY, int posZ)
    {
        return tetrisGrid.gridArray[posX, posY, posZ].blockInThisCell != null;
    }


    public static void Fill_a_cell_with_a_block(Block block, bool destroyVisualEmptyCell = false)
    {
        Vector3Int pos = block.blockPositionOnGrid;

        tetrisGrid.gridArray[pos.x, pos.y, pos.z].blockInThisCell = block;
        tetrisGrid.floorsFullCellsNumberArray[pos.y]++;

        if (destroyVisualEmptyCell && tetrisGrid.gridArray[pos.x, pos.y, pos.z].visualMarker != null)
        {
            Destroy(tetrisGrid.gridArray[pos.x, pos.y, pos.z].visualMarker);
        }
    }


    public static void CheckIfTheseFloorsAreFull(List<int> floorsToCheck)
    {
        // Order floors to check from highest to lowest avoid missing checking floors that are higher than floors already checked.
        floorsToCheck = floorsToCheck.OrderByDescending(x => x).ToList();

        for (int i = 0; i < floorsToCheck.Count; i++)
        {
            if (floorsToCheck[i] >= tetrisGrid.gridArray.GetLength(1) || floorsToCheck[i] < 0)
            {
                DebugLogs.ShowMessage("This floor is outside of the bounds of the grid!");
            }
            else
            {
                if (tetrisGrid.floorsFullCellsNumberArray[floorsToCheck[i]] == totalNumberOfCellsInEachFloor)
                {
                    DebugLogs.ShowMessage("The floor " + i + " is full!");
                    FloorsFall(floorsToCheck[i]);
                }
            }
        }
    }


    private static void FloorsFall(int floorFull)
    {
        int sizeX = tetrisGrid.gridArray.GetLength(0);
        int sizeY = tetrisGrid.gridArray.GetLength(1);
        int sizeZ = tetrisGrid.gridArray.GetLength(2);


        // Destroy the actual dead block Game Objects of all the cells of the full floor, and empty the grid cell.
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                if (tetrisGrid.gridArray[x, floorFull, z].blockInThisCell != null)
                {
                    Destroy(tetrisGrid.gridArray[x, floorFull, z].blockInThisCell.gameObject);
                }
                else
                {
                    DebugLogs.ShowMessage("There is something wrong: the full floor " + floorFull + " don't have a deadblock on the cell: " + x + ", " + floorFull + ", " + z + "!");
                }
            }
        }


        // Each floor, starting at the full floor, inherits the number of full cells of the floor directly above it.
        // (We start at the full floor index, and stop at the second-to-last floor index: the highest floor will be treated separatly.)
        for (int y = floorFull; y < sizeY - 1; y++)
        {
            //If a floor is completely empty, no need to continue: higher floors are empty too.
            if (tetrisGrid.floorsFullCellsNumberArray[y] == 0)
            {
                return;
            }

            tetrisGrid.floorsFullCellsNumberArray[y] = tetrisGrid.floorsFullCellsNumberArray[y + 1];
        }


        // Now, each cell of the grid inherits the block (if there is one) of the cell directly above it.
        // (We start at the full floor index, and stop at the second-to-last floor index: the highest floor will be treated separatly.)
        for (int x = 0; x < sizeX; x++)
        {
            // For Y, we start at the full floor index, and stop at the second-to-last floor index.
            for (int y = floorFull; y < sizeY - 1; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    // If there is a block directly above this grid cell...
                    if (tetrisGrid.gridArray[x, y + 1, z].blockInThisCell != null)
                    {
                        // ...the block is falling.
                        tetrisGrid.gridArray[x, y + 1, z].blockInThisCell.transform.position = tetrisGrid.gridArray[x, y, z].worldPosition;
                        tetrisGrid.gridArray[x, y + 1, z].blockInThisCell.blockPositionOnGrid = new Vector3Int(x, y, z);
                        tetrisGrid.gridArray[x, y, z].blockInThisCell = tetrisGrid.gridArray[x, y + 1, z].blockInThisCell;
                    }
                    else
                    {
                        // If there is no dead block directly above this grid cell, it becomes or stays empty.
                        tetrisGrid.gridArray[x, y, z].blockInThisCell = null;
                    }
                }
            }
        }


        // The highest floor is always empty: no blocks to fall on it.
        tetrisGrid.floorsFullCellsNumberArray[sizeY - 1] = 0;
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                if (tetrisGrid.gridArray[x, sizeY - 1, z].blockInThisCell != null)
                {
                    Destroy(tetrisGrid.gridArray[x, sizeY - 1, z].blockInThisCell.gameObject);
                    tetrisGrid.gridArray[x, sizeY - 1, z].blockInThisCell = null;
                }
            }
        }
    }
}
