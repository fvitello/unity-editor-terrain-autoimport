using UnityEngine;
using UnityEditor;
using System.IO;

using System;
using System.Collections;
using System.Collections.Generic;


public class CreateTiledTerrain : EditorWindow
{

    private static EditorWindow window;

    private static Vector2 tileAmount = Vector2.one;



    private static int width = 4097;
    private static int lenght = 4097;
    private static int height = 680;

    private int heightmapResoltion = 4097;
    private int detailResolution = 1024;
    private int detailResolutionPerPatch = 8;
    private int controlTextureResolution = 512;
    private int baseTextureReolution = 1024;

    private string path = string.Empty;
    private string originpath = string.Empty;
    private string prefix = string.Empty;
    private string[] outVal = new string[676];

    private string m_Path;
    private Terrain m_Terrain;
    private TerrainData terrainData
    {
        get
        {
            if (m_Terrain != null)
                return m_Terrain.terrainData;
            else
                return null;
        }
    }

    internal enum Depth { Bit8 = 1, Bit16 = 2 }
    internal enum ByteOrder { Mac = 1, Windows = 2 }

    private Vector3 m_TerrainSize = new Vector3(width, height, lenght);
    private Depth m_Depth = Depth.Bit16;
    private int m_Width = 1;
    private int m_Height = 1;
    private ByteOrder m_ByteOrder = ByteOrder.Mac;
    private bool m_FlipVertically = false;

    private int xoff;
    private int yoff;


    [MenuItem("Terrain/Create Tiled Terrain")]
    public static void CreateWindow()
    {
        window = EditorWindow.GetWindow(typeof(CreateTiledTerrain));
        window.title = "Tiled Terrain";
        window.minSize = new Vector2(500f, 700f);


    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        tileAmount = EditorGUILayout.Vector2Field("Amount", tileAmount);
        EditorGUILayout.EndHorizontal();

        xoff = EditorGUILayout.IntField("X offset", xoff);
        yoff = EditorGUILayout.IntField("Y offset", yoff);


        width = EditorGUILayout.IntField("Terrain Width", width);
        lenght = EditorGUILayout.IntField("Terrain Lenght", lenght);
        height = EditorGUILayout.IntField("Terrain Height", height);

        EditorGUILayout.Space();

        heightmapResoltion = EditorGUILayout.IntField("Heightmap Resoltion", heightmapResoltion);
        heightmapResoltion = Mathf.ClosestPowerOfTwo(heightmapResoltion) + 1;
        heightmapResoltion = Mathf.Clamp(heightmapResoltion, 33, 4097);

        detailResolution = EditorGUILayout.IntField("Detail Resolution", detailResolution);
        detailResolution = Mathf.ClosestPowerOfTwo(detailResolution);
        detailResolution = Mathf.Clamp(detailResolution, 0, 4097);

        detailResolutionPerPatch = EditorGUILayout.IntField("Detail Resolution Per Patch", detailResolutionPerPatch);
        detailResolutionPerPatch = Mathf.ClosestPowerOfTwo(detailResolutionPerPatch);
        detailResolutionPerPatch = Mathf.Clamp(detailResolutionPerPatch, 8, 128);

        controlTextureResolution = EditorGUILayout.IntField("Control Texture Resolution", controlTextureResolution);
        controlTextureResolution = Mathf.ClosestPowerOfTwo(controlTextureResolution);
        controlTextureResolution = Mathf.Clamp(controlTextureResolution, 16, 512);

        baseTextureReolution = EditorGUILayout.IntField("Base Texture Reolution", baseTextureReolution);
        baseTextureReolution = Mathf.ClosestPowerOfTwo(baseTextureReolution);
        baseTextureReolution = Mathf.Clamp(baseTextureReolution, 16, 1024);

        EditorGUILayout.Space();
        prefix = EditorGUILayout.TextField("Tiles Prefix:", prefix);

        EditorGUILayout.Space();
        GUILayout.Label("Import tiles in path :");
        originpath = EditorGUILayout.TextField("", originpath);
        if (GUILayout.Button("Open"))
        {

            originpath = EditorUtility.OpenFolderPanel("Load Tiles", "", "");
            //ValidatePath();
            //CreateTerrain();

            //path = string.Empty;
        }

        EditorGUILayout.Space();
        GUILayout.Label("Path were to save TerrainDate:");
        path = EditorGUILayout.TextField("Assets/", path);

        if (GUILayout.Button("Create"))
        {
            ValidatePath();
            //GameObject parent = (GameObject)Instantiate(new GameObject("Terrain"));
            //parent.transform.position = new Vector3(0, 0, 0);



            CreateTerrain();

            path = string.Empty;
        }


    }

    private void ValidatePath()
    {
        if (path == string.Empty) path = "TiledTerrain/TerrainData/";

        string pathToCheck = Application.dataPath + "/" + path;
        if (Directory.Exists(pathToCheck) == false)
        {
            Directory.CreateDirectory(pathToCheck);
        }
    }

    private void CreateTerrain()
    {


        GameObject parent = (GameObject)Instantiate(new GameObject("Terrain"));
        parent.transform.position = new Vector3(0, 0, 0);


        //FV Trovare un modo migliore per generare l'alfabeto
        int i = 0;
      // for (char Outer = 'A'; Outer <= 'Z'; Outer++)
        {
            for (char Inner = 'A'; Inner <= 'Z'; Inner++)
            {
                      //     outVal[i] = Outer.ToString() + Inner.ToString();
                           outVal[i] = Inner.ToString();

                i++;
            }
        }
        Debug.Log("xam: " + tileAmount.x + " yam: " + tileAmount.y);

        //i = 0;

         i = xoff;
        
        // i = 8;
        //for (int x = 1; x <= tileAmount.x; x++)
        TerrainData terrainData;
        GameObject terrain;
        Texture2D currentTexture;
        for (int x = 1; x <= tileAmount.x; x++)
        {
            for (int y = 01; y <= tileAmount.y; y++)
            {
                 y = y + yoff;
                //		int x = 1;
                //		int y = 0;
              //  string name = prefix + "_" + outVal[i] + "_" + y.ToString("00");
                string name = prefix + "_" + outVal[i] + "" + y.ToString();
                m_Path = originpath + "/" + name + ".raw";

                //		m_Path = "/Users/fxbio6600/Downloads/Tiles_523x512/Tiles_AT_20.raw";

                terrainData = new TerrainData();
                terrainData.size = new Vector3(width / 16f, height, lenght / 16f);

                terrainData.baseMapResolution = baseTextureReolution;
                terrainData.heightmapResolution = heightmapResoltion;
                terrainData.alphamapResolution = controlTextureResolution;
                terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

                terrainData.name = name;
                terrain = (GameObject)Terrain.CreateTerrainGameObject(terrainData);

                terrain.name = name;
                terrain.transform.parent = parent.transform;
                //terrain.transform.position = new Vector3((lenght - 1) * (x - 1), 0, (width - 1) * (y - 1));
                terrain.transform.position = new Vector3((width - 1) * (-y + 1), 0, (lenght - 1) * (x - 1));

                AssetDatabase.CreateAsset(terrainData, "Assets/" + path + name + ".asset");

                Debug.Log(m_Path);
                Debug.Log(x + " " + y);

                m_Terrain = terrain.GetComponent<Terrain>();

                PickRawDefaults(m_Path);
                terrainData.heightmapResolution = Mathf.Max(m_Width, m_Height);
                terrainData.size = m_TerrainSize;


                ReadRaw(m_Path);

                currentTexture = (Texture2D)Resources.Load("Textures/" + name, typeof(Texture2D));

                terrain.GetComponent<Terrain>().AddTexture(currentTexture, width - 1);
                Resources.UnloadAsset(currentTexture);

             
    terrain.GetComponent<Terrain>().Flush();


                float bar = (float)(y + (x - 1) * tileAmount.y) / (float)(tileAmount.x * tileAmount.y);
                EditorUtility.DisplayProgressBar("Progress Bar", "Import running..." + (y + (x - 1) * tileAmount.y).ToString() + " / " + (tileAmount.x * tileAmount.y).ToString() + " -> " + (bar * 100).ToString("0.0") + "%", bar);


                terrainData = null;
                terrain = null;



                y = y - yoff;

            }
            i++;



        }
        EditorUtility.ClearProgressBar();


    }




    void PickRawDefaults(string path)
    {
        FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);
        int fileSize = (int)file.Length;
        file.Close();

        m_TerrainSize = terrainData.size;

        if (terrainData.heightmapWidth * terrainData.heightmapHeight == fileSize)
        {
            m_Width = terrainData.heightmapWidth;
            m_Height = terrainData.heightmapHeight;
            m_Depth = Depth.Bit8;
        }
        else if (terrainData.heightmapWidth * terrainData.heightmapHeight * 2 == fileSize)
        {
            m_Width = terrainData.heightmapWidth;
            m_Height = terrainData.heightmapHeight;
            m_Depth = Depth.Bit16;
        }
        else
        {
            m_Depth = Depth.Bit16;

            int pixels = fileSize / (int)m_Depth;
            int width = Mathf.RoundToInt(Mathf.Sqrt(pixels));
            int height = Mathf.RoundToInt(Mathf.Sqrt(pixels));
            if ((width * height * (int)m_Depth) == fileSize)
            {
                m_Width = width;
                m_Height = height;
                return;
            }


            m_Depth = Depth.Bit8;

            pixels = fileSize / (int)m_Depth;
            width = Mathf.RoundToInt(Mathf.Sqrt(pixels));
            height = Mathf.RoundToInt(Mathf.Sqrt(pixels));
            if ((width * height * (int)m_Depth) == fileSize)
            {
                m_Width = width;
                m_Height = height;
                return;
            }

            m_Depth = Depth.Bit16;
        }
    }

    void ReadRaw(string path)
    {
        // Read data
        byte[] data;
        using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
        {
            data = br.ReadBytes(m_Width * m_Height * (int)m_Depth);
            br.Close();
        }

        int heightmapWidth = terrainData.heightmapWidth;
        int heightmapHeight = terrainData.heightmapHeight;
        float[,] heights = new float[heightmapHeight, heightmapWidth];
        if (m_Depth == Depth.Bit16)
        {
            float normalize = 1.0F / (1 << 16);
            for (int y = 0; y < heightmapHeight; ++y)
            {
                for (int x = 0; x < heightmapWidth; ++x)
                {
                    int index = Mathf.Clamp(x, 0, m_Width - 1) + Mathf.Clamp(y, 0, m_Height - 1) * m_Width;
                    if ((m_ByteOrder == ByteOrder.Mac) == System.BitConverter.IsLittleEndian)
                    {
                        // Yay, seems like this is the easiest way to swap bytes in C#. NUTS
                        byte temp;
                        temp = data[index * 2];
                        data[index * 2 + 0] = data[index * 2 + 1];
                        data[index * 2 + 1] = temp;
                    }

                    ushort compressedHeight = System.BitConverter.ToUInt16(data, index * 2);

                    float height = compressedHeight * normalize;
                    int destY = m_FlipVertically ? heightmapHeight - 1 - y : y;
                    heights[destY, x] = height;
                }
            }
        }
        else
        {
            float normalize = 1.0F / (1 << 8);
            for (int y = 0; y < heightmapHeight; ++y)
            {
                for (int x = 0; x < heightmapWidth; ++x)
                {
                    int index = Mathf.Clamp(x, 0, m_Width - 1) + Mathf.Clamp(y, 0, m_Height - 1) * m_Width;
                    byte compressedHeight = data[index];
                    float height = compressedHeight * normalize;
                    int destY = m_FlipVertically ? heightmapHeight - 1 - y : y;
                    heights[destY, x] = height;
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }


}