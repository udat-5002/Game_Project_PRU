using System.Collections.Generic;
using UnityEngine;

public static class QuestWaypointRegistry
{
    static readonly Dictionary<string, Vector3> Points = new Dictionary<string, Vector3>();

    public static void Clear() => Points.Clear();

    public static void Register(string questStepId, Vector3 worldPos) =>
        Points[questStepId] = worldPos;

    public static bool TryGet(string questStepId, out Vector3 pos) =>
        Points.TryGetValue(questStepId, out pos);

    public static IReadOnlyDictionary<string, Vector3> All => Points;
}
