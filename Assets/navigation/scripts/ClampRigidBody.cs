using UnityEngine;

public class ClampRigidBody : MonoBehaviour
{
    private Vector3 min;
    private Vector3 max;

    [SerializeField]
    private GameObject SliderBar;

    public StartOnSliderPosition StartSliderPosition = StartOnSliderPosition.Nothing;

    public enum StartOnSliderPosition
    {
        Nothing, Min, Max
    }

    [SerializeField]
    private GameObject MinimumObject;
    [SerializeField]
    private GameObject MaximumObject;

    Rigidbody rb;

    void Awake()
    {
        min = MinimumObject.transform.position;
        max = MaximumObject.transform.position;

        switch (StartSliderPosition)
        {
            case StartOnSliderPosition.Min:
                transform.position = min;
                break;
            case StartOnSliderPosition.Max:
                transform.position = max;
                break;
        }

        rb = GetComponent<Rigidbody>();

        if (SliderBar)
        {
            float distance = Vector3.Distance(min, max);
            SliderBar.transform.position = Vector3.Lerp(min, max, 0.5f);
            SliderBar.transform.localScale = new Vector3(0.1f, 0.1f, distance); ;
            SliderBar.transform.LookAt(max);
        }
    }

    void Update()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, min.x, max.x),
            Mathf.Clamp(transform.position.y, min.y, max.y),
            Mathf.Clamp(transform.position.z, min.z, max.z));
    }
}
