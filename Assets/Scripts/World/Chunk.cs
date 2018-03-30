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
                    Vector3 position = new Vector3(x, y, z);
                    Block newBlock = new Block(position, Random.value < 0.5f);
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
