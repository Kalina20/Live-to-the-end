using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class QuestionsImporter : EditorWindow
{
    private TextAsset tsvFile;

    [MenuItem("Tools/Импорт карточек из Excel (TSV)")]
    public static void ShowWindow()
    {
        GetWindow<QuestionsImporter>("Импорт карточек");
    }

    private void OnGUI()
    {
        GUILayout.Label("Генератор Scriptable Objects", EditorStyles.boldLabel);
        
        tsvFile = (TextAsset)EditorGUILayout.ObjectField("Текстовый файл (TSV)", tsvFile, typeof(TextAsset), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Сгенерировать вопросы", GUILayout.Height(40)))
        {
            if (tsvFile != null)
            {
                ImportData();
            }
            else
            {
                Debug.LogError("Сначала выберите текстовый файл с таблицей!");
            }
        }
    }

    private void ImportData()
    {
        string[] lines = tsvFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        // Словарь: Ключ = "Месяц_ТипСтека", Значение = Список вопросов для этого стека
        Dictionary<string, List<QuestionData>> parsedQuestions = new Dictionary<string, List<QuestionData>>();

        for (int i = 1; i < lines.Length; i++) // Пропускаем первую строку (шапку)
        {
            string[] cols = lines[i].Split('\t'); // Разделяем по табуляции
            if (cols.Length < 11) continue;

            string monthEn = TranslateMonth(cols[0]);
            int stackNumber = ParseInt(cols[1]);
            
            QuestionData q = new QuestionData();
            q.questionText = cols[2].Trim();
            
            q.answers = new AnswerData[2];
            q.answers[0] = new AnswerData { 
                answerText = cols[3].Trim(), 
                moneyChange = ParseInt(cols[4]), friendshipChange = ParseInt(cols[5]), knowledgeChange = ParseInt(cols[6]) 
            };
            q.answers[1] = new AnswerData { 
                answerText = cols[7].Trim(), 
                moneyChange = ParseInt(cols[8]), friendshipChange = ParseInt(cols[9]), knowledgeChange = ParseInt(cols[10]) 
            };

            if (cols.Length >= 14)
            {
                q.isCheckEvent = ParseBool(cols[11]);
                q.statToCheck = ParseStatType(cols[12]);
                q.checkThreshold = ParseInt(cols[13]);
            }

            string dictKey = $"{monthEn}_{stackNumber}";
            if (!parsedQuestions.ContainsKey(dictKey))
            {
                parsedQuestions[dictKey] = new List<QuestionData>();
            }
            parsedQuestions[dictKey].Add(q);
        }

        CreateScriptableObjects(parsedQuestions);
        Debug.Log("Импорт успешно завершен!");
    }

    private void CreateScriptableObjects(Dictionary<string, List<QuestionData>> data)
    {
        string basePath = "Assets/GameData/Questions";
        if (!AssetDatabase.IsValidFolder(basePath)) AssetDatabase.CreateFolder("Assets/GameData", "Questions");

        foreach (var kvp in data)
        {
            string[] keys = kvp.Key.Split('_');
            string month = keys[0];
            int stack = int.Parse(keys[1]);

            string monthPath = $"{basePath}/{month}";
            if (!AssetDatabase.IsValidFolder(monthPath)) AssetDatabase.CreateFolder(basePath, month);

            string assetPath = $"{monthPath}/{month}_Stack_{stack}.asset";
            
            QuestionSet qSet = AssetDatabase.LoadAssetAtPath<QuestionSet>(assetPath);
            if (qSet == null)
            {
                qSet = ScriptableObject.CreateInstance<QuestionSet>();
                AssetDatabase.CreateAsset(qSet, assetPath);
            }

            qSet.monthName = month;
            qSet.stackNumber = stack;
            
            qSet.questions = new QuestionData[4];
            for (int i = 0; i < 4; i++)
            {
                if (i < kvp.Value.Count)
                    qSet.questions[i] = kvp.Value[i];
                else
                    qSet.questions[i] = new QuestionData(); 
            }

            EditorUtility.SetDirty(qSet);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private int ParseInt(string val)
    {
        if (string.IsNullOrWhiteSpace(val)) return 0;
        val = val.Replace("+", "").Trim();
        if (int.TryParse(val, out int res)) return res;
        return 0;
    }

    private bool ParseBool(string val)
    {
        if (string.IsNullOrWhiteSpace(val)) return false;
        val = val.ToLower().Trim();
        return val == "true" || val == "да" || val == "1" || val == "истина";
    }

    private StatType ParseStatType(string val)
    {
        if (string.IsNullOrWhiteSpace(val)) return StatType.None;
        val = val.ToLower().Trim();
        if (val.Contains("know") || val.Contains("знан")) return StatType.Knowledge;
        if (val.Contains("friend") || val.Contains("друж")) return StatType.Friendship;
        if (val.Contains("money") || val.Contains("день")) return StatType.Money;
        return StatType.None;
    }

    private string TranslateMonth(string ruMonth)
    {
        string m = ruMonth.ToLower().Trim();
        if (m.Contains("сентяб")) return "September";
        if (m.Contains("октяб")) return "October";
        if (m.Contains("нояб")) return "November";
        if (m.Contains("декаб")) return "December";
        if (m.Contains("январ")) return "January";
        if (m.Contains("феврал")) return "February";
        if (m.Contains("март")) return "March";
        if (m.Contains("апрел")) return "April";
        if (m.Contains("май") || m.Contains("мая")) return "May";
        if (m.Contains("июн")) return "June";
        if (m.Contains("июл")) return "July";
        if (m.Contains("август")) return "August";
        return ruMonth;
    }
}