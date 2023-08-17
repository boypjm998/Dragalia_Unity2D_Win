
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using package with 'AnimationUtility' class


//using static UnityEditor.Progress;

public class TriggerTest : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float duration = 1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private int triangleCount;
    private float minZ;
    private float maxZ;

    private void Start()
    {
        mesh = meshFilter.mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        triangleCount = triangles.Length / 3;

        minZ = float.MaxValue;
        maxZ = float.MinValue;
        foreach (Vector3 vertex in vertices)
        {
            if (vertex.y < minZ)
            {
                minZ = vertex.z;
            }
            if (vertex.y > maxZ)
            {
                maxZ = vertex.z;
            }
        }

        StartCoroutine(PrintModel());
    }

    private IEnumerator PrintModel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float currentY = minZ + (maxZ - minZ) * (elapsedTime / duration);
            List<int> currentTriangles = new List<int>();
            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 v1 = vertices[triangles[i * 3]];
                Vector3 v2 = vertices[triangles[i * 3 + 1]];
                Vector3 v3 = vertices[triangles[i * 3 + 2]];
                if (v1.z <= currentY || v2.z <= currentY || v3.z <= currentY)
                {
                    currentTriangles.Add(triangles[i * 3]);
                    currentTriangles.Add(triangles[i * 3 + 1]);
                    currentTriangles.Add(triangles[i * 3 + 2]);
                }
            }
            mesh.triangles = currentTriangles.ToArray();
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mesh.triangles = triangles;
    }
}
