using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Building building = hit.collider.GetComponent<Building>();
                if (building != null)
                {
                    // Находим ближайшего NPC
                    NPC npc = FindClosestNPC(building.transform.position);

                    if (npc != null)
                    {
                        // Назначаем NPC на постройку
                        building.AssignNPC(npc);
                    }
                    else
                    {
                        Debug.Log("Нет свободных NPC!");
                    }
                }
            }
        }
    }

    // Поиск ближайшего NPC
    NPC FindClosestNPC(Vector3 position)
    {
        NPC[] npcs = FindObjectsOfType<NPC>();
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (NPC npc in npcs)
        {
            if (npc.GetAssignedBuilding() == null) // Ищем только свободных NPC
            {
                float distance = Vector3.Distance(npc.transform.position, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNPC = npc;
                }
            }
        }

        return closestNPC;
    }
}