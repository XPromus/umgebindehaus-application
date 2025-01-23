using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Material transparentMaterialBase;
    private Material originalMaterial;
    private Material transparentMaterial;
    private MeshRenderer meshRenderer;
    private bool isTransparent;

    private void Start()
    {
        meshRenderer = transform.GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        transparentMaterial = new Material(transparentMaterialBase);
        //transparentMaterial.color = new Color(originalMaterial.color.r, originalMaterial.color.g, originalMaterial.color.b);
    }

    public void OnPointerDown(PointerEventData eventData) {
        
    }

    public void OnPointerUp(PointerEventData eventData) {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var target = eventData.pointerClick;
        Debug.Log("Object Clicked: " + target.transform.position);
        switch (isTransparent)
        {
            case true:
                ChangeMaterialToOriginal(target);
                break;
            case false:
                ChangeMaterialToTransparent(target);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        
    }

    private void ChangeMaterialToTransparent(GameObject targetGameObject)
    {
        meshRenderer.sharedMaterial = transparentMaterial;
        isTransparent = true;
    }

    private void ChangeMaterialToOriginal(GameObject targetGameObject)
    {
        meshRenderer.sharedMaterial = originalMaterial;
        isTransparent = false;
    }
    
}
