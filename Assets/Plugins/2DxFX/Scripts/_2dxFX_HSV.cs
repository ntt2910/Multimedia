using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

[ExecuteInEditMode]
[AddComponentMenu("2DxFX/Standard/HSV")]
[System.Serializable]
public class _2dxFX_HSV : MonoBehaviour
{
    [HideInInspector] public Material ForceMaterial;
    [HideInInspector] public bool ActiveChange = true;
    private string shader = "2DxFX/Standard/HSV";
    [HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

    [HideInInspector] [Range(0, 360)] public float _HueShift = 180f;
    [HideInInspector] [Range(-2, 2)] public float _Saturation = 1f;
    [HideInInspector] [Range(-2, 2)] public float _ValueBrightness = 1f;

    [HideInInspector] public int ShaderChange;
    private Material tempMaterial;
    private Material defaultMaterial;
    private Image CanvasImage;
    private bool _useImage;
    private SpriteRenderer _cacheRender;

    private void Awake() {
        if (gameObject.GetComponent<Image>() != null) {
            _useImage = true;
            CanvasImage = gameObject.GetComponent<Image>();
        }
        else {
            _cacheRender = gameObject.GetComponent<SpriteRenderer>();
        }
    }

    private void Start() {
        ShaderChange = 0;
    }

    private void Update() {
        CallUpdate();
    }

    public void CallUpdate() {
        if ((ShaderChange == 0) && ForceMaterial != null) {
            ShaderChange = 1;
            if (tempMaterial != null)
                DestroyImmediate(tempMaterial);
            if (_useImage) {
                CanvasImage.material = ForceMaterial;
            }
            else {
                _cacheRender.sharedMaterial = ForceMaterial;
            }

            ForceMaterial.hideFlags = HideFlags.None;
            ForceMaterial.shader = Shader.Find(shader);
        }

        if (ForceMaterial == null && ShaderChange == 1) {
            if (tempMaterial != null)
                DestroyImmediate(tempMaterial);
            tempMaterial = new Material(Shader.Find(shader)) {hideFlags = HideFlags.None};
            if (_useImage) {
                CanvasImage.material = tempMaterial;
            }
            else {
                _cacheRender.sharedMaterial = tempMaterial;
            }

            ShaderChange = 0;
        }

    #if UNITY_EDITOR
        string dfname = "";
        if (gameObject.GetComponent<SpriteRenderer>() != null)
            dfname = GetComponent<Renderer>().sharedMaterial.shader.name;
        if (gameObject.GetComponent<Image>() != null) {
            Image img = gameObject.GetComponent<Image>();
            if (img.material == null) dfname = "Sprites/Default";
        }

        if (dfname == "Sprites/Default") {
            ForceMaterial.shader = Shader.Find(shader);
            ForceMaterial.hideFlags = HideFlags.None;
            if (gameObject.GetComponent<SpriteRenderer>() != null) {
                GetComponent<Renderer>().sharedMaterial = ForceMaterial;
            }
            else if (gameObject.GetComponent<Image>() != null) {
                Image img = gameObject.GetComponent<Image>();
                if (img.material == null) {
                    CanvasImage.material = ForceMaterial;
                }
            }
        }
    #endif
        if (ActiveChange) {
            if (_useImage) {
                CanvasImage.material.SetFloat("_Alpha", 1 - _Alpha);
                CanvasImage.material.SetFloat("_HueShift", _HueShift);
                CanvasImage.material.SetFloat("_Sat", _Saturation);
                CanvasImage.material.SetFloat("_Val", _ValueBrightness);
            }
            else {
                _cacheRender.sharedMaterial.SetFloat("_Alpha", 1 - _Alpha);
                _cacheRender.sharedMaterial.SetFloat("_HueShift", _HueShift);
                _cacheRender.sharedMaterial.SetFloat("_Sat", _Saturation);
                _cacheRender.sharedMaterial.SetFloat("_Val", _ValueBrightness);
            }
        }
    }

    private void OnDestroy() {
        if (gameObject.GetComponent<Image>() != null) {
            if (CanvasImage == null) CanvasImage = gameObject.GetComponent<Image>();
        }

        if (Application.isPlaying == false && Application.isEditor) {
            if (tempMaterial != null) DestroyImmediate(tempMaterial);

            if (gameObject.activeSelf && defaultMaterial != null) {
                if (gameObject.GetComponent<SpriteRenderer>() != null) {
                    GetComponent<Renderer>().sharedMaterial = defaultMaterial;
                    GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
                }
                else if (gameObject.GetComponent<Image>() != null) {
                    CanvasImage.material = defaultMaterial;
                    CanvasImage.material.hideFlags = HideFlags.None;
                }
            }
        }
    }

    private void OnDisable() {
        if (gameObject.GetComponent<Image>() != null) {
            if (CanvasImage == null) CanvasImage = gameObject.GetComponent<Image>();
        }

        if (gameObject.activeSelf && defaultMaterial != null) {
            if (gameObject.GetComponent<SpriteRenderer>() != null) {
                GetComponent<Renderer>().sharedMaterial = defaultMaterial;
                GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
            }
            else if (gameObject.GetComponent<Image>() != null) {
                CanvasImage.material = defaultMaterial;
                CanvasImage.material.hideFlags = HideFlags.None;
            }
        }
    }

    private void OnEnable() {
        if (gameObject.GetComponent<Image>() != null) {
            if (CanvasImage == null) CanvasImage = gameObject.GetComponent<Image>();
        }

        if (defaultMaterial == null) {
            defaultMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        if (ForceMaterial == null) {
            ActiveChange = true;
            tempMaterial = new Material(Shader.Find(shader)) {hideFlags = HideFlags.None};
            if (gameObject.GetComponent<SpriteRenderer>() != null) {
                GetComponent<Renderer>().sharedMaterial = tempMaterial;
            }
            else if (gameObject.GetComponent<Image>() != null) {
                CanvasImage.material = tempMaterial;
            }
        }
        else {
            ForceMaterial.shader = Shader.Find(shader);
            ForceMaterial.hideFlags = HideFlags.None;
            if (gameObject.GetComponent<SpriteRenderer>() != null) {
                GetComponent<Renderer>().sharedMaterial = ForceMaterial;
            }
            else if (gameObject.GetComponent<Image>() != null) {
                CanvasImage.material = ForceMaterial;
            }
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_HSV)), CanEditMultipleObjects]
public class _2dxFX_HSV_Editor : Editor
{
    private SerializedObject m_object;

    public void OnEnable() {
        m_object = new SerializedObject(targets);
    }

    public override void OnInspectorGUI() {
        m_object.Update();
        DrawDefaultInspector();

        _2dxFX_HSV _2dxScript = (_2dxFX_HSV) target;

        Texture2D icon = Resources.Load("2dxfxinspector") as Texture2D;
        if (icon) {
            float ih = icon.height;
            float iw = icon.width;
            float result = ih / iw;
            float w = Screen.width;
            result = result * w;
            var r = GUILayoutUtility.GetRect(ih, result);
            EditorGUI.DrawTextureTransparent(r, icon);
        }

        EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"),
            new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));

        if (_2dxScript.ForceMaterial == null) {
            _2dxScript.ActiveChange = true;
        }
        else {
            if (GUILayout.Button("Remove Shared Material")) {
                _2dxScript.ForceMaterial = null;
                _2dxScript.ShaderChange = 1;
                _2dxScript.ActiveChange = true;
                _2dxScript.CallUpdate();
            }

            EditorGUILayout.PropertyField(m_object.FindProperty("ActiveChange"),
                new GUIContent("Change Material Property", "Change The Material Property"));
        }

        if (_2dxScript.ActiveChange) {
            EditorGUILayout.BeginVertical("Box");

            Texture2D icone = Resources.Load("2dxfx-icon-color") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_HueShift"),
                new GUIContent("Hue Shift", icone, "Change hue colors"));
            icone = Resources.Load("2dxfx-icon-contrast") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Saturation"),
                new GUIContent("Color Saturation", icone, "Change the saturation"));
            icone = Resources.Load("2dxfx-icon-brightness") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_ValueBrightness"),
                new GUIContent("Brighntess", icone, "Change the brightness"));

            EditorGUILayout.BeginVertical("Box");


            icone = Resources.Load("2dxfx-icon-fade") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"),
                new GUIContent("Fading", icone, "Fade from nothing to showing"));

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        m_object.ApplyModifiedProperties();
    }
}
#endif