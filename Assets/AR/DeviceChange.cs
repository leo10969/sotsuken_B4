using System;
using UnityEngine;

public class DeviceChange : MonoBehaviour
{
    public static DeviceChange Instance { get; private set; }

    public event Action OnOrientationChange;
    public event Action OnLandscape;
    public event Action OnPortrait;

    private bool isOrientationLandscape;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isOrientationLandscape = Screen.width > Screen.height;
    }

    private void Update()
    {
        if (isOrientationLandscape != (Screen.width > Screen.height))
        {
            isOrientationLandscape = Screen.width > Screen.height;
            OnOrientationChange?.Invoke();

            if (isOrientationLandscape)
            {
                OnLandscape?.Invoke();
            }
            else
            {
                OnPortrait?.Invoke();
            }
        }
    }
}
