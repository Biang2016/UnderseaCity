using System.Collections.Generic;
using System.IO;
using System.Linq;
using BiangStudio;
using BiangStudio.Singleton;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class ConfigManager : TSingletonBaseManager<ConfigManager>
{
    public enum GUID_Separator
    {
        BuildingInfo = 2000,
    }

    public class TypeDefineConfig<T> where T : Object
    {
        public TypeDefineConfig(string typeNamePrefix, string prefabFolder, bool includeSubFolder)
        {
            TypeNamePrefix = typeNamePrefix;
            PrefabFolder_Relative = prefabFolder;
            IncludeSubFolder = includeSubFolder;
        }

        private string TypeNamePrefix;
        private string TypeNamesConfig_File => $"{TypeNamesConfigFolder_Build}/{TypeNamePrefix}Names.config";
        private string TypeNamesConfigFolder_Build => Application.streamingAssetsPath + $"/Configs/{TypeNamePrefix}";
        private string PrefabFolder_Relative;
        private bool IncludeSubFolder;

        public Dictionary<string, ushort> TypeIndexDict = new Dictionary<string, ushort>();
        public SortedDictionary<ushort, string> TypeNameDict = new SortedDictionary<ushort, string>();

#if UNITY_EDITOR
        public void ExportTypeNames()
        {
            TypeNameDict.Clear();
            TypeIndexDict.Clear();
            string folder = TypeNamesConfigFolder_Build;
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);

            ushort index = 1;
            DirectoryInfo di = new DirectoryInfo(Application.dataPath + PrefabFolder_Relative);

            if (CommonUtils.IsBaseType(typeof(T), typeof(MonoBehaviour)))
            {
                foreach (FileInfo fi in di.GetFiles("*.prefab", IncludeSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    if (index == ushort.MaxValue)
                    {
                        Debug.LogError($"{typeof(T).Name}类型数量超过{ushort.MaxValue}");
                        break;
                    }

                    string relativePath = CommonUtils.ConvertAbsolutePathToProjectPath(fi.FullName);
                    GameObject obj = (GameObject) AssetDatabase.LoadAssetAtPath<Object>(relativePath);
                    T t = obj.GetComponent<T>();
                    if (t != null)
                    {
                        TypeNameDict.Add(index, t.name);
                        TypeIndexDict.Add(t.name, index);
                        index++;
                    }
                    else
                    {
                        Debug.LogError($"Prefab {fi.Name} 不含{typeof(T).Name}脚本，已跳过");
                    }
                }
            }
            else
            {
                foreach (FileInfo fi in di.GetFiles("*.*", IncludeSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    if (fi.Name.EndsWith(".meta")) continue;
                    if (index == ushort.MaxValue)
                    {
                        Debug.LogError($"{typeof(T).Name}类型数量超过{ushort.MaxValue}");
                        break;
                    }

                    string relativePath = CommonUtils.ConvertAbsolutePathToProjectPath(fi.FullName);
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
                    T t = (T) obj;
                    if (t != null)
                    {
                        TypeNameDict.Add(index, t.name);
                        TypeIndexDict.Add(t.name, index);
                        index++;
                    }
                    else
                    {
                        Debug.LogError($"文件 {fi.Name} 不是{typeof(T).Name}类型，已跳过");
                    }
                }
            }

            //string json = JsonConvert.SerializeObject(TypeIndexDict, Formatting.Indented);
            //StreamWriter sw = new StreamWriter(TypeNamesConfig_File);
            //sw.Write(json);
            //sw.Close();
        }
#endif

        public void LoadTypeNames()
        {
            FileInfo fi = new FileInfo(TypeNamesConfig_File);
            if (fi.Exists)
            {
                StreamReader sr = new StreamReader(TypeNamesConfig_File);
                string content = sr.ReadToEnd();
                sr.Close();
                TypeIndexDict.Clear();
                TypeNameDict.Clear();
                //TypeIndexDict = JsonConvert.DeserializeObject<Dictionary<string, ushort>>(content);
                foreach (KeyValuePair<string, ushort> kv in TypeIndexDict)
                {
                    TypeNameDict.Add(kv.Value, kv.Key);
                }
            }
        }

        public void Clear()
        {
            TypeIndexDict.Clear();
            TypeNameDict.Clear();
        }
    }

    public static int GRID_SIZE = 1;

    [ShowInInspector]
    [LabelText("FX类型表")]
    public static readonly TypeDefineConfig<FX> FXTypeDefineDict = new TypeDefineConfig<FX>("FX", "/Resources/Prefabs/FX", true);

    public static string DesignRoot = "/Designs/";

    public override void Awake()
    {
        LoadAllConfigs();
    }

    #region Export

#if UNITY_EDITOR
    [MenuItem("开发工具/配置/序列化配置")]
    public static void ExportConfigs()
    {
        // http://www.sirenix.net/odininspector/faq?Search=&t-11=on#faq

        DataFormat dataFormat = DataFormat.Binary;
        FXTypeDefineDict.ExportTypeNames();
        AssetDatabase.Refresh();
        LoadAllConfigs();
    }
#endif

    #endregion

    #region Load

    public static bool IsLoaded = false;

#if UNITY_EDITOR
    [MenuItem("开发工具/配置/加载配置")]
#endif
    public static void LoadAllConfigs()
    {
        if (IsLoaded) return;
        DataFormat dataFormat = DataFormat.Binary;
        FXTypeDefineDict.LoadTypeNames();
        IsLoaded = true;
    }

    #endregion

    #region Getter

    // -------- Get All Type Names --------

    public static IEnumerable<string> GetAllFXTypeNames()
    {
        LoadAllConfigs();
        List<string> res = FXTypeDefineDict.TypeIndexDict.Keys.ToList();
        res.Insert(0, "None");
        return res;
    }

    // -------- Get All Type Names --------

    public static string GetFXName(ushort fxTypeIndex)
    {
        if (!IsLoaded) LoadAllConfigs();
        FXTypeDefineDict.TypeNameDict.TryGetValue(fxTypeIndex, out string fxTypeName);
        return fxTypeName;
    }

    public static ushort GetFXTypeIndex(string fxTypeName)
    {
        if (!IsLoaded) LoadAllConfigs();
        FXTypeDefineDict.TypeIndexDict.TryGetValue(fxTypeName, out ushort fxTypeIndex);
        return fxTypeIndex;
    }

    #endregion
}