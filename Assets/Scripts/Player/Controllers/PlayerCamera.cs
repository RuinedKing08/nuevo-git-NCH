using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform _camera;

    [Header("Runtime Settings")]
    public float[] _gain = new float[2];
    private float _sensibility = 1f;
    private Vector3 _eulerAngles;

    private PlayerState tempstate;
    public PlayerState TempState => tempstate;
    public void Initialize(Transform Target)
    {
        transform.position = Target.position;
        transform.rotation = Target.rotation;
        transform.eulerAngles = _eulerAngles = Target.eulerAngles;

      

    }

    public void ProcessInput(CameraInput input)
    {

       
        UpdateRotation(input);
    }

    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.Look.y * _gain[0], input.Look.x * _gain[1]) * _sensibility;

        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -50f, 50f);
        transform.eulerAngles = _eulerAngles;
    }
    public void UpdatePosition(Transform Target)
    {
        transform.position = Target.position;
    }

}
