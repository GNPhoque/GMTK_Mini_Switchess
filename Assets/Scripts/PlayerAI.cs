using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAI : MonoBehaviour
{
	public Turn turn;

	private void OnEnable()
	{
		GameTurn.OnTurnChanged += GameTurn_OnTurnChanged;
	}

	private void OnDisable()
	{
		GameTurn.OnTurnChanged -= GameTurn_OnTurnChanged;
	}

	private void GameTurn_OnTurnChanged(Turn obj)
	{
		if (obj == turn) PlayBestAction();
	}

	void PlayBestAction()
	{
		Sim_Board sim_Board = new Sim_Board()
		{
			cells = Board.instance.cells.Select(x => new Sim_Cell(x)).ToArray(),
			turn = GameTurn.turn
			//turn = GameTurn.turn == Turn.White ? Turn.Black : Turn.White
		};

		sim_Board.SetNextTurnBoards();
		Sim_Board bestBoard = sim_Board.nextTurnBoards.OrderBy(x => minimax(x, 1, false)).First();

		Cell from = bestBoard.leadingAction.fromCell;
		Cell to = bestBoard.leadingAction.toCell;

		to.SetPiece(from.piece);
		from.piece = null;
		GameTurn.EndTurn();
	}

	int minimax(Sim_Board sim, int depth, bool white)
	{
		bool gameover = false;//TODO determine gameover
		if (depth == 0 || gameover)
		{
			return sim.GetValue(white);
		}

		if (white)
		{
			int max = int.MinValue;
			foreach (Sim_Board simu in sim.nextTurnBoards)
			{
				int eval = minimax(simu, depth - 1, !white);
				max = Mathf.Max(max,eval);
			}
			return max;
		}

		else
		{
			int min = int.MaxValue;
			foreach (Sim_Board simu in sim.nextTurnBoards)
			{
				int eval = minimax(simu, depth - 1, !white);
				min = Mathf.Max(min, eval);
			}
			return min;
		}
	}
}

public class Sim_Board
{
	public Sim_PieceMovement leadingAction;
	public Sim_Cell[] cells;
	public Turn turn;
	public Dictionary<Sim_Cell, List<Sim_Cell>> possibleMoves = new Dictionary<Sim_Cell, List<Sim_Cell>>();
	public List<Sim_Board> nextTurnBoards = new List<Sim_Board>();

	//public (CellPosition from, CellPosition to) GetBestAction()
	//{
	//	int 
	//} 

	public void SetNextTurnBoards()
	{
		SetPossibleMoves();
		foreach (var sourceCell in possibleMoves)
		{
			foreach (var move in sourceCell.Value)
			{
				SimulateAction(sourceCell.Key, move);
			}
		}
	}

	public int GetValue(bool white)
	{
		int ret = 0;
		foreach (var cell in cells.Where(x => x.piece != null && x.piece.data.color == (white ? Turn.White : Turn.Black)))
		{
			ret += cell.piece.data.value;
		}
		return ret;
	}

	void SetPossibleMoves()
	{
		foreach (var cell in cells)
		{
			if (cell.piece == null || !cell.piece.data.canBePlayed) continue;

			//If cell has a piece, get all available movements for this piece
			possibleMoves.Add(cell, GetPossibleMovesFromCell(cell));
		}
	}

	List<Sim_Cell> GetPossibleMovesFromCell(Sim_Cell from)
	{
		List<Sim_Cell> possibleMoves = new List<Sim_Cell>();

		//Non Pawn pieces
		if (from.piece.data.attacksSameAsMovement)
		{
			foreach (var direction in from.piece.data.moveDirections)
			{
				foreach (var move in direction.positions)
				{
					Sim_Cell destination = cells.FirstOrDefault(x => x.position == from.position + move);
					if (destination == null) break; //Target cell out of the board : change direction

					Sim_Piece posPiece = destination.piece;

					//No piece at target cell : can move there
					if (posPiece == null)
					{
						possibleMoves.Add(destination);
					}

					else
					{
						//Ally piece : stop direction
						if (posPiece.data.color == from.piece.data.color)
							break;

						//Enemy piece : can capture & stop direction
						possibleMoves.Add(destination);
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
					Sim_Cell destination = cells.FirstOrDefault(x => x.position == from.position + move);
					if (destination == null) break; //Target cell out of the board //TODO : change to Queen?

					Sim_Piece posPiece = destination.piece;
					//No piece at target cell : can move there
					if (posPiece == null)
					{
						possibleMoves.Add(destination);
					}
				}
			}

			//Attacks
			foreach (var direction in from.piece.data.attackDirections)
			{
				foreach (var move in direction.positions)
				{
					Sim_Cell destination = cells.FirstOrDefault(x => x.position == from.position + move);
					if (destination == null) break; //Target cell out of the board //TODO : change to Queen?

					Sim_Piece posPiece = destination.piece;
					//Enemy piece at target cell : can capture
					if (posPiece != null && posPiece.data.color != from.piece.data.color)
					{
						possibleMoves.Add(destination);
					}
				}
			}
		}

		return possibleMoves;
	}

	void SimulateAction(Sim_Cell from, Sim_Cell to)
	{
		Sim_Cell[] simCells = cells.Select(x => x.Copy()).ToArray();

		simCells.First(x => x.position == to.position).piece = from.piece.Copy();
		simCells.First(x => x.position == from.position).piece = null;

		Sim_Board sim = new Sim_Board()
		{
			cells = simCells,
			turn = turn == Turn.White ? Turn.Black : Turn.White,
			leadingAction = new Sim_PieceMovement(from, to)
		};

		nextTurnBoards.Add(sim);
	}
}

public class Sim_Cell
{
	public CellPosition position;
	public Sim_Piece piece;

	public Sim_Cell() { }

	public Sim_Cell(Cell cell)
	{
		position = cell.position;
		if(cell.piece != null)
			piece = new Sim_Piece(cell.piece);
	}

	public Sim_Cell Copy()
	{
		return new Sim_Cell() { piece = piece, position = position };
	}
}

public class Sim_Piece
{
	public PieceData data;

	public Sim_Piece() { }

	public Sim_Piece(Piece piece)
	{
		data = piece.data;
	}

	public Sim_Piece Copy()
	{
		return new Sim_Piece() { data = data };
	}
}

public class Sim_PieceMovement
{
	Sim_Cell from, to;
	public Cell fromCell => Board.instance.GetCell(from.position);
	public Cell toCell => Board.instance.GetCell(to.position);

	public Sim_PieceMovement(Sim_Cell _from, Sim_Cell _to)
	{
		from = _from;
		to = _to;
	}
}