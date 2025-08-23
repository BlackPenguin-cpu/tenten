using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class StageDataClass
{
    public int stageNum;
    public int chapterNum;
    public BigInteger targetScore;
    public int canPlaceBlock;
}

public class SheetInfo
{
    public string sheetRange;
    public long gid;
}

public class ReadSpreadSheet : MonoBehaviour
{
    public static ReadSpreadSheet instance;

    public List<StageDataClass> sheetDatas;
    [FormerlySerializedAs("loadComplete")] public bool isLoadComplete = false;

    private readonly string ADDRESS =
        "https://docs.google.com/spreadsheets/d/1nINbpc53B_zdKAHt_voVsnWlmk6fd8t3qSwnZ5uZV5Q";

    private readonly SheetInfo stageDataSheetData = new SheetInfo()
    {
        sheetRange = "A2:D",
        gid = 0,
    };

    public static string GetTSVAddress(string address, string sheetRange, long gid)
    {
        return $"{address}/export?format=tsv&range={sheetRange}&gid={gid}";
    }

    private T GetData<T>(string[] datas)
    {
        object data = Activator.CreateInstance(typeof(T));

        // 클래스에 있는 변수들을 순서대로 저장한 배열
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < fields.Length; i++)
        {
            try
            {
                // string > parse
                Type type = fields[i].FieldType;

                if (string.IsNullOrEmpty(datas[i])) continue;

                // 변수에 맞는 자료형으로 파싱해서 넣는다
                if (type == typeof(int))
                    fields[i].SetValue(data, int.Parse(datas[i]));

                else if (type == typeof(float))
                    fields[i].SetValue(data, float.Parse(datas[i]));

                else if (type == typeof(bool))
                    fields[i].SetValue(data, bool.Parse(datas[i]));

                else if (type == typeof(string))
                    fields[i].SetValue(data, datas[i]);
                
                else if (type == typeof(BigInteger))
                    fields[i].SetValue(data, BigInteger.Parse(datas[i]));

                // enum
                else
                    fields[i].SetValue(data, Enum.Parse(type, datas[i]));
            }
            catch (Exception e)
            {
                Debug.LogError($"SpreadSheet Error : {e.Message}");
            }
        }

        return (T)data;
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public async UniTask LoadData()
    {
        //main
        UnityWebRequest www =
            UnityWebRequest.Get(GetTSVAddress(ADDRESS, stageDataSheetData.sheetRange, stageDataSheetData.gid));
        await www.SendWebRequest();

        var dataList = new List<StageDataClass>();

        foreach (var data in www.downloadHandler.text.Split('\n'))
        {
            var sheetData = GetData<StageDataClass>(data.Split('\t'));
            dataList.Add(sheetData);
        }

        sheetDatas = dataList;

        Debug.Log("Load Complete");
        isLoadComplete = true;
    }
}