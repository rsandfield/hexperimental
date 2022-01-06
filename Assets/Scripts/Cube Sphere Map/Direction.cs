using UnityEngine;

namespace CubeSphere {
    public enum Direction {
        Up, Right, Down, Left
    }

    public static class DirectionMethods {
        public static Direction RotateClockwise(this Direction direction) {
            return direction.Rotate(1);
        }

        public static Direction RotateCounterclockwise(this Direction direction) {
            return direction.Rotate(3);
        }

        public static Direction Rotate(this Direction direction, int nineties) {
            return (Direction) (((int) direction + nineties) % 4);
        }

        public static Direction Random() {
            return (Direction) UnityEngine.Random.Range(0, 4);
        }
    }
}