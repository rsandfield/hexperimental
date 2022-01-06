using System.Collections.Generic;
using ProceduralMeshes;
using ProceduralMeshes.Streams;
using UnityEngine;

namespace CubeSphere {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Face : MonoBehaviour {
		public int index { get; private set; }
		Mesh mesh;
		List<Color> colors;
		public Cell[] cells { get; private set; }
		Edge[] edges;
		public int sideLength {get; private set;}
		bool redrawQueued = false;

		public void Update() {
			if(redrawQueued) RedrawColors();
		}

		public void GenerateFace (int index, int resolution, Vector3 axis, float angle) {
			this.index = index;
			name = "Face " + index;
			transform.Rotate(axis, angle);

			Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
			Mesh.MeshData meshData = meshDataArray[0];
			mesh = GetComponent<MeshFilter>().mesh;

			MeshJob<ProceduralMeshes.Generators.CubeSphere, SingleStream>.ScheduleParallel(
				mesh, meshData, resolution, default
			).Complete();
			Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

			sideLength = resolution;
			cells = new Cell[resolution * resolution];
			colors = new List<Color>();
			for (int ci = 0; ci < cells.Length; ci++) {
				// Find the center of the grid cell in world space
				Vector3 position = Vector3.zero;
				for(int vi = 0; vi < 4; vi++) {
					position += mesh.vertices[ci * 4 + vi];
				}
				position /= 4;
				
				cells[ci] = new Cell(this, ci, Color.white, position.normalized);

				for(int vi = 0; vi < 4; vi++) {
					colors.Add(Color.white);
				}
			}

			redrawQueued = true;
		}

		public void AssignEdges(Edge north, Edge east, Edge south, Edge west) {
			edges = new Edge[4];
			edges[0] = north;
			edges[1] = east;
			edges[2] = south;
			edges[3] = west;
		}

		public void QueueRedraw () {
			redrawQueued = true;
		}

		private void RedrawColors () {
			redrawQueued = false;
			for(int ci = 0; ci < cells.Length; ci++)
			{
				for(int vi = 0; vi < 4; vi++) {
					colors[ci * 4 + vi] = cells[ci].color;
				}
			}
			mesh.SetColors(colors);
		}

		public int GetCellWorldIndex(Vector2Int coords) {
			if(coords.x < 0) {
				coords.x += sideLength;
				return edges[(int) Direction.Left].GetNeighbor(EdgeCrossDirection.Right).GetCellWorldIndex(coords);
			}
			if(coords.x >= sideLength) {
				coords.x -= sideLength;
				return edges[(int) Direction.Right].GetNeighbor(EdgeCrossDirection.Left).GetCellWorldIndex(coords);
			}
			if(coords.y < 0) {
				coords.y += sideLength;
				return edges[(int) Direction.Down].GetNeighbor(EdgeCrossDirection.Left).GetCellWorldIndex(coords);
			}
			if(coords.y >= sideLength) {
				coords.y -= sideLength;
				return edges[(int) Direction.Up].GetNeighbor(EdgeCrossDirection.Right).GetCellWorldIndex(coords);
			}
			
			return index * sideLength * sideLength + CoordsToIndex(coords);
		}

		public Cell GetCell(Vector2Int coords) {
			ValidateCoords(coords);
			return cells[CoordsToIndex(coords)];
		}

		public void SetCellColor(Vector2Int coords, Color color) {
			ValidateCoords(coords);
			SetCellColor(CoordsToIndex(coords), color);
		}

		public void SetCellColor(int index, Color color) {
			if(index < 0 || index >= sideLength * sideLength) {
				throw new System.Exception("Cell index " + index + " out of range.");
			}
			cells[index].color = color;
			QueueRedraw();
		}

		private void ValidateCoords(Vector2Int coords) {
			if(coords.x < 0 || coords.x >= sideLength || coords.y < 0 || coords.y >= sideLength) {
				throw new System.Exception("Cell coordinates " + coords + " out of range.");
			}
		}

		private int CoordsToIndex(Vector2Int coords) {
			return coords.y * sideLength + coords.x;
		}

		public int IndexOffset() {
			return index * sideLength * sideLength;
		}
	}
}