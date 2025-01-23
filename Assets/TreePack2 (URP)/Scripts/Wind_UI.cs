using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wind_UI : MonoBehaviour
{
    public GameObject globalWind, localWind, translucency;
    public Material[] targetMaterials;

    public Slider speedSlider;
    public Slider frequencySlider;
    public Slider bendSlider;

    public Slider localWindSpeed;
    public Slider localWindScale;
    public Slider localWindIntensity;

    public Slider translucencyStrength;
    public Slider translucencyScattering;
    public Slider translucencyAmbient;
    public Slider translucencyShadow;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }
    private void Start()
    {
        
        speedSlider.value = GameObject.FindObjectOfType<TSP_WindController>().speed;
        frequencySlider.value = GameObject.FindObjectOfType<TSP_WindController>().worldFrequency;
        bendSlider.value = GameObject.FindObjectOfType<TSP_WindController>().bendAmount;

        // Load Default values
        for (int a = 0; a < targetMaterials.Length; a++)
        {            
            targetMaterials[a].SetFloat("_TransAmbient", translucencyAmbient.value);
            targetMaterials[a].SetFloat("_TransShadow", translucencyShadow.value);
            targetMaterials[a].SetFloat("_TransScattering", translucencyScattering.value);
            targetMaterials[a].SetFloat("_TransStrength", translucencyStrength.value);
            targetMaterials[a].SetFloat("_LocalWindSpeed", localWindSpeed.value);
            targetMaterials[a].SetFloat("_LocalWindScale", localWindScale.value);
            targetMaterials[a].SetFloat("_LocalWindIntensity", localWindIntensity.value);
        }
    }

    public void Update_Speed()
    {
        GameObject.FindObjectOfType<TSP_WindController>().speed = speedSlider.value;
    }
    public void Update_Frequency()
    {
        GameObject.FindObjectOfType<TSP_WindController>().worldFrequency = frequencySlider.value;
    }
    public void Update_Bend()
    {
        GameObject.FindObjectOfType<TSP_WindController>().bendAmount = bendSlider.value;
    }
    //________________________________________________________    
    
    public void Update_TranslucencyAmbient()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_TransAmbient", translucencyAmbient.value);
        }
    }
    public void Update_TranslucencyShadow()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_TransShadow", translucencyShadow.value);
        }
    }
    public void Update_TranslucencyScattering()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_TransScattering", translucencyScattering.value);
        }
    }
    public void Update_TranslucencyStrength()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_TransStrength", translucencyStrength.value);
        }
    }
    //________________________________________________________
    public void Update_LocalWindSpeed()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_LocalWindSpeed", localWindSpeed.value);
        }
    }
    public void Update_LocalWindScale()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_LocalWindScale", localWindScale.value);
        }
    }
    public void Update_LocalWindIntensity()
    {
        for (int a = 0; a < targetMaterials.Length; a++)
        {
            targetMaterials[a].SetFloat("_LocalWindIntensity", localWindIntensity.value);
        }
    }
    //________________________________________________________
    public void Enable_Object(GameObject target)
    {
        target.SetActive(true);
    }
    public void Disable_Object(GameObject target)
    {
        target.SetActive(false);
    }
}