using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField]
    private int chunkSize;

    [SerializeField]
    [Range(1, 16)]
    private int worldSize = 1;

    [SerializeField]
    private int chunkColums = 4;

    [SerializeField]
    private Material textureAtlas;

    private Dictionary<string, Chunk> chunkHash;

    public World()
    {
        chunkHash = new Dictionary<string, Chunk>();
    }

	// Use this for initialization
	void Start () {
	    StartCoroutine(createChunks());
	}

    private IEnumerator createChunks()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                for (int i = 0; i < chunkColums; i++)
                {
                    Vector3 chunkPosition = new Vector3(transform.position.x + x * chunkSize,
                        transform.position.y + chunkSize * i, 
                        transform.position.z + z * chunkSize);

                    Chunk chunk = new Chunk(chunkPosition, textureAtlas, chunkSize, chunkSize);
                    chunk.GameObject.transform.parent = transform;
                    chunkHash[chunk.GameObject.name] = chunk;
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> pair in chunkHash)
        {
            pair.Value.buildMesh(chunkSize, chunkSize, chunkSize);
        }

        yield return null;
    }
    
}
