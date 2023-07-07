using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
	[SerializeField] PieceData data;
	[SerializeField] Image image;

	private void Start()
	{
		image.sprite = data.sprite;
	}
}
