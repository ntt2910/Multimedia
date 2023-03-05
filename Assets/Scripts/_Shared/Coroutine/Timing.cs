using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

// /////////////////////////////////////////////////////////////////////////////////////////
//                              More Effective Coroutines
//                                        v3.06.2
// 
// This is an improved implementation of coroutines that boasts zero per-frame memory allocations,
// runs about twice as fast as Unity's built in coroutines and has a range of extra features.
// 
// This is the free version. MEC also has a pro version, which can be found here:
// https://www.assetstore.unity3d.com/en/#!/content/68480
// The pro version contains exactly the same core that the free version uses, but also
// contains additional features.
// 
// For manual, support, or upgrade guide visit http://trinary.tech/
//
// Created by Teal Rogers
// Trinary Software
// All rights preserved
// /////////////////////////////////////////////////////////////////////////////////////////

namespace BW.Coroutine
{
    public class Timing : MonoBehaviour
    {
        /// <summary>
        /// The time between calls to SlowUpdate.
        /// </summary>
        [Tooltip("How quickly the SlowUpdate segment ticks.")]
        public float TimeBetweenSlowUpdateCalls = 1f / 7f;
        /// <summary>
        /// The amount that each coroutine should be seperated inside the Unity profiler. NOTE: When the profiler window
        /// is not open this value is ignored and all coroutines behave as if "None" is selected.
        /// </summary>
        [Tooltip("How much data should be sent to the profiler window when it's open.")]
        public DebugInfoType ProfilerDebugAmount;
        /// <summary>
        /// The number of coroutines that are being run in the Update segment.
        /// </summary>
        [Tooltip("A count of the number of Update coroutines that are currently running."), Space(12)]
        public int UpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the FixedUpdate segment.
        /// </summary>
        [Tooltip("A count of the number of FixedUpdate coroutines that are currently running.")]
        public int FixedUpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the LateUpdate segment.
        /// </summary>
        [Tooltip("A count of the number of LateUpdate coroutines that are currently running.")]
        public int LateUpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the SlowUpdate segment.
        /// </summary>
        [Tooltip("A count of the number of SlowUpdate coroutines that are currently running.")]
        public int SlowUpdateCoroutines;
        /// <summary>
        /// The time in seconds that the current segment has been running.
        /// </summary>
        [System.NonSerialized]
        public float localTime;
        /// <summary>
        /// The time in seconds that the current segment has been running.
        /// </summary>
        public static float LocalTime { get { return Instance.localTime; } }
        /// <summary>
        /// The amount of time in fractional seconds that elapsed between this frame and the last frame.
        /// </summary>
        [System.NonSerialized]
        public float deltaTime;
        /// <summary>
        /// The amount of time in fractional seconds that elapsed between this frame and the last frame.
        /// </summary>
        public static float DeltaTime { get { return Instance.deltaTime; } }
        /// <summary>
        /// Used for advanced coroutine control.
        /// </summary>
        public static System.Func<IEnumerator<float>, CoroutineHandle, IEnumerator<float>> ReplacementFunction;
        /// <summary>
        /// This event fires just before each segment is run.
        /// </summary>
        public static event System.Action OnPreExecute;
        /// <summary>
        /// You can use "yield return Timing.WaitForOneFrame;" inside a coroutine function to go to the next frame. 
        /// </summary>
        public const float WaitForOneFrame = float.NegativeInfinity;
        /// <summary>
        /// The main thread that (almost) everything in unity runs in.
        /// </summary>
        public static System.Threading.Thread MainThread { get; private set; }
        /// <summary>
        /// The handle of the current coroutine that is running.
        /// </summary>
        public static CoroutineHandle CurrentCoroutine
        {
            get
            {
                for (int i = 1; i < 16; i++)
                    if (ActiveInstances[i] != null && ActiveInstances[i]._currentCoroutine.IsValid)
                        return ActiveInstances[i]._currentCoroutine;
                return default(CoroutineHandle);
            }
        }
        /// <summary>
        /// The handle of the current coroutine that is running.
        /// </summary>
        public CoroutineHandle currentCoroutine { get { return this._currentCoroutine; } }
        private CoroutineHandle _currentCoroutine;

        private static object _tmpRef;
        private static bool _tmpBool;
        private static CoroutineHandle _tmpHandle;

        private int _currentUpdateFrame;
        private int _currentLateUpdateFrame;
        private int _currentSlowUpdateFrame;
        private int _nextUpdateProcessSlot;
        private int _nextLateUpdateProcessSlot;
        private int _nextFixedUpdateProcessSlot;
        private int _nextSlowUpdateProcessSlot;
        private int _lastUpdateProcessSlot;
        private int _lastLateUpdateProcessSlot;
        private int _lastFixedUpdateProcessSlot;
        private int _lastSlowUpdateProcessSlot;
        private float _lastUpdateTime;
        private float _lastLateUpdateTime;
        private float _lastFixedUpdateTime;
        private float _lastSlowUpdateTime;
        private float _lastSlowUpdateDeltaTime;
        private ushort _framesSinceUpdate;
        private ushort _expansions = 1;
        private byte _instanceID;

        private readonly Dictionary<CoroutineHandle, HashSet<CoroutineHandle>> _waitingTriggers = new Dictionary<CoroutineHandle, HashSet<CoroutineHandle>>();
        private readonly HashSet<CoroutineHandle> _allWaiting = new HashSet<CoroutineHandle>();
        private readonly Dictionary<CoroutineHandle, ProcessIndex> _handleToIndex = new Dictionary<CoroutineHandle, ProcessIndex>();
        private readonly Dictionary<ProcessIndex, CoroutineHandle> _indexToHandle = new Dictionary<ProcessIndex, CoroutineHandle>();
        private readonly Dictionary<CoroutineHandle, string> _processTags = new Dictionary<CoroutineHandle, string>();
        private readonly Dictionary<string, HashSet<CoroutineHandle>> _taggedProcesses = new Dictionary<string, HashSet<CoroutineHandle>>();

        private IEnumerator<float>[] UpdateProcesses = new IEnumerator<float>[InitialBufferSizeLarge];
        private IEnumerator<float>[] LateUpdateProcesses = new IEnumerator<float>[InitialBufferSizeSmall];
        private IEnumerator<float>[] FixedUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
        private IEnumerator<float>[] SlowUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];

        private bool[] UpdatePaused = new bool[InitialBufferSizeLarge];
        private bool[] LateUpdatePaused = new bool[InitialBufferSizeSmall];
        private bool[] FixedUpdatePaused = new bool[InitialBufferSizeMedium];
        private bool[] SlowUpdatePaused = new bool[InitialBufferSizeMedium];
        private bool[] UpdateHeld = new bool[InitialBufferSizeLarge];
        private bool[] LateUpdateHeld = new bool[InitialBufferSizeSmall];
        private bool[] FixedUpdateHeld = new bool[InitialBufferSizeMedium];
        private bool[] SlowUpdateHeld = new bool[InitialBufferSizeMedium];

        private const ushort FramesUntilMaintenance = 64;
        private const int ProcessArrayChunkSize = 64;
        private const int InitialBufferSizeLarge = 256;
        private const int InitialBufferSizeMedium = 64;
        private const int InitialBufferSizeSmall = 8;

        private static Timing[] ActiveInstances = new Timing[16];
        private static Timing _instance;

        public static Timing Instance
        {
            get
            {
                if (_instance == null || !_instance.gameObject)
                {
                    GameObject instanceHome = GameObject.Find("Timing Controller");

                    if (instanceHome == null)
                    {
                        instanceHome = new GameObject { name = "Timing Controller" };

                        DontDestroyOnLoad(instanceHome);
                    }

                    _instance = instanceHome.GetComponent<Timing>() ?? instanceHome.AddComponent<Timing>();

                    _instance.InitializeInstanceID();
                }

                return _instance;
            }
            set { _instance = value; }
        }

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                this.deltaTime = _instance.deltaTime;

            if (MainThread == null)
                MainThread = System.Threading.Thread.CurrentThread;
        }

        void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        void OnEnable()
        {
            InitializeInstanceID();
        }

        void OnDisable()
        {
            if (this._instanceID < ActiveInstances.Length)
                ActiveInstances[this._instanceID] = null;
        }

        private void InitializeInstanceID()
        {
            if (ActiveInstances[this._instanceID] == null)
            {
                if (this._instanceID == 0x00) this._instanceID++;

                for (; this._instanceID <= 0x10; this._instanceID++)
                {
                    if (this._instanceID == 0x10)
                    {
                        Destroy(gameObject);
                        throw new System.OverflowException("You are only allowed 15 different contexts for MEC to run inside at one time.");
                    }

                    if (ActiveInstances[this._instanceID] == null)
                    {
                        ActiveInstances[this._instanceID] = this;
                        break;
                    }
                }
            }
        }

        void Update()
        {
            if (OnPreExecute != null)
                OnPreExecute();

            if (this._lastSlowUpdateTime + this.TimeBetweenSlowUpdateCalls < Time.realtimeSinceStartup && this._nextSlowUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.SlowUpdate };
                if (UpdateTimeValues(coindex.seg)) this._lastSlowUpdateProcessSlot = this._nextSlowUpdateProcessSlot;

                for (coindex.i = 0; coindex.i < this._lastSlowUpdateProcessSlot; coindex.i++)
                {
                    try
                    {
                        if (!this.SlowUpdatePaused[coindex.i] && !this.SlowUpdateHeld[coindex.i] && this.SlowUpdateProcesses[coindex.i] != null && !(this.localTime < this.SlowUpdateProcesses[coindex.i].Current))
                        {
                            this._currentCoroutine = this._indexToHandle[coindex];

                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                            {
                                Profiler.BeginSample(this.ProfilerDebugAmount == DebugInfoType.SeperateTags ? ("Processing Coroutine (Slow Update)" +
                                                                                                               (this._processTags.ContainsKey(this._indexToHandle[coindex]) ? ", tag " + this._processTags[this._indexToHandle[coindex]] : ", no tag"))
                                        : "Processing Coroutine (Slow Update)");
                            }

                            if (!this.SlowUpdateProcesses[coindex.i].MoveNext())
                            {
                                if (this._indexToHandle.ContainsKey(coindex))
                                    KillCoroutinesOnInstance(this._indexToHandle[coindex]);
                            }
                            else if (this.SlowUpdateProcesses[coindex.i] != null && float.IsNaN(this.SlowUpdateProcesses[coindex.i].Current))
                            {
                                if (ReplacementFunction != null)
                                {
                                    this.SlowUpdateProcesses[coindex.i] = ReplacementFunction(this.SlowUpdateProcesses[coindex.i], this._indexToHandle[coindex]);
                                    ReplacementFunction = null;
                                }
                                coindex.i--;
                            }

                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                                Profiler.EndSample();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }

            if (this._nextUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.Update };
                if (UpdateTimeValues(coindex.seg)) this._lastUpdateProcessSlot = this._nextUpdateProcessSlot;

                for (coindex.i = 0; coindex.i < this._lastUpdateProcessSlot; coindex.i++)
                {
                    try
                    {
                        if (!this.UpdatePaused[coindex.i] && !this.UpdateHeld[coindex.i] && this.UpdateProcesses[coindex.i] != null && !(this.localTime < this.UpdateProcesses[coindex.i].Current))
                        {
                            this._currentCoroutine = this._indexToHandle[coindex];

                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                            {
                                Profiler.BeginSample(this.ProfilerDebugAmount == DebugInfoType.SeperateTags ? ("Processing Coroutine" +
                                                                                                               (this._processTags.ContainsKey(this._indexToHandle[coindex]) ? ", tag " + this._processTags[this._indexToHandle[coindex]] : ", no tag"))
                                        : "Processing Coroutine");
                            }

                            if (!this.UpdateProcesses[coindex.i].MoveNext())
                            {
                                if (this._indexToHandle.ContainsKey(coindex))
                                    KillCoroutinesOnInstance(this._indexToHandle[coindex]);
                            }
                            else if (this.UpdateProcesses[coindex.i] != null && float.IsNaN(this.UpdateProcesses[coindex.i].Current))
                            {
                                if (ReplacementFunction != null)
                                {
                                    this.UpdateProcesses[coindex.i] = ReplacementFunction(this.UpdateProcesses[coindex.i], this._indexToHandle[coindex]);
                                    ReplacementFunction = null;
                                }
                                coindex.i--;
                            }

                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                                Profiler.EndSample();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }

            this._currentCoroutine = default(CoroutineHandle);

            if (++this._framesSinceUpdate > FramesUntilMaintenance)
            {
                this._framesSinceUpdate = 0;

                if (this.ProfilerDebugAmount != DebugInfoType.None)
                    Profiler.BeginSample("Maintenance Task");

                RemoveUnused();

                if (this.ProfilerDebugAmount != DebugInfoType.None)
                    Profiler.EndSample();
            }
        }

        void FixedUpdate()
        {
            if (OnPreExecute != null)
                OnPreExecute();

            if (this._nextFixedUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.FixedUpdate };
                if (UpdateTimeValues(coindex.seg)) this._lastFixedUpdateProcessSlot = this._nextFixedUpdateProcessSlot;

                for (coindex.i = 0; coindex.i < this._lastFixedUpdateProcessSlot; coindex.i++)
                {
                    try
                    {
                        if (!this.FixedUpdatePaused[coindex.i] && !this.FixedUpdateHeld[coindex.i] && this.FixedUpdateProcesses[coindex.i] != null && !(this.localTime < this.FixedUpdateProcesses[coindex.i].Current))
                        {
                            this._currentCoroutine = this._indexToHandle[coindex];


                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                            {
                                Profiler.BeginSample(this.ProfilerDebugAmount == DebugInfoType.SeperateTags ? ("Processing Coroutine" +
                                                                                                               (this._processTags.ContainsKey(this._indexToHandle[coindex]) ? ", tag " + this._processTags[this._indexToHandle[coindex]] : ", no tag"))
                                        : "Processing Coroutine");
                            }

                            if (!this.FixedUpdateProcesses[coindex.i].MoveNext())
                            {
                                if (this._indexToHandle.ContainsKey(coindex))
                                    KillCoroutinesOnInstance(this._indexToHandle[coindex]);
                            }
                            else if (this.FixedUpdateProcesses[coindex.i] != null && float.IsNaN(this.FixedUpdateProcesses[coindex.i].Current))
                            {
                                if (ReplacementFunction != null)
                                {
                                    this.FixedUpdateProcesses[coindex.i] = ReplacementFunction(this.FixedUpdateProcesses[coindex.i], this._indexToHandle[coindex]);
                                    ReplacementFunction = null;
                                }
                                coindex.i--;
                            }

                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                                Profiler.EndSample();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                this._currentCoroutine = default(CoroutineHandle);
            }
        }

        void LateUpdate()
        {
            if (OnPreExecute != null)
                OnPreExecute();

            if (this._nextLateUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.LateUpdate };
                if (UpdateTimeValues(coindex.seg)) this._lastLateUpdateProcessSlot = this._nextLateUpdateProcessSlot;

                for (coindex.i = 0; coindex.i < this._lastLateUpdateProcessSlot; coindex.i++)
                {
                    try
                    {
                        if (!this.LateUpdatePaused[coindex.i] && !this.LateUpdateHeld[coindex.i] && this.LateUpdateProcesses[coindex.i] != null && !(this.localTime < this.LateUpdateProcesses[coindex.i].Current))
                        {
                            this._currentCoroutine = this._indexToHandle[coindex];


                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                            {
                                Profiler.BeginSample(this.ProfilerDebugAmount == DebugInfoType.SeperateTags ? ("Processing Coroutine" +
                                                                                                               (this._processTags.ContainsKey(this._indexToHandle[coindex]) ? ", tag " + this._processTags[this._indexToHandle[coindex]] : ", no tag"))
                                        : "Processing Coroutine");
                            }

                            if (!this.LateUpdateProcesses[coindex.i].MoveNext())
                            {
                                if (this._indexToHandle.ContainsKey(coindex))
                                    KillCoroutinesOnInstance(this._indexToHandle[coindex]);
                            }
                            else if (this.LateUpdateProcesses[coindex.i] != null && float.IsNaN(this.LateUpdateProcesses[coindex.i].Current))
                            {
                                if (ReplacementFunction != null)
                                {
                                    this.LateUpdateProcesses[coindex.i] = ReplacementFunction(this.LateUpdateProcesses[coindex.i], this._indexToHandle[coindex]);
                                    ReplacementFunction = null;
                                }
                                coindex.i--;
                            }

                            if (this.ProfilerDebugAmount != DebugInfoType.None && this._indexToHandle.ContainsKey(coindex))
                                Profiler.EndSample();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                this._currentCoroutine = default(CoroutineHandle);
            }
        }

        private void RemoveUnused()
        {
            var waitTrigsEnum = this._waitingTriggers.GetEnumerator();
            while (waitTrigsEnum.MoveNext())
            {
                if (waitTrigsEnum.Current.Value.Count == 0)
                {
                    this._waitingTriggers.Remove(waitTrigsEnum.Current.Key);
                    waitTrigsEnum = this._waitingTriggers.GetEnumerator();
                    continue;
                }

                if (this._handleToIndex.ContainsKey(waitTrigsEnum.Current.Key) && CoindexIsNull(this._handleToIndex[waitTrigsEnum.Current.Key]))
                {
                    CloseWaitingProcess(waitTrigsEnum.Current.Key);
                    waitTrigsEnum = this._waitingTriggers.GetEnumerator();
                }
            }

            ProcessIndex outer, inner;
            outer.seg = inner.seg = Segment.Update;

            for (outer.i = inner.i = 0; outer.i < this._nextUpdateProcessSlot; outer.i++)
            {
                if (this.UpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        this.UpdateProcesses[inner.i] = this.UpdateProcesses[outer.i];
                        this.UpdatePaused[inner.i] = this.UpdatePaused[outer.i];
                        this.UpdateHeld[inner.i] = this.UpdateHeld[outer.i];

                        if (this._indexToHandle.ContainsKey(inner))
                        {
                            RemoveTag(this._indexToHandle[inner]);
                            this._handleToIndex.Remove(this._indexToHandle[inner]);
                            this._indexToHandle.Remove(inner);
                        }

                        this._handleToIndex[this._indexToHandle[outer]] = inner;
                        this._indexToHandle.Add(inner, this._indexToHandle[outer]);
                        this._indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < this._nextUpdateProcessSlot; outer.i++)
            {
                this.UpdateProcesses[outer.i] = null;
                this.UpdatePaused[outer.i] = false;
                this.UpdateHeld[outer.i] = false;

                if (this._indexToHandle.ContainsKey(outer))
                {
                    RemoveTag(this._indexToHandle[outer]);

                    this._handleToIndex.Remove(this._indexToHandle[outer]);
                    this._indexToHandle.Remove(outer);
                }
            }

            this._lastUpdateProcessSlot -= this._nextUpdateProcessSlot - inner.i;
            this.UpdateCoroutines = this._nextUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.FixedUpdate;
            for (outer.i = inner.i = 0; outer.i < this._nextFixedUpdateProcessSlot; outer.i++)
            {
                if (this.FixedUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        this.FixedUpdateProcesses[inner.i] = this.FixedUpdateProcesses[outer.i];
                        this.FixedUpdatePaused[inner.i] = this.FixedUpdatePaused[outer.i];
                        this.FixedUpdateHeld[inner.i] = this.FixedUpdateHeld[outer.i];

                        if (this._indexToHandle.ContainsKey(inner))
                        {
                            RemoveTag(this._indexToHandle[inner]);
                            this._handleToIndex.Remove(this._indexToHandle[inner]);
                            this._indexToHandle.Remove(inner);
                        }

                        this._handleToIndex[this._indexToHandle[outer]] = inner;
                        this._indexToHandle.Add(inner, this._indexToHandle[outer]);
                        this._indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < this._nextFixedUpdateProcessSlot; outer.i++)
            {
                this.FixedUpdateProcesses[outer.i] = null;
                this.FixedUpdatePaused[outer.i] = false;
                this.FixedUpdateHeld[outer.i] = false;

                if (this._indexToHandle.ContainsKey(outer))
                {
                    RemoveTag(this._indexToHandle[outer]);

                    this._handleToIndex.Remove(this._indexToHandle[outer]);
                    this._indexToHandle.Remove(outer);
                }
            }

            this._lastFixedUpdateProcessSlot -= this._nextFixedUpdateProcessSlot - inner.i;
            this.FixedUpdateCoroutines = this._nextFixedUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.LateUpdate;
            for (outer.i = inner.i = 0; outer.i < this._nextLateUpdateProcessSlot; outer.i++)
            {
                if (this.LateUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        this.LateUpdateProcesses[inner.i] = this.LateUpdateProcesses[outer.i];
                        this.LateUpdatePaused[inner.i] = this.LateUpdatePaused[outer.i];
                        this.LateUpdateHeld[inner.i] = this.LateUpdateHeld[outer.i];

                        if (this._indexToHandle.ContainsKey(inner))
                        {
                            RemoveTag(this._indexToHandle[inner]);
                            this._handleToIndex.Remove(this._indexToHandle[inner]);
                            this._indexToHandle.Remove(inner);
                        }

                        this._handleToIndex[this._indexToHandle[outer]] = inner;
                        this._indexToHandle.Add(inner, this._indexToHandle[outer]);
                        this._indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < this._nextLateUpdateProcessSlot; outer.i++)
            {
                this.LateUpdateProcesses[outer.i] = null;
                this.LateUpdatePaused[outer.i] = false;
                this.LateUpdateHeld[outer.i] = false;

                if (this._indexToHandle.ContainsKey(outer))
                {
                    RemoveTag(this._indexToHandle[outer]);

                    this._handleToIndex.Remove(this._indexToHandle[outer]);
                    this._indexToHandle.Remove(outer);
                }
            }

            this._lastLateUpdateProcessSlot -= this._nextLateUpdateProcessSlot - inner.i;
            this.LateUpdateCoroutines = this._nextLateUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.SlowUpdate;
            for (outer.i = inner.i = 0; outer.i < this._nextSlowUpdateProcessSlot; outer.i++)
            {
                if (this.SlowUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        this.SlowUpdateProcesses[inner.i] = this.SlowUpdateProcesses[outer.i];
                        this.SlowUpdatePaused[inner.i] = this.SlowUpdatePaused[outer.i];
                        this.SlowUpdateHeld[inner.i] = this.SlowUpdateHeld[outer.i];

                        if (this._indexToHandle.ContainsKey(inner))
                        {
                            RemoveTag(this._indexToHandle[inner]);
                            this._handleToIndex.Remove(this._indexToHandle[inner]);
                            this._indexToHandle.Remove(inner);
                        }

                        this._handleToIndex[this._indexToHandle[outer]] = inner;
                        this._indexToHandle.Add(inner, this._indexToHandle[outer]);
                        this._indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < this._nextSlowUpdateProcessSlot; outer.i++)
            {
                this.SlowUpdateProcesses[outer.i] = null;
                this.SlowUpdatePaused[outer.i] = false;
                this.SlowUpdateHeld[outer.i] = false;

                if (this._indexToHandle.ContainsKey(outer))
                {
                    RemoveTag(this._indexToHandle[outer]);

                    this._handleToIndex.Remove(this._indexToHandle[outer]);
                    this._indexToHandle.Remove(outer);
                }
            }

            this._lastSlowUpdateProcessSlot -= this._nextSlowUpdateProcessSlot - inner.i;
            this.SlowUpdateCoroutines = this._nextSlowUpdateProcessSlot = inner.i;
        }

        /// <summary>
        /// Run a new coroutine in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine)
        {
            return coroutine == null ? new CoroutineHandle()
                : Instance.RunCoroutineInternal(coroutine, Segment.Update, null, new CoroutineHandle(Instance._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, string tag)
        {
            return coroutine == null ? new CoroutineHandle()
                : Instance.RunCoroutineInternal(coroutine, Segment.Update, tag, new CoroutineHandle(Instance._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="segment">The segment that the coroutine should run in.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment segment)
        {
            return coroutine == null ? new CoroutineHandle()
                : Instance.RunCoroutineInternal(coroutine, segment, null, new CoroutineHandle(Instance._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="segment">The segment that the coroutine should run in.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment segment, string tag)
        {
            return coroutine == null ? new CoroutineHandle()
                : Instance.RunCoroutineInternal(coroutine, segment, tag, new CoroutineHandle(Instance._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine)
        {
            return coroutine == null ? new CoroutineHandle()
                 : RunCoroutineInternal(coroutine, Segment.Update, null, new CoroutineHandle(this._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, string tag)
        {
            return coroutine == null ? new CoroutineHandle()
                 : RunCoroutineInternal(coroutine, Segment.Update, tag, new CoroutineHandle(this._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="segment">The segment that the coroutine should run in.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment segment)
        {
            return coroutine == null ? new CoroutineHandle()
                 : RunCoroutineInternal(coroutine, segment, null, new CoroutineHandle(this._instanceID), true);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="segment">The segment that the coroutine should run in.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment segment, string tag)
        {
            return coroutine == null ? new CoroutineHandle()
                 : RunCoroutineInternal(coroutine, segment, tag, new CoroutineHandle(this._instanceID), true);
        }


        private CoroutineHandle RunCoroutineInternal(IEnumerator<float> coroutine, Segment segment, string tag, CoroutineHandle handle, bool prewarm)
        {
            ProcessIndex slot = new ProcessIndex { seg = segment };

            if (this._handleToIndex.ContainsKey(handle))
            {
                this._indexToHandle.Remove(this._handleToIndex[handle]);
                this._handleToIndex.Remove(handle);
            }

            float currentLocalTime = this.localTime;
            float currentDeltaTime = this.deltaTime;
            CoroutineHandle cachedHandle = this._currentCoroutine;
            this._currentCoroutine = handle;

            switch (segment)
            {
                case Segment.Update:

                    if (this._nextUpdateProcessSlot >= this.UpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = this.UpdateProcesses;
                        bool[] oldPausedArray = this.UpdatePaused;
                        bool[] oldHeldArray = this.UpdateHeld;

                        this.UpdateProcesses = new IEnumerator<float>[this.UpdateProcesses.Length + (ProcessArrayChunkSize * this._expansions++)];
                        this.UpdatePaused = new bool[this.UpdateProcesses.Length];
                        this.UpdateHeld = new bool[this.UpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            this.UpdateProcesses[i] = oldProcArray[i];
                            this.UpdatePaused[i] = oldPausedArray[i];
                            this.UpdateHeld[i] = oldHeldArray[i];
                        }
                    }

                    if (UpdateTimeValues(slot.seg)) this._lastUpdateProcessSlot = this._nextUpdateProcessSlot;

                    slot.i = this._nextUpdateProcessSlot++;
                    this.UpdateProcesses[slot.i] = coroutine;

                    if (null != tag)
                        AddTag(tag, handle);

                    this._indexToHandle.Add(slot, handle);
                    this._handleToIndex.Add(handle, slot);

                    while (prewarm)
                    {
                        if (!this.UpdateProcesses[slot.i].MoveNext())
                        {
                            if (this._indexToHandle.ContainsKey(slot))
                                KillCoroutinesOnInstance(this._indexToHandle[slot]);

                            prewarm = false;
                        }
                        else if (this.UpdateProcesses[slot.i] != null && float.IsNaN(this.UpdateProcesses[slot.i].Current))
                        {
                            if (ReplacementFunction != null)
                            {
                                this.UpdateProcesses[slot.i] = ReplacementFunction(this.UpdateProcesses[slot.i], this._indexToHandle[slot]);
                                ReplacementFunction = null;
                            }
                            prewarm = !this.UpdatePaused[slot.i] || !this.UpdateHeld[slot.i];
                        }
                        else
                        {
                            prewarm = false;
                        }
                    }

                    break;

                case Segment.FixedUpdate:

                    if (this._nextFixedUpdateProcessSlot >= this.FixedUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = this.FixedUpdateProcesses;
                        bool[] oldPausedArray = this.FixedUpdatePaused;
                        bool[] oldHeldArray = this.FixedUpdateHeld;

                        this.FixedUpdateProcesses = new IEnumerator<float>[this.FixedUpdateProcesses.Length + (ProcessArrayChunkSize * this._expansions++)];
                        this.FixedUpdatePaused = new bool[this.FixedUpdateProcesses.Length];
                        this.FixedUpdateHeld = new bool[this.FixedUpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            this.FixedUpdateProcesses[i] = oldProcArray[i];
                            this.FixedUpdatePaused[i] = oldPausedArray[i];
                            this.FixedUpdateHeld[i] = oldHeldArray[i];
                        }
                    }

                    if (UpdateTimeValues(slot.seg)) this._lastFixedUpdateProcessSlot = this._nextFixedUpdateProcessSlot;

                    slot.i = this._nextFixedUpdateProcessSlot++;
                    this.FixedUpdateProcesses[slot.i] = coroutine;

                    if (null != tag)
                        AddTag(tag, handle);

                    this._indexToHandle.Add(slot, handle);
                    this._handleToIndex.Add(handle, slot);

                    while (prewarm)
                    {
                        if (!this.FixedUpdateProcesses[slot.i].MoveNext())
                        {
                            if (this._indexToHandle.ContainsKey(slot))
                                KillCoroutinesOnInstance(this._indexToHandle[slot]);

                            prewarm = false;
                        }
                        else if (this.FixedUpdateProcesses[slot.i] != null && float.IsNaN(this.FixedUpdateProcesses[slot.i].Current))
                        {
                            if (ReplacementFunction != null)
                            {
                                this.FixedUpdateProcesses[slot.i] = ReplacementFunction(this.FixedUpdateProcesses[slot.i], this._indexToHandle[slot]);
                                ReplacementFunction = null;
                            }
                            prewarm = !this.FixedUpdatePaused[slot.i] || !this.FixedUpdateHeld[slot.i];
                        }
                        else
                        {
                            prewarm = false;
                        }
                    }

                    break;

                case Segment.LateUpdate:

                    if (this._nextLateUpdateProcessSlot >= this.LateUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = this.LateUpdateProcesses;
                        bool[] oldPausedArray = this.LateUpdatePaused;
                        bool[] oldHeldArray = this.LateUpdateHeld;

                        this.LateUpdateProcesses = new IEnumerator<float>[this.LateUpdateProcesses.Length + (ProcessArrayChunkSize * this._expansions++)];
                        this.LateUpdatePaused = new bool[this.LateUpdateProcesses.Length];
                        this.LateUpdateHeld = new bool[this.LateUpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            this.LateUpdateProcesses[i] = oldProcArray[i];
                            this.LateUpdatePaused[i] = oldPausedArray[i];
                            this.LateUpdateHeld[i] = oldHeldArray[i];
                        }
                    }

                    if (UpdateTimeValues(slot.seg)) this._lastLateUpdateProcessSlot = this._nextLateUpdateProcessSlot;

                    slot.i = this._nextLateUpdateProcessSlot++;
                    this.LateUpdateProcesses[slot.i] = coroutine;

                    if (tag != null)
                        AddTag(tag, handle);

                    this._indexToHandle.Add(slot, handle);
                    this._handleToIndex.Add(handle, slot);

                    while (prewarm)
                    {
                        if (!this.LateUpdateProcesses[slot.i].MoveNext())
                        {
                            if (this._indexToHandle.ContainsKey(slot))
                                KillCoroutinesOnInstance(this._indexToHandle[slot]);

                            prewarm = false;
                        }
                        else if (this.LateUpdateProcesses[slot.i] != null && float.IsNaN(this.LateUpdateProcesses[slot.i].Current))
                        {
                            if (ReplacementFunction != null)
                            {
                                this.LateUpdateProcesses[slot.i] = ReplacementFunction(this.LateUpdateProcesses[slot.i], this._indexToHandle[slot]);
                                ReplacementFunction = null;
                            }
                            prewarm = !this.LateUpdatePaused[slot.i] || !this.LateUpdateHeld[slot.i];
                        }
                        else
                        {
                            prewarm = false;
                        }
                    }

                    break;

                case Segment.SlowUpdate:

                    if (this._nextSlowUpdateProcessSlot >= this.SlowUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = this.SlowUpdateProcesses;
                        bool[] oldPausedArray = this.SlowUpdatePaused;
                        bool[] oldHeldArray = this.SlowUpdateHeld;

                        this.SlowUpdateProcesses = new IEnumerator<float>[this.SlowUpdateProcesses.Length + (ProcessArrayChunkSize * this._expansions++)];
                        this.SlowUpdatePaused = new bool[this.SlowUpdateProcesses.Length];
                        this.SlowUpdateHeld = new bool[this.SlowUpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            this.SlowUpdateProcesses[i] = oldProcArray[i];
                            this.SlowUpdatePaused[i] = oldPausedArray[i];
                            this.SlowUpdateHeld[i] = oldHeldArray[i];
                        }
                    }

                    if (UpdateTimeValues(slot.seg)) this._lastSlowUpdateProcessSlot = this._nextSlowUpdateProcessSlot;

                    slot.i = this._nextSlowUpdateProcessSlot++;
                    this.SlowUpdateProcesses[slot.i] = coroutine;

                    if (tag != null)
                        AddTag(tag, handle);

                    this._indexToHandle.Add(slot, handle);
                    this._handleToIndex.Add(handle, slot);

                    while (prewarm)
                    {
                        if (!this.SlowUpdateProcesses[slot.i].MoveNext())
                        {
                            if (this._indexToHandle.ContainsKey(slot))
                                KillCoroutinesOnInstance(this._indexToHandle[slot]);

                            prewarm = false;
                        }
                        else if (this.SlowUpdateProcesses[slot.i] != null && float.IsNaN(this.SlowUpdateProcesses[slot.i].Current))
                        {
                            if (ReplacementFunction != null)
                            {
                                this.SlowUpdateProcesses[slot.i] = ReplacementFunction(this.SlowUpdateProcesses[slot.i], this._indexToHandle[slot]);
                                ReplacementFunction = null;
                            }
                            prewarm = !this.SlowUpdatePaused[slot.i] || !this.SlowUpdateHeld[slot.i];
                        }
                        else
                        {
                            prewarm = false;
                        }
                    }

                    break;

                default:
                    handle = new CoroutineHandle();
                    break;
            }

            this.localTime = currentLocalTime;
            this.deltaTime = currentDeltaTime;
            this._currentCoroutine = cachedHandle;

            return handle;
        }

        /// <summary>
        /// This will kill all coroutines running on the main MEC instance and reset the context.
        /// </summary>
        /// <returns>The number of coroutines that were killed.</returns>
        public static int KillCoroutines()
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance();
        }

        /// <summary>
        /// This will kill all coroutines running on the current MEC instance and reset the context.
        /// </summary>
        /// <returns>The number of coroutines that were killed.</returns>
        public int KillCoroutinesOnInstance()
        {
            int retVal = this._nextUpdateProcessSlot + this._nextLateUpdateProcessSlot + this._nextFixedUpdateProcessSlot + this._nextSlowUpdateProcessSlot;

            this.UpdateProcesses = new IEnumerator<float>[InitialBufferSizeLarge];
            this.UpdatePaused = new bool[InitialBufferSizeLarge];
            this.UpdateHeld = new bool[InitialBufferSizeLarge];
            this.UpdateCoroutines = 0;
            this._nextUpdateProcessSlot = 0;

            this.LateUpdateProcesses = new IEnumerator<float>[InitialBufferSizeSmall];
            this.LateUpdatePaused = new bool[InitialBufferSizeSmall];
            this.LateUpdateHeld = new bool[InitialBufferSizeSmall];
            this.LateUpdateCoroutines = 0;
            this._nextLateUpdateProcessSlot = 0;

            this.FixedUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
            this.FixedUpdatePaused = new bool[InitialBufferSizeMedium];
            this.FixedUpdateHeld = new bool[InitialBufferSizeMedium];
            this.FixedUpdateCoroutines = 0;
            this._nextFixedUpdateProcessSlot = 0;

            this.SlowUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
            this.SlowUpdatePaused = new bool[InitialBufferSizeMedium];
            this.SlowUpdateHeld = new bool[InitialBufferSizeMedium];
            this.SlowUpdateCoroutines = 0;
            this._nextSlowUpdateProcessSlot = 0;

            this._processTags.Clear();
            this._taggedProcesses.Clear();
            this._handleToIndex.Clear();
            this._indexToHandle.Clear();
            this._waitingTriggers.Clear();
            this._expansions = (ushort)((this._expansions / 2) + 1);

            return retVal;
        }

        /// <summary>
        /// Kills the instances of the coroutine handle if it exists.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public static int KillCoroutines(CoroutineHandle handle)
        {
            return ActiveInstances[handle.Key] != null ? GetInstance(handle.Key).KillCoroutinesOnInstance(handle) : 0;
        }

        /// <summary>
        /// Kills the instance of the coroutine handle on this Timing instance if it exists.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public int KillCoroutinesOnInstance(CoroutineHandle handle)
        {
            bool foundOne = false;

            if (this._handleToIndex.ContainsKey(handle))
            {
                if (this._waitingTriggers.ContainsKey(handle))
                    CloseWaitingProcess(handle);

                foundOne = CoindexExtract(this._handleToIndex[handle]) != null;
                RemoveTag(handle);
            }

            return foundOne ? 1 : 0;
        }

        /// <summary>
        /// Kills all coroutines that have the given tag.
        /// </summary>
        /// <param name="tag">All coroutines with this tag will be killed.</param>
        /// <returns>The number of coroutines that were found and killed.</returns>
        public static int KillCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance(tag);
        }

        /// <summary> 
        /// Kills all coroutines that have the given tag.
        /// </summary>
        /// <param name="tag">All coroutines with this tag will be killed.</param>
        /// <returns>The number of coroutines that were found and killed.</returns>
        public int KillCoroutinesOnInstance(string tag)
        {
            if (tag == null) return 0;
            int numberFound = 0;

            while (this._taggedProcesses.ContainsKey(tag))
            {
                var matchEnum = this._taggedProcesses[tag].GetEnumerator();
                matchEnum.MoveNext();

                if (Nullify(this._handleToIndex[matchEnum.Current]))
                {
                    if (this._waitingTriggers.ContainsKey(matchEnum.Current))
                        CloseWaitingProcess(matchEnum.Current);

                    numberFound++;
                }

                RemoveTag(matchEnum.Current);

                if (this._handleToIndex.ContainsKey(matchEnum.Current))
                {
                    this._indexToHandle.Remove(this._handleToIndex[matchEnum.Current]);
                    this._handleToIndex.Remove(matchEnum.Current);
                }
            }

            return numberFound;
        }

        /// <summary>
        /// This will pause all coroutines running on the current MEC instance until ResumeCoroutines is called.
        /// </summary>
        /// <returns>The number of coroutines that were paused.</returns>
        public static int PauseCoroutines()
        {
            return _instance == null ? 0 : _instance.PauseCoroutinesOnInstance();
        }

        /// <summary>
        /// This will pause all coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <returns>The number of coroutines that were paused.</returns>
        public int PauseCoroutinesOnInstance()
        {
            int count = 0;
            int i;
            for (i = 0; i < this._nextUpdateProcessSlot; i++)
            {
                if (!this.UpdatePaused[i] && this.UpdateProcesses[i] != null)
                {
                    count++;
                    this.UpdatePaused[i] = true;

                    if (this.UpdateProcesses[i].Current > GetSegmentTime(Segment.Update))
                        this.UpdateProcesses[i] = _InjectDelay(this.UpdateProcesses[i], this.UpdateProcesses[i].Current - GetSegmentTime(Segment.Update));
                }
            }

            for (i = 0; i < this._nextLateUpdateProcessSlot; i++)
            {
                if (!this.LateUpdatePaused[i] && this.LateUpdateProcesses[i] != null)
                {
                    count++;
                    this.LateUpdatePaused[i] = true;

                    if (this.LateUpdateProcesses[i].Current > GetSegmentTime(Segment.LateUpdate))
                        this.LateUpdateProcesses[i] = _InjectDelay(this.LateUpdateProcesses[i], this.LateUpdateProcesses[i].Current - GetSegmentTime(Segment.LateUpdate));
                }
            }

            for (i = 0; i < this._nextFixedUpdateProcessSlot; i++)
            {
                if (!this.FixedUpdatePaused[i] && this.FixedUpdateProcesses[i] != null)
                {
                    count++;
                    this.FixedUpdatePaused[i] = true;

                    if (this.FixedUpdateProcesses[i].Current > GetSegmentTime(Segment.FixedUpdate))
                        this.FixedUpdateProcesses[i] = _InjectDelay(this.FixedUpdateProcesses[i], this.FixedUpdateProcesses[i].Current - GetSegmentTime(Segment.FixedUpdate));
                }
            }

            for (i = 0; i < this._nextSlowUpdateProcessSlot; i++)
            {
                if (!this.SlowUpdatePaused[i] && this.SlowUpdateProcesses[i] != null)
                {
                    count++;
                    this.SlowUpdatePaused[i] = true;

                    if (this.SlowUpdateProcesses[i].Current > GetSegmentTime(Segment.SlowUpdate))
                        this.SlowUpdateProcesses[i] = _InjectDelay(this.SlowUpdateProcesses[i], this.SlowUpdateProcesses[i].Current - GetSegmentTime(Segment.SlowUpdate));
                }
            }

            return count;
        }

        /// <summary>
        /// This will pause any matching coroutines until ResumeCoroutines is called.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to pause.</param>
        /// <returns>The number of coroutines that were paused (0 or 1).</returns>
        public static int PauseCoroutines(CoroutineHandle handle)
        {
            return ActiveInstances[handle.Key] != null ? GetInstance(handle.Key).PauseCoroutinesOnInstance(handle) : 0;
        }

        /// <summary>
        /// This will pause any matching coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to pause.</param>
        /// <returns>The number of coroutines that were paused (0 or 1).</returns>
        public int PauseCoroutinesOnInstance(CoroutineHandle handle)
        {
            return this._handleToIndex.ContainsKey(handle) && !CoindexIsNull(this._handleToIndex[handle]) && !SetPause(this._handleToIndex[handle], true) ? 1 : 0;
        }

        /// <summary>
        /// This will pause any matching coroutines running on the current MEC instance until ResumeCoroutines is called.
        /// </summary>
        /// <param name="tag">Any coroutines with a matching tag will be paused.</param>
        /// <returns>The number of coroutines that were paused.</returns>
        public static int PauseCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.PauseCoroutinesOnInstance(tag);
        }

        /// <summary>
        /// This will pause any matching coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <param name="tag">Any coroutines with a matching tag will be paused.</param>
        /// <returns>The number of coroutines that were paused.</returns>
        public int PauseCoroutinesOnInstance(string tag)
        {
            if (tag == null || !this._taggedProcesses.ContainsKey(tag))
                return 0;

            int count = 0;
            var matchesEnum = this._taggedProcesses[tag].GetEnumerator();

            while (matchesEnum.MoveNext())
                if (!CoindexIsNull(this._handleToIndex[matchesEnum.Current]) && !SetPause(this._handleToIndex[matchesEnum.Current], true))
                    count++;

            return count;
        }

        /// <summary>
        /// This resumes all coroutines on the current MEC instance if they are currently paused, otherwise it has
        /// no effect.
        /// </summary>
        /// <returns>The number of coroutines that were resumed.</returns>
        public static int ResumeCoroutines()
        {
            return _instance == null ? 0 : _instance.ResumeCoroutinesOnInstance();
        }

        /// <summary>
        /// This resumes all coroutines on this MEC instance if they are currently paused, otherwise it has no effect.
        /// </summary>
        /// <returns>The number of coroutines that were resumed.</returns>
        public int ResumeCoroutinesOnInstance()
        {
            int count = 0;
            ProcessIndex coindex;
            for (coindex.i = 0, coindex.seg = Segment.Update; coindex.i < this._nextUpdateProcessSlot; coindex.i++)
            {
                if (this.UpdatePaused[coindex.i] && this.UpdateProcesses[coindex.i] != null)
                {
                    this.UpdatePaused[coindex.i] = false;
                    count++;
                }
            }

            for (coindex.i = 0, coindex.seg = Segment.LateUpdate; coindex.i < this._nextLateUpdateProcessSlot; coindex.i++)
            {
                if (this.LateUpdatePaused[coindex.i] && this.LateUpdateProcesses[coindex.i] != null)
                {
                    this.LateUpdatePaused[coindex.i] = false;
                    count++;
                }
            }

            for (coindex.i = 0, coindex.seg = Segment.FixedUpdate; coindex.i < this._nextFixedUpdateProcessSlot; coindex.i++)
            {
                if (this.FixedUpdatePaused[coindex.i] && this.FixedUpdateProcesses[coindex.i] != null)
                {
                    this.FixedUpdatePaused[coindex.i] = false;
                    count++;
                }
            }

            for (coindex.i = 0, coindex.seg = Segment.SlowUpdate; coindex.i < this._nextSlowUpdateProcessSlot; coindex.i++)
            {
                if (this.SlowUpdatePaused[coindex.i] && this.SlowUpdateProcesses[coindex.i] != null)
                {
                    this.SlowUpdatePaused[coindex.i] = false;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// This will resume any matching coroutines.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to resume.</param>
        /// <returns>The number of coroutines that were resumed (0 or 1).</returns>
        public static int ResumeCoroutines(CoroutineHandle handle)
        {
            return ActiveInstances[handle.Key] != null ? GetInstance(handle.Key).ResumeCoroutinesOnInstance(handle) : 0;
        }

        /// <summary>
        /// This will resume any matching coroutines running on this MEC instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to resume.</param>
        /// <returns>The number of coroutines that were resumed (0 or 1).</returns>
        public int ResumeCoroutinesOnInstance(CoroutineHandle handle)
        {
            return this._handleToIndex.ContainsKey(handle) &&
                   !CoindexIsNull(this._handleToIndex[handle]) && SetPause(this._handleToIndex[handle], false) ? 1 : 0;
        }

        /// <summary>
        /// This resumes any matching coroutines on the current MEC instance if they are currently paused, otherwise it has
        /// no effect.
        /// </summary>
        /// <param name="tag">Any coroutines previously paused with a matching tag will be resumend.</param>
        /// <returns>The number of coroutines that were resumed.</returns>
        public static int ResumeCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.ResumeCoroutinesOnInstance(tag);
        }

        /// <summary>
        /// This resumes any matching coroutines on this MEC instance if they are currently paused, otherwise it has no effect.
        /// </summary>
        /// <param name="tag">Any coroutines previously paused with a matching tag will be resumend.</param>
        /// <returns>The number of coroutines that were resumed.</returns>
        public int ResumeCoroutinesOnInstance(string tag)
        {
            if (tag == null || !this._taggedProcesses.ContainsKey(tag))
                return 0;
            int count = 0;

            var indexesEnum = this._taggedProcesses[tag].GetEnumerator();
            while (indexesEnum.MoveNext())
            {
                if (!CoindexIsNull(this._handleToIndex[indexesEnum.Current]) && SetPause(this._handleToIndex[indexesEnum.Current], false))
                {
                    count++;
                }
            }

            return count;
        }

        private bool UpdateTimeValues(Segment segment)
        {
            switch (segment)
            {
                case Segment.Update:
                    if (this._currentUpdateFrame != Time.frameCount)
                    {
                        this.deltaTime = Time.deltaTime;
                        this._lastUpdateTime += this.deltaTime;
                        this.localTime = this._lastUpdateTime;
                        this._currentUpdateFrame = Time.frameCount;
                        return true;
                    }
                    else
                    {
                        this.deltaTime = Time.deltaTime;
                        this.localTime = this._lastUpdateTime;
                        return false;
                    }
                case Segment.LateUpdate:
                    if (this._currentLateUpdateFrame != Time.frameCount)
                    {
                        this.deltaTime = Time.deltaTime;
                        this._lastLateUpdateTime += this.deltaTime;
                        this.localTime = this._lastLateUpdateTime;
                        this._currentLateUpdateFrame = Time.frameCount;
                        return true;
                    }
                    else
                    {
                        this.deltaTime = Time.deltaTime;
                        this.localTime = this._lastLateUpdateTime;
                        return false;
                    }
                case Segment.FixedUpdate:
                    this.deltaTime = Time.fixedDeltaTime;
                    this.localTime = Time.fixedTime;

                    if (this._lastFixedUpdateTime + 0.0001f < Time.fixedTime)
                    {
                        this._lastFixedUpdateTime = Time.fixedTime;
                        return true;
                    }

                    return false;
                case Segment.SlowUpdate:
                    if (this._currentSlowUpdateFrame != Time.frameCount)
                    {
                        this.deltaTime = this._lastSlowUpdateDeltaTime = Time.realtimeSinceStartup - this._lastSlowUpdateTime;
                        this.localTime = this._lastSlowUpdateTime = Time.realtimeSinceStartup;
                        this._currentSlowUpdateFrame = Time.frameCount;
                        return true;
                    }
                    else
                    {
                        this.deltaTime = this._lastSlowUpdateDeltaTime;
                        this.localTime = this._lastSlowUpdateTime;
                        return false;
                    }
            }
            return true;
        }

        private float GetSegmentTime(Segment segment)
        {
            switch (segment)
            {
                case Segment.Update:
                    if (this._currentUpdateFrame == Time.frameCount)
                        return this._lastUpdateTime;
                    else
                        return this._lastUpdateTime + Time.deltaTime;
                case Segment.LateUpdate:
                    if (this._currentUpdateFrame == Time.frameCount)
                        return this._lastLateUpdateTime;
                    else
                        return this._lastLateUpdateTime + Time.deltaTime;
                case Segment.FixedUpdate:
                    return Time.fixedTime;
                case Segment.SlowUpdate:
                    return Time.realtimeSinceStartup;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Retrieves the MEC manager that corresponds to the supplied instance id.
        /// </summary>
        /// <param name="ID">The instance ID.</param>
        /// <returns>The manager, or null if not found.</returns>
        public static Timing GetInstance(byte ID)
        {
            if (ID >= 0x10)
                return null;
            return ActiveInstances[ID];
        }

        private void AddTag(string tag, CoroutineHandle coindex)
        {
            this._processTags.Add(coindex, tag);

            if (this._taggedProcesses.ContainsKey(tag))
                this._taggedProcesses[tag].Add(coindex);
            else
                this._taggedProcesses.Add(tag, new HashSet<CoroutineHandle> { coindex });
        }

        private void RemoveTag(CoroutineHandle coindex)
        {
            if (this._processTags.ContainsKey(coindex))
            {
                if (this._taggedProcesses[this._processTags[coindex]].Count > 1)
                    this._taggedProcesses[this._processTags[coindex]].Remove(coindex);
                else
                    this._taggedProcesses.Remove(this._processTags[coindex]);

                this._processTags.Remove(coindex);
            }
        }

        /// <returns>Whether it was already null.</returns>
        private bool Nullify(ProcessIndex coindex)
        {
            bool retVal;

            switch (coindex.seg)
            {
                case Segment.Update:
                    retVal = this.UpdateProcesses[coindex.i] != null;
                    this.UpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = this.FixedUpdateProcesses[coindex.i] != null;
                    this.FixedUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = this.LateUpdateProcesses[coindex.i] != null;
                    this.LateUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = this.SlowUpdateProcesses[coindex.i] != null;
                    this.SlowUpdateProcesses[coindex.i] = null;
                    return retVal;
                default:
                    return false;
            }
        }

        private IEnumerator<float> CoindexExtract(ProcessIndex coindex)
        {
            IEnumerator<float> retVal;

            switch (coindex.seg)
            {
                case Segment.Update:
                    retVal = this.UpdateProcesses[coindex.i];
                    this.UpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = this.FixedUpdateProcesses[coindex.i];
                    this.FixedUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = this.LateUpdateProcesses[coindex.i];
                    this.LateUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = this.SlowUpdateProcesses[coindex.i];
                    this.SlowUpdateProcesses[coindex.i] = null;
                    return retVal;
                default:
                    return null;
            }
        }

        private IEnumerator<float> CoindexPeek(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return this.UpdateProcesses[coindex.i];
                case Segment.FixedUpdate:
                    return this.FixedUpdateProcesses[coindex.i];
                case Segment.LateUpdate:
                    return this.LateUpdateProcesses[coindex.i];
                case Segment.SlowUpdate:
                    return this.SlowUpdateProcesses[coindex.i];
                default:
                    return null;
            }
        }

        private bool CoindexIsNull(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return this.UpdateProcesses[coindex.i] == null;
                case Segment.FixedUpdate:
                    return this.FixedUpdateProcesses[coindex.i] == null;
                case Segment.LateUpdate:
                    return this.LateUpdateProcesses[coindex.i] == null;
                case Segment.SlowUpdate:
                    return this.SlowUpdateProcesses[coindex.i] == null;
                default:
                    return true;
            }
        }

        private bool SetPause(ProcessIndex coindex, bool newPausedState)
        {
            if (CoindexPeek(coindex) == null)
                return false;

            bool isPaused;

            switch (coindex.seg)
            {
                case Segment.Update:
                    isPaused = this.UpdatePaused[coindex.i];
                    this.UpdatePaused[coindex.i] = newPausedState;

                    if (newPausedState && this.UpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.UpdateProcesses[coindex.i] = _InjectDelay(this.UpdateProcesses[coindex.i], this.UpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isPaused;
                case Segment.FixedUpdate:
                    isPaused = this.FixedUpdatePaused[coindex.i];
                    this.FixedUpdatePaused[coindex.i] = newPausedState;

                    if (newPausedState && this.FixedUpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.FixedUpdateProcesses[coindex.i] = _InjectDelay(this.FixedUpdateProcesses[coindex.i], this.FixedUpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isPaused;
                case Segment.LateUpdate:
                    isPaused = this.LateUpdatePaused[coindex.i];
                    this.LateUpdatePaused[coindex.i] = newPausedState;

                    if (newPausedState && this.LateUpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.LateUpdateProcesses[coindex.i] = _InjectDelay(this.LateUpdateProcesses[coindex.i], this.LateUpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isPaused;
                case Segment.SlowUpdate:
                    isPaused = this.SlowUpdatePaused[coindex.i];
                    this.SlowUpdatePaused[coindex.i] = newPausedState;

                    if (newPausedState && this.SlowUpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.SlowUpdateProcesses[coindex.i] = _InjectDelay(this.SlowUpdateProcesses[coindex.i], this.SlowUpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isPaused;
                default:
                    return false;
            }
        }

        private bool SetHeld(ProcessIndex coindex, bool newHeldState)
        {
            if (CoindexPeek(coindex) == null)
                return false;

            bool isHeld;

            switch (coindex.seg)
            {
                case Segment.Update:
                    isHeld = this.UpdateHeld[coindex.i];
                    this.UpdateHeld[coindex.i] = newHeldState;

                    if (newHeldState && this.UpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.UpdateProcesses[coindex.i] = _InjectDelay(this.UpdateProcesses[coindex.i], this.UpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isHeld;
                case Segment.FixedUpdate:
                    isHeld = this.FixedUpdateHeld[coindex.i];
                    this.FixedUpdateHeld[coindex.i] = newHeldState;

                    if (newHeldState && this.FixedUpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.FixedUpdateProcesses[coindex.i] = _InjectDelay(this.FixedUpdateProcesses[coindex.i], this.FixedUpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isHeld;
                case Segment.LateUpdate:
                    isHeld = this.LateUpdateHeld[coindex.i];
                    this.LateUpdateHeld[coindex.i] = newHeldState;

                    if (newHeldState && this.LateUpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.LateUpdateProcesses[coindex.i] = _InjectDelay(this.LateUpdateProcesses[coindex.i], this.LateUpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isHeld;
                case Segment.SlowUpdate:
                    isHeld = this.SlowUpdateHeld[coindex.i];
                    this.SlowUpdateHeld[coindex.i] = newHeldState;

                    if (newHeldState && this.SlowUpdateProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
                        this.SlowUpdateProcesses[coindex.i] = _InjectDelay(this.SlowUpdateProcesses[coindex.i], this.SlowUpdateProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));

                    return isHeld;
                default:
                    return false;
            }
        }

        private IEnumerator<float> _InjectDelay(IEnumerator<float> proc, float delayTime)
        {
            yield return WaitForSecondsOnInstance(delayTime);

            _tmpRef = proc;
            ReplacementFunction = ReturnTmpRefForRepFunc;
            yield return float.NaN;
        }

        private bool CoindexIsPaused(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return this.UpdatePaused[coindex.i];
                case Segment.FixedUpdate:
                    return this.FixedUpdatePaused[coindex.i];
                case Segment.LateUpdate:
                    return this.LateUpdatePaused[coindex.i];
                case Segment.SlowUpdate:
                    return this.SlowUpdatePaused[coindex.i];
                default:
                    return false;
            }
        }

        private bool CoindexIsHeld(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return this.UpdateHeld[coindex.i];
                case Segment.FixedUpdate:
                    return this.FixedUpdateHeld[coindex.i];
                case Segment.LateUpdate:
                    return this.LateUpdateHeld[coindex.i];
                case Segment.SlowUpdate:
                    return this.SlowUpdateHeld[coindex.i];
                default:
                    return false;
            }
        }

        private void CoindexReplace(ProcessIndex coindex, IEnumerator<float> replacement)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    this.UpdateProcesses[coindex.i] = replacement;
                    return;
                case Segment.FixedUpdate:
                    this.FixedUpdateProcesses[coindex.i] = replacement;
                    return;
                case Segment.LateUpdate:
                    this.LateUpdateProcesses[coindex.i] = replacement;
                    return;
                case Segment.SlowUpdate:
                    this.SlowUpdateProcesses[coindex.i] = replacement;
                    return;
            }
        }

        /// <summary>
        /// Use "yield return Timing.WaitForSeconds(time);" to wait for the specified number of seconds.
        /// </summary>
        /// <param name="waitTime">Number of seconds to wait.</param>
        public static float WaitForSeconds(float waitTime)
        {
            if (float.IsNaN(waitTime)) waitTime = 0f;
            return LocalTime + waitTime;
        }

        /// <summary>
        /// Use "yield return timingInstance.WaitForSecondsOnInstance(time);" to wait for the specified number of seconds.
        /// </summary>
        /// <param name="waitTime">Number of seconds to wait.</param>
        public float WaitForSecondsOnInstance(float waitTime)
        {
            if (float.IsNaN(waitTime)) waitTime = 0f;
            return this.localTime + waitTime;
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(otherCoroutine);" to pause the current 
        /// coroutine until otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        public static float WaitUntilDone(CoroutineHandle otherCoroutine)
        {
            return WaitUntilDone(otherCoroutine, true);
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(otherCoroutine, false);" to pause the current 
        /// coroutine until otherCoroutine is done, supressing warnings.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        /// <param name="warnOnIssue">Post a warning to the console if no hold action was actually performed.</param>
        public static float WaitUntilDone(CoroutineHandle otherCoroutine, bool warnOnIssue)
        {
            Timing inst = GetInstance(otherCoroutine.Key);

            if (inst != null && inst._handleToIndex.ContainsKey(otherCoroutine))
            {
                if (inst.CoindexIsNull(inst._handleToIndex[otherCoroutine]))
                    return 0f;

                if (!inst._waitingTriggers.ContainsKey(otherCoroutine))
                {
                    inst.CoindexReplace(inst._handleToIndex[otherCoroutine],
                        inst._StartWhenDone(otherCoroutine, inst.CoindexPeek(inst._handleToIndex[otherCoroutine])));
                    inst._waitingTriggers.Add(otherCoroutine, new HashSet<CoroutineHandle>());
                }

                if (inst._currentCoroutine == otherCoroutine)
                {
                    Assert.IsFalse(warnOnIssue, "A coroutine cannot wait for itself.");
                    return WaitForOneFrame;
                }
                if (!inst._currentCoroutine.IsValid)
                {
                    Assert.IsFalse(warnOnIssue, "The two coroutines are not running on the same MEC instance.");
                    return WaitForOneFrame;
                }

                inst._waitingTriggers[otherCoroutine].Add(inst._currentCoroutine);
                if (!inst._allWaiting.Contains(inst._currentCoroutine))
                    inst._allWaiting.Add(inst._currentCoroutine);
                inst.SetHeld(inst._handleToIndex[inst._currentCoroutine], true);
                inst.SwapToLast(otherCoroutine, inst._currentCoroutine);

                return float.NaN;
            }

            Assert.IsFalse(warnOnIssue, "WaitUntilDone cannot hold: The coroutine handle that was passed in is invalid.\n" + otherCoroutine);
            return WaitForOneFrame;
        }

        private IEnumerator<float> _StartWhenDone(CoroutineHandle handle, IEnumerator<float> proc)
        {
            if (!this._waitingTriggers.ContainsKey(handle)) yield break;

            try
            {
                if (proc.Current > this.localTime)
                    yield return proc.Current;

                while (proc.MoveNext())
                    yield return proc.Current;
            }
            finally
            {
                CloseWaitingProcess(handle);
            }
        }

        private void SwapToLast(CoroutineHandle firstHandle, CoroutineHandle lastHandle)
        {
            if (firstHandle.Key != lastHandle.Key)
                return;

            ProcessIndex firstIndex = this._handleToIndex[firstHandle];
            ProcessIndex lastIndex = this._handleToIndex[lastHandle];

            if (firstIndex.seg != lastIndex.seg || firstIndex.i < lastIndex.i)
                return;

            IEnumerator<float> tempCoptr = CoindexPeek(firstIndex);
            CoindexReplace(firstIndex, CoindexPeek(lastIndex));
            CoindexReplace(lastIndex, tempCoptr);

            this._indexToHandle[firstIndex] = lastHandle;
            this._indexToHandle[lastIndex] = firstHandle;
            this._handleToIndex[firstHandle] = lastIndex;
            this._handleToIndex[lastHandle] = firstIndex;
            bool tmpB = SetPause(firstIndex, CoindexIsPaused(lastIndex));
            SetPause(lastIndex, tmpB);
            tmpB = SetHeld(firstIndex, CoindexIsHeld(lastIndex));
            SetHeld(lastIndex, tmpB);
        }

        private void CloseWaitingProcess(CoroutineHandle handle)
        {
            if (!this._waitingTriggers.ContainsKey(handle)) return;

            var tasksEnum = this._waitingTriggers[handle].GetEnumerator();
            this._waitingTriggers.Remove(handle);

            while (tasksEnum.MoveNext())
            {
                if (this._handleToIndex.ContainsKey(tasksEnum.Current) && !HandleIsInWaitingList(tasksEnum.Current))
                {
                    SetHeld(this._handleToIndex[tasksEnum.Current], false);
                    this._allWaiting.Remove(tasksEnum.Current);
                }
            }
        }

        private bool HandleIsInWaitingList(CoroutineHandle handle)
        {
            var triggersEnum = this._waitingTriggers.GetEnumerator();
            while (triggersEnum.MoveNext())
                if (triggersEnum.Current.Value.Contains(handle))
                    return true;

            return false;
        }

        private static IEnumerator<float> ReturnTmpRefForRepFunc(IEnumerator<float> coptr, CoroutineHandle handle)
        {
            return _tmpRef as IEnumerator<float>;
        }

#if !UNITY_2018_3_OR_NEWER
        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(wwwObject);" to pause the current 
        /// coroutine until the wwwObject is done.
        /// </summary>
        /// <param name="wwwObject">The www object to pause for.</param>
        public static float WaitUntilDone(WWW wwwObject)
        {
            if (wwwObject == null || wwwObject.isDone) return 0f;

            _tmpRef = wwwObject;
            ReplacementFunction = WaitUntilDoneWwwHelper;
            return float.NaN;
        }


        private static IEnumerator<float> WaitUntilDoneWwwHelper(IEnumerator<float> coptr, CoroutineHandle handle)
        {
            return _StartWhenDone(_tmpRef as WWW, coptr);
        }

        private static IEnumerator<float> _StartWhenDone(WWW www, IEnumerator<float> pausedProc)
        {
            while (!www.isDone)
                yield return WaitForOneFrame;

            _tmpRef = pausedProc;
            ReplacementFunction = ReturnTmpRefForRepFunc;
            yield return float.NaN;
        }
#endif

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(operation);" to pause the current 
        /// coroutine until the operation is done.
        /// </summary>
        /// <param name="operation">The operation variable returned.</param>
        public static float WaitUntilDone(AsyncOperation operation)
        {
            if (operation == null || operation.isDone) return float.NaN;

            CoroutineHandle handle = CurrentCoroutine;
            Timing inst = GetInstance(CurrentCoroutine.Key);
            if (inst == null) return float.NaN;

            _tmpRef = _StartWhenDone(operation, inst.CoindexPeek(inst._handleToIndex[handle]));
            ReplacementFunction = ReturnTmpRefForRepFunc;
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(AsyncOperation operation, IEnumerator<float> pausedProc)
        {
            while (!operation.isDone)
                yield return WaitForOneFrame;

            _tmpRef = pausedProc;
            ReplacementFunction = ReturnTmpRefForRepFunc;
            yield return float.NaN;
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(operation);" to pause the current 
        /// coroutine until the operation is done.
        /// </summary>
        /// <param name="operation">The operation variable returned.</param>
        public static float WaitUntilDone(CustomYieldInstruction operation)
        {
            if (operation == null || !operation.keepWaiting) return float.NaN;

            CoroutineHandle handle = CurrentCoroutine;
            Timing inst = GetInstance(CurrentCoroutine.Key);
            if (inst == null) return float.NaN;

            _tmpRef = _StartWhenDone(operation, inst.CoindexPeek(inst._handleToIndex[handle]));
            ReplacementFunction = ReturnTmpRefForRepFunc;
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(CustomYieldInstruction operation, IEnumerator<float> pausedProc)
        {
            while (operation.keepWaiting)
                yield return WaitForOneFrame;

            _tmpRef = pausedProc;
            ReplacementFunction = ReturnTmpRefForRepFunc;
            yield return float.NaN;
        }

        /// <summary>
        /// Keeps this coroutine from executing until UnlockCoroutine is called with a matching key.
        /// </summary>
        /// <param name="coroutine">The handle to the coroutine to be locked.</param>
        /// <param name="key">The key to use. A new key can be generated by calling "new CoroutineHandle(0)".</param>
        /// <returns>Whether the lock was successful.</returns>
        public bool LockCoroutine(CoroutineHandle coroutine, CoroutineHandle key)
        {
            if (coroutine.Key != this._instanceID || key == new CoroutineHandle() || key.Key != 0)
                return false;

            if (!this._waitingTriggers.ContainsKey(key))
                this._waitingTriggers.Add(key, new HashSet<CoroutineHandle> { coroutine });
            else
                this._waitingTriggers[key].Add(coroutine);

            SetHeld(this._handleToIndex[coroutine], true);

            return true;
        }

        /// <summary>
        /// Unlocks a coroutine that has been locked, so long as the key matches.
        /// </summary>
        /// <param name="coroutine">The handle to the coroutine to be unlocked.</param>
        /// <param name="key">The key that the coroutine was previously locked with.</param>
        /// <returns>Whether the coroutine was successfully unlocked.</returns>
        public bool UnlockCoroutine(CoroutineHandle coroutine, CoroutineHandle key)
        {
            if (coroutine.Key != this._instanceID || key == new CoroutineHandle() ||
                !this._handleToIndex.ContainsKey(coroutine) || !this._waitingTriggers.ContainsKey(key))
                return false;

            this._waitingTriggers[key].Remove(coroutine);

            SetHeld(this._handleToIndex[coroutine], HandleIsInWaitingList(coroutine));

            return true;
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallDelayed(float delay, System.Action action)
        {
            return action == null ? new CoroutineHandle() : RunCoroutine(Instance._DelayedCall(delay, action, null));
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallDelayedOnInstance(float delay, System.Action action)
        {
            return action == null ? new CoroutineHandle() : RunCoroutineOnInstance(_DelayedCall(delay, action, null));
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        /// <param name="cancelWith">A GameObject that will be checked to make sure it hasn't been destroyed before calling the action.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallDelayed(float delay, System.Action action, GameObject cancelWith)
        {
            return action == null ? new CoroutineHandle() : RunCoroutine(Instance._DelayedCall(delay, action, cancelWith));
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        /// <param name="cancelWith">A GameObject that will be checked to make sure it hasn't been destroyed before calling the action.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallDelayedOnInstance(float delay, System.Action action, GameObject cancelWith)
        {
            return action == null ? new CoroutineHandle() : RunCoroutineOnInstance(_DelayedCall(delay, action, cancelWith));
        }

        private IEnumerator<float> _DelayedCall(float delay, System.Action action, GameObject cancelWith)
        {
            yield return WaitForSecondsOnInstance(delay);

            if (ReferenceEquals(cancelWith, null) || cancelWith != null)
                action();
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically(float timeframe, float period, System.Action action, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance(float timeframe, float period, System.Action action, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="segment">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically(float timeframe, float period, System.Action action, Segment segment, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), segment);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="segment">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance(float timeframe, float period, System.Action action, Segment segment, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), segment);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously(float timeframe, System.Action action, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance(float timeframe, System.Action action, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously(float timeframe, System.Action action, Segment timing, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance(float timeframe, System.Action action, Segment timing, System.Action onDone = null)
        {
            return action == null ? new CoroutineHandle() : RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), timing);
        }

        private IEnumerator<float> _CallContinuously(float timeframe, float period, System.Action action, System.Action onDone)
        {
            double startTime = this.localTime;
            while (this.localTime <= startTime + timeframe)
            {
                yield return WaitForSecondsOnInstance(period);

                action();
            }

            if (onDone != null)
                onDone();
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically<T>
            (T reference, float timeframe, float period, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance<T>
            (T reference, float timeframe, float period, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically<T>(T reference, float timeframe, float period, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously<T>(T reference, float timeframe, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance<T>(T reference, float timeframe, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously<T>(T reference, float timeframe, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance<T>(T reference, float timeframe, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? new CoroutineHandle() :
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), timing);
        }

        private IEnumerator<float> _CallContinuously<T>(T reference, float timeframe, float period,
            System.Action<T> action, System.Action<T> onDone = null)
        {
            double startTime = this.localTime;
            while (this.localTime <= startTime + timeframe)
            {
                yield return WaitForSecondsOnInstance(period);

                action(reference);
            }

            if (onDone != null)
                onDone(reference);
        }

        private struct ProcessIndex : System.IEquatable<ProcessIndex>
        {
            public Segment seg;
            public int i;

            public bool Equals(ProcessIndex other)
            {
                return this.seg == other.seg && this.i == other.i;
            }

            public override bool Equals(object other)
            {
                if (other is ProcessIndex)
                    return Equals((ProcessIndex)other);
                return false;
            }

            public static bool operator ==(ProcessIndex a, ProcessIndex b)
            {
                return a.seg == b.seg && a.i == b.i;
            }

            public static bool operator !=(ProcessIndex a, ProcessIndex b)
            {
                return a.seg != b.seg || a.i != b.i;
            }

            public override int GetHashCode()
            {
                return (((int) this.seg - 2) * (int.MaxValue / 3)) + this.i;
            }
        }
    }

    /// <summary>
    /// The timing segment that a coroutine is running in or should be run in.
    /// </summary>
    public enum Segment
    {
        /// <summary>
        /// Sometimes returned as an error state
        /// </summary>
        Invalid = -1,
        /// <summary>
        /// This is the default timing segment
        /// </summary>
        Update,
        /// <summary>
        /// This is primarily used for physics calculations
        /// </summary>
        FixedUpdate,
        /// <summary>
        /// This is run immediately after update
        /// </summary>
        LateUpdate,
        /// <summary>
        /// This executes, by default, about as quickly as the eye can detect changes in a text field
        /// </summary>
        SlowUpdate
    }

    /// <summary>
    /// How much debug info should be sent to the Unity profiler. NOTE: Setting this to anything above none shows up in the profiler as a 
    /// decrease in performance and a memory alloc. Those effects do not translate onto device.
    /// </summary>
    public enum DebugInfoType
    {
        /// <summary>
        /// None coroutines will be separated in the Unity profiler
        /// </summary>
        None,
        /// <summary>
        /// The Unity profiler will identify each coroutine individually
        /// </summary>
        SeperateCoroutines,
        /// <summary>
        /// Coroutines will be separated and any tags or layers will be identified
        /// </summary>
        SeperateTags
    }

    /// <summary>
    /// A handle for a MEC coroutine.
    /// </summary>
    public struct CoroutineHandle : System.IEquatable<CoroutineHandle>
    {
        private const byte ReservedSpace = 0x0F;
        private static readonly int[] NextIndex = { ReservedSpace + 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private readonly int _id;

        public byte Key { get { return (byte)(this._id & ReservedSpace); } }

        public CoroutineHandle(byte ind)
        {
            if (ind > ReservedSpace)
                ind -= ReservedSpace;

            this._id = NextIndex[ind] + ind;
            NextIndex[ind] += ReservedSpace + 1;
        }

        public bool Equals(CoroutineHandle other)
        {
            return this._id == other._id;
        }

        public override bool Equals(object other)
        {
            if (other is CoroutineHandle)
                return Equals((CoroutineHandle)other);
            return false;
        }

        public static bool operator ==(CoroutineHandle a, CoroutineHandle b)
        {
            return a._id == b._id;
        }

        public static bool operator !=(CoroutineHandle a, CoroutineHandle b)
        {
            return a._id != b._id;
        }

        public override int GetHashCode()
        {
            return this._id;
        }

        /// <summary>
        /// Is true if this handle may have been a valid handle at some point. (i.e. is not an uninitialized handle, error handle, or a key to a coroutine lock)
        /// </summary>
        public bool IsValid
        {
            get { return Key != 0; }
        }
    }
}

public static class MECExtensionMethods
{
    /// <summary>
    /// Cancels this coroutine when the supplied game object is destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject">The GameObject to test.</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject)
    {
        while (BW.Coroutine.Timing.MainThread != System.Threading.Thread.CurrentThread || (gameObject && gameObject.activeInHierarchy && coroutine.MoveNext()))
            yield return coroutine.Current;
    }

    /// <summary>
    /// Cancels this coroutine when the supplied game objects are destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject1">The first GameObject to test.</param>
    /// <param name="gameObject2">The second GameObject to test</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject1, GameObject gameObject2)
    {
        while (BW.Coroutine.Timing.MainThread != System.Threading.Thread.CurrentThread || (gameObject1 && gameObject1.activeInHierarchy &&
                gameObject2 && gameObject2.activeInHierarchy && coroutine.MoveNext()))
            yield return coroutine.Current;
    }

    /// <summary>
    /// Cancels this coroutine when the supplied game objects are destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject1">The first GameObject to test.</param>
    /// <param name="gameObject2">The second GameObject to test</param>
    /// <param name="gameObject3">The third GameObject to test.</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine,
        GameObject gameObject1, GameObject gameObject2, GameObject gameObject3)
    {
        while (BW.Coroutine.Timing.MainThread != System.Threading.Thread.CurrentThread || (gameObject1 && gameObject1.activeInHierarchy &&
                gameObject2 && gameObject2.activeInHierarchy && gameObject3 && gameObject3.activeInHierarchy && coroutine.MoveNext()))
            yield return coroutine.Current;
    }
}
