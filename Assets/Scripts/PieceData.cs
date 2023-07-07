using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PieceData : ScriptableObject
{
	public Turn color;
	public CellPosition cellPosition;
	public CellPosition[] movements;
	public int value;

	public Sprite sprite;

	[field: SerializeField] public bool canBePlayed { get => GameTurn.turn == color ^ GameTurn.isSwitched; }
}
