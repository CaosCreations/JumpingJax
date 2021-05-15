#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class AutoSave : EditorWindow
{
    private static readonly GUIStyle GuiStyleLabel = new GUIStyle();

    private static bool _autoSave;
    private static bool _saveOnPlay;
    private static bool _saveAssets;
    private static bool _debugLog;
    private static bool _backup;
    private static bool _showBackup;

    private static DateTime _lastAutoSave = DateTime.Now;

    private static Texture2D _pmLogo;
    private static Texture2D _asOnOff;
    private static Texture2D _asEnable;
    private static Texture2D _asDisable;
    private static Texture2D _asInfo;
    private static Texture2D _asOnplay;
    private static Texture2D _asTime;
    private static Texture2D _asAsset;
    private static Texture2D _asBackup;

    private static int _saveInterval = 5;
    private static int _saveIntervalSlider = 5;

    private static string _path;
    private static string _backupPath;
    private static string _currentBackupPath;
    private static string _backupUserName;
    private static int _backupCount;

    private static Texture2D AsAsset
    {
        get
        {
            if (_asAsset != null)
                return _asAsset;

            return _asAsset = AssetDatabase.LoadAssetAtPath(Path + "asset.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsDisable
    {
        get
        {
            if (_asDisable != null)
                return _asDisable;

            return _asDisable = AssetDatabase.LoadAssetAtPath(Path + "disable.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsEnable
    {
        get
        {
            if (_asEnable != null)
                return _asEnable;

            return _asEnable = AssetDatabase.LoadAssetAtPath(Path + "enable.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsInfo
    {
        get
        {
            if (_asInfo != null)
                return _asInfo;

            return _asInfo = AssetDatabase.LoadAssetAtPath(Path + "info.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsOnOff
    {
        get
        {
            if (_asOnOff != null)
                return _asOnOff;

            return _asOnOff = AssetDatabase.LoadAssetAtPath(Path + "onoff.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsOnPlay
    {
        get
        {
            if (_asOnplay != null)
                return _asOnplay;

            return _asOnplay = AssetDatabase.LoadAssetAtPath(Path + "play.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsTime
    {
        get
        {
            if (_asTime != null)
                return _asTime;

            return _asTime = AssetDatabase.LoadAssetAtPath(Path + "time.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D AsBackup
    {
        get
        {
            if (_asBackup != null)
                return _asBackup;

            return _asBackup = AssetDatabase.LoadAssetAtPath(Path + "backup.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static Texture2D PmLogo
    {
        get
        {
            if (_pmLogo != null)
                return _pmLogo;

            return _pmLogo = AssetDatabase.LoadAssetAtPath(Path + "logo.png", typeof(Texture2D)) as Texture2D;
        }
    }

    private static string BackupPath
    {
        get
        {
            if (!string.IsNullOrEmpty(_backupPath)) return _backupPath;

            var res = Directory.GetFiles(Application.dataPath, "AutoSave.cs", SearchOption.AllDirectories);
            var backupPath = res[0].Replace("\\", "/");
            backupPath = backupPath.Replace(Application.dataPath, "");
            backupPath = backupPath.Substring(1);
            _backupPath = Directory.GetParent(Directory.GetParent(backupPath).ToString()) + "/Backup/";

            return _backupPath;
        }
    }

    private static int BackupCount
    {
        get
        {
            _backupCount = EditorPrefs.GetInt("PM_AS_BACKUPCOUNT");
            return _backupCount;
        }
    }

    private static string Path
    {
        get
        {
            if (!string.IsNullOrEmpty(_path)) return _path;

            var res = Directory.GetFiles(Application.dataPath, "AutoSave.cs", SearchOption.AllDirectories);
            var assetPath = res[0].Replace("\\", "/");
            assetPath = "Assets" + assetPath.Replace(Application.dataPath, "");
            _path = Directory.GetParent(Directory.GetParent(assetPath).ToString()) + "/Textures/";

            return _path;
        }
    }

    private static void AutoSaveOff()
    {
        EditorApplication.update -= EditorUpdate;
        EditorApplication.playModeStateChanged -= OnEnterInPlayMode;
        _autoSave = false;
        Log(0, "AutoSave - OFF !");
    }

    private static void AutoSaveOn()
    {
        _lastAutoSave = DateTime.Now;
        EditorApplication.update += EditorUpdate;
        EditorApplication.playModeStateChanged += OnEnterInPlayMode;
        _autoSave = true;
        Log(0, "AutoSave - ON !");
    }

    private static void EditorUpdate()
    {
        if (_lastAutoSave.AddMinutes(_saveInterval) > DateTime.Now) return;
        if (!SceneManager.GetActiveScene().isDirty) return;
        Save();
        _lastAutoSave = DateTime.Now;
    }

    private static void LoadSettings()
    {
        _autoSave = EditorPrefs.GetBool("PM_AS_AUTOSAVE", true);
        _saveOnPlay = EditorPrefs.GetBool("PM_AS_SAVEONPLAY", true);
        _saveAssets = EditorPrefs.GetBool("PM_AS_SAVEASSET", true);
        _debugLog = EditorPrefs.GetBool("PM_AS_DEBUGLOG", true);
        _saveIntervalSlider = _saveInterval = EditorPrefs.GetInt("PM_AS_SAVEINTERVAL", 2);
        _backup = EditorPrefs.GetBool("PM_AS_BACKUP", true);
        _backupPath = EditorPrefs.GetString("PM_AS_BACKUPPATH");
        _backupCount = EditorPrefs.GetInt("PM_AS_BACKUPCOUNT");
    }

    private static void OnEnterInPlayMode(PlayModeStateChange state)
    {
        if (_saveOnPlay && !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) Save();
    }

    [MenuItem("Tools/ProjectMakers/AutoSave")]
    private static void OpenWindow()
    {
        GetWindow<AutoSave>("AutoSave");
    }

    private static void Save()
    {
        var activeScene = SceneManager.GetActiveScene();
        var curSceneName = activeScene.name;

        if (_backup)
        {
            var curBackupPath = EditorPrefs.GetString("PM_AS_BACKUPPATH");

            if (!curBackupPath.EndsWith("/"))
            {
                curBackupPath += "/";
            }

            _backupUserName = SystemInfo.deviceName;
            curBackupPath = curBackupPath + _backupUserName + "/";
            var curPath = "Assets/" + curBackupPath + curSceneName;
            CreateBackupFolder(curPath, curBackupPath, curSceneName);

            try
            {
                EditorSceneManager.SaveScene(activeScene, BackupFileName(curPath, curSceneName), true);
                Log(0, "AutoSave - Create a backup for - " + curSceneName + " - Scene!");
                ClearBackupFolder(curPath);
            }
            catch (Exception e)
            {
                Log(2, "AutoSave - Create a !! NO !! backup for - " + curSceneName + " - Scene!");
                Log(2, "Exception: " + e);
                throw;
            }
        }

        try
        {
            EditorSceneManager.SaveScene(activeScene);
            if (_saveAssets) AssetDatabase.SaveAssets();
            Log(0, "AutoSave - " + activeScene.name + " is saved!");
        }
        catch (Exception e)
        {
            Log(2, "AutoSave - " + activeScene.name + " is !! NOT !! saved!");
            Log(2, "Exception: " + e);
            throw;
        }
    }

    private static void ClearBackupFolder(string curpath)
    {
        var fileInfo = new DirectoryInfo(curpath).GetFiles();

        if (fileInfo.Length >= _backupCount * 2)
            foreach (var fi in fileInfo.OrderByDescending(x => x.LastWriteTime).Skip(_backupCount * 2))
                fi.Delete();

        AssetDatabase.Refresh();
    }

    private static string BackupFileName(string curpath, string curSceneName)
    {
        var filename = DateTime.Now.Ticks.ToString();
        filename = filename.Remove(filename.Length - 6);
        filename = filename.Remove(0, 3);
        filename = int.Parse(filename).ToString("#,###");
        filename = filename.Replace(",", ".");
        filename = curpath + "/" + curSceneName + " v." + filename + ".unity";
        return filename;
    }

    private static void CreateBackupFolder(string curpath, string curBackupPath, string curSceneName)
    {
        if (AssetDatabase.IsValidFolder(curpath)) return;

        var splittingPath = curBackupPath.Split('/');

        for (var j = 0; j < splittingPath.Length; j++)
        {
            var proofPath = "Assets/";

            for (var k = 0; k < j; k++)
                proofPath = proofPath + splittingPath[k] + "/";


            if (AssetDatabase.IsValidFolder(proofPath.Substring(0, proofPath.Length - 1))) continue;

            var curParentFolder = Directory.GetParent(Directory.GetParent(proofPath).ToString()).ToString();
            AssetDatabase.CreateFolder(curParentFolder.Substring(0, curParentFolder.Length), splittingPath[j - 1]);
            Log(0, "AutoSave - Create a backup folder: " + curParentFolder + "/" +  splittingPath[j - 1]);
        }

        AssetDatabase.CreateFolder("Assets/" + curBackupPath.Substring(0, curBackupPath.Length - 1), curSceneName);
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetBool("PM_AS_AUTOSAVE", _autoSave);
        EditorPrefs.SetBool("PM_AS_SAVEONPLAY", _saveOnPlay);
        EditorPrefs.SetBool("PM_AS_SAVEASSET", _saveAssets);
        EditorPrefs.SetBool("PM_AS_DEBUGLOG", _debugLog);
        EditorPrefs.SetBool("PM_AS_BACKUP", _backup);
        EditorPrefs.SetInt("PM_AS_SAVEINTERVAL", _saveInterval);
        EditorPrefs.SetString("PM_AS_BACKUPPATH", _backupPath);
        EditorPrefs.SetInt("PM_AS_BACKUPCOUNT", _backupCount);
    }

    private static void Log(int type, string body)
    {
        if (!_debugLog) return;

        switch (type)
        {
            case 1:
                Debug.LogWarning(body);
                break;
            case 2:
                Debug.LogError(body);
                break;
            default:
                Debug.Log(body);
                break;
        }
    }

    private void OnEnable()
    {
        //LoadSettings();
        if (_autoSave) AutoSaveOn();
    }

#if UNITY_2018_3_OR_NEWER
    private class Provider : SettingsProvider
    {
        public Provider(string path, SettingsScope scopes = SettingsScope.User) : base(path, scopes)
        {
        }

        public override void OnGUI(string searchContext)
        {
            Preference();
        }
    }

    [SettingsProvider]
    static SettingsProvider ThisSettingsProvider()
    {
        return new Provider("Preferences/ProjectMakers/AutoSave");
    }
#else
    [PreferenceItem("ProjectMakers/AutoSave")]
#endif

    private void OnGUI()
    {
        Preference();
    }

    private static void CheckChanges()
    {
        if (_autoSave != EditorPrefs.GetBool("PM_AS_AUTOSAVE", true) ||
            _saveOnPlay != EditorPrefs.GetBool("PM_AS_SAVEONPLAY", true) ||
            _debugLog != EditorPrefs.GetBool("PM_AS_DEBUGLOG", true) ||
            _saveInterval != EditorPrefs.GetInt("PM_AS_SAVEINTERVAL", 2) ||
            _backup != EditorPrefs.GetBool("PM_AS_BACKUP", true) ||
            _backupPath != EditorPrefs.GetString("PM_AS_BACKUPPATH") ||
            _backupCount != EditorPrefs.GetInt("PM_AS_BACKUPCOUNT")
        )
        {
            LoadSettings();
        }
    }

    private static void Preference()
    {
        GuiStyleLabel.fontSize = 16;
        GuiStyleLabel.fontStyle = FontStyle.Bold;

        CheckChanges();

        if (_saveInterval != _saveIntervalSlider)
        {
            _saveInterval = _saveIntervalSlider;
            Log(0, "AutoSave - Saveinterval = " + _saveInterval + " min!");

            SaveSettings();
        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(PmLogo);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.Label(_debugLog ? AsEnable : AsDisable, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16));

        GUILayout.EndHorizontal();
        GUILayout.Space(-4);

        if (GUILayout.Button(new GUIContent(AsInfo, "Create Debug.Log"), GUILayout.MaxHeight(28),
            GUILayout.MaxWidth(28)))
        {
            _debugLog = !_debugLog;
            Log(0, "AutoSave - Debug.Log = " + _debugLog + " !");
            SaveSettings();
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.Label(_saveOnPlay ? AsEnable : AsDisable, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16));

        GUILayout.EndHorizontal();
        GUILayout.Space(-4);

        if (GUILayout.Button(new GUIContent(AsOnPlay, "Save on Play"), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28)))
        {
            _saveOnPlay = !_saveOnPlay;
            Log(0, "AutoSave - Save on Play = " + _saveOnPlay + " !");
            SaveSettings();
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.Label(_saveAssets ? AsEnable : AsDisable, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16));

        GUILayout.EndHorizontal();
        GUILayout.Space(-4);

        if (GUILayout.Button(new GUIContent(AsAsset, "Save Assets"), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28)))
        {
            _saveAssets = !_saveAssets;
            Log(0, "AutoSave - Save Assets = " + _saveAssets + " !");
            SaveSettings();
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.Label(_backup ? AsEnable : AsDisable, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16));

        GUILayout.EndHorizontal();
        GUILayout.Space(-4);

        if (GUILayout.Button(new GUIContent(AsBackup, "Save Project in backupfolder"), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28)))
        {
            _backup = !_backup;
            Log(0, "AutoSave - Backup = " + _backup + " !");
            SaveSettings();
        }

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.Label(string.Empty, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16));

        GUILayout.EndHorizontal();
        GUILayout.Space(2);
        GUILayout.BeginHorizontal();

        _saveIntervalSlider = EditorGUILayout.IntSlider(string.Empty, _saveIntervalSlider, 1, 30);

        GUILayout.BeginVertical();
        GUILayout.Space(-4);

        EditorGUILayout.LabelField(new GUIContent(AsTime, "Save interval in minutes"), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28));

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.Label(_autoSave ? AsEnable : AsDisable, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16));

        GUILayout.EndHorizontal();
        GUILayout.Space(-4);

        if (GUILayout.Button(new GUIContent(AsOnOff, "AutoSave ON/OFF"), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28)))
        {
            if (_autoSave) AutoSaveOff();
            else AutoSaveOn();

            SaveSettings();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        GUI.skin.button.normal.textColor = Color.white;
        GUI.skin.button.fontSize = 10;
        GUI.skin.button.fontStyle = FontStyle.Normal;
        GUI.backgroundColor = new Color(0.63f, 0f, 0f);

        if (GUILayout.Button("Save it manually!"))
            Save();

        GUI.skin.button.normal.textColor = Color.black;
        GUI.skin.button.fontSize = 12;
        GUI.skin.button.fontStyle = FontStyle.Normal;
        GUI.backgroundColor = Color.white;

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Space(24);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        var pathSize = GUI.skin.box.CalcSize(new GUIContent(BackupPath));

        EditorGUIUtility.labelWidth = 115;

        EditorPrefs.SetString("PM_AS_BACKUPPATH", EditorGUILayout.TextField("Backup save path: ", BackupPath, GUILayout.MinWidth(EditorGUIUtility.labelWidth + pathSize.x)));

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(24);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        var countSize = GUI.skin.box.CalcSize(new GUIContent(BackupCount.ToString()));

        EditorGUIUtility.labelWidth = 88;

        EditorPrefs.SetInt("PM_AS_BACKUPCOUNT", EditorGUILayout.IntField("Backup count: ", BackupCount, GUILayout.MinWidth(EditorGUIUtility.labelWidth + countSize.x + 8), GUILayout.MaxWidth(EditorGUIUtility.labelWidth + countSize.x + 8)));

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Space(16);
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("ProjectMakers.de",EditorStyles.toolbarButton))
        {
            Application.OpenURL("https://projectmakers.de");
        }

        GUILayout.EndHorizontal();
    }
}
#endif