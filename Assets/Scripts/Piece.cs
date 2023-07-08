using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
	[SerializeField] public PieceData data;
	[SerializeField] Image image;
	[SerializeField] public RectTransform rt;
	public Vector2 startPosition;

	public void Setup(PieceData _data)
	{
		data = _data;
		image.sprite = data.sprite;
		image.color = data.color == Turn.White ? Color.white : Color.black;
	}
}
