using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	public RectTransform rt;
	public Image image;
	public CellPosition position;
	public Piece piece;
	public Turn color;
	public Color cellColor;
	public Vector2 startPosition;

	public void Setup(CellPosition position, Turn color, Color cellColor)
	{
		this.position = position;
		rt.anchoredPosition = new Vector2(position.x * rt.sizeDelta.x, position.y * rt.sizeDelta.y);
		this.color = color;
		this.cellColor = cellColor;
		image.color = cellColor;
	}

	public void SetPiece(Piece p)
	{
		if (piece)
		{
			Destroy(piece.gameObject);
		}
		piece = p;
		p.rt.anchoredPosition = rt.anchoredPosition;
	}

	public void Glow()
	{
		image.color = Color.red;
	}

	public void Unglow()
	{
		image.color = cellColor;
	}

	private void ShowPossibleMoves()
	{
		//show possible moves
		List<CellPosition> moves = new List<CellPosition>();
		foreach (CellPosition p in piece.data.movements)
		{
			moves.Add(piece.data.color == Turn.White ? position + p : position - p);
			GameTurn.ShowMoveCells(moves);
		}
	}

	#region EVENT TRIGGER
	public void PointerEnter()
	{
		print("enter");
		if (!piece) return;

		GameTurn.hoveringCell = this;
		//Highlight cell
		//if not dragging piece : 
		if (!GameTurn.isDragging)
		{
			ShowPossibleMoves();
		}
	}

	public void PointerExit()
	{
		print("exit");
		GameTurn.hoveringCell = null;
		//Unhighlight cell
		//if not dragging hide moves
		if (!GameTurn.isDragging)
		{
			GameTurn.HideMoveCells();
		}
	}

	public void PointerDrag()
	{
		print($"drag ({position.x},{position.y}), isDragging = {GameTurn.isDragging}");
		if (!piece) return;
		//if not dragging save starting position
		if (!GameTurn.isDragging)
		{
			//startPosition = piece?.rt.anchoredPosition;
			GameTurn.dragStartCell = this;
			GameTurn.draggingPiece = piece;
			GameTurn.isDragging = true;
		}
		else
		{
			GameTurn.draggingPiece.rt.anchoredPosition = Input.mousePosition;
		}
		//move to cursor position
	}

	public void PointerDrop()
	{
		if (!GameTurn.isDragging) return;

		print($"drop ({position.x},{position.y})");
		//if possible move : move to cell (and attack)
		if (true)
		{
			piece = GameTurn.draggingPiece;
			GameTurn.dragStartCell.piece = null;
			GameTurn.draggingPiece.rt.anchoredPosition = rt.anchoredPosition;
			ShowPossibleMoves();
			GameTurn.isDragging = false;
		}
		//else return to start position
		else
		{
			rt.anchoredPosition = startPosition;
		}
	} 
	#endregion
}
