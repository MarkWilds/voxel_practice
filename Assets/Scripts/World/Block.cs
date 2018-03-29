using UnityEngine;

public class Block
{

    private float tilesizeUV;
    public Vector3 Position { get; private set; }
    public bool isSolid { get; set; }

    public Block(Vector3 pos, bool solid)
    {
        Position = pos;
        isSolid = solid;
    }

    public Mesh build(Transform transform, int tileIndex, float tileSizeNormalized)
    {
        tilesizeUV = tileSizeNormalized;
        Mesh cubeMesh = new Mesh();
        CombineInstance[] quads = new CombineInstance[6];
        quads[0] = createQuad(transform, Vector3.back, tileIndex);
        quads[1] = createQuad(transform, Vector3.forward, tileIndex);
        quads[2] = createQuad(transform, Vector3.left, tileIndex);
        quads[3] = createQuad(transform, Vector3.right, tileIndex);
        quads[4] = createQuad(transform, Vector3.down, tileIndex);
        quads[5] = createQuad(transform, Vector3.up, tileIndex);

        cubeMesh.CombineMeshes(quads);

        return cubeMesh;
    }

    private CombineInstance createQuad(Transform transform, Vector3 planeNormal, int tileIndex)
    {
        CombineInstance combine = new CombineInstance();
        combine.transform = transform.localToWorldMatrix * Matrix4x4.Translate(Position);
        combine.mesh = new Mesh
        {
            vertices = createPositions(planeNormal),
            uv = createUVArray(tileIndex),

            normals = new[]
            {
                planeNormal, planeNormal, planeNormal, planeNormal
            },

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
        float size = 0.5f;
        float dot = Vector3.Dot(Vector3.up, planeNormal);
        bool isNotParallel = Mathf.Abs(dot) < 1.0f - Mathf.Epsilon;

        Vector3 helperAxis = isNotParallel ? Vector3.up : Vector3.right;
        Vector3 xAxis = Vector3.Cross(planeNormal, helperAxis);
        Vector3 yAxis = Vector3.Cross(xAxis, planeNormal);

//        xAxis *= size;
//        yAxis *= size;
//        planeNormal *= size;

        return new[]
        {
            -xAxis + -yAxis + planeNormal,
            -xAxis + yAxis + planeNormal,
            xAxis + yAxis + planeNormal,
            xAxis + -yAxis + planeNormal
        };
    }

    private Vector2[] createUVArray(int tileIndex)
    {
        float uvX = (tileIndex % 16) * tilesizeUV;
        float uvY = tileIndex / 16 * tilesizeUV;

        return new[]
        {
            new Vector2(uvX, uvY),
            new Vector2(uvX, uvY + tilesizeUV),
            new Vector2(uvX + tilesizeUV, uvY + tilesizeUV),
            new Vector2(uvX + tilesizeUV, uvY)
        };
    }
}
