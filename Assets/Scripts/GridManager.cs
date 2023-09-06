using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static TetrisGrid tetrisGrid;

    public GameObject cubePrefabToSeeEmptyCells;
    public GameObject cubePrefabToSeeFullCells;
    static private GameObject cubePrefabToSeeFullCells_static;
    [Space(5)]
    [Range(4, 10)] public int sizeX = 5;
    [Range(8, 20)] public int sizeY = 12;
    [Range(4, 10)] public int sizeZ = 5;
    [Space(5)]
    public static Vector3 gridOriginPosition = Vector3.zero;
    [Space(5)]
    public static float scaleOfCells = 0.2f;

    private void Start()
    {

        CreateGrid();
        DisplayGridWithSeparateBlocks();
        //DisplayGridWithOneBlock(cubePrefabToSeeEmptyCells);

        cubePrefabToSeeFullCells_static = cubePrefabToSeeFullCells;
    }



    public void CreateGrid()
    {
        tetrisGrid = new TetrisGrid(sizeX, sizeY, sizeZ, gridOriginPosition, scaleOfCells);
    }


    public void DisplayGridWithSeparateBlocks()
    {
        int xSize = tetrisGrid.gridArray.GetLength(0);
        int ySize = tetrisGrid.gridArray.GetLength(1);
        int zSize = tetrisGrid.gridArray.GetLength(2);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
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


    public static void SetThisCellAsFull(int posX, int posY, int posZ, bool visuallyMarkAsFull = false)
    {
        tetrisGrid.gridArray[posX, posY, posZ].isFull = true;

        if (visuallyMarkAsFull)
        {
            if (tetrisGrid.gridArray[posX, posY, posZ].visualGameObject != null)
            {
                Vector3 positionOfCell = tetrisGrid.gridArray[posX, posY, posZ].visualGameObject.transform.position;
                Destroy(tetrisGrid.gridArray[posX, posY, posZ].visualGameObject);
                tetrisGrid.gridArray[posX, posY, posZ].visualGameObject = Instantiate(cubePrefabToSeeFullCells_static);
                float sizeOfVisual = scaleOfCells - scaleOfCells * 0.075f;
                tetrisGrid.gridArray[posX, posY, posZ].visualGameObject.transform.localScale = new Vector3(sizeOfVisual, sizeOfVisual, sizeOfVisual);
                tetrisGrid.gridArray[posX, posY, posZ].visualGameObject.transform.position = positionOfCell;
            }
        }
    }
}
