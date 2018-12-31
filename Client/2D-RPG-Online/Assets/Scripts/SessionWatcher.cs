using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to count developer or player hit the play button.
/// </summary>
/// <remarks>
/// <para>Work with ScriptableObject</para>
/// </remarks>
public class SessionWatcher : MonoBehaviour {
    
    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static SessionWatcher instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    /// <summary>
    /// Session data scriptable object.
    /// </summary>
    [Header("Initialization")]
    [SerializeField]
    private Session_SO _session;

    /// <summary>
    /// To show current session ID on inspector.
    /// </summary>
    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _sessionIDDebug;

    /// <summary>
    /// Getter of SessionID.
    /// </summary>
    public int SessionID {
        get {
            return _session.ID;
        }
    }

    /// <summary>
    /// Increment sessionID just one.
    /// </summary>
    private void Start() {
        if (IsItSameSystem()) {
            _session = Instantiate(_session);
        }

        _session.ID++;

        _session.lastDeviceUniqueIdentifier = _session.deviceUniqueIdentifier;

        _session.batteryLevel = SystemInfo.batteryLevel;
        _session.batteryStatus = SystemInfo.batteryStatus;

        _session.deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        _session.deviceModel = SystemInfo.deviceModel;
        _session.deviceName = SystemInfo.deviceName;
        _session.deviceType = SystemInfo.deviceType;
        _session.graphicsDeviceID = SystemInfo.graphicsDeviceID;
        _session.graphicsDeviceName = SystemInfo.graphicsDeviceName;
        _session.graphicsDeviceType = SystemInfo.graphicsDeviceType;
        _session.graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        _session.graphicsDeviceVendorID = SystemInfo.graphicsDeviceVendorID;
        _session.graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
        _session.graphicsMemorySize = SystemInfo.graphicsMemorySize;
        _session.graphicsMultiThreaded = SystemInfo.graphicsMultiThreaded;
        _session.graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
        _session.graphicsUVStartsAtTop = SystemInfo.graphicsUVStartsAtTop;
        _session.maxCubemapSize = SystemInfo.maxCubemapSize;
        _session.maxTextureSize = SystemInfo.maxTextureSize;
        _session.npotSupport = SystemInfo.npotSupport;
        _session.operatingSystem = SystemInfo.operatingSystem;
        _session.operatingSystemFamily = SystemInfo.operatingSystemFamily;
        _session.processorCount = SystemInfo.processorCount;
        _session.processorFrequency = SystemInfo.processorFrequency;
        _session.processorType = SystemInfo.processorType;
        _session.supportedRenderTargetCount = SystemInfo.supportedRenderTargetCount;
        _session.supports2DArrayTextures = SystemInfo.supports2DArrayTextures;
        _session.supports32bitsIndexBuffer = SystemInfo.supports32bitsIndexBuffer;
        _session.supports3DRenderTextures = SystemInfo.supports3DRenderTextures;
        _session.supports3DTextures = SystemInfo.supports3DTextures;
        _session.supportsAccelerometer = SystemInfo.supportsAccelerometer;
        _session.supportsAsyncCompute = SystemInfo.supportsAsyncCompute;
        _session.supportsAsyncGPUReadback = SystemInfo.supportsAsyncGPUReadback;
        _session.supportsAudio = SystemInfo.supportsAudio;
        _session.supportsComputeShaders = SystemInfo.supportsComputeShaders;
        _session.supportsCubemapArrayTextures = SystemInfo.supportsCubemapArrayTextures;
        _session.supportsGPUFence = SystemInfo.supportsGPUFence;
        _session.supportsGyroscope = SystemInfo.supportsGyroscope;
        _session.supportsHardwareQuadTopology = SystemInfo.supportsHardwareQuadTopology;
        _session.supportsImageEffects = SystemInfo.supportsImageEffects;
        _session.supportsInstancing = SystemInfo.supportsInstancing;
        _session.supportsLocationService = SystemInfo.supportsLocationService;
        _session.supportsMipStreaming = SystemInfo.supportsMipStreaming;
        _session.supportsMotionVectors = SystemInfo.supportsMotionVectors;
        _session.supportsMultisampleAutoResolve = SystemInfo.supportsMultisampleAutoResolve;
        _session.supportsMultisampledTextures = SystemInfo.supportsMultisampledTextures;
        _session.supportsRawShadowDepthSampling = SystemInfo.supportsRawShadowDepthSampling;
        _session.supportsRenderToCubemap = SystemInfo.supportsRenderToCubemap;
        _session.supportsShadows = SystemInfo.supportsShadows;
        _session.supportsSparseTextures = SystemInfo.supportsSparseTextures;
        _session.supportsTextureWrapMirrorOnce = SystemInfo.supportsTextureWrapMirrorOnce;
        _session.supportsVibration = SystemInfo.supportsVibration;
        _session.systemMemorySize = SystemInfo.systemMemorySize;
        _session.unsupportedIdentifier = SystemInfo.unsupportedIdentifier;
        _session.usesReversedZBuffer = SystemInfo.usesReversedZBuffer;

        _sessionIDDebug = _session.ID;
    }
    
    /// <summary>
    /// Returns all session informations.
    /// </summary>
    /// <returns></returns>
    public string Introduce() {
        string message = string.Empty;

        if (!IsItSameSystem()) {
            message += "\nAll informations on https://docs.unity3d.com/ScriptReference/SystemInfo.html";
            message += "\n batteryLevel: " + _session.batteryLevel;
            message += "\n batteryStatus: " + _session.batteryStatus;
            message += "\n deviceUniqueIdentifier: " + _session.deviceUniqueIdentifier;
            message += "\n deviceModel: " + _session.deviceModel;
            message += "\n deviceName: " + _session.deviceName;
            message += "\n deviceType: " + _session.deviceType;
            message += "\n graphicsDeviceID: " + _session.graphicsDeviceID;
            message += "\n graphicsDeviceName: " + _session.graphicsDeviceName;
            message += "\n graphicsDeviceType: " + _session.graphicsDeviceType;
            message += "\n graphicsDeviceVendor: " + _session.graphicsDeviceVendor;
            message += "\n graphicsDeviceVendorID: " + _session.graphicsDeviceVendorID;
            message += "\n graphicsDeviceVersion: " + _session.graphicsDeviceVersion;
            message += "\n graphicsMemorySize: " + _session.graphicsMemorySize;
            message += "\n graphicsMultiThreaded: " + _session.graphicsMultiThreaded;
            message += "\n graphicsShaderLevel: " + _session.graphicsShaderLevel;
            message += "\n graphicsUVStartsAtTop: " + _session.graphicsUVStartsAtTop;
            message += "\n maxCubemapSize: " + _session.maxCubemapSize;
            message += "\n maxTextureSize: " + _session.maxTextureSize;
            message += "\n npotSupport: " + _session.npotSupport;
            message += "\n operatingSystem: " + _session.operatingSystem;
            message += "\n operatingSystemFamily: " + _session.operatingSystemFamily;
            message += "\n processorFrequency: " + _session.processorFrequency;
            message += "\n processorType: " + _session.processorType;
            message += "\n supportedRenderTargetCount: " + _session.supportedRenderTargetCount;
            message += "\n supports2DArrayTextures: " + _session.supports2DArrayTextures;
            message += "\n supports32bitsIndexBuffer: " + _session.supports32bitsIndexBuffer;
            message += "\n supports3DRenderTextures: " + _session.supports3DRenderTextures;
            message += "\n supports3DTextures: " + _session.supports3DTextures;
            message += "\n supportsAccelerometer: " + _session.supportsAccelerometer;
            message += "\n supportsAsyncCompute: " + _session.supportsAsyncCompute;
            message += "\n supportsAsyncGPUReadback: " + _session.supportsAsyncGPUReadback;
            message += "\n supportsAudio: " + _session.supportsAudio;
            message += "\n supportsComputeShaders: " + _session.supportsComputeShaders;
            message += "\n supportsCubemapArrayTextures: " + _session.supportsCubemapArrayTextures;
            message += "\n supportsGPUFence: " + _session.supportsGPUFence;
            message += "\n supportsGyroscope: " + _session.supportsGyroscope;
            message += "\n supportsHardwareQuadTopology: " + _session.supportsHardwareQuadTopology;
            message += "\n supportsImageEffects: " + _session.supportsImageEffects;
            message += "\n supportsInstancing: " + _session.supportsInstancing;
            message += "\n supportsLocationService: " + _session.supportsLocationService;
            message += "\n supportsMipStreaming: " + _session.supportsMipStreaming;
            message += "\n supportsMotionVectors: " + _session.supportsMotionVectors;
            message += "\n supportsMultisampleAutoResolve: " + _session.supportsMultisampleAutoResolve;
            message += "\n supportsMultisampledTextures: " + _session.supportsMultisampledTextures;
            message += "\n supportsRawShadowDepthSampling: " + _session.supportsRawShadowDepthSampling;
            message += "\n supportsRenderToCubemap: " + _session.supportsRenderToCubemap;
            message += "\n supportsMultisampledTextures: " + _session.supportsMultisampledTextures;
            message += "\n supportsShadows: " + _session.supportsShadows;
            message += "\n supportsSparseTextures: " + _session.supportsSparseTextures;
            message += "\n supportsTextureWrapMirrorOnce: " + _session.supportsTextureWrapMirrorOnce;
            message += "\n supportsVibration: " + _session.supportsVibration;
            message += "\n systemMemorySize: " + _session.systemMemorySize;
            message += "\n unsupportedIdentifier: " + _session.unsupportedIdentifier;
            message += "\n usesReversedZBuffer: " + _session.usesReversedZBuffer;
        }

        return message;
    }

    private bool IsItSameSystem() {
        return _session.lastDeviceUniqueIdentifier == SystemInfo.deviceUniqueIdentifier ? true : false;
    }
}

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