namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using Andoco.Core;

    /// <summary>
    /// Rectangular grid of cells.
    /// </summary>
    [System.Serializable]
    public class GridSpace
    {
        public IntVector2 gridSize;
        public Vector2 cellSize;
        public Vector3 worldOffset;

		public GridSpace()
		{
		}

		public GridSpace(IntVector2 gridSize, Vector2 cellSize)
		{
            this.gridSize = gridSize;
            this.cellSize = cellSize;
		}

        public int NumCells { get { return gridSize.x * gridSize.y; } }

        public int NumColumns { get { return gridSize.x; } }

        public int NumRows { get { return gridSize.y; } }

        public IntVector2 GetCell(int index)
        {
            var column = index % gridSize.x;
            var row = (index / gridSize.x);
            var cell = new IntVector2(column, row);

            return cell;
        }

        public int GetIndex(IntVector2 cell)
        {
            return cell.y * gridSize.x + cell.x;
        }

		public IntVector2 GetCenterCell()
		{
			return new IntVector2(this.gridSize.x / 2, this.gridSize.y / 2);
		}

        /// <summary>
        /// Gets the cell rectangle in world coordinates, aligned horizontally with the X and Z axes.
        /// </summary>
        public Rect GetRect(int row, int column)
        {
            var x = worldOffset.x + column * cellSize.x;
            var y = worldOffset.z + row * cellSize.y;
            var rect = new Rect(x, y, cellSize.x, cellSize.y);

            return rect;
        }

        /// <summary>
        /// Gets the cell rectangle in world coordinates, aligned horizontally with the X and Z axes.
        /// </summary>
        public Rect GetRect(int index)
        {
            var column = index % gridSize.x;
            var row = (index / gridSize.x);
            var rect = GetRect(row, column);

            return rect;
        }

        /// <summary>
        /// Gets the cell bounds in world coordinates, aligned horizontally with the X and Z axes.
        /// </summary>
        public Bounds GetBounds(int row, int column)
        {
            var rect = GetRect(row, column);
            var worldPos = new Vector3(rect.center.x, worldOffset.y, rect.center.y);
            var size = new Vector3(cellSize.x, 0f, cellSize.y);
            var bounds = new Bounds(worldPos, size);

            return bounds;
        }

        /// <summary>
        /// Gets the cell bounds in world coordinates, aligned horizontally with the X and Z axes.
        /// </summary>
        public Bounds GetBounds(int index)
        {
            var cell = GetCell(index);

            return GetBounds(cell.y, cell.x);
        }

        public Bounds GetBounds()
        {
            var center = worldOffset + new Vector3((gridSize.x * cellSize.x) / 2f, 0f, (gridSize.y * cellSize.y) / 2f);
            var size = new Vector3(gridSize.x * cellSize.x, 0f, gridSize.y * cellSize.y);

            return new Bounds(center, size);
        }

        public bool TryGetNeighbour(int index, CompassDirection.Principal direction, out int neighbourIndex)
        {
            var cell = GetCell(index);
            IntVector2 ncell;

            switch (direction)
            {
                case CompassDirection.Principal.N:
                    ncell = new IntVector2(cell.x, cell.y + 1);
                break;
                case CompassDirection.Principal.NE:
                    ncell = new IntVector2(cell.x + 1, cell.y + 1);
                break;
                case CompassDirection.Principal.E:
                    ncell = new IntVector2(cell.x + 1, cell.y);
                break;
                case CompassDirection.Principal.SE:
                    ncell = new IntVector2(cell.x + 1, cell.y - 1);
                break;
                case CompassDirection.Principal.S:
                    ncell = new IntVector2(cell.x, cell.y - 1);
                break;
                case CompassDirection.Principal.SW:
                    ncell = new IntVector2(cell.x - 1, cell.y - 1);
                break;
                case CompassDirection.Principal.W:
                    ncell = new IntVector2(cell.x - 1, cell.y);
                break;
                case CompassDirection.Principal.NW:
                    ncell = new IntVector2(cell.x - 1, cell.y + 1);
                break;
                default:
                    throw new UnityException(string.Format("Unsupported direction {0}", direction));
            }

            if (InBounds(ncell))
            {
                neighbourIndex = GetIndex(ncell);
                return true;
            }

            neighbourIndex = default(int);

            return false;
        }

        public bool InBounds(IntVector2 cell)
        {
            return cell.x >= 0 && cell.x < gridSize.x && cell.y >= 0 && cell.y < gridSize.y;
        }
    }
}