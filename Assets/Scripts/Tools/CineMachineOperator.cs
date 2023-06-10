using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CineMachineOperator : MonoBehaviour
{

    [SerializeField] private int priority;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    [SerializeField] float intensityModifier = 1;
    public static CineMachineOperator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null && priority >= 0)
        {
            Instance = this;
        }

        
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        
    }

    public void SetInstance()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void CamaraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity * intensityModifier;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    public void StopCameraShake()
    {
        shakeTimer = 0f;
    }

    private void Update()
    {
        shakeTimer -= Time.deltaTime;
        if (shakeTimer <= 0f)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;

            Mathf.Lerp(startingIntensity, 0f, shakeTimer / shakeTimerTotal);
   
        }
    }


}
