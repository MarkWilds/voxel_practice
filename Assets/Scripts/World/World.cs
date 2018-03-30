using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField]
    private int chunkSizeHorizontal;

    [SerializeField]
    private int chunkSizeVertical;

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
        for (int i = 0; i < chunkColums; i++)
        {
            Vector3 chunkPosition = new Vector3(transform.position.x, chunkSizeVertical * i, transform.position.z);
            Chunk chunk = new Chunk(chunkPosition, textureAtlas, chunkSizeHorizontal, chunkSizeVertical);
            chunk.GameObject.transform.parent = transform;
            chunkHash[chunk.GameObject.name] = chunk;
        }

        foreach (KeyValuePair<string, Chunk> pair in chunkHash)
        {
            pair.Value.buildMesh(chunkSizeHorizontal, chunkSizeVertical, chunkSizeHorizontal);
        }

        yield return null;
    }
    
}
