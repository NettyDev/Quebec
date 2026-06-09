using UnityEngine;

public class MeshColliderCombiner : MonoBehaviour
{
    [Tooltip("If true, disables the MeshRenderers of the children so the object becomes invisible but still has collision.")]
    public bool hideMeshesAfterCombine = true;

    [Tooltip("If true, the script will automatically combine meshes when the game starts (so you don't have to right-click).")]
    public bool combineOnAwake = false;

    void Awake()
    {
        // Automatyczne scalanie przy starcie gry
        if (combineOnAwake)
        {
            CombineMeshesForCollision();
        }
    }

    // Adding an option in the right-click context menu of the script in the Inspector
    [ContextMenu("Scal Meshe i stworz jeden MeshCollider")]
    public void CombineMeshesForCollision()
    {
        // 1. Get all standard MeshFilters AND animated SkinnedMeshRenderers (true = include inactive objects just in case)
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
        SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        
        // Array to hold data of the meshes we want to combine
        CombineInstance[] combine = new CombineInstance[meshFilters.Length + skinnedRenderers.Length];

        // Create an empty, invisible mesh just for the physics engine
        Mesh combinedCollisionMesh = new Mesh();
        combinedCollisionMesh.name = "Combined_Collision_Mesh";

        int validMeshesCount = 0;

        // 2. Loop through all found standard meshes
        for (int i = 0; i < meshFilters.Length; i++)
        {
            // Skip if the mesh is missing or if it's the parent itself (to avoid infinite loops)
            if (meshFilters[i].sharedMesh == null || meshFilters[i].gameObject == this.gameObject)
            {
                continue;
            }

            // Assign the mesh data
            combine[validMeshesCount].mesh = meshFilters[i].sharedMesh;
            
            // VERY IMPORTANT: Convert local positions of children to the parent's coordinate space
            // This ensures the hitboxes are placed exactly where the visual parts are
            combine[validMeshesCount].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            
            // Disable the renderer to make it invisible to the player but keep it physical for raycasts
            if (hideMeshesAfterCombine)
            {
                MeshRenderer mr = meshFilters[i].GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.enabled = false; // Turns off visibility
                }
            }
            
            validMeshesCount++;
        }

        // 2.5 Loop through all found Skinned (Animated) meshes
        for (int i = 0; i < skinnedRenderers.Length; i++)
        {
            if (skinnedRenderers[i].sharedMesh == null || skinnedRenderers[i].gameObject == this.gameObject)
            {
                continue;
            }

            // Używamy BakeMesh, aby "wypiec" aktualną pozę potwora do nowej, tymczasowej siatki.
            // To omija wiele problemów z siatkami animowanymi i daje niesamowicie precyzyjny hitbox!
            Mesh bakedMesh = new Mesh();
            skinnedRenderers[i].BakeMesh(bakedMesh);

            combine[validMeshesCount].mesh = bakedMesh;
            combine[validMeshesCount].transform = transform.worldToLocalMatrix * skinnedRenderers[i].transform.localToWorldMatrix;
            
            if (hideMeshesAfterCombine)
            {
                skinnedRenderers[i].enabled = false;
            }
            
            validMeshesCount++;
        }

        if (validMeshesCount == 0)
        {
            Debug.LogWarning("Nie znaleziono zadnych meshy (zwyklych ani animowanych) w dzieciach tego obiektu!");
            return;
        }

        // Create a final array with the exact size of valid meshes
        CombineInstance[] finalCombine = new CombineInstance[validMeshesCount];
        System.Array.Copy(combine, finalCombine, validMeshesCount);

        // 3. Combine them all into one single mesh
        // Parameters: (array of meshes, combine submeshes into one, use matrices)
        combinedCollisionMesh.CombineMeshes(finalCombine, true, true);

        // 4. Add or get the MeshCollider component on the parent
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        
        // 5. Assign our newly baked megamesh to the collider
        meshCollider.sharedMesh = combinedCollisionMesh;

        Debug.Log($"SUKCES: Scalono {validMeshesCount} elementow w jeden zoptymalizowany Mesh Collider! Renderery ukryte: {hideMeshesAfterCombine}");
    }
}