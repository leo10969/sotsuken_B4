using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraImageController : MonoBehaviour
{
    public ARCameraManager cameraManager;

    public Texture2D m_Texture;
    //private MeshRenderer mRenderer;

    private void Start()
    {
        //mRenderer = GetComponent<MeshRenderer>();
    }

    void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCpuImage image;
        if (!cameraManager.TryAcquireLatestCpuImage(out image))
            return;

        var conversionParams = new XRCpuImage.ConversionParams
        (
            image,
            TextureFormat.RGBA32,
            XRCpuImage.Transformation.MirrorY

        );

        if (m_Texture == null || m_Texture.width != image.width || m_Texture.height != image.height)
        {
            m_Texture = new Texture2D(conversionParams.outputDimensions.x,
                                     conversionParams.outputDimensions.y,
                                     conversionParams.outputFormat, false);
        }

        var buffer = m_Texture.GetRawTextureData<byte>();
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        m_Texture.Apply();
        //mRenderer.material.mainTexture = m_Texture;

        buffer.Dispose();
        image.Dispose();
    }



}
