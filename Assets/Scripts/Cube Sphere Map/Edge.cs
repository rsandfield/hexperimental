using UnityEngine;

namespace CubeSphere {
    public class Edge {
        public Face neighbor { get; private set; }
        Direction rotation;

        public Edge(Face neighbor, Direction rotation) {
            this.neighbor = neighbor;
            this.rotation = rotation;
        }

        public int GetCellWorldIndex(Vector2Int coords) {
            Vector2Int rotated = RotateCoordinates(coords);
            return neighbor.GetCellWorldIndex(rotated);
        }

        public Vector2Int RotateCoordinates(Vector2Int coords) {
            int maxIndex = neighbor.sideLength - 1;
            switch(rotation) {
                case Direction.Right:
                    return new Vector2Int(maxIndex - coords.y, coords.x);
                case Direction.Down:
                    return new Vector2Int(maxIndex - coords.x, maxIndex - coords.y);
                case Direction.Left:
                    return new Vector2Int(coords.y, maxIndex - coords.x);
                default:
                    return coords;
            }
        }
    }
}