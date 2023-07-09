using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButton : MonoBehaviour
{
	public void PointerClick()
	{
		if (GameTurn.turn == GameTurn.playerTurn) Switch();
	}

	public void Switch()
	{
		GameTurn.isSwitched = !GameTurn.isSwitched;
	}
}
