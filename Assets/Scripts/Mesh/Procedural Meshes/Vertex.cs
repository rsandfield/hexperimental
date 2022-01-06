using Unity.Mathematics;

namespace ProceduralMeshes {

    public struct Vertex {
        public float3 position, normal;
        public float4 tangent;
        public float2 texCoord0;

        public override System.String ToString() {
            return position.ToString();
        }
    }
}