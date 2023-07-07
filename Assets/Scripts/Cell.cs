using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	public RectTransform rt;
	public Image image;
	public CellPosition position;
	public PieceData piece;
	public Turn color;

	public void Setup(CellPosition position, Turn color)
	{
		this.position = position;
		rt.anchoredPosition = new Vector2(position.x * rt.sizeDelta.x, position.y * rt.sizeDelta.y);
		this.color = color;
		image.color = color == Turn.White ? Color.white : Color.black;
	}
}
