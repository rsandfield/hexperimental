using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeGrid : MonoBehaviour {

	public int size;
    public float scale;

	private Mesh mesh;
	private Vector3[] vertices;
	private Vector3[] normals;
    private Color32[] cubeUV;

	private void Awake () {
		Generate();
	}

	private void Generate () {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Sphere";
        CreateVertices();
        CreateTriangles();
	}

    private void CreateVertices () {
        int vertexCount = size < 2 ?
            8 :
            2 * (size + 1) * (size + 1) + 4 * (size - 1) * size;
        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertexCount];
        cubeUV = new Color32[vertexCount];

        int v = 0;
        // Fill the barrel
        for (int y = 0; y <= size; y++) {
            float height = y;

            for (int x = 0; x <= size; x++) {
                SetVertex(v++, x, height, 0);
            }
            for (int z = 1; z <= size; z++) {
                SetVertex(v++, size, height, z);
            }
            for (int x = size - 1; x >= 0; x--) {
                SetVertex(v++, x, height, size);
            }
            for (int z = size - 1; z > 0; z--) {
                SetVertex(v++, 0, height, z);
            }
        }
        // Top
        for (int z = 1; z < size; z++) {
            for (int x = 1; x < size; x++) {
                SetVertex(v++, x, size, z);
            }
        }
        // Bottom
        for (int z = 1; z < size; z++) {
            for (int x = 1; x < size; x++) {
                SetVertex(v++, x, 0, z);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    private void SetVertex (int i, float x, float y, float z) {
        Vector3 vector = new Vector3(x, y, z) * 2f / size - Vector3.one;

        // Square the vector components
		float x2 = vector.x * vector.x;
		float y2 = vector.y * vector.y;
		float z2 = vector.z * vector.z;
		
        // Spherize the original vector
		normals[i] = new Vector3(
		    vector.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f),
		    vector.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f),
		    vector.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f)
        );
		vertices[i] = normals[i] * scale;
		cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
	}

    private void CreateTriangles () {
        int faceSize = size * size * 12;
        int[] trianglesX = new int[faceSize];
        int[] trianglesY = new int[faceSize];
        int[] trianglesZ = new int[faceSize];

        int ring = size * 4;
        int tX = 0, tY = 0, tZ = 0;
        int v = 0;

        for (int y = 0; y < size; y++, v++) {
			for (int q = 0; q < size; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < size; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < size; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < size - 1; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
		}

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        mesh.subMeshCount = 3;
		mesh.SetTriangles(trianglesZ, 0);
		mesh.SetTriangles(trianglesX, 1);
		mesh.SetTriangles(trianglesY, 2);
    }

    private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * size;

        // Bottom
		for (int x = 0; x < size - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);
        
        int vMin = ring * (size + 1) - 1;
		int vMid = vMin + 1;
		int vMax = v + 2;

        // Middle
        for (int z = 1; z < size - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + size - 1);
			for (int x = 1; x < size - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid, vMid + 1, vMid + size - 1, vMid + size);
			}
			t = SetQuad(triangles, t, vMid, vMax, vMid + size - 1, vMax + 1);
		}

        // Top
        int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
		for (int x = 1; x < size - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
		t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;

        // Bottom
		int vMid = vertices.Length - (size - 1) * (size - 1);
		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
		for (int x = 1; x < size - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= size - 2;
		int vMax = v + 2;

        // Middle
		for (int z = 1; z < size - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + size - 1, vMin + 1, vMid);
			for (int x = 1; x < size - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + size - 1, vMid + size, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + size - 1, vMax + 1, vMid, vMax);
		}

        // Top
		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < size - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
		
		return t;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11) {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

	private void OnDrawGizmos () {
		if (vertices == null) {
			return;
		}
		for (int i = 0; i < vertices.Length; i++) {
		    Gizmos.color = Color.blue;
			Gizmos.DrawSphere(vertices[i], 0.1f);
		    Gizmos.color = Color.yellow;
            Gizmos.DrawRay(vertices[i], normals[i]);
		}
	}
}
