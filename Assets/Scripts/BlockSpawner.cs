using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;  // Assign your block prefab in the Inspector.

     public GameObject gridPrefab; 
    public Transform spawnPointParent;
    public List<Transform> spawnPoints;    // A Transform positioned at the top-center of your grid.

    void Start()
    {
        SpawnBlock();
    }

    public void CreateSpawnPoints(int width,int height)
    {
        for(int i=height+2,j = 0;j<width;j++)
        {
            Transform spawnPoint = Instantiate(gridPrefab,new Vector3(j,i,0),Quaternion.identity).transform;
            spawnPoints.Add(spawnPoint);
            spawnPoint.parent = spawnPointParent;
        }
    }
    // Call this method to spawn a new block.
    public void SpawnBlock()
    {
        int r = Random.Range(0,spawnPoints.Count);
        GameObject blockObj = Instantiate(blockPrefab,  spawnPoints[r].position, Quaternion.identity);
        Block block = blockObj.GetComponent<Block>();
        // Random number from 1 to 7.
        block.Initialize(Random.Range(1, 8));
    }
}
