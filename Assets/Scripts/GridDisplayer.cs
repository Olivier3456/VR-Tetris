using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefabToSeeEmptyCells;

    [SerializeField] private bool displayGridWithOneBlock = true;
    [SerializeField] private bool displayGridWithSeparateBlocks = false;



    public void DisplayGrid(TetrisGrid tetrisGrid)
    {
        if (displayGridWithOneBlock)
        {
            DisplayGridWithOneBlock(tetrisGrid);
        }
        if (displayGridWithSeparateBlocks)
        {
            DisplayGridWithSeparateBlocks(tetrisGrid);
        }
    }



    private void DisplayGridWithSeparateBlocks(TetrisGrid tetrisGrid)
    {
        int sizeX = tetrisGrid.xNumberOfCells;
        int sizeY = tetrisGrid.yNumberOfCells;
        int sizeZ = tetrisGrid.zNumberOfCells;

        GameObject gridCellVisuals = new GameObject();
        gridCellVisuals.name = "Grid cells visuals";

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    GameObject obj = Instantiate(cubePrefabToSeeEmptyCells, tetrisGrid.gridArray[x, y, z].worldPosition, Quaternion.identity, gridCellVisuals.transform);
                    float sizeOfVisual = tetrisGrid.sizeOfCells - tetrisGrid.sizeOfCells * 0.075f;
                    obj.transform.localScale = new Vector3(sizeOfVisual, sizeOfVisual, sizeOfVisual);                    
                }
            }
        }
    }

    private void DisplayGridWithOneBlock(TetrisGrid tetrisGrid)
    {
        int sizeX = tetrisGrid.xNumberOfCells;
        int sizeY = tetrisGrid.yNumberOfCells;
        int sizeZ = tetrisGrid.zNumberOfCells;
        float scaleOfCells = tetrisGrid.sizeOfCells;

        Vector3 scale = new Vector3(sizeX * scaleOfCells, sizeY * scaleOfCells, sizeZ * scaleOfCells);

        Vector3 position = new Vector3(tetrisGrid.worldPosition.x + scale.x * 0.5f,
                                       tetrisGrid.worldPosition.y + scale.y * 0.5f,
                                       tetrisGrid.worldPosition.z + scale.z * 0.5f)
        /* offset */     - new Vector3(scaleOfCells * 0.5f, scaleOfCells * 0.5f, scaleOfCells * 0.5f);

        GameObject cube = Instantiate(cubePrefabToSeeEmptyCells, position, Quaternion.identity);
        cube.transform.localScale = scale;
    }
}
