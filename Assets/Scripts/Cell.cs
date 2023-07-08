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

	public void GlowGreen()
	{
		image.color = Color.green;
	}

	public void GlowRed()
	{
		image.color = Color.red;
	}

	public void Unglow()
	{
		image.color = cellColor;
	}

	private void ShowPossibleMoves()
	{
		GameTurn.ShowPossibleMoves(this);
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
			piece.startPosition = piece.rt.anchoredPosition;
			piece.rt.anchorMin = new Vector2(.5f, .5f);
			piece.rt.anchorMax = new Vector2(.5f, .5f);
			GameTurn.dragStartCell = this;
			GameTurn.draggingPiece = piece;
			GameTurn.isDragging = true;
		}
		Vector2 endPos = Input.mousePosition + Board.instance.mouseOffset;
		GameTurn.draggingPiece.rt.anchoredPosition = endPos;
		//move to cursor position
	}

	public void PointerDrop()
	{
		if (!GameTurn.isDragging) return;

		print($"drop ({position.x},{position.y})");
		//if possible move : move to cell (and attack)
		if (GameTurn.moveCells.Contains(this))
		{
			if (piece) Destroy(piece.gameObject);
			piece = GameTurn.draggingPiece;
			piece.rt.anchorMin = new Vector2(0f, 0f);
			piece.rt.anchorMax = new Vector2(0f, 0f);
			GameTurn.dragStartCell.piece = null;
			GameTurn.draggingPiece.rt.anchoredPosition = rt.anchoredPosition;
			ShowPossibleMoves();
		}
		//else return to start position
		else
		{
			GameTurn.draggingPiece.rt.anchoredPosition = GameTurn.draggingPiece.startPosition;
			GameTurn.draggingPiece.rt.anchorMin = new Vector2(0f, 0f);
			GameTurn.draggingPiece.rt.anchorMax = new Vector2(0f, 0f);
			GameTurn.HideMoveCells();
		}

		GameTurn.isDragging = false;
	}
	#endregion
}
