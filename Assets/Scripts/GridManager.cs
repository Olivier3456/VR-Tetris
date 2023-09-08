using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static TetrisGrid tetrisGrid;

    public GameObject cubePrefabToSeeEmptyCells;
    public GameObject cubePrefabToSeeFullCells;
    static private GameObject cubePrefabToSeeFullCells_static;
    [Space(5)]
    [Range(3, 10)] public int sizeX = 5;
    [Range(8, 20)] public int sizeY = 12;
    [Range(3, 10)] public int sizeZ = 5;
    [Space(5)]
    public static Vector3 gridOriginPosition = Vector3.zero;
    [Space(5)]
    public static float scaleOfCells = 0.1f;

    private static int totalNumberOfCellsInEachFloor;

    public static Block[,,] deadBlocks;

    private void Start()
    {
        CreateGrid();
        DisplayGridWithSeparateBlocks();
        //DisplayGridWithOneBlock(cubePrefabToSeeEmptyCells);
        totalNumberOfCellsInEachFloor = sizeX * sizeZ;
        // Debug.Log("totalNumberOfCellsInEachFloor = " + totalNumberOfCellsInEachFloor);   // OK
        cubePrefabToSeeFullCells_static = cubePrefabToSeeFullCells;
        deadBlocks = new Block[sizeX, sizeY, sizeZ];
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

                    tetrisGrid.gridArray[x, y, z].visualGameObject = obj;
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
        return tetrisGrid.gridArray[posX, posY, posZ].isFull;
    }


    public static void SetThisCellAsFull(int posX, int posY, int posZ, bool destroyVisualEmptyCell = true, bool visuallyMarkAsFull = false)
    {
        tetrisGrid.gridArray[posX, posY, posZ].isFull = true;
        tetrisGrid.floorsFullCellsNumberArray[posY]++;

        //Debug.Log("GridManager.SetThisCellAsFull() ---> tetrisGrid.floorsFullCellsNumberArray[" + posY + "] = " + tetrisGrid.floorsFullCellsNumberArray[posY]); // OK

        if (destroyVisualEmptyCell && tetrisGrid.gridArray[posX, posY, posZ].visualGameObject != null)
        {
            Destroy(tetrisGrid.gridArray[posX, posY, posZ].visualGameObject);
        }

        if (visuallyMarkAsFull)
        {
            Vector3 positionOfCell = tetrisGrid.gridArray[posX, posY, posZ].worldPosition;
            tetrisGrid.gridArray[posX, posY, posZ].visualGameObject = Instantiate(cubePrefabToSeeFullCells_static);
            float sizeOfVisual = scaleOfCells - scaleOfCells * 0.075f;
            tetrisGrid.gridArray[posX, posY, posZ].visualGameObject.transform.localScale = new Vector3(sizeOfVisual, sizeOfVisual, sizeOfVisual);
            tetrisGrid.gridArray[posX, posY, posZ].visualGameObject.transform.position = positionOfCell;
        }
    }

    
    public static void CheckIfTheseFloorsAreFull(List<int> floorsToCheck)
    {
        // Order floors to check from highest to lowest: avoid to miss floors to check that are higher than floors already checked.
        floorsToCheck = floorsToCheck.OrderByDescending(x => x).ToList();

        int numberOfFullFloors = 0; // Not currently in use.

        for (int i = 0; i < floorsToCheck.Count; i++)
        {
            if (floorsToCheck[i] >= tetrisGrid.gridArray.GetLength(1) || floorsToCheck[i] < 0)
            {
                Debug.LogError("This floor is outside of the bounds of the grid!");
            }
            else
            {                
                //Debug.Log("tetrisGrid.floorsFullCellsNumberArray[floorsToCheck[" + i + "] = " + tetrisGrid.floorsFullCellsNumberArray[floorsToCheck[i]]); // Seems OK

                if (tetrisGrid.floorsFullCellsNumberArray[floorsToCheck[i]] == totalNumberOfCellsInEachFloor)
                {
                    Debug.Log("The floor " + i + " is full!");
                    numberOfFullFloors++;
                    FloorsFall(i);
                }
            }
        }
    }


    private static void FloorsFall(int floorFull)
    {
        int sizeX = tetrisGrid.gridArray.GetLength(0);
        int sizeY = tetrisGrid.gridArray.GetLength(1);
        int sizeZ = tetrisGrid.gridArray.GetLength(2);


        for (int i = floorFull; i < tetrisGrid.floorsFullCellsNumberArray.Length - 1; i++)
        {
            // If a floor is completely empty, no need to continue: higher floors are empty too.
            if (tetrisGrid.floorsFullCellsNumberArray[i] == 0)
            {
                return;
            }

            // Each floor, starting at the full floor, inherits the number of full cells of the floor directly above it.
            tetrisGrid.floorsFullCellsNumberArray[i] = tetrisGrid.floorsFullCellsNumberArray[i + 1];

            // Now, each cell of the grid inherits the full status (true or false) of the cell directly above it,
            // and inherits its dead block, if there is one.
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = floorFull; y < sizeY - 1; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        tetrisGrid.gridArray[x, y, z].isFull = tetrisGrid.gridArray[x, y + 1, z].isFull;

                        // If their is a dead block directly above this grid cell...
                        if (deadBlocks[x, y + 1, z] != null)
                        {
                            // move down the block above.
                            deadBlocks[x, y + 1, z].transform.position = tetrisGrid.gridArray[x, y, z].worldPosition;

                            // Destroy the dead block gameObject if it exists and replace it in deadBlocks[,,] by the block above.
                            if (deadBlocks[x, y, z] != null)
                            {
                                Destroy(deadBlocks[x, y, z].gameObject);
                                deadBlocks[x, y, z] = deadBlocks[x, y + 1, z];
                            }
                        }
                        else
                        {
                            // No dead block directly above this grid cell: destroy its dead block if it exists.
                            if (deadBlocks[x, y, z] != null)
                            {
                                Destroy(deadBlocks[x, y, z].gameObject);
                                deadBlocks[x, y, z] = null;
                            }
                        }
                    }
                }
            }
        }

        // The highest floor is always empty: no dead blocks to fall on it.
        tetrisGrid.floorsFullCellsNumberArray[tetrisGrid.floorsFullCellsNumberArray.Length - 1] = 0;
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                deadBlocks[x, sizeY, z] = null;
            }
        }
    }
}
