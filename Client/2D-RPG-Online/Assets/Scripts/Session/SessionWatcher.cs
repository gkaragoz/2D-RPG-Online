using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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

        Init();
    }

    #endregion

    [Header("Debug")]
    /// <summary>
    /// Session data scriptable object.
    /// </summary>
    [SerializeField]
    [Utils.ReadOnly]
    private Session_SO _session;

    /// <summary>
    /// To show current session ID on inspector.
    /// </summary>
    [SerializeField]
    [Utils.ReadOnly]
    private int _sessionIDDebug;

    private bool HasSessionData {
        get { return File.Exists(FULL_PATH) ? true : false; }
    }

    private const string ON_APP_START_LOG = "<<<<<NEW SESSION>>>>>";
    private const string ON_APP_QUIT_LOG = "<<<<<END SESSION>>>>>";

    /// <summary>
    /// Getter of SessionID.
    /// </summary>
    public int SessionID {
        get {
            return _session.ID;
        }
    }

    /// <summary>
    /// Session data path string.
    /// </summary>
    private const string SESSION_DATA_PATH = "Assets/ScriptableObjects/";

    /// <summary>
    /// Session data file name.
    /// </summary>
    private const string SESSION_DATA_FILE_NAME = "SessionData.asset";

    /// <summary>
    /// Session data full path.
    /// </summary>
    private const string FULL_PATH = SESSION_DATA_PATH + SESSION_DATA_FILE_NAME;

    /// <summary>
    /// Increment sessionID just one.
    /// Delete old session data and create new session data.
    /// See <see cref="ON_APP_START_LOG"/>
    /// See <see cref="Introduce"/>
    /// </summary>
    private void Init() {
        //if (HasSessionData) {
        //    if (_session == null) {
        //        _session = AssetDatabase.LoadAssetAtPath<Session_SO>(FULL_PATH);
        //    }

        //    LogManager.instance.AddLog(ON_APP_START_LOG, Log.Type.Info);

        //    if (IsItSameSystem()) {
        //        SetSystemInfos();
        //    } else {
        //        DeleteSessionData();
        //        _session = CreateSessionData();
        //        SetSystemInfos();
        //        LogManager.instance.AddLog(Introduce(), Log.Type.Info);
        //    }
        //} else {
        //    _session = CreateSessionData();

        //    LogManager.instance.AddLog(ON_APP_START_LOG, Log.Type.Info);

        //    SetSystemInfos();
        //    LogManager.instance.AddLog(Introduce(), Log.Type.Info);
        //}
    }
    
    /// <summary>
    /// Returns all session informations.
    /// </summary>
    /// <returns></returns>
    public string Introduce() {
        string message = string.Empty;

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

        return message;
    }

    /// <summary>
    /// Sets system infos to scriptable object.
    /// </summary>
    private void SetSystemInfos() {
        _session.lastDeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;

        _session.ID++;
        _sessionIDDebug = _session.ID;

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
    }

    /// <summary>
    /// Create new session data.
    /// </summary>
    /// <returns></returns>
    private Session_SO CreateSessionData() {
        Session_SO asset = ScriptableObject.CreateInstance<Session_SO>();

        //AssetDatabase.CreateAsset(asset, FULL_PATH);
        //AssetDatabase.SaveAssets();

        Debug.Log("[SessionWatcher] Created new session data on path: " + FULL_PATH);

        return asset;
    }

    /// <summary>
    /// Delete if session data exists.
    /// </summary>
    private void DeleteSessionData() {
        //AssetDatabase.DeleteAsset(FULL_PATH);
        //AssetDatabase.SaveAssets();

        Debug.Log("[SessionWatcher] Deleted session data on path : " + FULL_PATH);
    }

    /// <summary>
    /// Check is the system same as previous session.
    /// </summary>
    /// <returns></returns>
    private bool IsItSameSystem() {
        return _session.lastDeviceUniqueIdentifier == SystemInfo.deviceUniqueIdentifier ? true : false;
    }

    /// <summary>
    /// Say END_SESSION log on application quit.
    /// See <see cref="ON_APP_QUIT_LOG"/>
    /// </summary>
    private void OnApplicationQuit() {
        LogManager.instance.AddLog(ON_APP_QUIT_LOG, Log.Type.Info);
    }

}