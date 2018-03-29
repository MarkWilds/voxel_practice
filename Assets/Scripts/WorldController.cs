using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public Material cubeMaterial;
    public int width = 10;
    public int height = 2;
    public int depth = 10;

    public int tilesetWidth = 16;
    public float tilesetDimension = 512.0f;
    public int tilesetIndex = 0;

    public IEnumerator BuildWorld()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float size = 0.5f;
                    Vector3 pos = new Vector3(x * size, y * size, z * size);
                    GameObject cube = new GameObject();
                    cube.transform.SetPositionAndRotation(pos, Quaternion.identity);
                    cube.transform.parent = transform;
                    cube.name = x + "_" + y + "_" + z;

                    createCube(cube, cubeMaterial, cube.name);
                }
                yield return null;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(BuildWorld());
    }

    private void createCube(GameObject go, Material material, string name)
    {
        Mesh cubeMesh = new Mesh();
        CombineInstance[] quads = new CombineInstance[6];
        quads[0] = createQuad(go.transform, Vector3.back);
        quads[1] = createQuad(go.transform, Vector3.forward);
        quads[2] = createQuad(go.transform, Vector3.left);
        quads[3] = createQuad(go.transform, Vector3.right);
        quads[4] = createQuad(go.transform, Vector3.down);
        quads[5] = createQuad(go.transform, Vector3.up);

        cubeMesh.CombineMeshes(quads);
        cubeMesh.RecalculateBounds();
        cubeMesh.name = name + "_mesh";

        go.AddComponent<MeshRenderer>().material = material;
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = cubeMesh;
    }

    private Vector2[] createUVArray(int tileIndex)
    {
        float tilesizeUV = tilesetWidth / tilesetDimension;
        float uvX = (tilesetIndex % 16) * tilesizeUV;
        float uvY = tilesetIndex / 16 * tilesizeUV;

        return new[]
        {
            new Vector2(uvX, uvY),
            new Vector2(uvX, uvY + tilesizeUV),
            new Vector2(uvX + tilesizeUV, uvY + tilesizeUV),
            new Vector2(uvX + tilesizeUV, uvY)
        };
    }

    private Vector3[] createPositions(Vector3 planeNormal)
    {
        float size = 0.5f;
        float dot = Vector3.Dot(Vector3.up, planeNormal);
        bool isNotParallel = Mathf.Abs(dot) < 1.0f - Mathf.Epsilon;

        Vector3 helperAxis = isNotParallel ? Vector3.up : Vector3.right;
        Vector3 xAxis = Vector3.Cross(planeNormal, helperAxis);
        Vector3 yAxis = Vector3.Cross(xAxis, planeNormal);

        xAxis *= size;
        yAxis *= size;
        planeNormal *= size;

        return new[]
        {
            -xAxis + -yAxis + planeNormal,
            -xAxis + yAxis + planeNormal,
            xAxis + yAxis + planeNormal,
            xAxis + -yAxis + planeNormal
        };
    }

    private CombineInstance createQuad(Transform transform, Vector3 planeNormal)
    {
        CombineInstance combine = new CombineInstance();
        combine.transform = transform.localToWorldMatrix;
        combine.mesh = new Mesh
        {
            vertices = createPositions(planeNormal),
            uv = createUVArray(tilesetIndex),

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
}