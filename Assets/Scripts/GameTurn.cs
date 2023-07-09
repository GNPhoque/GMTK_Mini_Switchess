using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class GameTurn
{
	public static Turn turn;
	public static Turn playerTurn;
	public static Turn aiTurn;
	public static bool isGameActive;
	public static bool isSwitched;
	public static bool isDragging;
	public static Piece draggingPiece;
	public static Cell dragStartCell;
	public static Cell hoveringCell;
	public static List<Cell> moveCells = new List<Cell>();

	public static Action<Turn> OnTurnChanged;

	public static void ShowPossibleMoves(Cell from)
	{
		if (!from.piece) return;

		HideMoveCells();
		moveCells.Clear();

		//Non Pawn pieces
		if (from.piece.data.attacksSameAsMovement)
		{
			foreach (var direction in from.piece.data.moveDirections)
			{
				foreach (var move in direction.positions)
				{
					Cell destination = Board.instance.GetCell(from.position + move);
					if (destination == null) break; //Target cell out of the board

					Piece posPiece = destination.piece;

					//No piece at target cell : can move there
					if (posPiece == null)
					{
						destination.GlowGreen();
						moveCells.Add(destination);
					}

					else
					{
						//Ally piece : stop direction
						if (posPiece.data.color == from.piece.data.color)
							break;

						//Enemy piece : can capture & stop direction
						destination.GlowRed();
						moveCells.Add(destination);
						break;
					}
				}
			}
		}

		//Pawn
		else
		{
			//Moves
			foreach (var direction in from.piece.data.moveDirections)
			{
				foreach (var move in direction.positions)
				{
					Cell destination = Board.instance.GetCell(from.position + move);
					if (destination == null) break; //Target cell out of the board

					Piece posPiece = destination.piece;
					//No piece at target cell : can move there
					if (posPiece == null)
					{
						destination.GlowGreen();
						moveCells.Add(destination);
					}
				}
			}

			//Attacks
			foreach (var direction in from.piece.data.attackDirections)
			{
				foreach (var move in direction.positions)
				{
					Cell destination = Board.instance.GetCell(from.position + move);
					if (destination == null) break;

					Piece posPiece = destination.piece;
					if (posPiece != null && posPiece.data.color != from.piece.data.color)
					{
						destination.GlowRed();
						moveCells.Add(destination);
					}
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
		if (!isGameActive) return;

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
