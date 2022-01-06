using CubeSphere;
using UnityEngine;

namespace LangtonAnt {
    public enum Action {
        Left, Right
    }

    public static class ActionMethods {
        public static Action Cycle(this Action action) {
            return (Action) (((int) action + 1) % 2);
        }

        public static Color GetColor(this Action action) {
            switch(action) {
                case Action.Left:
                    return Color.white;
                case Action.Right:
                    return Color.black;
                default:
                    return Color.gray;
            }
        }

        public static Direction Turn(this Action action, Direction direction) {
            switch(action) {
                case Action.Left:
                    return direction.RotateCounterclockwise();
                case Action.Right:
                    return direction.RotateClockwise();
                default:
                    return direction;
            }
        }
    }
}