using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
	public static FloatingTextManager Instance;
	[Header("Floating Text")]
	public GameObject FloatingText;

	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}

	public void ShowText(FloatingTextParameter para, Vector2 ownerPosition, float scaleTextSize = 1)
	{
		if (FloatingText == null)
		{
			Debug.LogError("Need place FloatingText to GameManage object");
			return;
		}

		GameObject floatingText = SpawnSystemHelper.GetNextObject(FloatingText, false);
		var _position = Camera.main.WorldToScreenPoint (para.localTextOffset + ownerPosition);
        //var _position = para.localTextOffset + ownerPosition;

		floatingText.transform.SetParent(MenuManager.Instance.transform, false);
		floatingText.transform.position = _position;

		var _FloatingText = floatingText.GetComponent<FloatingText>();
		_FloatingText.SetText(para.message, para.textColor, para.localTextOffset + ownerPosition, scaleTextSize);
		floatingText.SetActive(true);
	}

	public void ShowText(string message, Vector2 localTextOffset, Color textColor, Vector2 ownerPosition, float scaleTextSize = 1)
	{
		FloatingTextParameter _para = new FloatingTextParameter();
		_para.message = message;
		_para.localTextOffset = localTextOffset;
		_para.textColor = textColor;

		ShowText(_para, ownerPosition, scaleTextSize);
	}

	public void ShowText(string message, Vector2 ownerPosition, float scaleTextSize = 1)
	{
		FloatingTextParameter _para = new FloatingTextParameter();
		_para.message = message;
		_para.localTextOffset = Vector2.zero;
		_para.textColor = Color.white;

		ShowText(_para, ownerPosition, scaleTextSize);
	}
}

[System.Serializable]
public class FloatingTextParameter
{
	[Header("Display Text Message")]
	public string message = "MESSAGE";
	public Vector2 localTextOffset = new Vector2(0, 1);
	public Color textColor = Color.yellow;
}
