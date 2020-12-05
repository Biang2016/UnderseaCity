using System.Collections.Generic;
using System.IO;
using System.Linq;
using BiangStudio;
using BiangStudio.Singleton;
using Newtonsoft.Json;
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

            string json = JsonConvert.SerializeObject(TypeIndexDict, Formatting.Indented);
            StreamWriter sw = new StreamWriter(TypeNamesConfig_File);
            sw.Write(json);
            sw.Close();
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
                TypeIndexDict = JsonConvert.DeserializeObject<Dictionary<string, ushort>>(content);
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
    public static int EDIT_AREA_HALF_SIZE = 9;
    public static int EDIT_AREA_FULL_SIZE => EDIT_AREA_HALF_SIZE * 2 + 1;
    public static bool ShowMechaEditorAreaGridPosText = true;

    #region Building占位配置

    public static string BuildingOriginalOccupiedGridInfoJsonFilePath => $"{BuildingOriginalOccupiedGridInfoJsonFileFolder}/BuildingOriginalOccupiedGridInfo.json";
    public static string BuildingOriginalOccupiedGridInfoJsonFileFolder = Application.streamingAssetsPath + "/Configs/BuildingPrefabConfigs";
    public static Dictionary<string, BuildingOriginalOccupiedGridInfo> BuildingOriginalOccupiedGridInfoDict = new Dictionary<string, BuildingOriginalOccupiedGridInfo>();

    private void LoadBuildingOccupiedGridPosDict()
    {
        StreamReader sr = new StreamReader(BuildingOriginalOccupiedGridInfoJsonFilePath);
        string content = sr.ReadToEnd();
        sr.Close();
        BuildingOriginalOccupiedGridInfoDict = JsonConvert.DeserializeObject<Dictionary<string, BuildingOriginalOccupiedGridInfo>>(content);
    }

    #endregion

    public static string AllBuildingConfigPath_Relative = "Configs/BuildingConfigs/AllBuildingConfig.asset";

    public static string AllBuildingConfigPath_Build = Application.streamingAssetsPath + "/" + AllBuildingConfigPath_Relative;

    [ShowInInspector]
    [LabelText("FX类型表")]
    public static readonly TypeDefineConfig<FX> FXTypeDefineDict = new TypeDefineConfig<FX>("FX", "/Resources/Prefabs/FX", true);

    [ShowInInspector]
    [LabelText("建筑配置表")]
    public static readonly Dictionary<string, BuildingConfig> BuildingConfigDict = new Dictionary<string, BuildingConfig>();

    public static string DesignRoot = "/Designs/";

    public override void Awake()
    {
        LoadBuildingOccupiedGridPosDict();
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
        Building.SerializeBuildingOccupiedPositions();
        ExportBuildingConfig(dataFormat);
        LoadBuildingConfig(dataFormat);
        AssetDatabase.Refresh();
        LoadAllConfigs();
    }
    private static void ExportBuildingConfig(DataFormat dataFormat)
    {
        string path = AllBuildingConfigPath_Build;
        FileInfo fi = new FileInfo(path);
        if (fi.Exists) fi.Delete();

        FileInfo configFileInfo = new FileInfo(Application.dataPath + DesignRoot + AllBuildingConfigPath_Relative);
        string relativePath = CommonUtils.ConvertAbsolutePathToProjectPath(configFileInfo.FullName);
        Object configObj = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
        AllBuildingConfigSSO configSSO = (AllBuildingConfigSSO)configObj;
        configSSO.RefreshConfigList();
        byte[] bytes = SerializationUtility.SerializeValue(configSSO.BuildingConfigList, dataFormat);
        File.WriteAllBytes(path, bytes);
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
        LoadBuildingConfig(dataFormat);
        IsLoaded = true;
    }

    private static void LoadBuildingConfig(DataFormat dataFormat)
    {
        BuildingConfigDict.Clear();

        FileInfo fi = new FileInfo(AllBuildingConfigPath_Build);
        if (fi.Exists)
        {
            byte[] bytes = File.ReadAllBytes(fi.FullName);
            List<BuildingConfig> configs = SerializationUtility.DeserializeValue<List<BuildingConfig>>(bytes, dataFormat);
            foreach (BuildingConfig config in configs)
            {
                if (BuildingConfigDict.ContainsKey(config.BuildingKey))
                {
                    Debug.LogError($"建筑配置重名:{config.BuildingKey}");
                }
                else
                {
                    BuildingConfigDict.Add(config.BuildingKey, config);
                }
            }
        }
        else
        {
            Debug.LogError("建筑配置表不存在");
        }
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