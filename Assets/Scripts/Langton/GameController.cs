using System.Collections;
using System.Collections.Generic;
using CubeSphere;
using UnityEngine;

namespace LangtonAnt {
    public class GameController : MonoBehaviour
    {
        public CubeSphere.CubeSphere world;
        public OrbitalCam cam;
        public Ant antPrefab;
        Ant ant;

        public Action[] actions { get; private set; }

        float nextUpdate = 0f;

        void Awake() {
            ant = Instantiate(antPrefab);
        }

        void Start() {
            actions = new Action[world.cells.Count];
            ant.Initialize(world.GetCell(world.faces[0], new Vector2Int(0, 0)), this);
        }

        void Update() {
            float time = Time.time;
            if(time >= nextUpdate) {
                nextUpdate = time + 1;
                Step();
            }
        }

        void Step() {
            ant.StepForward();
            Vector3 position = ant.transform.position;
            float x = Mathf.Asin(position.y) * Mathf.Rad2Deg;
            float y = Mathf.Asin(position.x) * Mathf.Rad2Deg;
            cam.transform.eulerAngles = new Vector3(x, -y, 0);
        }

        public void CycleCell(int worldIndex) {
            actions[worldIndex] = actions[worldIndex].Cycle();
        }
    }
}