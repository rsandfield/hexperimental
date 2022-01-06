using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators {

	public struct CubeSphere : IMeshGenerator {

		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f, 0f, 1f));

		public int VertexCount => 4 * Resolution * Resolution;

		public int IndexCount => 6 * Resolution * Resolution;

		public int JobLength => Resolution;

		public int Resolution { get; set; }

		public void Execute<S> (int z, S streams) where S : struct, IMeshStreams {
			int vi = 4 * Resolution * z, ti = 2 * Resolution * z;

			for (int x = 0; x < Resolution; x++, vi += 4, ti += 2) {
				var xCoordinates = 2f * float2(x, x + 1f) / Resolution - 1f;
				var zCoordinates = 2f * float2(z, z + 1f) / Resolution - 1f;

				var vertex = new Vertex();
				vertex.normal.y = 1f;
				vertex.tangent.xw = float2(1f, -1f);

                float y = 1f;

				vertex.position = Spherize(new Vector3(xCoordinates.x, y, zCoordinates.x));
				streams.SetVertex(vi + 0, vertex);

				vertex.position = Spherize(new Vector3(xCoordinates.y, y, zCoordinates.x));
				vertex.texCoord0 = float2(1f, 0f);
				streams.SetVertex(vi + 1, vertex);
				
				vertex.position = Spherize(new Vector3(xCoordinates.x, y, zCoordinates.y));
				vertex.texCoord0 = float2(0f, 1f);
				streams.SetVertex(vi + 2, vertex);

				vertex.position = Spherize(new Vector3(xCoordinates.y, y, zCoordinates.y));
				vertex.texCoord0 = 1f;
				streams.SetVertex(vi + 3, vertex);

				streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
				streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
			}
		}

        private Vector3 Spherize(Vector3 vector) {
            // Square the vector components
            float x2 = vector.x * vector.x;
            float y2 = vector.y * vector.y;
            float z2 = vector.z * vector.z;
            
            // Spherize the original vector
            return new Vector3(
                vector.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f),
                vector.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f),
                vector.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f)
            );
        }
	}
}