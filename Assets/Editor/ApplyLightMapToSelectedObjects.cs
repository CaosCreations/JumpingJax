using UnityEngine;
using UnityEditor;

public class ApplyLightMapToSelectedObjects : EditorWindow
{
    [SerializeField] public GameObject ReferenceObject;
    [SerializeField] public string LightMapParameterName = "Lightmap Parameters name";

    [MenuItem("Tools/CaosCreations/Apply Light Map To Selected Objects")]
    private static void CreateApplyLightMapToSelectedObjects()
    {
        GetWindow<ApplyLightMapToSelectedObjects>();
    }

    [System.Obsolete]
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        ReferenceObject = (GameObject)EditorGUILayout.ObjectField("Light Map Reference Game Object", ReferenceObject, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        GUILayout.Label(
            "This tool cannot properly retrieve the name of the\n" +
            "reference object's Lightmap Parameters.\n" +
            "If lightmaps are used please provide that name here."
            );
        LightMapParameterName = EditorGUILayout.TextField(LightMapParameterName);

        if (GUILayout.Button("Apply Lightmap Settings"))
        {
            var referenceParams = ReferenceObject.GetComponentInChildren<MeshRenderer>();

            foreach (GameObject selected in Selection.gameObjects)
            {
                var selectedLightmap = selected.GetComponentInChildren<MeshRenderer>();
                ApplyLightMapSettings(ref selectedLightmap, ref referenceParams);
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }

    public void SetLightMapParameters(LightmapParameters lmp, MeshRenderer copyTo)
    {
        SerializedObject so = new SerializedObject(copyTo);
        SerializedProperty sp = so.FindProperty("m_LightmapParameters");
        sp.objectReferenceValue = lmp;
        so.ApplyModifiedProperties();
    }

    public void GetLightMapParameters(LightmapParameters LightmapParams, MeshRenderer copyFrom)
    {
        SerializedObject serialo = new SerializedObject(copyFrom);
        SerializedProperty sp = serialo.FindProperty("m_LightmapParameters");
        LightmapParams = (LightmapParameters) sp.objectReferenceValue;
    }

    void ApplyLightMapSettings(ref MeshRenderer copyTo, ref MeshRenderer copyFrom)
    {
        //Global Illumination is only settable from StaticEditorFlags
        StaticEditorFlags fromStaticVars = GameObjectUtility.GetStaticEditorFlags(copyFrom.gameObject);
        if (fromStaticVars.HasFlag(StaticEditorFlags.ContributeGI))
        {
            GameObjectUtility.SetStaticEditorFlags(copyTo.gameObject, StaticEditorFlags.ContributeGI);
        }
        copyTo.enabled = copyFrom.enabled;
        copyTo.forceRenderingOff = copyFrom.forceRenderingOff;
        copyTo.receiveShadows = copyFrom.receiveShadows;
        copyTo.receiveGI = copyFrom.receiveGI;
        //LightmapParams is not an exposed parameter, so if we're using them I need to access the serialized object
        if (copyTo.receiveGI == ReceiveGI.Lightmaps)
        {
            LightmapParameters lmp = new LightmapParameters();
            GetLightMapParameters(lmp, copyFrom);
            lmp.name = LightMapParameterName;
            SetLightMapParameters(lmp, copyTo);
        }
        copyTo.allowOcclusionWhenDynamic = copyFrom.allowOcclusionWhenDynamic;
        copyTo.stitchLightmapSeams = copyFrom.stitchLightmapSeams;
        copyTo.lightProbeUsage = copyFrom.lightProbeUsage;
        copyTo.rayTracingMode = copyFrom.rayTracingMode;
        copyTo.lightProbeProxyVolumeOverride = copyFrom.lightProbeProxyVolumeOverride;
        copyTo.probeAnchor = copyFrom.probeAnchor;
        copyTo.lightmapIndex = copyFrom.lightmapIndex;
        copyTo.realtimeLightmapIndex = copyFrom.realtimeLightmapIndex;
        copyTo.lightmapScaleOffset = copyFrom.lightmapScaleOffset;
        copyTo.realtimeLightmapScaleOffset = copyFrom.realtimeLightmapScaleOffset;
        copyTo.renderingLayerMask = copyFrom.renderingLayerMask;
        copyTo.reflectionProbeUsage = copyFrom.reflectionProbeUsage;
        copyTo.shadowCastingMode = copyFrom.shadowCastingMode;
        copyTo.scaleInLightmap = copyFrom.scaleInLightmap;
    }
        
}