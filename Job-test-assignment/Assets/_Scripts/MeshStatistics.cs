using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

public class MeshStatictic : IEquatable<MeshStatictic>
{
    public string name;
    public int vertices, polygons;
    public int count = 0;
    public bool readable;

    public MeshStatictic(MeshFilter meshFilter)
    {
        name = meshFilter.sharedMesh.name;
        vertices = meshFilter.sharedMesh.vertexCount;
        polygons = meshFilter.sharedMesh.triangles.Length;
        readable = meshFilter.sharedMesh.isReadable;
    }

    public void DisplayInfo()
    {
        GUILayout.Label($"Mesh name: {name}");
        GUILayout.Label($"Number of vertices: {vertices}");
        GUILayout.Label($"Number of polygons: {polygons}");
        GUILayout.Label($"Number of uses on scene: {count}");
        GUILayout.Label($"Sum of vertices on scene: {count * vertices}");
        GUILayout.Label($"Readable: {readable}");
    }

    public bool Equals(MeshStatictic other)
    {
        return name.Equals(other.name);
    }

    public override int GetHashCode()
    {
        int hashMeshFilterName = name == null ? 0 : name.GetHashCode();

        return hashMeshFilterName; 
    }
}

public class MeshStatistics : EditorWindow
{
    private List<MeshStatictic> _meshFilters;

    [MenuItem("Window/Mesh Statistics")]
    public static void ShowWindow()
    {
        GetWindow<MeshStatistics>("Mesh Statistics");
    }

    void OnGUI()
    {
        _meshFilters = GetAllMeshesOnlyInScene();
        GetAllSumOfMeshes(_meshFilters);
        IEnumerable<MeshStatictic> _meshFiltersNoDuplicates = _meshFilters.Distinct();

        foreach (var meshFilter in _meshFiltersNoDuplicates)
        {
            meshFilter.DisplayInfo();
            EditorGUILayout.Space();
        }

    }

    /// <summary>
    /// Считает количество всех MeshFilter на сцене, которые используют MeshRenderer.
    /// </summary>
    List<MeshStatictic> GetAllMeshesOnlyInScene()
    {
        List<MeshStatictic> MeshesInScene = new List<MeshStatictic>();

        foreach (MeshFilter go in Resources.FindObjectsOfTypeAll(typeof(MeshFilter)) as MeshFilter[])
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) &&
                !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                && go.GetComponent<MeshRenderer>() != null)
            {
                MeshesInScene.Add(new MeshStatictic(go.GetComponent<MeshFilter>()));
            }
        }

        return MeshesInScene;
    }

    /// <summary>
    /// Считает все дубликаты в списке.
    /// </summary>
    void GetAllSumOfMeshes(List<MeshStatictic> meshesInScene)
    {
        foreach (var item in meshesInScene)
        {
            foreach (var subItem in meshesInScene)
            {
                if (item.name == subItem.name) item.count++;
            }
        }
    }


}
