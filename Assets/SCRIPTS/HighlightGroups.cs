using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sorter.v2;
using UnityEngine.Experimental.Rendering;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class HighlightGroups : MonoBehaviour
{
    [SerializeField] private HouseSorter houseSorter;
    
    // Object Hirachie
    [SerializeField]
    private List<GameObject> ObjectsToCheck;

    private List<ObjectsOriginalMaterial> OriginalMaterialList = new List<ObjectsOriginalMaterial>();
    // 
    [Serializable]
    struct ObjectsOriginalMaterial
    {
        public GameObject gameObject;
        public Material[] materials;

        public ObjectsOriginalMaterial(GameObject gameObject, Material[] materials)
        {
            this.gameObject = gameObject;
            this.materials = materials;
        }
    }
    // Create List of Objects and their original Materials
    [Serializable]
    struct ObjectGroup
    {
        public String GroupName;
        public List<string> Keywords;
        public List<GameObject> ObjectList;
    }

    [Header("Object Groups")]
    [SerializeField]
    private ObjectGroup[] groups;

    [SerializeField]
    private Material DeHighlight;

    public List<int> ActiveGroups;

    private int LastGroup = -1;

    // Start is called before the first frame update
    void Start()
    {
        // create list of all models and get their original materials
        foreach (var ObjectToCheck in ObjectsToCheck)
        {
            foreach (var childTransform in ObjectToCheck.GetComponentsInChildren<Renderer>())
            {
                if (childTransform.GetComponent<Renderer>())
                {
                    OriginalMaterialList.Add(new ObjectsOriginalMaterial(childTransform.gameObject, childTransform.GetComponent<Renderer>().materials));
                }

                foreach (var ObjGroup in groups)
                {
                    //if (childTransform.gameObject.name.Contains(ObjGroup.Keywords[0]))
                    //{
                    //if (ObjGroup.Keywords.Skip(1).ToArray().Any(childTransform.gameObject.name.Contains))
                    if (ObjGroup.Keywords.ToArray().Any(childTransform.gameObject.name.Contains))
                    {
                        ObjGroup.ObjectList.Add(childTransform.gameObject);
                    }
                    //}
                }
            }
        }
        
        houseSorter.Sort();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HighlightGroup(int group)
    {
        ResetToOriginalMaterials();
        if (LastGroup != group)
        {
            LastGroup = group;
            foreach (var obj in OriginalMaterialList)
            {
                if (!groups[group].ObjectList.Contains(obj.gameObject))
                {
                    Material[] materials = obj.gameObject.GetComponent<Renderer>().materials;
                    for (int i = 0; i < obj.gameObject.GetComponent<Renderer>().materials.Length; i++)
                    {
                        materials[i] = DeHighlight;
                    }
                    obj.gameObject.GetComponent<Renderer>().materials = materials;

                    if (obj.gameObject.GetComponent<MeshCollider>())
                    {
                        obj.gameObject.GetComponent<MeshCollider>().enabled = false;
                    }
                }
            }
        }
        else
        {
            LastGroup = -1;
        }
    }

    private Material[] GetMaterialListForObject(GameObject obj)
    {
        foreach (var item in OriginalMaterialList)
        {
            if (obj == item.gameObject)
            {
                return (item.materials);
            }
        }
        return (null);
    }

    public void DeHighlightGroup()
    {

        foreach (var obj in OriginalMaterialList)
        {
            Material[] materials = obj.gameObject.GetComponent<Renderer>().materials;
            for (int j = 0; j < obj.gameObject.GetComponent<Renderer>().materials.Length; j++)
            {
                materials[j] = DeHighlight;
            }
            obj.gameObject.GetComponent<Renderer>().materials = materials;

            if (obj.gameObject.GetComponent<MeshCollider>())
            {
                obj.gameObject.GetComponent<MeshCollider>().enabled = false;
            }
        }


        foreach (var al in ActiveGroups)
        {
            foreach (var obj in groups[al].ObjectList)
            {
                obj.gameObject.GetComponent<Renderer>().materials = GetMaterialListForObject(obj);

                if (obj.gameObject.GetComponent<MeshCollider>())
                {
                    obj.gameObject.GetComponent<MeshCollider>().enabled = true;
                }

            }
        }

    }

    public void ToggledHighlightGroup(int group)
    {
        if (!ActiveGroups.Contains(group))
        {
            ActiveGroups.Add(group);
        }
        else
        {
            ActiveGroups.Remove(group);
        }

        if (ActiveGroups.Count == 0)
        {
            ResetToOriginalMaterials();
        }
        else
        {
            DeHighlightGroup();
        }
    }


    public void ResetToOriginalMaterials()
    {
        foreach (var obj in OriginalMaterialList)
        {
            obj.gameObject.GetComponent<Renderer>().materials = obj.materials;
            if (obj.gameObject.GetComponent<MeshCollider>())
            {
                obj.gameObject.GetComponent<MeshCollider>().enabled = true;
            }
        }
    }

}
