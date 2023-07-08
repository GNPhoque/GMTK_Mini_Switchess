using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI turnText;

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
		turnText.text = $"Turn : {obj}";
	}
}
