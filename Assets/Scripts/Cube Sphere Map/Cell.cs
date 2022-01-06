using UnityEngine;

namespace CubeSphere {
    public struct Cell
    {
        public Face face;
        public int index;
        public Color color;
        public Vector3 position;

        public Cell(Face face, int index, Color color, Vector3 position) {
            this.face = face;
            this.index = index;
            this.color = color;
            this.position = position;
        }

        public Vector2Int FaceCoordinates () {
            return new Vector2Int(index % face.sideLength, index / face.sideLength);
        }

        public int WorldIndex() {
            return face.IndexOffset() + index;
        }

        override public string ToString() {
            return face.name + "[" + index + "] " + FaceCoordinates() + " " + color;
        }
    }
}