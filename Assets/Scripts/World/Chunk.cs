using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
    public int tilesetWidth = 16;
    public float tilesetDimension = 512.0f;
    public int tilesetIndex = 250;

    private Block[,,] blocks;
    private GameObject chunkGameObject;
    private Material chunkMaterial;

    public GameObject GameObject
    {
        get
        {
            return chunkGameObject;
        }
    }

    private float fBM(float x, float y, int octaves, float persistance)
    {
        float total = 0.0f;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency * 0.01f, y * frequency * 0.01f) * amplitude;
            maxValue += amplitude;
            amplitude += persistance;
            frequency *= 2;
        }

        return total / maxValue * 180;
    }

    public Chunk(Vector3 position, Material material, int sizeHorizontal, int sizeVertical)
    {
        chunkGameObject = new GameObject(string.Format("{0}_{1}_{2}", position.x, position.y, position.z) );
        chunkGameObject.transform.position = position;
        chunkMaterial = material;
        buildData(sizeHorizontal, sizeVertical);
    }

    private void buildData(int sizeHorizontal, int sizeVertical)
    {
        blocks = new Block[sizeHorizontal, sizeVertical, sizeHorizontal];
        for (int z = 0; z < sizeHorizontal; z++)
        {
            for (int y = 0; y < sizeVertical; y++)
            {
                for (int x = 0; x < sizeHorizontal; x++)
                {
                    Vector3 chunkPosition = chunkGameObject.transform.position;
                    Vector3 position = new Vector3(x, y, z);
                    float perlin = fBM(chunkPosition.x + x, chunkPosition.z + z, 3, 0.4f);

                    Block newBlock = new Block(position, perlin > chunkPosition.y + y);
                    blocks[x, y, z] = newBlock;
                }
            }
        }
    }

    public void buildMesh( int width, int height, int depth)
    {
        List<CombineInstance> blockMeshes = new List<CombineInstance>();
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Block block = blocks[x, y, z];
                    if (!block.IsSolid)
                        continue;

                    Mesh mesh = block.build(blocks, chunkGameObject.transform.position, tilesetIndex, tilesetWidth / tilesetDimension);
                    if (mesh != null)
                    {
                        CombineInstance blockMesh = new CombineInstance();
                        blockMesh.mesh = mesh;
                        blockMesh.transform = Matrix4x4.Translate(block.Position);
                        blockMeshes.Add(blockMesh);
                    }
                }
            }
        }

        combineMeshes(blockMeshes.ToArray());
    }

    private void combineMeshes(CombineInstance[] combineMeshes )
    {
        Mesh chunkMesh = new Mesh();
        chunkMesh.name = "Chunk";
        chunkMesh.CombineMeshes(combineMeshes);
        chunkMesh.RecalculateBounds();

        chunkGameObject.AddComponent<MeshRenderer>().material = chunkMaterial;
        MeshFilter meshFilter = chunkGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunkMesh;
    }
}
