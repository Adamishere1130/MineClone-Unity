using UnityEngine;

public static class AutoAnimalSpawnerBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void CreateSpawner()
    {
        var go = new GameObject("AutoAnimalSpawner");
        go.AddComponent<AutoAnimalSpawner>(); // uses default settings
        Object.DontDestroyOnLoad(go);
    }
}
