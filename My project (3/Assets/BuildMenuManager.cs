using UnityEngine;
using UnityEngine.UI;

public class BuildMenuManager : MonoBehaviour
{
    public GameObject buildMenuPanel; // Панель меню построек
    public GameObject[] buildingPrefabs; // Префабы построек (0 - лесорубы, 1 - камнетесы, 2 - бар)
    private GameObject currentBuilding; // Текущая постройка для размещения
    private bool isBuilding = false; // Режим размещения постройки

    void Start()
    {
        if (buildMenuPanel == null)
        {
            Debug.LogError("Панель меню не назначена!");
            return;
        }

        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            Debug.LogError("Префабы построек не назначены!");
            return;
        }

        // Находим кнопки и настраиваем их
        Button lumberjackButton = GameObject.Find("LumberjackButton").GetComponent<Button>();
        Button masonButton = GameObject.Find("MasonButton").GetComponent<Button>();
        Button barButton = GameObject.Find("BarButton").GetComponent<Button>();

        // Назначаем методы на кнопки
        lumberjackButton.onClick.AddListener(() => SelectBuilding(0));
        masonButton.onClick.AddListener(() => SelectBuilding(1));
        barButton.onClick.AddListener(() => SelectBuilding(2));
    }

    void Update()
    {
        if (buildMenuPanel == null)
        {
            Debug.LogError("Панель меню не назначена!");
            return;
        }

        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            Debug.LogError("Префабы построек не назначены!");
            return;
        }

        // Открываем/закрываем меню по нажатию Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleBuildMenu();
        }

        // Если в режиме размещения постройки
        if (isBuilding)
        {
            // Перемещаем постройку за курсором
            MoveBuildingWithMouse();

            // Размещаем постройку по нажатию ЛКМ
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding();
            }

            // Отмена размещения по нажатию ПКМ
            if (Input.GetMouseButtonDown(1))
            {
                CancelBuilding();
            }
        }
    }

    void ToggleBuildMenu()
    {
        // Включаем/выключаем меню
        buildMenuPanel.SetActive(!buildMenuPanel.activeSelf);

        // Ставим игру на паузу
        Time.timeScale = buildMenuPanel.activeSelf ? 0f : 1f;

        // Включаем/выключаем курсор
        Cursor.visible = buildMenuPanel.activeSelf;
        Cursor.lockState = buildMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // Выбор постройки
    public void SelectBuilding(int buildingIndex)
    {
        if (buildingIndex >= 0 && buildingIndex < buildingPrefabs.Length)
        {
            // Создаем постройку для размещения
            currentBuilding = Instantiate(buildingPrefabs[buildingIndex]);
            isBuilding = true;

            // Закрываем меню
            buildMenuPanel.SetActive(false);
        }
    }

    void MoveBuildingWithMouse()
    {
        // Перемещаем постройку за курсором
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            currentBuilding.transform.position = hit.point;
        }
    }

    void PlaceBuilding()
    {
        // Фиксируем постройку на месте
        isBuilding = false;
        currentBuilding = null;

        // Возвращаем игру в нормальное состояние
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void CancelBuilding()
    {
        // Отменяем размещение постройки
        Destroy(currentBuilding);
        isBuilding = false;

        // Возвращаем игру в нормальное состояние
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}