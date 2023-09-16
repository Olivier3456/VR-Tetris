using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public Vector3 worldPosition;
    public Block blockInThisCell;
}


public struct TetrisGrid
{
    public int xNumberOfCells;
    public int yNumberOfCells;
    public int zNumberOfCells;

    public Vector3 worldPosition;

    public float sizeOfCells;

    public Cell[,,] gridArray;

    public int[] floorsFullCellsNumberArray;


    // constructor
    public TetrisGrid(int xNumberOfCells, int yNumberOfCells, int zNumberOfCells, Vector3 worldPosition, float scaleOfCells)
    {
        this.xNumberOfCells = xNumberOfCells;
        this.yNumberOfCells = yNumberOfCells;
        this.zNumberOfCells = zNumberOfCells;
        this.worldPosition = worldPosition;
        sizeOfCells = scaleOfCells;

        floorsFullCellsNumberArray = new int[yNumberOfCells];
        for (int i = 0; i < floorsFullCellsNumberArray.Length; i++)
        {
            floorsFullCellsNumberArray[i] = 0;
        }

        gridArray = new Cell[xNumberOfCells, yNumberOfCells, zNumberOfCells];

        for (int i = 0; i < xNumberOfCells; i++)
        {
            for (int j = 0; j < yNumberOfCells; j++)
            {
                for (int k = 0; k < zNumberOfCells; k++)
                {
                    gridArray[i, j, k] = new Cell();

                    Vector3 cellWorldPosition = new Vector3(worldPosition.x + (i * scaleOfCells),
                                                            worldPosition.y + (j * scaleOfCells),
                                                            worldPosition.z + (k * scaleOfCells));

                    gridArray[i, j, k].worldPosition = cellWorldPosition;
                }
            }
        }
    }
}
