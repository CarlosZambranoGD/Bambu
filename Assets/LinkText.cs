using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContactsHandler : MonoBehaviour//, IPointerClickHandler
{
    public TextMeshProUGUI TextMeshPro;
    public Canvas Canvas;                                                                                                                                 
    public Camera Camera;

    private void Awake ()
    {
        TextMeshPro.GetComponent<TextMeshProUGUI>();
        Camera = Canvas.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay ? null : Canvas.worldCamera;
    }
 
    private void Update()
    {
        // ReSharper disable once InvertIf
        if ((Input.GetMouseButtonDown(0)) || (Input.touchCount > 0))
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(TextMeshPro.GetComponent<TextMeshProUGUI>(), Input.mousePosition, Camera);
            Debug.Log(linkIndex);
            if (linkIndex == -1) return;
            var linkInfo = TextMeshPro.textInfo.linkInfo[linkIndex];

            switch (linkInfo.GetLinkID())
            {
                case "netgamez":
                    Application.OpenURL("market://details?id=com.netgamez.limelight.unofficial");
                    break;
            }
        }
    }
}
