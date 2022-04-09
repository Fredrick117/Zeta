using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Galaxy Galaxy;

    //private XDocument GalaxyData;

    private void Awake()
    {
        //GameData = new GalaxyGameData();

        //string filePath = Application.persistentDataPath + "/galaxy.data";

        //FileStream dataStream = new FileStream(filePath, FileMode.Create);

        //BinaryFormatter formatter = new BinaryFormatter();
        //formatter.Serialize(dataStream, GameData);
        //dataStream.Close();

        //if (File.Exists(filePath))
        //{
        //    FileStream openStream = new FileStream(filePath, FileMode.Open);

        //    GalaxyGameData data2 = formatter.Deserialize(openStream) as GalaxyGameData;

        //    openStream.Close();

        //    foreach (string s in data2.StarSystemNames) 
        //    {
        //        Debug.Log(s);
        //    }
        //}


        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Galaxy = new Galaxy();

        GameObject.FindGameObjectWithTag("SystemGenerator").gameObject.GetComponent<SystemGenerator>().GenerateSystems();
    }

    public void AddSystem(StarSystem system)
    {
        Galaxy.StarSystems.Add(system);
    }

    public void AddPlanet(StarSystem system, Object obj)
    {
        // Todo: refactor
        Galaxy.StarSystems[Galaxy.StarSystems.IndexOf(system)].Objects.Add(obj);
    }

    /// <summary>
    /// Generates an XML file that stores data that has been generated by the SystemGenerator class.
    /// </summary>
    //public void GenerateXMLFile()
    //{
    //    var xmlSettings = new XmlWriterSettings() { Indent = true };

    //    using (XmlWriter writer = XmlWriter.Create("galaxydata.xml", xmlSettings))
    //    {
    //        writer.WriteStartDocument();
    //        writer.WriteStartElement("Galaxy");
    //        writer.WriteElementString("StarSystem", "Sol");
    //        writer.WriteEndElement();
    //        writer.WriteEndDocument();
    //    }
    //}
}