using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridDisplayer : MonoBehaviour
{
    [SerializeField] private bool displayGridWithOneBlock = false;
    [SerializeField] private bool displayGridWithSeparateBlocks = false;
    [SerializeField] private bool displayGridWithImages = true;
    [Space(10)]
    [SerializeField] private GameObject cubePrefabToSeeEmptyCells;
    [SerializeField] private GameObject canvasPrefabToDisplayGrid;
    [SerializeField] private GameObject squareImageGridDisplay;


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
        if (displayGridWithImages)
        {
            DisplayGridWithImages(tetrisGrid);
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


    private void DisplayGridWithImages(TetrisGrid tetrisGrid)
    {
        float sizeOfImage = tetrisGrid.sizeOfCells;

        Vector2 sizeOfImages = new Vector2(sizeOfImage, sizeOfImage);

        int sizeX = tetrisGrid.xNumberOfCells;
        int sizeY = tetrisGrid.yNumberOfCells;
        int sizeZ = tetrisGrid.zNumberOfCells;

        GameObject canvas = Instantiate(canvasPrefabToDisplayGrid, tetrisGrid.worldPosition, Quaternion.identity);

        for (int y = 0; y < sizeY; y++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Vector3 position;
                Vector3 offset = new Vector3(sizeOfImage * 0.5f, 0, 0);

                RectTransform imageRectTransform;

                position = tetrisGrid.gridArray[0, y, z].worldPosition - offset;
                imageRectTransform = Instantiate(squareImageGridDisplay, position, Quaternion.identity, canvas.transform).GetComponent<RectTransform>();
                imageRectTransform.sizeDelta = sizeOfImages;
                imageRectTransform.Rotate(Vector3.up * 90);


                position = tetrisGrid.gridArray[tetrisGrid.xNumberOfCells - 1, y, z].worldPosition + offset;
                imageRectTransform = Instantiate(squareImageGridDisplay, position, Quaternion.identity, canvas.transform).GetComponent<RectTransform>();
                imageRectTransform.sizeDelta = sizeOfImages;
                imageRectTransform.Rotate(Vector3.up * 90);
            }
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3 position;
                Vector3 offset = new Vector3(0, 0, sizeOfImage * 0.5f);

                RectTransform imageRectTransform;

                position = tetrisGrid.gridArray[x, y, 0].worldPosition - offset;
                imageRectTransform = Instantiate(squareImageGridDisplay, position, Quaternion.identity, canvas.transform).GetComponent<RectTransform>();
                imageRectTransform.sizeDelta = sizeOfImages;

                position = tetrisGrid.gridArray[x, y, tetrisGrid.zNumberOfCells - 1].worldPosition + offset;
                imageRectTransform = Instantiate(squareImageGridDisplay, position, Quaternion.identity, canvas.transform).GetComponent<RectTransform>();
                imageRectTransform.sizeDelta = sizeOfImages;
            }
        }
    }
}
