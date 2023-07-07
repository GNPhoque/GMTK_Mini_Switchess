using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTurn
{
	public static Turn turn = Turn.White;
	public static bool isSwitched;

	public static event Action<Turn> OnTurnChanged;

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
