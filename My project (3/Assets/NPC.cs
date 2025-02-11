using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public enum NPCState { Idle, Working, Socializing, Delivering } // Состояния NPC
    public NPCState currentState = NPCState.Idle; // Текущее состояние

    private Building assignedBuilding; // Назначенная постройка
    private NavMeshAgent agent; // Компонент для перемещения
    private GameObject targetCylinder; // Цилиндр, который добывает лесоруб
    private float miningTimer = 0f; // Таймер добычи
    private GameObject chest; // Сундук для сдачи цилиндров

    void Start()
    {
        // Получаем компонент NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден на NPC!");
        }

        // Находим сундук
        chest = GameObject.FindWithTag("Chest");
        if (chest == null)
        {
            Debug.LogError("Сундук не найден!");
        }
    }

    // Назначение на постройку
    public void AssignToBuilding(Building building)
    {
        if (assignedBuilding != null)
        {
            Debug.Log("NPC уже назначен на другую постройку!");
            return;
        }

        assignedBuilding = building;
        currentState = NPCState.Working;

        if (building.GetBuildingType() == Building.BuildingType.Lumberjack)
        {
            // Лесорубы идут добывать цилиндры
            FindAndMineCylinder();
        }
        else if (building.GetBuildingType() == Building.BuildingType.Bar)
        {
            // NPC в баре остается на месте и приманивает других NPC
            MoveToBar();
        }

        Debug.Log($"NPC назначен на постройку: {building.GetBuildingType()}");
    }

    // Получение назначенной постройки
    public Building GetAssignedBuilding()
    {
        return assignedBuilding;
    }

    void Update()
    {
        if (assignedBuilding != null)
        {
            switch (assignedBuilding.GetBuildingType())
            {
                case Building.BuildingType.Lumberjack:
                    HandleLumberjackBehavior();
                    break;
                case Building.BuildingType.Bar:
                    HandleBarBehavior();
                    break;
            }
        }
        else
        {
            // Если NPC не назначен на постройку, он бездельничает
            currentState = NPCState.Idle;
        }
    }

    // Поведение для лесопилки
    void HandleLumberjackBehavior()
    {
        switch (currentState)
        {
            case NPCState.Working:
                HandleMining();
                break;
            case NPCState.Delivering:
                HandleDelivery();
                break;
        }
    }

    // Добыча цилиндра
    void HandleMining()
    {
        if (targetCylinder == null)
        {
            // Если цилиндр не назначен, ищем новый
            FindAndMineCylinder();
        }
        else
        {
            // Если NPC достиг цилиндра, начинаем добычу
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                miningTimer += Time.deltaTime;

                if (miningTimer >= 2f) // Добыча длится 2 секунды
                {
                    Destroy(targetCylinder); // Уничтожаем цилиндр
                    targetCylinder = null;
                    miningTimer = 0f;

                    // Переходим в состояние доставки
                    currentState = NPCState.Delivering;
                    MoveToPosition(chest.transform.position); // Идем к сундуку
                }
            }
        }
    }

    // Доставка цилиндра в сундук
    void HandleDelivery()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Цилиндр доставлен в сундук!");

            // Возвращаемся к добыче
            currentState = NPCState.Working;
            FindAndMineCylinder();
        }
    }

    // Поиск и добыча цилиндров
    void FindAndMineCylinder()
    {
        GameObject[] cylinders = GameObject.FindGameObjectsWithTag("Cylinder");
        if (cylinders.Length > 0)
        {
            // Выбираем случайный цилиндр
            int randomIndex = Random.Range(0, cylinders.Length);
            targetCylinder = cylinders[randomIndex];

            // Перемещаем NPC к цилиндру
            MoveToPosition(targetCylinder.transform.position);
        }
        else
        {
            Debug.Log("Цилиндры не найдены! NPC идет в бар.");
            GoToBar();
        }
    }

    // Перемещение NPC в бар
    void MoveToBar()
    {
        // Перемещаем NPC к бару
        MoveToPosition(assignedBuilding.transform.position);
    }

    // Перемещение NPC в бар (если нет цилиндров)
    void GoToBar()
    {
        // Находим ближайший бар
        Building[] bars = FindObjectsOfType<Building>();
        Building closestBar = null;
        float closestDistance = Mathf.Infinity;

        foreach (Building bar in bars)
        {
            if (bar.GetBuildingType() == Building.BuildingType.Bar)
            {
                float distance = Vector3.Distance(transform.position, bar.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBar = bar;
                }
            }
        }

        // Перемещаем NPC к бару
        if (closestBar != null)
        {
            MoveToPosition(closestBar.transform.position);
        }
    }

    // Поведение для бара
    void HandleBarBehavior()
    {
        if (currentState == NPCState.Working)
        {
            // Если NPC достиг бара, переключаем состояние на "общение"
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                currentState = NPCState.Socializing;
                Debug.Log("Работник бара на месте. Приманиваем других NPC...");

                // Привлекаем других NPC в бар
                AttractOtherNPCs();
            }
        }
    }

    // Привлечение других NPC в бар
    void AttractOtherNPCs()
    {
        NPC[] npcs = FindObjectsOfType<NPC>();
        foreach (NPC npc in npcs)
        {
            if (npc != this && npc.GetAssignedBuilding() == null)
            {
                npc.SetState(NPCState.Socializing);
                npc.MoveToPosition(assignedBuilding.transform.position + Random.insideUnitSphere * 5f); // NPC подходят к бару
            }
        }
    }

    // Перемещение NPC к указанной позиции
    public void MoveToPosition(Vector3 position)
    {
        if (agent != null)
        {
            // Ищем ближайшую точку на NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogWarning("Точка назначения не на NavMesh!");
            }
        }
    }

    // Переключение состояния
    public void SetState(NPCState newState)
    {
        currentState = newState;
    }
}