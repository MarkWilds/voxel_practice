using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    public int tilesetWidth = 16;
    public float tilesetDimension = 512.0f;
    public int tilesetIndex = 0;

    private Block[,,] blocks;
    public Material material;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(build(16,8,16));
    }

    public IEnumerator build( int width, int height, int depth)
    {
        blocks = new Block[width, height, depth];
        
        Mesh chunkMesh = new Mesh();
        CombineInstance[] blockMeshes = new CombineInstance[width * height * depth];

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Block newBlock = new Block(position, Random.value < 0.5f);
                    blocks[x, y, z] = newBlock;
                }
            }
        }

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if(blocks[x, y, z].isSolid)
                        continue;
                    
                    int indexCombined = x + y * width + z * height * width;
                    blockMeshes[indexCombined].mesh = blocks[x, y, z].build(transform, tilesetIndex, tilesetWidth / tilesetDimension);
                    blockMeshes[indexCombined].transform = Matrix4x4.Translate(blocks[x, y, z].Position);
                }
            }
        }

        chunkMesh.name = "Chunk";
        chunkMesh.CombineMeshes(blockMeshes);
        chunkMesh.RecalculateBounds();

        gameObject.AddComponent<MeshRenderer>().material = material;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunkMesh;

        yield return null;
    }
}
