using UnityEngine;

public class PlayerIkHeadController : MonoBehaviour
{
    public CharacterSkeleton skeleton;
    
    public Transform cameraTransform;

    [Range(0f, 1f)] public float headWeight = 0.7f;
    [Range(0f, 1f)] public float chestWeight = 0.3f;

    private Quaternion headIntialRotation;
    private Quaternion chestIntialRotation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        headIntialRotation = skeleton.Head.localRotation;
        chestIntialRotation = skeleton.UpperChest.localRotation;
    }

    // Update is called once per frame
    public void UpdateHead()
    {
        float pitch = cameraTransform.eulerAngles.x;
        pitch = pitch > 180f ? pitch-360f : pitch;
        pitch = Mathf.Clamp(pitch,-50f,50f);


        skeleton.UpperChest.localRotation = chestIntialRotation * Quaternion.Euler(0f, 0f, pitch * chestWeight);

        skeleton.Head.localRotation = headIntialRotation * Quaternion.Euler(0f, 0f, pitch * headWeight);

    }
}
