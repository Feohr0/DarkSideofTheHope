// Scripts/Combat/EnemyData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "StS/Enemy", fileName = "New Enemy")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public int MaxHealth;
    public Sprite Portrait;
    public EnemyIntent[] IntentPattern; // Döngüsel dizi

    public EnemyIntent GetIntent(int turnIndex)
    {
        if (IntentPattern == null || IntentPattern.Length == 0)
            return new EnemyIntent { Type = IntentType.Unknown };
        return IntentPattern[turnIndex % IntentPattern.Length];
    }
}

[System.Serializable]
public struct EnemyIntent
{
    public IntentType Type;
    public int        Value;
    public Sprite     Icon;
}