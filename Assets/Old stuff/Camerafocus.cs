using UnityEngine;
using Unity.Cinemachine;
public class Camerafocus : MonoBehaviour
{
    public CinemachineBrain brain;
    public ICinemachineCamera Cam1;
    public ICinemachineCamera Cam2;

    void Start()
    {
        Cam1 = GetComponent<CinemachineCamera>();
        Cam2 = GetComponent<CinemachineCamera>();

        //override parameters
        int layer = 1;
        int priority = 1;
        float weight = 1f;
        float blendtime = 0f;

        brain.SetCameraOverride(layer, priority, Cam1, Cam2, weight, blendtime);
    }
}

