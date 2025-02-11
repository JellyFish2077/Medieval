using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType { Lumberjack, Bar } // Типы построек
    public BuildingType buildingType; // Тип текущей постройки

    private NPC assignedNPC; // Назначенный NPC

    // Назначение NPC на постройку
    public void AssignNPC(NPC npc)
    {
        if (assignedNPC != null)
        {
            Debug.Log("На эту постройку уже назначен NPC!");
            return;
        }

        assignedNPC = npc;
        npc.AssignToBuilding(this);

        Debug.Log($"NPC назначен на постройку: {buildingType}");
    }

    // Получение назначенного NPC
    public NPC GetAssignedNPC()
    {
        return assignedNPC;
    }

    // Получение типа постройки
    public BuildingType GetBuildingType()
    {
        return buildingType;
    }
}