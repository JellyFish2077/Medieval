using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingSystem : MonoBehaviour
{
    public GameObject[] buildingPrefabs; // Префабы зданий
    public GameObject[] decorationPrefabs; // Префабы декораций
    public GameObject[] roadPrefabs; // Префабы дорожек

    public LayerMask groundLayer; // Слой для размещения построек
    public LayerMask obstacleLayer; // Слой для объектов, которые мешают строительству
    public LayerMask buildingLayer; // Слой для зданий

    public KeyCode buildKey = KeyCode.B; // Клавиша для активации строительства
    public Text currentBuildingText; // Ссылка на текстовый элемент
    public GlobalResourceManager globalResourceManager; // Ссылка на менеджер ресурсов
    public ContextHintUI contextHintUI; // Ссылка на ContextHintUI
    public SettlementLevel settlementProgress; //Ссылка на Settlementprogress


    private GameObject currentBuildingPrefab; // Текущий префаб для строительства
    private GameObject buildingPreview; // Временный объект для предпросмотра
    private bool isBuilding = false; // Режим строительства
    private float rotationAngle = 0f; // Текущий угол поворота здания

    private int currentBuildingIndex = 0; // Текущий индекс здания
    private BuildingType currentBuildingType = BuildingType.Buildings; // Текущий тип постройки
    public float rotationSpeed = 100f; // Скорость поворота здания


    

    void Update()
    {
        if (Input.GetKeyDown(buildKey))
        {
            if (isBuilding)
            {
                CancelBuilding();
            }
            else
            {
                StartBuilding(currentBuildingType, currentBuildingIndex);
            }
        }

        if (isBuilding)
        {
            HandleBuildingPreview();
            HandleBuildingPlacement();
            HandleBuildingRotation();
            HandleBuildingSelection(); // Обработка выбора здания с помощью колесика мыши
            UpdateContextHint();

        }

        // Обработка смены типа постройки по кнопкам
        HandleBuildingTypeChange();
    }

    // Начало строительства
    public void StartBuilding(BuildingType type, int buildingIndex)
    {
        GameObject[] prefabs = GetPrefabsByType(type);

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.Log("Нет доступных построек для строительства!");
            return;
        }

        if (buildingIndex < 0 || buildingIndex >= prefabs.Length)
        {
            Debug.Log("Недопустимый индекс постройки!");
            return;
        }

        if (buildingPreview != null)
        {
            Destroy(buildingPreview);
        }

        currentBuildingPrefab = prefabs[buildingIndex];
        isBuilding = true;
        rotationAngle = 0f;

        buildingPreview = Instantiate(currentBuildingPrefab);
        buildingPreview.GetComponent<Collider>().enabled = false;
        SetPreviewColor(true);

        SetBuildingTextVisibility(true);
        UpdateBuildingText();
    }
    public void Build(GameObject buildingPrefab, BuildingType type)
    {
        if (settlementProgress.CanBuild(type))
        {
            // Логика строительства
            Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);

            // Увеличиваем счетчик построек
            settlementProgress.IncrementBuildingCount(BuildingType.Buildings);
            // Добавляем опыт за постройку
            settlementProgress.AddExperience(10);
        }
        else
        {
            Debug.Log($"Достигнут лимит построек для типа {type}!");
        }
    }
    // Получение массива префабов по типу
    private GameObject[] GetPrefabsByType(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Buildings:
                return buildingPrefabs;
            case BuildingType.Decorations:
                return decorationPrefabs;
            case BuildingType.Roads:
                return roadPrefabs;
            default:
                return null;
        }
    }

    // Обработка предпросмотра здания
    private void HandleBuildingPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            buildingPreview.transform.position = hit.point;
            buildingPreview.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }

    // Обработка поворота здания
    private void HandleBuildingRotation()
    {
        if (Input.GetKey(KeyCode.Q)) // Поворот влево
        {
            rotationAngle -= rotationSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E)) // Поворот вправо
        {
            rotationAngle += rotationSpeed * Time.deltaTime;
        }

        // Ограничиваем угол поворота от 0 до 360 градусов
        rotationAngle = Mathf.Repeat(rotationAngle, 360f);
    }

    // Обработка выбора здания с помощью колесика мыши
    private void HandleBuildingSelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            GameObject[] prefabs = GetPrefabsByType(currentBuildingType);

            if (prefabs != null && prefabs.Length > 0)
            {
                if (scroll > 0) // Прокрутка вверх
                {
                    currentBuildingIndex = (currentBuildingIndex + 1) % prefabs.Length;
                }
                else if (scroll < 0) // Прокрутка вниз
                {
                    currentBuildingIndex = (currentBuildingIndex - 1 + prefabs.Length) % prefabs.Length;
                }

                StartBuilding(currentBuildingType, currentBuildingIndex);
            }
        }
    }

    // Обработка смены типа постройки по кнопкам
    private void HandleBuildingTypeChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Клавиша 1 для зданий
        {
            ChangeBuildingType(BuildingType.Buildings);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Клавиша 2 для декораций
        {
            ChangeBuildingType(BuildingType.Decorations);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Клавиша 3 для дорожек
        {
            ChangeBuildingType(BuildingType.Roads);
        }
    }

    // Смена типа постройки
    private void ChangeBuildingType(BuildingType newType)
    {
        if (currentBuildingType != newType)
        {
            currentBuildingType = newType;
            currentBuildingIndex = 0; // Сбрасываем индекс при смене типа

            if (isBuilding)
            {
                StartBuilding(currentBuildingType, currentBuildingIndex);
            }
        }
    }

    // Обработка размещения здания
    private void HandleBuildingPlacement()
    {
        if (Input.GetMouseButtonDown(0)) // ЛКМ для размещения
        {
            if (CanPlaceBuilding())
            {
                PlaceBuilding();
            }
            else
            {
                Debug.Log("Невозможно разместить здание здесь!");
                EventLogManager.Instance.AddMessage("Невозможно разместить здание здесь!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // Отмена строительства
        {
            CancelBuilding();
        }
    }

    // Проверка, можно ли разместить здание
    private bool CanPlaceBuilding()
    {
        Collider previewCollider = buildingPreview.GetComponent<Collider>();
        if (previewCollider != null)
        {
            previewCollider.enabled = false;
        }

        BoxCollider buildingCollider = buildingPreview.GetComponent<BoxCollider>();
        Vector3 halfSize = buildingCollider.size / 2;

        Collider[] colliders = Physics.OverlapBox(buildingPreview.transform.position, halfSize, buildingPreview.transform.rotation, buildingLayer);

        if (previewCollider != null)
        {
            previewCollider.enabled = true;
        }

        return colliders.Length == 0;
    }

    // Размещение здания
    private void PlaceBuilding()
    {
        GlobalResourceManager globalResourceManager = FindObjectOfType<GlobalResourceManager>();
        BuildingCost buildingCost = currentBuildingPrefab.GetComponent<BuildingCost>();

        if (globalResourceManager != null && buildingCost != null)
        {
            if (globalResourceManager.CanAfford(buildingCost.woodCost, buildingCost.stoneCost))
            {
                globalResourceManager.SpendResources(buildingCost.woodCost, buildingCost.stoneCost);
                Instantiate(currentBuildingPrefab, buildingPreview.transform.position, Quaternion.Euler(0, rotationAngle, 0));
                EventLogManager.Instance.AddMessage($"Построено здание: {currentBuildingPrefab.name}");
            }
            else
            {
                Debug.Log("Недостаточно ресурсов для строительства!");
                EventLogManager.Instance.AddMessage("Недостаточно ресурсов для строительства!");
            }
        }
        else
        {
            Debug.Log("GlobalResourceManager или BuildingCost не найдены!");
        }
    }

    // Отмена строительства
    private void CancelBuilding()
    {
        if (buildingPreview != null)
        {
            Destroy(buildingPreview);
            buildingPreview = null;
        }

        isBuilding = false;
        currentBuildingPrefab = null;

        SetBuildingTextVisibility(false);
    }

    // Установка цвета предпросмотра
    private void SetPreviewColor(bool canPlace)
    {
        Renderer renderer = buildingPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = canPlace ? Color.green : Color.red;
        }
    }

    // Обновление текстового элемента
    private void UpdateBuildingText()
    {
        if (currentBuildingText != null)
        {
            currentBuildingText.text = $"Текущий тип постройки: {currentBuildingType}\nТекущая постройка: {currentBuildingPrefab.name}";
        }
    }

    // Управление видимостью текстового элемента
    private void SetBuildingTextVisibility(bool isVisible)
    {
        if (currentBuildingText != null)
        {
            currentBuildingText.gameObject.SetActive(isVisible);
        }
    }
    private void UpdateContextHint()
    {
        if (currentBuildingPrefab != null)
        {
            BuildingCost buildingCost = currentBuildingPrefab.GetComponent<BuildingCost>();
            if (buildingCost != null)
            {
                string costText = $"Стоимость: Дерево - {buildingCost.woodCost}, Камень - {buildingCost.stoneCost}";
                contextHintUI.ShowHint($"ЛКМ - Разместить здание\nEsc - Отмена\nQ/E (зажать) - Плавный поворот\nКолесико мыши - Смена здания \n1-3 - Смена типа здания\n{costText}");
            }
            else
            {
                contextHintUI.ShowHint("ЛКМ - Разместить здание\nEsc - Отмена\nQ/E (зажать) - Плавный поворот\nКолесико мыши - Смена здания \n1-3 Смена типа здания");
            }
        }
    }
    // Методы для UI
    public void SetBuildingType(int typeIndex)
    {
        currentBuildingType = (BuildingType)typeIndex;
        currentBuildingIndex = 0; // Сбрасываем индекс при смене типа
    }

    public void SetBuildingIndex(int index)
    {
        currentBuildingIndex = index;
    }
}