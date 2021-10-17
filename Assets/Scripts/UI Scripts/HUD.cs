using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public GameObject HUD_UI;

    public void HideHUD()
    {
        HUD_UI.SetActive(false);
    }

}
