using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	public int sizeX, sizeY;
	public Cell cellPrefab;
	public List<Cell> cells;

	private void Start()
	{
		CreateBoard();
	}

	public void CreateBoard()
	{
		foreach (Cell cell in cells)
		{
			Destroy(cell.gameObject);
		}
		cells.Clear();

		bool nextCellIsWhite = false;
		for (int y = 0; y < sizeY; y++)
		{
			for (int x = 0; x < sizeX; x++)
			{
				Cell cell = Instantiate(cellPrefab, transform);
				cell.Setup(new CellPosition(x, y), nextCellIsWhite ? Turn.White : Turn.Black);
				nextCellIsWhite = !nextCellIsWhite;
				cells.Add(cell);
			}
		}
	}

	public Cell GetCell(CellPosition position)
	{
		return cells.First(x => x.position == position);
	}
}

[Serializable]
public class CellPosition
{
	public int x;
	public int y;

	public CellPosition(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}