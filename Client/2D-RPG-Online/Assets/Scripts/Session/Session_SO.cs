using UnityEngine;

/// <summary>
/// This class is a scriptable object for SessionWatcher class.
/// See <see cref="SessionWatcher"/>
/// </summary>
[CreateAssetMenu(fileName = "SessionData", menuName = "Session/Create Session", order = 1)]
public class Session_SO : ScriptableObject {
    public int ID;

    public string lastDeviceUniqueIdentifier;

    public float batteryLevel; //The current battery level (Read Only).
    public BatteryStatus batteryStatus; //Returns the current status of the device's battery (Read Only).

    public string deviceUniqueIdentifier; //A unique device identifier. It is guaranteed to be unique for every device (Read Only).
    public string deviceModel; //The model of the device (Read Only).
    public string deviceName; //The user defined name of the device (Read Only).
    public DeviceType deviceType; //Returns the kind of device the application is running on (Read Only).

    public int graphicsDeviceID; //The identifier code of the graphics device(Read Only).
    public string graphicsDeviceName; //The name of the graphics device(Read Only).
    public UnityEngine.Rendering.GraphicsDeviceType graphicsDeviceType; //The graphics API type used by the graphics device(Read Only).
    public string graphicsDeviceVendor; //The vendor of the graphics device(Read Only).
    public int graphicsDeviceVendorID; //The identifier code of the graphics device vendor(Read Only).
    public string graphicsDeviceVersion; //The graphics API type and driver version used by the graphics device(Read Only).
    public int graphicsMemorySize; //Amount of video memory present(Read Only).
    public bool graphicsMultiThreaded; //Is graphics device using multi-threaded rendering(Read Only)?

    public int graphicsShaderLevel; //Graphics device shader capability level(Read Only).
                                    /*
                                    50 Shader Model 5.0 (DX11.0) 
                                    46 OpenGL 4.1 capabilities (Shader Model 4.0 + tessellation) 
                                    45 Metal / OpenGL ES 3.1 capabilities (Shader Model 3.5 + compute shaders) 
                                    40 Shader Model 4.0 (DX10.0) 
                                    35 OpenGL ES 3.0 capabilities (Shader Model 3.0 + integers, texture arrays, instancing) 
                                    30 Shader Model 3.0 
                                    25 Shader Model 2.5 (DX11 feature level 9.3 feature set) 
                                    20 Shader Model 2.0.
                                    */

    public bool graphicsUVStartsAtTop; //Returns true if the texture UV coordinate convention for this platform has Y starting at the top of the image.
    public int maxCubemapSize; //Maximum Cubemap texture size (Read Only).
    public int maxTextureSize; //Maximum texture size (Read Only).
    public NPOTSupport npotSupport; //What NPOT (non-power of two size) texture support does the GPU provide? (Read Only)
    public string operatingSystem; //Operating system name with version (Read Only).
    public OperatingSystemFamily operatingSystemFamily; //Returns the operating system family the game is running on (Read Only).
    public int processorCount; //Number of processors present (Read Only).
    public int processorFrequency; //Processor frequency in MHz (Read Only).
    public string processorType; //Processor name (Read Only).
    public int supportedRenderTargetCount; //How many simultaneous render targets (MRTs) are supported? (Read Only)
    public bool supports2DArrayTextures; //Are 2D Array textures supported? (Read Only)
    public bool supports32bitsIndexBuffer; //Are 32-bit index buffers supported? (Read Only)
    public bool supports3DRenderTextures; //Are 3D (volume) RenderTextures supported? (Read Only)
    public bool supports3DTextures; //Are 3D (volume) textures supported? (Read Only)
    public bool supportsAccelerometer; //Is an accelerometer available on the device?
    public bool supportsAsyncCompute; //Returns true when the platform supports asynchronous compute queues and false if otherwise.Note that asynchronous compute queues are only supported on PS4.
    public bool supportsAsyncGPUReadback; //Returns true if asynchronous readback of GPU data is available for this device and false otherwise.
    public bool supportsAudio; //Is there an Audio device available for playback? (Read Only)
    public bool supportsComputeShaders; //Are compute shaders supported? (Read Only)
    public bool supportsCubemapArrayTextures; //Are Cubemap Array textures supported? (Read Only)
    public bool supportsGPUFence; //Returns true when the platform supports GPUFences and false if otherwise.Note that GPUFences are only supported on PS4.
    public bool supportsGyroscope; //Is a gyroscope available on the device?
    public bool supportsHardwareQuadTopology; //Does the hardware support quad topology? (Read Only)
    public bool supportsImageEffects; //Are image effects supported? (Read Only)
    public bool supportsInstancing; //Is GPU draw call instancing supported? (Read Only)
    public bool supportsLocationService; //Is the device capable of reporting its location?
    public bool supportsMipStreaming; //Is streaming of texture mip maps supported? (Read Only)
    public bool supportsMotionVectors; //Whether motion vectors are supported on this platform.
    public bool supportsMultisampleAutoResolve; //Returns true if multisampled textures are resolved automatically
    public int supportsMultisampledTextures; //Are multisampled textures supported? (Read Only)
    public bool supportsRawShadowDepthSampling; //Is sampling raw depth from shadowmaps supported? (Read Only)
    public bool supportsRenderToCubemap; //Are cubemap render textures supported? (Read Only)
    public bool supportsShadows; //Are built-in shadows supported? (Read Only)
    public bool supportsSparseTextures; //Are sparse textures supported? (Read Only)
    public int supportsTextureWrapMirrorOnce; //Returns true if the 'Mirror Once' texture wrap mode is supported. (Read Only)
    public bool supportsVibration; //Is the device capable of providing the user haptic feedback by vibration?
    public int systemMemorySize; //Amount of system memory present (Read Only).
    public string unsupportedIdentifier; //Value returned by SystemInfo string properties which are not supported on the current platform.
    public bool usesReversedZBuffer; //This property is true if the current platform uses a reversed depth buffer (where values range from 1 at the near plane and 0 at far plane), and false if the depth buffer is normal (0 is near, 1 is far). (Read Only)
}