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
			edges = new Edge[24];

			for(int i = 0; i < 4; i++) {
				int flip = (i % 2) * 2;
				faces[i].AssignEdges(
					new Edge(faces[5], (Direction) ((i + flip) % 4)),
					new Edge(faces[(i + 3) % 4], Direction.Up),
					new Edge(faces[4], (Direction) ((4 - i + flip) % 4)),
					new Edge(faces[(i + 1) % 4], Direction.Up)
				);
			}

			faces[4].AssignEdges(
				new Edge(faces[0], Direction.Up),
				new Edge(faces[3], Direction.Right),
				new Edge(faces[2], Direction.Down),
				new Edge(faces[1], Direction.Left)
			);
			
			faces[5].AssignEdges(
				new Edge(faces[2], Direction.Down),
				new Edge(faces[3], Direction.Left),
				new Edge(faces[0], Direction.Up),
				new Edge(faces[1], Direction.Right)
			);
		}

		private void ExplodeSouth() {
			foreach(Face face in faces) {
				face.transform.localRotation = Quaternion.Euler(Vector3.zero);
				face.SetCellColor(new Vector2Int(0, 0), Color.blue);
			}
			faces[0].transform.position = Vector3.up * 1.5f;
			faces[1].transform.position = Vector3.right * 1.5f;
			faces[2].transform.position = Vector3.down * 1.5f;
			faces[3].transform.position = Vector3.left * 1.5f;
			faces[5].transform.position = Vector3.left * 3f;

			faces[1].transform.Rotate(0, 90, 0);
			faces[2].transform.Rotate(0, 180, 0);
			faces[3].transform.Rotate(0, 270, 0);
		}

		private void TestEdges(int i, Color color) {
			Face face = faces[i];
			face.AnnounceNeighbors();

			for(int p = -1; p <= resolution; p++) {
				Cell cell = cells[face.GetCellWorldIndex(new Vector2Int(0, p))];
				cell.face.SetCellColor(cell.FaceCoordinates(), color);

				cell = cells[face.GetCellWorldIndex(new Vector2Int(p, 1))];
				cell.face.SetCellColor(cell.FaceCoordinates(), color);
			}
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