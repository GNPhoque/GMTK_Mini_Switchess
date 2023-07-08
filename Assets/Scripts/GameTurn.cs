using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTurn
{
	public static Turn turn = Turn.White;
	public static bool isSwitched;
	public static bool isDragging;
	public static Piece draggingPiece;
	public static Cell dragStartCell;
	public static Cell hoveringCell;
	public static List<Cell> moveCells = new List<Cell>();

	public static event Action<Turn> OnTurnChanged;

	public static void ShowMoveCells(List<CellPosition> positions)
	{
		HideMoveCells();
		moveCells.Clear();
		foreach (var p in positions)
		{
			Cell cell = Board.instance.GetCell(p);
			if(cell == null) continue;

			cell.Glow();
			moveCells.Add(cell);
		}
	}

	public static void HideMoveCells()
	{
		foreach (var cell in moveCells)
		{
			cell.Unglow();
		}
	}

	public static void EndTurn()
	{
		switch (turn)
		{
			case Turn.White:
				turn = Turn.Black;
				break;
			case Turn.Black:
				turn = Turn.White;
				break;
		}
		OnTurnChanged?.Invoke(turn);
	}
}

public enum Turn
{
	White,
	Black
}
