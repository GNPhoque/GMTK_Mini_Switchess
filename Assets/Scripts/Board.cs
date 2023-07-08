using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	public int sizeX, sizeY;
	public Color colorWhite;
	public Color colorBlack;
	public Cell cellPrefab;
	public List<Cell> cells;
	public Vector3 mouseOffset;

	public static Board instance;

	private void Awake()
	{
		if (instance) Destroy(instance.gameObject);
		instance = this;
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
				cell.Setup(new CellPosition(x, y), nextCellIsWhite ? Turn.White : Turn.Black, nextCellIsWhite ? colorWhite : colorBlack);
				nextCellIsWhite = !nextCellIsWhite;
				cells.Add(cell);
			}
		}
	}

	public Cell GetCell(CellPosition position)
	{
		return cells.FirstOrDefault(x => x.position == position);
	}

	public void OnBackgroundDrop()
	{
		print("background drop");
		GameTurn.draggingPiece.rt.anchoredPosition = GameTurn.draggingPiece.startPosition;
		GameTurn.draggingPiece.rt.anchorMin = new Vector2(0f, 0f);
		GameTurn.draggingPiece.rt.anchorMax = new Vector2(0f, 0f);
		GameTurn.HideMoveCells();
		GameTurn.isDragging = false;
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

	public static CellPosition operator + (CellPosition a, CellPosition b)
	{
		return new CellPosition(a.x + b.x, a.y + b.y);
	}

	public static CellPosition operator - (CellPosition a, CellPosition b)
	{
		return new CellPosition(a.x - b.x, a.y - b.y);
	}

	public static bool operator == (CellPosition a, CellPosition b)
	{
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator != (CellPosition a, CellPosition b)
	{
		return a.x != b.x || a.y != b.y;
	}
}

[Serializable]
public class MoveDirection
{
	public CellPosition[] positions;
}