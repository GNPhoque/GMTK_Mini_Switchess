using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PieceData : ScriptableObject
{
	public Turn color;
	public CellPosition cellPosition;
	public CellPosition[] movements;
	public CellPosition[] attacks;
	public int value;
	public bool attacksSameAsMovement;

	public Sprite sprite;

	[field: SerializeField] public bool canBePlayed { get => GameTurn.turn == color ^ GameTurn.isSwitched; }
}
