using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] Board board;
	[SerializeField] PieceData[] pieceDatas;
	[SerializeField] Piece piecePrefab;
	[SerializeField] PlayerAI ai;

	[SerializeField] GameObject endGamePanel;
	[SerializeField] TextMeshProUGUI winnerText;

	private void Start()
	{
		board.CreateBoard();
		SetupPieces();
		Cell.OnKingCaptured += Cell_OnKingCaptured;
	}

	private void Cell_OnKingCaptured(bool white)
	{
		//END GAME
		GameTurn.isGameActive = false;
		//SHOW WINNER
		endGamePanel.SetActive(true);
		winnerText.text = $"{(white?"WHITE" : "BLACK")} PLAYER WINS";
	}

	void SetupPieces()
	{
		//WHITES
		SetupPiece(pieceDatas[0], new CellPosition(0, 1));
		SetupPiece(pieceDatas[0], new CellPosition(1, 1));
		SetupPiece(pieceDatas[0], new CellPosition(2, 1));
		SetupPiece(pieceDatas[0], new CellPosition(3, 1));
		SetupPiece(pieceDatas[0], new CellPosition(4, 1));

		SetupPiece(pieceDatas[1], new CellPosition(0, 0));
		SetupPiece(pieceDatas[2], new CellPosition(1, 0));
		SetupPiece(pieceDatas[3], new CellPosition(2, 0));
		SetupPiece(pieceDatas[4], new CellPosition(3, 0));
		SetupPiece(pieceDatas[5], new CellPosition(4, 0));

		////BLACKS
		SetupPiece(pieceDatas[6], new CellPosition(0, 3));
		SetupPiece(pieceDatas[6], new CellPosition(1, 3));
		SetupPiece(pieceDatas[6], new CellPosition(2, 3));
		SetupPiece(pieceDatas[6], new CellPosition(3, 3));
		SetupPiece(pieceDatas[6], new CellPosition(4, 3));

		SetupPiece(pieceDatas[7], new CellPosition(0, 4));
		SetupPiece(pieceDatas[8], new CellPosition(1, 4));
		SetupPiece(pieceDatas[9], new CellPosition(2, 4));
		SetupPiece(pieceDatas[10], new CellPosition(3, 4));
		SetupPiece(pieceDatas[11], new CellPosition(4, 4));
	}

	private void SetupPiece(PieceData pieceData, CellPosition cellPosition)
	{
		Piece p = Instantiate(piecePrefab, board.transform);
		p.Setup(pieceData);
		board.GetCell(cellPosition).SetPiece(p);
	}
}
