using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private int chunkSize;

    [SerializeField] [UnityEngine.Range(1, 16)] private int radius = 1;

    [SerializeField] private int chunkColums = 4;

    [SerializeField] private Material textureAtlas;

    private Dictionary<string, Chunk> chunkHash;
    private List<string> chunkRemoveList;
    private bool isBuilding;

    public World()
    {
        chunkHash = new Dictionary<string, Chunk>();
        chunkRemoveList = new List<string>();
    }

    void Update()
    {
        if (!isBuilding)
        {
            StartCoroutine(updateChunks());
        }
    }

    private IEnumerator updateChunks()
    {
        isBuilding = true;
        int playerChunkX = (int) (player.transform.position.x / chunkSize);
        int playerChunkY = (int) (player.transform.position.z / chunkSize);

        // set all chunks to old, this can be seen as a clear
        foreach (Chunk chunk in chunkHash.Values)
        {
            chunk.State = Chunk.ChunkState.OLD;
        }

        // set chunks within radius to current or new when the chunk is new within a radius
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                for (int y = 0; y < chunkColums; y++)
                {
                    Vector3 chunkPosition = new Vector3((playerChunkX + x) * chunkSize,
                        y * chunkSize,
                        (playerChunkY + z) * chunkSize);
                    string chunkIdentifier = Chunk.getChunkIdentifier(chunkPosition);

                    Chunk chunk;
                    if (chunkHash.TryGetValue(chunkIdentifier, out chunk))
                    {
                        chunk.State = Chunk.ChunkState.CURRENT;
                    }
                    else
                    {
                        chunk = new Chunk(chunkPosition, textureAtlas, chunkSize, chunkSize);
                        chunk.State = Chunk.ChunkState.NEW;
                        chunk.GameObject.transform.parent = transform;
                        chunkHash[chunk.GameObject.name] = chunk;
                    }
                }
            }
        }

        // build new chunks, collect old chunks
        foreach (KeyValuePair<string, Chunk> pair in chunkHash)
        {
            Chunk chunk = pair.Value;

            if (chunk.State == Chunk.ChunkState.NEW)
            {
                chunk.buildMesh(chunkSize, chunkSize, chunkSize);
            }
            else if (chunk.State == Chunk.ChunkState.OLD)
            {
                chunkRemoveList.Add(pair.Key);
            }

            yield return null;
        }
        
        // remove old chunks
        foreach (string chunkIdentifier in chunkRemoveList)
        {
            Chunk chunk;
            if (chunkHash.TryGetValue(chunkIdentifier, out chunk))
            {
                Destroy(chunk.GameObject);
                chunkHash.Remove(chunkIdentifier);
                chunk = null;
            }

        }
        chunkRemoveList.Clear();
        isBuilding = false;
    }
}