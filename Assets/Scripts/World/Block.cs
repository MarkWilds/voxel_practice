using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// refactor to put generation of quads in the cunk code and only make this class contain data
/// </summary>
public class Block
{
    private float tilesizeUV;
    public Vector3 Position { get; private set; }
    public bool IsSolid { get; set; }

    public Block(Vector3 pos, bool solid)
    {
        Position = pos;
        IsSolid = solid;
    }

    public Mesh build(Block[,,] blocks, Vector3 chunkPosition, int tileIndex, float tileSizeNormalized)
    {
        tilesizeUV = tileSizeNormalized;
        List<CombineInstance> combineList = new List<CombineInstance>();

        if(!hasSolidNeightbour(blocks, (int)Position.x, (int)Position.y, (int)Position.z - 1))
            combineList.Add(createQuad(chunkPosition, Vector3.back, tileIndex));

        if (!hasSolidNeightbour(blocks, (int)Position.x, (int)Position.y, (int)Position.z + 1))
            combineList.Add(createQuad(chunkPosition, Vector3.forward, tileIndex));

        if (!hasSolidNeightbour(blocks, (int)Position.x - 1, (int)Position.y, (int)Position.z))
            combineList.Add(createQuad(chunkPosition, Vector3.left, tileIndex));

        if (!hasSolidNeightbour(blocks, (int)Position.x + 1, (int)Position.y, (int)Position.z))
            combineList.Add(createQuad(chunkPosition, Vector3.right, tileIndex));

        if (!hasSolidNeightbour(blocks, (int)Position.x, (int)Position.y - 1, (int)Position.z))
            combineList.Add(createQuad(chunkPosition, Vector3.down, tileIndex));

        if (!hasSolidNeightbour(blocks, (int)Position.x, (int)Position.y + 1, (int)Position.z))
            combineList.Add(createQuad(chunkPosition, Vector3.up, tileIndex));

        if (combineList.Count <= 0)
            return null;

        Mesh cubeMesh = new Mesh();
        cubeMesh.CombineMeshes(combineList.ToArray());

        return cubeMesh;
    }

    /// <summary>
    /// HACKY, dont use exceptions to regulate flow.... :P bad Mark
    /// </summary>
    private bool hasSolidNeightbour(Block[,,] blocks, int x, int y, int z)
    {
        try
        {
            return blocks[x, y, z].IsSolid;
        }
        catch (Exception ex)
        {
        }

        return false;
    }

    private CombineInstance createQuad(Vector3 chunkPosition, Vector3 planeNormal, int tileIndex)
    {
        CombineInstance combine = new CombineInstance();
        combine.transform = Matrix4x4.Translate(chunkPosition) * Matrix4x4.Translate(Position);
        combine.mesh = new Mesh
        {
            vertices = createPositions(planeNormal),
            uv = createUVArray(tileIndex),

            normals = new[]
            {
                planeNormal, planeNormal, planeNormal, planeNormal
            },

            // faces are clockwise
            triangles = new[]
            {
                0, 1, 2,
                2, 3, 0
            }
        };

        return combine;
    }

    private Vector3[] createPositions(Vector3 planeNormal)
    {
        float dot = Vector3.Dot(Vector3.up, planeNormal);
        bool isNotParallel = Mathf.Abs(dot) < 1.0f - Mathf.Epsilon;

        Vector3 helperAxis = isNotParallel ? Vector3.up : Vector3.right;
        Vector3 xAxis = Vector3.Cross(planeNormal, helperAxis);
        Vector3 yAxis = Vector3.Cross(xAxis, planeNormal);

        return new[]
        {
            -xAxis + -yAxis + planeNormal,
            -xAxis + yAxis + planeNormal,
            xAxis + yAxis + planeNormal,
            xAxis + -yAxis + planeNormal
        };
    }

    /// <summary>
    /// Unity uses anti-clockwise uv coordinate winding
    /// yeah i know hardcoded, cant be arsed.
    /// </summary>
    private Vector2[] createUVArray(int tileIndex)
    {
        // n & 0xF == n % 16
        float uvX = (tileIndex & 0xF) * tilesizeUV;

        // n >> 4 == n / 16
        // 4 bits have 16 possibilities [0, 15]
        float uvY = (tileIndex >> 4) * tilesizeUV;

        return new[]
        {
            new Vector2(uvX, uvY),
            new Vector2(uvX, uvY + tilesizeUV),
            new Vector2(uvX + tilesizeUV, uvY + tilesizeUV),
            new Vector2(uvX + tilesizeUV, uvY)
        };
    }
}
