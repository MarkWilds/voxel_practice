using System;
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

    private MeshBuilder meshBuilder;

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

        meshBuilder = new MeshBuilder();
        meshBuilder.TileIndex = 250;
        meshBuilder.TileSizeUV = 32 / 512.0f;
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
                    float cavePerlin = Perlin.cavePerlin(chunkPosition.x + x, chunkPosition.y + y, chunkPosition.z + z);
                    float heightPerlin = Perlin.fractalBrownianMotion(chunkPosition.x + x, chunkPosition.z + z, 4, 0.5f);
                    heightPerlin *= 64;

                    Block newBlock = new Block(position, cavePerlin < 0.45f || heightPerlin > chunkPosition.y + y);
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

                    Mesh mesh = buildBlock(block);
                    if (mesh != null)
                    {
                        CombineInstance blockMesh = new CombineInstance();
                        blockMesh.mesh = mesh;
                        blockMesh.transform = chunkGameObject.transform.localToWorldMatrix * Matrix4x4.Translate(block.Position);
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

    public Mesh buildBlock(Block block)
    {
        List<CombineInstance> combineList = new List<CombineInstance>();

        createBlockSide(block.Position, Vector3.back, combineList);
        createBlockSide(block.Position, Vector3.forward, combineList);
        createBlockSide(block.Position, Vector3.left, combineList);
        createBlockSide(block.Position, Vector3.right, combineList);
        createBlockSide(block.Position, Vector3.down, combineList);
        createBlockSide(block.Position, Vector3.up, combineList);

        if (combineList.Count <= 0)
            return null;

        Mesh cubeMesh = new Mesh();
        cubeMesh.CombineMeshes(combineList.ToArray());

        return cubeMesh;
    }

    private void createBlockSide(Vector3 position, Vector3 sideNormal, List<CombineInstance> combineList)
    {
        if (!hasSolidNeightbour(position + sideNormal))
        {
            CombineInstance combine = new CombineInstance();
            combine.transform = Matrix4x4.Translate(position);
            combine.mesh = meshBuilder.createQuad(sideNormal);
            combineList.Add(combine);
        } 
    }

    /// <summary>
    /// HACKY, dont use exceptions to regulate flow.... :P bad Mark
    /// </summary>
    private bool hasSolidNeightbour(Vector3 coordinates)
    {
        try
        {
            return blocks[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z].IsSolid;
        }
        catch (Exception ex)
        {
        }

        return false;
    }
}
