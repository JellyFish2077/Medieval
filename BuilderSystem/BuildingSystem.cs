using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingSystem : MonoBehaviour
{
    public GameObject[] buildingPrefabs; // ������� ������
    public GameObject[] decorationPrefabs; // ������� ���������
    public GameObject[] roadPrefabs; // ������� �������

    public LayerMask groundLayer; // ���� ��� ���������� ��������
    public LayerMask obstacleLayer; // ���� ��� ��������, ������� ������ �������������
    public LayerMask buildingLayer; // ���� ��� ������

    public KeyCode buildKey = KeyCode.B; // ������� ��� ��������� �������������
    public Text currentBuildingText; // ������ �� ��������� �������
    public GlobalResourceManager globalResourceManager; // ������ �� �������� ��������
    public ContextHintUI contextHintUI; // ������ �� ContextHintUI
    public SettlementLevel settlementProgress; //������ �� Settlementprogress


    private GameObject currentBuildingPrefab; // ������� ������ ��� �������������
    private GameObject buildingPreview; // ��������� ������ ��� �������������
    private bool isBuilding = false; // ����� �������������
    private float rotationAngle = 0f; // ������� ���� �������� ������

    private int currentBuildingIndex = 0; // ������� ������ ������
    private BuildingType currentBuildingType = BuildingType.Buildings; // ������� ��� ���������
    public float rotationSpeed = 100f; // �������� �������� ������


    

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
            HandleBuildingSelection(); // ��������� ������ ������ � ������� �������� ����
            UpdateContextHint();

        }

        // ��������� ����� ���� ��������� �� �������
        HandleBuildingTypeChange();
    }

    // ������ �������������
    public void StartBuilding(BuildingType type, int buildingIndex)
    {
        GameObject[] prefabs = GetPrefabsByType(type);

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.Log("��� ��������� �������� ��� �������������!");
            return;
        }

        if (buildingIndex < 0 || buildingIndex >= prefabs.Length)
        {
            Debug.Log("������������ ������ ���������!");
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
            // ������ �������������
            Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);

            // ����������� ������� ��������
            settlementProgress.IncrementBuildingCount(BuildingType.Buildings);
            // ��������� ���� �� ���������
            settlementProgress.AddExperience(10);
        }
        else
        {
            Debug.Log($"��������� ����� �������� ��� ���� {type}!");
        }
    }
    // ��������� ������� �������� �� ����
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

    // ��������� ������������� ������
    private void HandleBuildingPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            buildingPreview.transform.position = hit.point;
            buildingPreview.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }

    // ��������� �������� ������
    private void HandleBuildingRotation()
    {
        if (Input.GetKey(KeyCode.Q)) // ������� �����
        {
            rotationAngle -= rotationSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E)) // ������� ������
        {
            rotationAngle += rotationSpeed * Time.deltaTime;
        }

        // ������������ ���� �������� �� 0 �� 360 ��������
        rotationAngle = Mathf.Repeat(rotationAngle, 360f);
    }

    // ��������� ������ ������ � ������� �������� ����
    private void HandleBuildingSelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            GameObject[] prefabs = GetPrefabsByType(currentBuildingType);

            if (prefabs != null && prefabs.Length > 0)
            {
                if (scroll > 0) // ��������� �����
                {
                    currentBuildingIndex = (currentBuildingIndex + 1) % prefabs.Length;
                }
                else if (scroll < 0) // ��������� ����
                {
                    currentBuildingIndex = (currentBuildingIndex - 1 + prefabs.Length) % prefabs.Length;
                }

                StartBuilding(currentBuildingType, currentBuildingIndex);
            }
        }
    }

    // ��������� ����� ���� ��������� �� �������
    private void HandleBuildingTypeChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // ������� 1 ��� ������
        {
            ChangeBuildingType(BuildingType.Buildings);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // ������� 2 ��� ���������
        {
            ChangeBuildingType(BuildingType.Decorations);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // ������� 3 ��� �������
        {
            ChangeBuildingType(BuildingType.Roads);
        }
    }

    // ����� ���� ���������
    private void ChangeBuildingType(BuildingType newType)
    {
        if (currentBuildingType != newType)
        {
            currentBuildingType = newType;
            currentBuildingIndex = 0; // ���������� ������ ��� ����� ����

            if (isBuilding)
            {
                StartBuilding(currentBuildingType, currentBuildingIndex);
            }
        }
    }

    // ��������� ���������� ������
    private void HandleBuildingPlacement()
    {
        if (Input.GetMouseButtonDown(0)) // ��� ��� ����������
        {
            if (CanPlaceBuilding())
            {
                PlaceBuilding();
            }
            else
            {
                Debug.Log("���������� ���������� ������ �����!");
                EventLogManager.Instance.AddMessage("���������� ���������� ������ �����!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // ������ �������������
        {
            CancelBuilding();
        }
    }

    // ��������, ����� �� ���������� ������
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

    // ���������� ������
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
                EventLogManager.Instance.AddMessage($"��������� ������: {currentBuildingPrefab.name}");
            }
            else
            {
                Debug.Log("������������ �������� ��� �������������!");
                EventLogManager.Instance.AddMessage("������������ �������� ��� �������������!");
            }
        }
        else
        {
            Debug.Log("GlobalResourceManager ��� BuildingCost �� �������!");
        }
    }

    // ������ �������������
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

    // ��������� ����� �������������
    private void SetPreviewColor(bool canPlace)
    {
        Renderer renderer = buildingPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = canPlace ? Color.green : Color.red;
        }
    }

    // ���������� ���������� ��������
    private void UpdateBuildingText()
    {
        if (currentBuildingText != null)
        {
            currentBuildingText.text = $"������� ��� ���������: {currentBuildingType}\n������� ���������: {currentBuildingPrefab.name}";
        }
    }

    // ���������� ���������� ���������� ��������
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
                string costText = $"���������: ������ - {buildingCost.woodCost}, ������ - {buildingCost.stoneCost}";
                contextHintUI.ShowHint($"��� - ���������� ������\nEsc - ������\nQ/E (������) - ������� �������\n�������� ���� - ����� ������ \n1-3 - ����� ���� ������\n{costText}");
            }
            else
            {
                contextHintUI.ShowHint("��� - ���������� ������\nEsc - ������\nQ/E (������) - ������� �������\n�������� ���� - ����� ������ \n1-3 ����� ���� ������");
            }
        }
    }
    // ������ ��� UI
    public void SetBuildingType(int typeIndex)
    {
        currentBuildingType = (BuildingType)typeIndex;
        currentBuildingIndex = 0; // ���������� ������ ��� ����� ����
    }

    public void SetBuildingIndex(int index)
    {
        currentBuildingIndex = index;
    }
}