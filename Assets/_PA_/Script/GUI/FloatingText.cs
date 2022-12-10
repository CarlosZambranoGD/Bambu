using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Text floatingText;
    Vector2 currentPos;

    int oriTextSize;

    public void SetText(string text, Color color, Vector2 worldPos, float scaleTextSize = 1)
    {
        if (oriTextSize == 0)
            oriTextSize = floatingText.resizeTextMaxSize;

        floatingText.color = color;
        floatingText.text = text;
        floatingText.resizeTextMaxSize = (int)(oriTextSize * scaleTextSize);
        currentPos = worldPos;
    }

    void Update()
    {
        //always stay the first position
        var _position = Camera.main.WorldToScreenPoint(currentPos);
        //var _position = currentPos;
        floatingText.transform.position = _position;
    }
}
