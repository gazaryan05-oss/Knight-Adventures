using UnityEngine;

public class DestructiblePlantsVisual : MonoBehaviour
{
    [SerializeField] private DestructiblePlants destructiblePlant;
    [SerializeField] private GameObject bushDeathVFXPrefab;

    private void Start()
    {
        // Если не назначен в Inspector — ищем на родителе
        if (destructiblePlant == null)
        {
            destructiblePlant = GetComponentInParent<DestructiblePlants>();
        }

        if (destructiblePlant == null)
        {
            Debug.LogError("DestructiblePlants not found on " + gameObject.name);
            return;
        }

        destructiblePlant.OnDestructibleTakeDamage += DestructiblePlant_OnDestructibleTakeDamage;
    }

    private void DestructiblePlant_OnDestructibleTakeDamage(object sender, System.EventArgs e) //buysinxpecin
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        if (bushDeathVFXPrefab != null && destructiblePlant != null)
        {
            Instantiate(bushDeathVFXPrefab, destructiblePlant.transform.position, Quaternion.identity); //effectsceneum
        }
    }

    private void OnDestroy()
    {
        if (destructiblePlant != null)
        {
            destructiblePlant.OnDestructibleTakeDamage -= DestructiblePlant_OnDestructibleTakeDamage;
        }
    }
}
