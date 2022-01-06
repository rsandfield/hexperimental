using UnityEngine;

namespace CubeSphere {
    public enum EdgeCrossDirection {
        Left, Right
    }

    public class Edge {
        Face[] faces;
        Direction[] rotations;

        public Edge(Face left, Face right, Direction rotationLeft, Direction rotationRight) {
            faces = new Face[] {left, right};
            rotations = new Direction[] {rotationLeft, rotationRight};
        }

        public Face GetNeighbor(EdgeCrossDirection direction, bool invert = false) {
            if(invert) {
                return faces[(int) direction];
            } else {
                return faces[1 - (int) direction];
            }
        }

        public Vector2Int RotateCoordinates(Vector2Int coords, EdgeCrossDirection direction) {
            int maxIndex = faces[0].sideLength - 1;
            switch(rotations[(int) direction]) {
                case Direction.Right:
                    coords = new Vector2Int(maxIndex - coords.y, coords.x);
                    break;
                case Direction.Down:
                    coords = new Vector2Int(maxIndex - coords.x, maxIndex - coords.y);
                    break;
                case Direction.Left:
                    coords = new Vector2Int(coords.y, maxIndex - coords.x);
                    break;
                default:
                    break;
            }
            return coords;
        }
    }
}