using System.Collections;
using System.Collections.Generic;
using CubeSphere;
using UnityEngine;

namespace LangtonAnt {
    public class Ant : MonoBehaviour
    {
        GameController game;
        Cell cell;
        Direction heading;

        public void Initialize(Cell cell, GameController gameController) {
            game = gameController;
            StepOn(cell);
            SetScale(1f / (float) game.world.resolution);
            name = "Ant";
            MeshRenderer filter = transform.GetChild(0).GetComponent<MeshRenderer>();
            filter.material.color = Color.gray;
            heading = DirectionMethods.Random();
        }

        public void SetScale(float size) {
            Transform body = transform.GetChild(0).transform;
            body.localScale = body.localScale * size;
        }

        public void SetPosition(Face face, Vector2Int coords) {
            SetPosition(face.GetCell(coords));
        }

        public void SetPosition(Cell cell) {
            this.cell = cell;
            transform.parent = cell.face.transform;
            transform.localPosition = cell.position;
            transform.LookAt(Vector3.zero);
        }

        public void StepOn(Face face, Vector2Int coords) {
            StepOn(game.world.GetCell(face, coords));
        }

        public void StepOn(Cell cell) {
            SetPosition(cell);
            
            int index = cell.WorldIndex();
            
            switch(game.actions[index]) {
                case Action.Right:
                    heading = heading.RotateClockwise();
                    game.world.SetCellColor(cell.face.index, cell.FaceCoordinates(), Color.white);
                    break;
                case Action.Left:
                    heading = heading.RotateCounterclockwise();
                    game.world.SetCellColor(cell.face.index, cell.FaceCoordinates(), Color.black);
                    break;
                default:
                    break;
            }

            game.CycleCell(index);
        }

        public void StepForward() {
            Vector2Int coords = cell.FaceCoordinates();

            switch(heading) {
                case Direction.Up:
                    coords += new Vector2Int(0, 1);
                    break;
                case Direction.Right:
                    coords += new Vector2Int(1, 0);
                    break;
                case Direction.Down:
                    coords += new Vector2Int(0, -1);
                    break;
                case Direction.Left:
                    coords += new Vector2Int(-1, 0);
                    break;
            }

            StepOn(cell.face, coords);
        }

        public override string ToString()
        {
            return name + " " + heading + " [" + cell.face.index + "] " + cell.FaceCoordinates();
        }
    }
}