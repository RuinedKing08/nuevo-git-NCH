using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class DodgeTest : MonoBehaviour
{
    public float activeTime = 2f;
    public float meshRefreshRate = 0.1f;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Transform pos1;
    public Material mat;

    public static DodgeTest Instance;

    private bool isTrailActive;
    public float delay = 0.5f;
    public string shaderRef;
    public float shaderRate = 0.1f;
    public float shaderRefresh = 0.05f;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        pos1 = GetComponent<Transform>();
        //InputsParent.Instance.SideStepInput().performed += ctx => Activate();
    }
    public void Activate()
    {
        if (!isTrailActive && !PlayerActions.isDodging && !PlayerActions.dodgeInCooldown)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }
    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if(skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for(int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(pos1.position, pos1.rotation);
            
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();

                Mesh mes = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mes);

                mf.mesh = mes;
                mr.material = mat;

                StartCoroutine(AlphaDown(mr.material, 0f, shaderRate, shaderRefresh));
                Destroy(obj, delay);
            }

        yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    IEnumerator AlphaDown(Material mat2, float end, float rate, float refresh)
    {
        float thing = mat2.GetFloat(shaderRef);

        while(thing > end)
        {
            thing -= rate;
            mat2.SetFloat(shaderRef, thing);
            yield return new WaitForSeconds(refresh);
        }
    }
}


