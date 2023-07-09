using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerAI : MonoBehaviour
{
	bool multithread = true;

	public Turn turn;
	int maxDepth = 4;
	Sim_Board currentTurnBoard;
	Sim_Board bestBoard;
	List<(Sim_Board board, int value)> simValues;

	event Action<Sim_Board,int> OnMinimaxEnded;
	event Action OnBestActionFound;

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
		if (obj == turn) StartCoroutine(StartSimulation());
	}

	IEnumerator StartSimulation()
	{
		//Simulation of current board state
		Debug.Log("Starting AI calculation");
		simValues = new List<(Sim_Board board, int value)>();
		List<Thread> threads = new List<Thread>();
		StaticHelper.currentSimulations = 0;
		currentTurnBoard = new Sim_Board()
		{
			cells = Board.instance.cells.Select(x => new Sim_Cell(x)).ToArray(),
			turn = GameTurn.turn
		};
		bestBoard = null;

		currentTurnBoard.SetNextTurnBoards();
		if (multithread)
		{
			OnMinimaxEnded += AddMinimaxedAction;

			foreach (var board in currentTurnBoard.nextTurnBoards)
			{
				Thread t = new Thread(() =>
				{
					ThreadedMinimax(board, maxDepth);
				});

				t.Start();
				threads.Add(t);
			}

			while (threads.Any(x => x.IsAlive)) yield return null;
			PlayBestAction();
			OnMinimaxEnded -= AddMinimaxedAction;
		}
		else
		{
			OnBestActionFound += PlayBestAction;
			Thread t = new Thread(() =>
			{
				//bestBoard = currentTurnBoard.nextTurnBoards.OrderBy(x => minimax(x, 4)).First();
				foreach (var sim in currentTurnBoard.nextTurnBoards)
				{
					int value = minimax(sim, maxDepth);
					simValues.Add((sim, value));
				}
				bestBoard = simValues.OrderBy(x => x.value).First().board;
				OnBestActionFound?.Invoke();
				OnBestActionFound -= PlayBestAction;
			});
			t.Start();
		}
	}

	void ThreadedMinimax(Sim_Board sim, int depth)
	{
		int value = minimax(sim, depth);
		OnMinimaxEnded?.Invoke(sim, value);
	}

	void AddMinimaxedAction(Sim_Board sim, int value)
	{
		simValues.Add((sim, value));
		Debug.Log("minimax ended");
		//if (simValues.Count == currentTurnBoard.nextTurnBoards.Count)
		//{
		//	PlayBestAction();
		//	OnMinimaxEnded -= AddMinimaxedAction;
		//}
	}

	void PlayBestAction()
	{
		Debug.Log($"{StaticHelper.currentSimulations} moves simulated");

		if (multithread)
		{
			bestBoard = simValues.OrderBy(x => x.value).First().board;
		}

		Cell from = bestBoard.leadingAction.fromCell;
		Cell to = bestBoard.leadingAction.toCell;

		to.SetPiece(from.piece);
		from.piece = null;
		GameTurn.EndTurn();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sim"> state of the game at the start of the turn</param>
	/// <param name="depth"> maximum turns remaining to simulate</param>
	/// <param name="white"> get best result for black or white</param>
	/// <returns></returns>
	int minimax(Sim_Board sim, int depth)
	{ 
		if (depth == 0 || sim.gameover)
		{
			return sim.GetValue();
		}

		sim.SetNextTurnBoards();
		if (sim.turn == Turn.White)
		{
			int max = int.MinValue;
			foreach (Sim_Board simu in sim.nextTurnBoards)
			{
				int eval = minimax(simu, depth - 1);
				max = Mathf.Max(max,eval);
			}
			return max;
		}

		else
		{
			int min = int.MaxValue;
			foreach (Sim_Board simu in sim.nextTurnBoards)
			{
				int eval = minimax(simu, depth - 1);
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
	public bool gameover;

	public void SetNextTurnBoards()
	{
		SetPossibleMoves();
		foreach (var sourceCell in possibleMoves)
		{
			foreach (var move in sourceCell.Value)
			{
				StaticHelper.currentSimulations++;
				SimulateAction(sourceCell.Key, move);
			}
		}
	}

	//negative is advantage for black, positive is advantage for white
	public int GetValue()
	{
		int ret = 0;
		foreach (var cell in cells.Where(x => x.piece != null))
		{
			if (cell.piece.data.color == Turn.White)
				ret += cell.piece.data.value;
			else
				ret -= cell.piece.data.value;
		}
		return ret;
	}

	void SetPossibleMoves()
	{
		foreach (var cell in cells)
		{
			if (cell.piece == null || !cell.piece.data.canBePlayed) continue;

			//If cell has a piece, get all available movements for this piece
			List<Sim_Cell> possibleMovesFromCell = GetPossibleMovesFromCell(cell);
			if (possibleMovesFromCell.Count > 0)
				possibleMoves.Add(cell, possibleMovesFromCell);
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

		bool king = to.piece?.data.value == 900;
		simCells.First(x => x.position == to.position).piece = from.piece.Copy();
		simCells.First(x => x.position == from.position).piece = null;

		Sim_Board sim = new Sim_Board()
		{
			cells = simCells,
			turn = turn == Turn.White ? Turn.Black : Turn.White,
			leadingAction = new Sim_PieceMovement(from, to),
		};

		if (king)
		{
			sim.gameover = true;
			Debug.Log($"Win condition on {turn} turn");
		}

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