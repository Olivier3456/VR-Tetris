using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public TetrisGrid tetrisGrid;

    public GameObject cubePrefabToSeeEmptyCells;
    [Space(5)]
    [Range(4, 6)] public int sizeX = 5;
    [Range(14, 20)] public int sizeY = 18;
    [Range(4, 6)] public int sizeZ = 5;
    [Space(5)]
    public Vector3 gridOriginPosition = Vector3.zero;
    [Space(5)]
    public float scaleOfCells = 0.1f;

    private int totalNumberOfCellsInEachFloor;

    public static GridManager instance;




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("An instance of GridManager already exists!");
        }

        CreateGrid();
    }


    private void Start()
    {
        //DisplayGridWithSeparateBlocks();
        DisplayGridWithOneBlock();
        totalNumberOfCellsInEachFloor = sizeX * sizeZ;
    }


    private void CreateGrid()
    {
        tetrisGrid = new TetrisGrid(sizeX, sizeY, sizeZ, gridOriginPosition, scaleOfCells);
    }


    private void DisplayGridWithSeparateBlocks()
    {
        GameObject gridCellVisuals = new GameObject();
        gridCellVisuals.name = "Grid cells visuals";

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    GameObject obj = Instantiate(cubePrefabToSeeEmptyCells, tetrisGrid.gridArray[x, y, z].worldPosition, Quaternion.identity, gridCellVisuals.transform);
                    float sizeOfVisual = scaleOfCells - scaleOfCells * 0.075f;
                    obj.transform.localScale = new Vector3(sizeOfVisual, sizeOfVisual, sizeOfVisual);

                    tetrisGrid.gridArray[x, y, z].visualMarker = obj;
                }
            }
        }
    }

    public void DisplayGridWithOneBlock()
    {
        Vector3 scale = new Vector3(sizeX * scaleOfCells, sizeY * scaleOfCells, sizeZ * scaleOfCells);

        Vector3 position = new Vector3(gridOriginPosition.x + scale.x * 0.5f,
                                       gridOriginPosition.y + scale.y * 0.5f,
                                       gridOriginPosition.z + scale.z * 0.5f)
        /* offset */     - new Vector3(scaleOfCells * 0.5f, scaleOfCells * 0.5f, scaleOfCells * 0.5f);

        GameObject cube = Instantiate(cubePrefabToSeeEmptyCells, position, Quaternion.identity);
        cube.transform.localScale = scale;
    }




    public bool IsThisCellFull(int posX, int posY, int posZ, Piece pieceToIgnore, bool ignoreAllPiecesAlive)
    {
        // (The piece to ignore is the piece which want to know if the grid cells of its blocks are full.)


        DebugLog.Log("Is this cell full() - ignoreAllPiecesAlive = " + ignoreAllPiecesAlive);

        if (ignoreAllPiecesAlive)
        {
            return tetrisGrid.gridArray[posX, posY, posZ].blockInThisCell != null && tetrisGrid.gridArray[posX, posY, posZ].blockInThisCell.piece != null;
        }
        else
        {
            return tetrisGrid.gridArray[posX, posY, posZ].blockInThisCell != null && tetrisGrid.gridArray[posX, posY, posZ].blockInThisCell.piece != pieceToIgnore;
        }
    }


    public void Fill_a_cell_with_a_block(Block block, bool isADeadPiece)
    {
        Vector3Int pos = block.positionOnGrid;

        tetrisGrid.gridArray[pos.x, pos.y, pos.z].blockInThisCell = block;

        if (isADeadPiece)
        {
            tetrisGrid.floorsFullCellsNumberArray[pos.y]++;
        }
    }

    /// <summary>
    /// This method is only called when a piece alive is falling, and its blocks were filling temporarily the cells.
    /// </summary>
    public void Empty_a_Cell(int x, int y, int z)
    {
        tetrisGrid.gridArray[x, y, z].blockInThisCell = null;
    }


    public void CheckIfTheseFloorsAreFull(List<int> floorsToCheck)
    {
        // Order floors to check from highest to lowest avoid missing checking floors that are higher than floors already checked.
        floorsToCheck = floorsToCheck.OrderByDescending(x => x).ToList();

        int numberOfLevelsFilledByThisPiece = 0;

        for (int i = 0; i < floorsToCheck.Count; i++)
        {
            if (floorsToCheck[i] >= tetrisGrid.gridArray.GetLength(1) || floorsToCheck[i] < 0)
            {
                DebugLog.Log("This floor is outside of the bounds of the grid!");
            }
            else
            {
                if (tetrisGrid.floorsFullCellsNumberArray[floorsToCheck[i]] == totalNumberOfCellsInEachFloor)
                {
                    FloorsFall(floorsToCheck[i]);
                    numberOfLevelsFilledByThisPiece++;
                }
            }
        }

        if (numberOfLevelsFilledByThisPiece > 0)
        {
            GameManager.instance.AddScoreWhenLevelFull(numberOfLevelsFilledByThisPiece);
            AudioManager.instance.Play_FullFloor();
        }
    }

    private int lowestEmptyFloor;
    private void FloorsFall(int fullFloor)
    {
        int sizeX = tetrisGrid.gridArray.GetLength(0);
        int sizeY = tetrisGrid.gridArray.GetLength(1);
        int sizeZ = tetrisGrid.gridArray.GetLength(2);

        Destroy_all_blocks_of_the_full_floor(fullFloor, sizeX, sizeZ);

        Floors_inherit_the_full_cell_number_of_the_floor_directly_above(fullFloor, sizeY);

        Each_grid_cell_inherits_the_block_just_above(fullFloor, sizeX, sizeY, sizeZ);

        // lowestEmptyFloor = sizeY means that none of the floors checked by the previous function was empty. The last floor is possibly not empty.
        if (lowestEmptyFloor == sizeY)
        {
            Empty_the_highest_grid_floor(sizeX, sizeY, sizeZ);
        }
    }

    private void Destroy_all_blocks_of_the_full_floor(int fullFloor, int sizeX, int sizeZ)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                if (tetrisGrid.gridArray[x, fullFloor, z].blockInThisCell != null)
                {
                    Destroy(tetrisGrid.gridArray[x, fullFloor, z].blockInThisCell.gameObject);
                }
                else
                {
                    DebugLog.Log("There is something wrong: the full floor " + fullFloor + " don't have a block on the cell: " + x + ", " + fullFloor + ", " + z + "!");
                }
            }
        }
    }

    private void Floors_inherit_the_full_cell_number_of_the_floor_directly_above(int fullFloor, int sizeY)
    {
        // We reset the lowest empty floor index to a value y cannot reach.
        lowestEmptyFloor = sizeY;

        // We start at the full floor index, and stop at the second-to-last floor index: the highest floor will be treated separatly.
        for (int y = fullFloor; y < sizeY - 1; y++)
        {
            //If a floor is completely empty, no need to continue: all higher floors are empty too.
            if (tetrisGrid.floorsFullCellsNumberArray[y] == 0)
            {
                lowestEmptyFloor = y;
                return;
            }

            tetrisGrid.floorsFullCellsNumberArray[y] = tetrisGrid.floorsFullCellsNumberArray[y + 1];
        }
    }

    private void Each_grid_cell_inherits_the_block_just_above(int fullFloor, int sizeX, int sizeY, int sizeZ)
    {
        for (int x = 0; x < sizeX; x++)
        {
            // We start at the full floor index, and stop either at the lowest empty floor, or at the second-to-last floor index: the highest floor will be treated separatly if it is not empty.
            for (int y = fullFloor; y < lowestEmptyFloor && y < sizeY - 1; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    // If there is a block directly above this grid cell...
                    if (tetrisGrid.gridArray[x, y + 1, z].blockInThisCell != null)
                    {
                        // ...the block is falling.
                        tetrisGrid.gridArray[x, y + 1, z].blockInThisCell.transform.position = tetrisGrid.gridArray[x, y, z].worldPosition;
                        tetrisGrid.gridArray[x, y + 1, z].blockInThisCell.positionOnGrid = new Vector3Int(x, y, z);
                        tetrisGrid.gridArray[x, y, z].blockInThisCell = tetrisGrid.gridArray[x, y + 1, z].blockInThisCell;
                    }
                    else
                    {
                        // If there is no block directly above this grid cell, it becomes or stays empty.
                        tetrisGrid.gridArray[x, y, z].blockInThisCell = null;
                    }
                }
            }
        }
    }

    private void Empty_the_highest_grid_floor(int sizeX, int sizeY, int sizeZ)
    {
        // The highest floor is always empty after a floor fall: no blocks to fall on it.
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
