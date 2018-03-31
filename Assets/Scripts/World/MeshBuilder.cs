using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder {

    public int TileIndex { private get; set; }

    public float TileSizeUV { private get; set; }

    public Mesh createQuad(Vector3 planeNormal)
    {
        return new Mesh
        {
            vertices = createPositions(planeNormal),
            uv = createUVArray(),

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
    private Vector2[] createUVArray()
    {
        // n & 0xF == n % 16
        float uvX = (TileIndex & 0xF) * TileSizeUV;

        // n >> 4 == n / 16
        // 4 bits have 16 possibilities [0, 15]
        float uvY = (TileIndex >> 4) * TileSizeUV;

        return new[]
        {
            new Vector2(uvX, uvY),
            new Vector2(uvX, uvY + TileSizeUV),
            new Vector2(uvX + TileSizeUV, uvY + TileSizeUV),
            new Vector2(uvX + TileSizeUV, uvY)
        };
    }
}
