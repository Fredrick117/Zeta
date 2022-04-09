using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SystemGenerator : MonoBehaviour
{
    // Number of systems to generate
    [SerializeField] private static int N = 21;

    // Maximum distance between systems
    [SerializeField] private float maxConnectionDistance = 100f;

    [SerializeField] private Sprite systemSprite;

    // Adjacency matrix, vertices, and edges
    private int[,] A;

    // make dictionary for connecting edges?
    private List<GameObject> V;

    private RectTransform mapContainer;

    private List<int> perfectSquares = new List<int> { 1, 4, 9, 25, 36, 49, 64, 81, 100 };

    int cellsPerRow;
    float rowWidth, cellSize;

    private void Awake()
    {
        mapContainer = gameObject.GetComponent<RectTransform>();

        

        V = new List<GameObject>();

        if (!perfectSquares.Contains(N))
        {
            N = GetNextLargestInt(N, perfectSquares);
        }

        cellsPerRow = (int)Mathf.Sqrt(N);
        rowWidth = mapContainer.rect.width;
        cellSize = rowWidth / cellsPerRow;

        for (int i = 0; i < cellsPerRow; i++)
        {
            for (int j = 0; j < cellsPerRow; j++)
            {
                CreateSpawnArea(cellSize, i, j);
            }
        }
    }

    private void Start()
    {
        //int cellsPerRow = (int)Mathf.Sqrt(N);
        //float rowWidth = mapContainer.rect.width;
        //float cellSize = rowWidth / cellsPerRow;

        //PopulateCells(cellSize);
        //CreateAdjacencyMatrix();
        //ConnectSystems();
    }

    public void GenerateSystems()
    {
        List<StarSystem> systemList = new List<StarSystem>();

        GameObject[] spawnAreaList = GameObject.FindGameObjectsWithTag("SpawnArea");

        for (int i = 0; i < N; i++)
        {
            int numObjects = Random.Range(1, 14);

            // DANGER: what if names dictionary is smaller than N?
            string starName = StarSystemNames[i];

            Star s = new Star(starName, 0, 0, Color.yellow);

            List<Object> objects = new List<Object>();

            objects.Add(s);

            RectTransform r = spawnAreaList[i].GetComponent<RectTransform>();
            float xMin = r.anchoredPosition.x - (cellSize / 2);
            float xMax = r.anchoredPosition.x + (cellSize / 2);
            float yMin = r.anchoredPosition.y - (cellSize / 2);
            float yMax = r.anchoredPosition.y + (cellSize / 2);

            StarSystem ss = new StarSystem(
                starName,
                RandomEnumValue<SystemType>(),
                Random.Range(xMin, xMax),
                Random.Range(yMin, yMax),
                // TODO: separate star and planets
                objects
            );

            for (int j = 0; j < numObjects - 1; j++)
            {
                objects.Add(new Planet(starName + " " + ToRomanNumerals(j + 2), RandomEnumValue<PlanetType>(), Random.Range(1, 20), 0, 0));
            }

            //Debug.Log("Star system " + i + " name: " + ss.Name);
            //Debug.Log("Star system " + i + " type: " + ss.Type);
            //foreach (Object o in ss.Objects)
            //{
            //    Debug.Log("Planet name: " + o.Name);
            //}

            RenderStarSystem(new Vector2(ss.Xlocation, ss.Ylocation));

            systemList.Add(ss);
        }

        // 4) add system list to galaxy object

        GameManager.Instance.Galaxy.StarSystems = systemList;
    }

    public void PopulateCells(float cellSize)
    {
        // TODO: create buffer
        // x position + [buffer], y position + [buffer]?
        GameObject[] spawnAreaList = GameObject.FindGameObjectsWithTag("SpawnArea");
        foreach (GameObject gameObject in spawnAreaList)
        {
            RectTransform r = gameObject.GetComponent<RectTransform>();
            float xMin = r.anchoredPosition.x - (cellSize / 2);
            float xMax = r.anchoredPosition.x + (cellSize / 2);
            float yMin = r.anchoredPosition.y - (cellSize / 2);
            float yMax = r.anchoredPosition.y + (cellSize / 2);

            //V.Add(CreateSystem(new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax))));
        }
    }

    private GameObject RenderStarSystem(Vector2 position)
    {
        GameObject gameObject = new GameObject("System", typeof(Image));
        gameObject.transform.SetParent(mapContainer, false);
        gameObject.GetComponent<Image>().sprite = systemSprite;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(11, 11);

        return gameObject;
    }

    private void CreateSpawnArea(float cellSize, int i, int j)
    {
        GameObject gameObject = new GameObject("SpawnArea" + ((i + j) + 1), typeof(Image));
        gameObject.tag = "SpawnArea";
        gameObject.transform.SetParent(mapContainer, false);
        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(cellSize, cellSize);
        Vector2 spawnPosition = new Vector2((cellSize / 2) + (i * cellSize), (mapContainer.rect.height - (cellSize / 2)) - (j * cellSize));
        rect.anchoredPosition = spawnPosition;
    }

    private void CreateAdjacencyMatrix()
    {
        A = new int[N, N];

        for (int i = 0; i < V.Count; i++)
        {
            for (int j = 0; j < V.Count; j++)
            {
                float dist = Vector2.Distance(V[i].transform.position, V[j].transform.position);

                if (dist != 0 && dist <= maxConnectionDistance)
                {
                    A[i, j] = 1;
                }
            }
        }
    }

    private void CreateSystemConnection(Vector2 A, Vector2 B)
    {
        GameObject gameObject = new GameObject("connector", typeof(Image));
        gameObject.transform.SetParent(mapContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector2 direction = (B - A).normalized;
        float distance = Vector2.Distance(A, B);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(distance, 2f);
        rect.anchoredPosition = A + direction * distance * .5f;
        rect.localEulerAngles = new Vector3(0, 0, (Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI));
    }

    private void ConnectSystems()
    {
        for (int i = 0; i < V.Count; i++)
            for (int j = 0; j < V.Count; j++)
                if (A[i, j] == 1)
                    CreateSystemConnection(V[i].GetComponent<RectTransform>().anchoredPosition, V[j].GetComponent<RectTransform>().anchoredPosition);
    }

    private int GetNextLargestInt(int num, List<int> list)
    {
        foreach (int i in list)
        {
            if (i > num)
            {
                return i;
            }
        }
        // refactor
        return num;
    }

    //private void PopulateSystem(List<Object> objList)
    //{
    //    int numPlanets = Random.Range(0, 15);

    //    for (int i = 0; i < numPlanets; i++)
    //    {
    //        ObjType obj = RandomEnumValue<ObjType>();

    //        if (obj == ObjType.HabitablePlanet)
    //        {
    //            Planet p = new Planet(RandomEnumValue<PlanetType>, );
    //        }
    //        else if (obj == ObjType.Star)
    //        {

    //        }
    //    }
    //}

    private T RandomEnumValue<T>()
    {
        var vals = System.Enum.GetValues(typeof(T));
        return (T)vals.GetValue(Random.Range(0, vals.Length - 1));
    }

    private string ToRomanNumerals(int num)
    {
        // TODO: add more
        switch (num)
        {
            case 1:
                return "I";
            case 2:
                return "II";
            case 3:
                return "III";
            case 4:
                return "IV";
            case 5:
                return "V";
            case 6:
                return "VI";
            case 7:
                return "VII";
            case 8:
                return "VIII";
            case 9:
                return "IX";
            case 10:
                return "X";
            case 11:
                return "XI";
            case 12:
                return "XII";
            case 13:
                return "XIII";
            case 14:
                return "XIV";
            case 15:
                return "XV";
            default:
                return "";
        }
    }

    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
    private static void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length - 2; i++)
        {
            int j = Random.Range(i, array.Length - 1);
            array[i] = array[j];
        }
    }

    public string[] StarSystemNames = new string[] {
        "Ahnar",
        "Zubana",
        "Komle",
        "Usta",
        "Zyb",
        "Sama",
        "Alhim",
        "Falha",
        "Zamok",
        "Korolis Ray",
        "FR-9097b",
        "Vovio",
        "Ovhil",
        "Cassio",
        "Urgarius",
        "Irel",
        "Daryus",
        "Noxus",
        "Demyr",
        "Ionia",
        "Demacia",
        "Lambda Paradisus",
        "Chi Caelum",
        "Meritum",
        "Harena",
        "Hio",
        "Urs",
        "Gav",
        "8970987c",
        "Ymir",
        "Hildr",
        "Signy",
        "Siv",
        "Skadi",
        "Nanna",
        "Papur",
        "Atla",
        "Eir",
        "Freya",
        "Saga",
        "Skul",
        "Var",
        "Bol",
        "Flot",
        "Duble",
        "Snotra",
        "Yggdrasil",
        "Eostre",
        "Tyr",
        "Yngvi",
        "Ull",
    };
}