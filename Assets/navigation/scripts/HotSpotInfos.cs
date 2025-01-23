using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HotSpotInfos : MonoBehaviour
{
    [SerializeField]
    private string Number = "1";
    [SerializeField]
    private string InfoText = "Das ist ein Info Text";
    [SerializeField]
    private bool ToggleInfoText = false;


    [SerializeField]
    private TextMeshProUGUI NumberUI;
    [SerializeField]
    private TextMeshProUGUI InfoTextUI;

    public void Start()
    {
        ToggleInfoText = false;
        InfoTextUI.transform.gameObject.SetActive(ToggleInfoText);
    }
    public void OnSetUpHotspot(string num, string nfotxt)
    {
        if (NumberUI != null)
        {
            NumberUI.text = num;
        }

        if (InfoTextUI != null)
        {
            InfoTextUI.text = nfotxt;
        }

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (NumberUI != null)
        {
            NumberUI.text = Number;
        }

        if (InfoTextUI != null)
        {
            InfoTextUI.text = InfoText;
            InfoTextUI.transform.gameObject.SetActive(ToggleInfoText);

        }
    }
#endif
}
