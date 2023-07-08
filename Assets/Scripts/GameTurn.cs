using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

	public static void ShowPossibleMoves(Cell from)
	{
		if (!from.piece) return;

		HideMoveCells();
		moveCells.Clear();

		//Non Pawn pieces
		if (from.piece.data.attacksSameAsMovement)
		{
			foreach (var move in from.piece.data.movements)
			{
				Cell destination = Board.instance.GetCell(from.position + move);
				if (destination == null) continue;

				moveCells.Add(destination);
				Piece posPiece = destination.piece;
				if (posPiece == null)
					destination.GlowGreen();
				else
					destination.GlowRed();
			}
		}

		//Pawn
		else
		{
			//Moves
			foreach (var move in from.piece.data.movements)
			{
				Cell destination = Board.instance.GetCell(from.piece.data.color == Turn.White ? from.position + move : from.position - move);
				if (destination == null) continue;

				Piece posPiece = destination.piece;
				if (posPiece == null)
				{
					destination.GlowGreen();
					moveCells.Add(destination);
				}
			}

			//Attacks
			foreach (var move in from.piece.data.attacks)
			{
				Cell destination = Board.instance.GetCell(from.piece.data.color == Turn.White ? from.position + move : from.position - move);
				if (destination == null) continue;

				Piece posPiece = destination.piece;
				if (posPiece != null)
				{
					destination.GlowRed();
					moveCells.Add(destination);
				}
			}
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
