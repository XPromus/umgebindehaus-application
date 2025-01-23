
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
using System;

public class QualityManager : MonoBehaviour
{
    [SerializeField]
    private Slider RenderScaleSlider;

    [SerializeField]
    private TMP_Text RenderScaleText;
    private string RenderScaleTextOriginal;

    [SerializeField]
    private TMP_Dropdown Quality;

    [SerializeField]
    private bool QualityToggle=true;

    [SerializeField]
    private TMP_Dropdown AntiAliasing;

    [SerializeField]
    private List<RenderPipelineAsset> RenderSettings = new List<RenderPipelineAsset>();

    [SerializeField]
    private List<ScriptableRendererFeature> RenderEffect = new List<ScriptableRendererFeature>();


    [SerializeField]
    private Light LightSource;

    [SerializeField]
    private TMP_Dropdown ShadowType;

    private void Start()
    {
        RenderScaleTextOriginal = RenderScaleText.text;

        //SetQuality();
        //SetAntialiasing();
        //SetRenderScale();
        //SetShadowType();
    }

    public void ToggleQuality()
    {
        QualityToggle = !QualityToggle;
        if (QualityToggle)
        {
            QualitySettings.SetQualityLevel(2, true);
        }
        else
        {
            QualitySettings.SetQualityLevel(0, true);
        }
    }
    public void SetQuality()
    {
        //GraphicsSettings.renderPipelineAsset = RenderSettings[Quality.value];
        QualitySettings.SetQualityLevel(Quality.value, true);
        //print(QualitySettings.GetQualityLevel());
    }

    public void SetEffects()
    {
        switch (Quality.value)
        {
            case 0:

                RenderEffect[0].SetActive(false);
                RenderEffect[1].SetActive(false);
                break;

            case 1:
                RenderEffect[0].SetActive(true);
                RenderEffect[1].SetActive(false);
                break;

            case 2:
                RenderEffect[0].SetActive(true);
                RenderEffect[1].SetActive(true);
                break;
        }
    }

    //private void Update()
    //{
    //    foreach (RenderFeatureToggle toggleObj in RenderEffect)
    //    {
    //        toggleObj.feature.SetActive(toggleObj.isEnabled);
    //    }
    //}

    public void SetAntialiasing()
    {
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        //QualitySettings.antiAliasing = AntiAliasing.value;
        urpAsset.msaaSampleCount = (int)Mathf.Pow(2, AntiAliasing.value);
        print(Mathf.Pow(2, AntiAliasing.value));
    }

    public void SetRenderScale()
    {
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        urpAsset.renderScale = RenderScaleSlider.value/10;

        RenderScaleText.text = RenderScaleTextOriginal +  urpAsset.renderScale.ToString();
    }

    public void SetShadowType()
    {
        switch (ShadowType.value)
        {
            case 0:
                LightSource.shadows = LightShadows.None;
                break;

            case 1:
                LightSource.shadows = LightShadows.Hard;
                break;

            case 2:
                LightSource.shadows = LightShadows.Soft;
                break;
        }
    }

    public void SetAmbientOcclusion()
    {

    }

    public void SetSSCC()
    {

    }

}
