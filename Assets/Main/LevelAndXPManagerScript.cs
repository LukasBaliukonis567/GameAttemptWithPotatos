using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public class LevelAndXPManagerScript : MonoBehaviour
{
    public List<int> xpThresholds = new List<int>();

    public static LevelAndXPManagerScript Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevelDataFromXML("LevelThresholds"); // without .xml
    }

    void Awake()
    {
        Instance = this;
    }


    void LoadLevelDataFromXML(string Filename) // Might switch to JSON later
    {
        TextAsset xmlFile = Resources.Load<TextAsset>("LevelThresholds");
        if (xmlFile == null)
        {
            Debug.LogError("XML file not found: " + Filename);
            return;
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);
        XmlNodeList levelNodes = xmlDoc.SelectNodes("/Levels/Level");
        if (levelNodes == null || levelNodes.Count == 0)
        {
            Debug.LogError("No levels found in XML file.");
            return;
        }
        foreach (XmlNode levelNode in levelNodes)
        {
            if (levelNode.Attributes["xp"] != null && int.TryParse(levelNode.Attributes["xp"].Value, out int xpThreshold))
            {
                xpThresholds.Add(xpThreshold);
            }
            else
            {
                Debug.LogError("Invalid XP threshold value: " + levelNode.InnerText);
            }
        }
    }

    public int GetLevelFromXP(int xp)
    {
        for (int i = 0; i < xpThresholds.Count; i++)
        {
            if (xp < xpThresholds[i])
            {
                return i; // Level is the index in the list
            }
        }
        return xpThresholds.Count; // If XP exceeds all thresholds, return max level
    }


    public int GetXPForNextLevelThreshold(int currentLevel)
    {
        if (currentLevel + 1 < xpThresholds.Count)
        {
            return xpThresholds[currentLevel + 1]; // XP required for next level
        }

        return int.MaxValue; // No next level
    }

    public int GetXPForCurrentLevel(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= xpThresholds.Count)
        {
            Debug.LogError("Invalid level: " + currentLevel);
            return -1; // Invalid level
        }
        return xpThresholds[currentLevel]; // XP needed for current level
    }
}
