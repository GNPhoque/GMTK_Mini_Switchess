using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] Board board;
	[SerializeField] PieceData[] pieceDatas;
	[SerializeField] Piece piecePrefab;

	private void Start()
	{
		board.CreateBoard();

		Piece p = Instantiate(piecePrefab, board.transform);
		p.Setup(pieceDatas[0]);
		board.GetCell(new CellPosition(0, 4)).SetPiece(p);

		p = Instantiate(piecePrefab, board.transform);
		p.Setup(pieceDatas[1]);
		board.GetCell(new CellPosition(0, 0)).SetPiece(p);

		p = Instantiate(piecePrefab, board.transform);
		p.Setup(pieceDatas[2]);
		board.GetCell(new CellPosition(3, 2)).SetPiece(p);
	}
}
