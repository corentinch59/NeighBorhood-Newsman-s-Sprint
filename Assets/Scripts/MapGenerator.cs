using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> buildingsList;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject crossRoadPrefab;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;//TODO remove if only square maps are needed
    private int gridSize =60;

    private int[,] grid; 

    private void Start()
    {
        grid = new int[gridWidth, gridHeight];
        GenerateMap();
    }

    //! Buildings with size > 1 won't work for now
    public void GenerateMap()
    {

        //* Generate buildings
        {
            float xOffset = 0;
            for (int i = 0; i < gridWidth; i++){
                float zOffset = 0;
                for (int j = 0; j < gridHeight; j++){
                    if (grid[i, j] == 1) continue;
                    GameObject buildingPrefab = buildingsList[Random.Range(0, buildingsList.Count)];
                    // Building buildingScript = buildingPrefab.GetComponent<Building>();
                    GameObject newObj = Instantiate(buildingPrefab, new Vector3(xOffset, 0, zOffset), Quaternion.identity);
                    newObj.transform.Rotate(0, 90 * Random.Range(0, 4), 0);
                    // for (int x = 0; x < buildingScript.Size.x; x++){ for (int y = 0; y < buildingScript.Size.y; y++){
                        grid[i + 0, j + 0] = 1;
                    // }}
                    zOffset += gridSize;
                }
                xOffset += gridSize;
            }
        }
        

        //* Generate roads between buildings
        {
            //TODO not generate road on buildings
            //? cross roads
            for (int i = -gridSize/2; i <= gridWidth*gridSize; i += gridSize){
                for (int j = -gridSize/2; j <= gridHeight*gridSize; j += gridSize){
                    Instantiate(crossRoadPrefab, new Vector3(i, 0, j), Quaternion.identity).transform.localScale = new Vector3(2, 1, 2);

                }
            }
            //? line roads horizontal
            for (int i = 0; i < gridWidth*gridSize; i += gridSize){
                for (int j = -gridSize/2; j <= gridHeight*gridSize; j += gridSize){
                    Instantiate(roadPrefab, new Vector3(i, 0, j), Quaternion.identity).transform.localScale = new Vector3(2, 1, 2);
                }
            }
            //? line roads vertical
            for (int i = -gridSize/2; i <= gridWidth*gridSize; i += gridSize) {
                for (int j = 0; j < gridHeight*gridSize; j += gridSize){
                    GameObject newObj = Instantiate(roadPrefab, new Vector3(i , 0, j ), Quaternion.identity);
                    newObj.transform.localScale = new Vector3(2, 1, 2);
                    newObj.transform.Rotate(0, 90, 0);
                }
            }
        }
       
    }


}