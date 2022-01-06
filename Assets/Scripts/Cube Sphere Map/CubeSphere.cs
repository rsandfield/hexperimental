using System.Collections.Generic;
using UnityEngine;

namespace CubeSphere {
	public class CubeSphere : MonoBehaviour {

		[SerializeField, Range(1, 16)]
		public int resolution = 1;

		public Face facePrefab;

		public Face[] faces { get; private set; }
		Edge[] edges;
		public List<Cell> cells { get; private set; }

		void Awake () {
			GenerateFacesAndCells();
			GenerateEdges();
			TestEdges();
		}

		private void GenerateFacesAndCells() {
			faces = new Face[6];
			
			for(int i = 0; i < 4; i++) {
				faces[i] = Instantiate<Face>(facePrefab, this.transform);
				faces[i].GenerateFace(i, resolution, Vector3.forward, 90 * i);
			}
			
			faces[4] = Instantiate<Face>(facePrefab, this.transform);
			faces[4].GenerateFace(4, resolution, Vector3.left, 90);

			faces[5] = Instantiate<Face>(facePrefab, this.transform);
			faces[5].GenerateFace(5, resolution, Vector3.left, 270);

			cells = new List<Cell>();
			for(int i = 0; i < 6; i++) {
				cells.AddRange(faces[i].cells);
			}
		}

		private void GenerateEdges() {
			edges = new Edge[12];
			for(int i = 0; i < 4; i++) {
				edges[i] = new Edge(faces[(i + 1) % 4], faces[i], Direction.Up, Direction.Up);	// Equator
				edges[i + 4] = new Edge(faces[i], faces[4], (Direction) ((i + 2) % 4), Direction.Up);		// South
				edges[i + 8] = new Edge(faces[5], faces[i], Direction.Up, Direction.Up);//(Direction) i);		// North
			}

			for(int i = 0; i < 4; i++) {
				faces[i].AssignEdges(edges[i + 8], edges[(i + 3) % 4], edges[i + 4], edges[i]);
			}
			faces[4].AssignEdges(edges[4], edges[5], edges[6], edges[7]);
			faces[5].AssignEdges(edges[8], edges[9], edges[10], edges[11]);
		}

		private void TestEdges() {
			Color[] colors = new Color[] { Color.blue, Color.red, Color.green, Color.cyan, Color.black };
			// for(int i = 0; i < 4; i++) {
				int i = 4;
				for(int p = -1; p <= resolution; p++) {
					Cell cell = cells[faces[i].GetCellWorldIndex(new Vector2Int(0, p))];
					cell.face.SetCellColor(cell.FaceCoordinates(), colors[i]);
					Debug.Log(cell);

					cell = cells[faces[i].GetCellWorldIndex(new Vector2Int(p, 1))];
					cell.face.SetCellColor(cell.FaceCoordinates(), colors[i]);
					Debug.Log(cell);
				}
			// }
		}

		public Cell GetCell(Face face, Vector2Int coords) {
			return cells[face.GetCellWorldIndex(coords)];
		}

		public void SetCellColor(int faceIndex, Vector2Int coords, Color color) {
			SetCellColor(faces[faceIndex].GetCell(coords).WorldIndex(), color);
		}

		public void SetCellColor(int worldIndex, Color color) {
			Cell cell = cells[worldIndex];
			cell.face.SetCellColor(cell.index, color);
		}
	}
}