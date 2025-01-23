using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Collections.Generic;

public class GetNames : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Text;

    [SerializeField]
    private Vector2 OffsetText = new Vector2(0, 0);

    [SerializeField]
    private bool Active = false;

    [SerializeField]
    private List<GameObject> ObjectsOfInterest = new List<GameObject>();

    [SerializeField]
    private List<Collider> ObjectsToCheckForNames = new List<Collider>();

    private Collider LastCollider;
    private RectTransform rectTransform;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Material HighLightMaterial;

    private Material[] OriginalMaterial;
    private GameObject PreviousHit;

    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        foreach (var ObjectToCheck in ObjectsOfInterest)
        {
            foreach (var childTransform in ObjectToCheck.GetComponentsInChildren<Collider>())
            {
                ObjectsToCheckForNames.Add(childTransform.GetComponent<Collider>());
            }
        }
    }



    public void ToggleGetNames()
    {
        Active = !Active;
        EmptyText();
    }

    private void EmptyText()
    {
        LastCollider = null;
        if (Text.text != "")
        {
            Text.text = "";
        }
       ResetToOriginalMaterial();

    }

    private void ResetToOriginalMaterial()
    {
        if (PreviousHit != null)
        {
            PreviousHit.GetComponent<Renderer>().materials = OriginalMaterial;
            PreviousHit = null;
        }
    }

    void Update()
    {
        if (Active)
        {
            //if (Input.GetMouseButtonDown(1))
            //{

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //print(hit.collider.name);
                if (ObjectsToCheckForNames.Contains(hit.collider))
                {

                    Vector2 mergedFactors = new Vector2(
                            (canvas.transform as RectTransform).sizeDelta.x / Screen.width,
                            (canvas.transform as RectTransform).sizeDelta.y / Screen.height);
                    //x and y -> *= (RefX/ScreenWidth) * (CanvasWidth/RefX)
                    Text.rectTransform.anchoredPosition = (Camera.main.WorldToScreenPoint(hit.point) * mergedFactors) + OffsetText;



                    if (LastCollider != hit.collider)
                    {
                        LastCollider = hit.collider;

                        if (HighLightMaterial)
                        {
                            if (PreviousHit != hit.transform.gameObject)
                            {
                                ResetToOriginalMaterial();

                                OriginalMaterial = hit.transform.GetComponent<Renderer>().materials;
                                PreviousHit = hit.transform.gameObject;

                                Material[] materials = PreviousHit.gameObject.GetComponent<Renderer>().materials;
                                for (int i = 0; i < PreviousHit.gameObject.GetComponent<Renderer>().materials.Length; i++)
                                {
                                    materials[i] = HighLightMaterial;
                                }
                                PreviousHit.gameObject.GetComponent<Renderer>().materials = materials;
                            }
                        }
                        string[] ud = hit.collider.name.Split('_');
                        if (ud.Count() == 5)
                        {
                            //string[] udE = ud[3].Split(' ');

                            if (ud[3].All(char.IsDigit))
                            {
                                Text.text = ud[0];
                            }
                            else
                            {
                                Text.text = ud[0] + " " + ud[3];
                            }
                        }

                        if (ud.Count() == 3)
                        {
                            if (ud[1].Length == 2)
                            {
                                string[] udE = ud[2].Split(' ');
                                if (udE.Length == 2)
                                {
                                    Text.text = ud[0];
                                }
                                else
                                {
                                    Text.text = ud[0] + " " + udE[0];
                                }
                            }
                            else
                            {
                                Text.text = ud[0] + " " + ud[1];
                            }
                        }

                        if (ud.Count() == 2)
                        {
                            Text.text = ud[0];
                        }

                        if (ud.Count() == 1)
                        {
                            Text.text = ud[0];
                        }
                    }
                }
                else
                {
                    EmptyText();
                }

            }
            else
            {
                EmptyText();
            }

        }
    }
}
