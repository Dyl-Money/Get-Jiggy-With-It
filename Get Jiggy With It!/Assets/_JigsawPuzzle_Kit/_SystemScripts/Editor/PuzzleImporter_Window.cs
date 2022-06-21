//-----------------------------------------------------------------------------------------------------	
// Editor script - Creates puzzle
// Generates atlas and textures, create/arrange/place objects according to info in Data-file
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



public class PuzzleImporter_Window : EditorWindow
{
	//=====================================================================================================	
	public class PuzzleLayerData
	{
		public string name;
		public Vector3 offset;
		public Vector2 size;
	}
	//=====================================================================================================	


	public TextAsset inputFile;				// File with data about objects
	public string inputFilePath;			// Path to data-file
	public string localDirectory;			// Directory with .png files

	public static Material material;        // Puzzle custom material
    public static Material assembledMaterial;// Material for assembled pieces
    public static int maxAtlasSize = 8192;	// Max Atlas size
	public static int pixelsPerUnit = 100;  // Sprites resolution

	// Atlas and shadow settings
	public static string texturePath = "Assets/_Atlas.png";
	public static bool useShadows;
	public static Vector3 shadowOffset = new Vector3(0.1f, -0.1f, 1);
	public static Color shadowColor = new Color(0, 0, 0, 0.5f);


	// Important internal variables - please don't change them blindly
	string[] lines;
	PuzzleLayerData[] layers;
	Texture2D[] textureArray;
	TextAsset oldInputFile;
	Rect[] atlasRects; 
	Texture2D atlas;
		 
	static Vector2 windowSize = new Vector2(450, 220);
	static Vector2 newWindowSize;
	static Color guiColor;
	static GUIStyle guiStyle  = new GUIStyle();

    static bool initialized;


    //-----------------------------------------------------------------------------------------------------	
    // Init editor interface window
    [MenuItem ("Tools/Puzzle Importer")]
	static void Init()
	{
       // Get existing open window or if none, make a new one:		
        PuzzleImporter_Window window = EditorWindow.GetWindow(typeof(PuzzleImporter_Window), true, "PuzzleImporter", true) as PuzzleImporter_Window;
        window.position = new Rect((Screen.width - windowSize.x) / 2 + 100, (Screen.height - windowSize.y) / 2 + 100, windowSize.x, windowSize.y);

        if (!initialized)
        {
            guiColor = GUI.color;
            guiStyle.normal.textColor = Color.red;
            guiStyle.fontSize = 10;

            PuzzleUtitlities.AddPuzzleTags();
            initialized = true;
        }

    }

	//-----------------------------------------------------------------------------------------------------
	// Create UI
	void OnGUI() 
	{
		newWindowSize = windowSize;

		// General image settings
		EditorGUILayout.Space();	  

		EditorGUILayout.LabelField("Puzzle source  settings", EditorStyles.boldLabel);	 
		GUILayout.BeginVertical("box");
			inputFile = EditorGUILayout.ObjectField(new GUIContent("Data file:", "Choose puzzle data-File to Import!") ,inputFile, typeof(TextAsset), false) as TextAsset;

			if (inputFile  &&  oldInputFile != inputFile)
			{
				inputFilePath = AssetDatabase.GetAssetPath(inputFile);
                localDirectory += localDirectory[2];
                localDirectory = localDirectory.Substring(localDirectory.IndexOf("Assets"));

				ParseTextFile ();   

				oldInputFile = inputFile;
			}

			if (localDirectory != ""  &&  layers != null) 
			{
				EditorGUILayout.LabelField(new GUIContent("Data-directory:  " + localDirectory, "Directory with .png files"));
				EditorGUILayout.LabelField(new GUIContent("Elements amount:  " + layers.Length.ToString(), "Number of puzzle-pieces found in data-file"));
				newWindowSize.y = newWindowSize.y + 35;
			}

		GUILayout.EndVertical();



		// Important generation settings
		EditorGUILayout.Space();	
		EditorGUILayout.LabelField("Generation settings", EditorStyles.boldLabel);	 
		// Sizes
		GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					pixelsPerUnit = EditorGUILayout.IntField(new GUIContent("Sprites size:", "Use this pixelsPerUnit value for sprites(puzzle pieces) generation - affects size in game-world"), pixelsPerUnit, GUILayout.MaxWidth(200));
					pixelsPerUnit = Mathf.Clamp (pixelsPerUnit, 10, 1024);
					maxAtlasSize = EditorGUILayout.IntField(new GUIContent("Atlas size:", "Maximal allowed size for generated atlas texture - affects quality"), maxAtlasSize, GUILayout.MaxWidth(200));	
					maxAtlasSize = Mathf.Clamp (maxAtlasSize, 1024, 8192);
				GUILayout.EndVertical();

				EditorGUILayout.Space();

				// Optional shadow settings
				GUILayout.BeginVertical();
					useShadows = EditorGUILayout.Toggle(new GUIContent("Generate shadows", "Generate additional sprites to simulate shadows under puzzle-pieces"), useShadows);
					if (useShadows)
					{
						shadowOffset = EditorGUILayout.Vector3Field(new GUIContent("Shadows offset", "Offset of shadow relatively to puzzle-piece"), shadowOffset);
						shadowColor =  EditorGUILayout.ColorField(new GUIContent("Shadows color", "Please pay attention to Alpha"), shadowColor);
						newWindowSize.y = newWindowSize.y + 35;
					}
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// Additional		
		texturePath = EditorGUILayout.TextField(new GUIContent("Atlas name:", "Name of generated atlas-asset, should be in 'Assets' folder. You can put it into subfolder adding path in front of it"), texturePath);
		if (File.Exists(texturePath)) 
		{
			newWindowSize.y = newWindowSize.y + 20;
			EditorGUILayout.LabelField("Atlas with this name already exists - Generation will overwrite it!", guiStyle);	
		}

        EditorGUILayout.Space();
        material = EditorGUILayout.ObjectField(new GUIContent("Use material:", "This custom Material will be used for generated puzzle-elements"), material, typeof(Material), false) as Material;

        EditorGUILayout.Space();
        assembledMaterial = EditorGUILayout.ObjectField(new GUIContent("Use assemble material:", "This custom Material will be used for assembled state of puzzle-elements"), assembledMaterial, typeof(Material), false) as Material;

        GUILayout.EndVertical();



		if(inputFile)  
		{
			newWindowSize.y = newWindowSize.y + 30;

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUI.color = Color.yellow;
			if(GUILayout.Button("GENERATE PUZZLE"))
			{ 
				// Generate objects and compose scene    		   
				CreateAtlas ();
				ConvertToSprites ();
                PuzzleController puzzle = CreateGameObjects().AddComponent<PuzzleController>();
                puzzle.pieceMaterial_assembled = assembledMaterial;
                puzzle.Prepare();

                EditorUtility.ClearProgressBar();	  
			}

			GUI.color = guiColor;	 
		} 


		if (this.position.size != newWindowSize)  
			this.position = new Rect(this.position.x, this.position.y, newWindowSize.x, newWindowSize.y);
	}

	//-----------------------------------------------------------------------------------------------------
	// Parse TextFile  with data about objects
	// Info about all layer(each as separated .png ) saved with structure:
	//   Layer_3.png - layer/file name 
	//   723         - X position of layer in the image
	//   790         - Y position of layer in the image
	//   75          - X size of layer (width)
	//   91          - Y size of layer (height)
	//   ---------   - separator-string before information about next layer 
	void ParseTextFile () 
	{
		int layerID = 0;
		int stringNum = 0;

		lines = inputFile.text.Split("\n"[0]);
		layers = new PuzzleLayerData[lines.Length/6];

		for (int i = 0; i < layers.Length; i++)  
			layers[i] = new PuzzleLayerData();

		foreach (var line in lines)
		{
			if (layerID < layers.Length)
				switch (stringNum)
				{
					case 0: 
						layers[layerID].name = line;
						stringNum++;
						break;

					case 1: 
						layers[layerID].offset.x = int.Parse(line);
						stringNum++;
						break;

					case 2: 
						layers[layerID].offset.y = int.Parse(line);
						stringNum++;
						break;

					case 3: 
						layers[layerID].size.x = int.Parse(line);
						stringNum++;
						break;

					case 4: 
						layers[layerID].size.y = int.Parse(line);
						stringNum++;
						break;

					case 5: 
						layers[layerID].offset.z = layerID; 
						stringNum = 0;
						layerID++;
						break;
				}

			EditorUtility.DisplayProgressBar( "Preparation",	"Parsing data-file...", layerID/(layers.Length-1.1f));
		}

		EditorUtility.ClearProgressBar();
	}

	//-----------------------------------------------------------------------------------------------------
	// Pack Textures(puzzle-pieces) to atlas 
	void CreateAtlas ()
	{ 
		// Import all textures to textureArray
		textureArray = new Texture2D[layers.Length];
		for (int i = 0; i < textureArray.Length; i++)   
		{
			textureArray[i] = new Texture2D((int)layers[i].size.x, (int)layers[i].size.y);
			textureArray[i] = AssetDatabase.LoadAssetAtPath(localDirectory + layers[i].name, typeof(Texture2D)) as Texture2D;

			EditorUtility.DisplayProgressBar( "Step 1/3",	"Compose atlas...", i/(textureArray.Length-1.1f));
		}


		// Make a new atlas texture
		atlas = new Texture2D(maxAtlasSize, maxAtlasSize);
		atlasRects = atlas.PackTextures(textureArray, 2, maxAtlasSize);

		byte[] atlasPng = atlas.EncodeToPNG();


		if (File.Exists(texturePath))
		{
			File.Delete(texturePath);
			AssetDatabase.Refresh();
		}


		File.WriteAllBytes(texturePath, atlasPng);

		// AssetDatabase.WriteImportSettingsIfDirty(texturePath);
		AssetDatabase.ImportAsset(texturePath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}	

	//-----------------------------------------------------------------------------------------------------
	// Convert atlas texture to Multiple sprite sheet		
	void ConvertToSprites () 
	{ 	
		// Create and initialize sprites
		SpriteMetaData[] sprites = new SpriteMetaData[atlasRects.Length];
		for (int i = 0; i < sprites.Length; i++) 
		{
			sprites[i].alignment = (int)SpriteAlignment.Custom;
			sprites[i].name = "piece_" + i.ToString();
			//sprites[i].pivot = new Vector2 (puzzleGrid[i].pivot.x, puzzleGrid[i].pivot.y);
			sprites[i].rect = new Rect(atlasRects[i].x * atlas.width, atlasRects[i].y * atlas.height, atlasRects[i].width * atlas.width, atlasRects[i].height * atlas.height);

			EditorUtility.DisplayProgressBar( "Step 2/3", "Convert textures to Sprites...", i/(sprites.Length-1.1f));
		}

		// Modify the importer settings
		TextureImporter atlasTextureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
		atlasTextureImporter.isReadable = true;
		atlasTextureImporter.mipmapEnabled = false;
		atlasTextureImporter.alphaIsTransparency = true;  	    
		atlasTextureImporter.maxTextureSize = maxAtlasSize;
		atlasTextureImporter.wrapMode = TextureWrapMode.Clamp;
		atlasTextureImporter.filterMode = FilterMode.Point;
		atlasTextureImporter.textureType = TextureImporterType.Sprite;
		atlasTextureImporter.spritePixelsPerUnit = pixelsPerUnit;
		atlasTextureImporter.spriteImportMode = SpriteImportMode.Multiple;
		atlasTextureImporter.spritesheet = sprites;


		AssetDatabase.WriteImportSettingsIfDirty(texturePath);
		AssetDatabase.ImportAsset(texturePath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	//-----------------------------------------------------------------------------------------------------
	// Generate puzzle-pieces gameObjects and compose them in the scene
	GameObject CreateGameObjects ()  
	{
		// Load sprites assets
		Object[] spritePieces = AssetDatabase.LoadAllAssetsAtPath(texturePath);
		ArrayUtility.RemoveAt(ref spritePieces, 0);

		GameObject puzzle = new GameObject();
		GameObject piece;
		GameObject shadow;
		SpriteRenderer spriteRenderer;
		Vector2 adjustement;


		puzzle.name = "Puzzle_" + inputFilePath;

		// Go through array and create gameObjects
		for (int i = 0; i < layers.Length; i++)
		{    
			EditorUtility.DisplayProgressBar( "Step 3/3", "Create GameObjects...", i/(layers.Length-1.1f));

			// Generate sprite
			piece = new GameObject();
			piece.name = "piece_" + i.ToString();
			spriteRenderer = piece.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = spritePieces[i] as Sprite;
			piece.transform.SetParent(puzzle.transform);


			adjustement = new Vector2(layers[i].size.x / (atlasRects[i].width * atlas.width),  layers[i].size.y / (atlasRects[i].height * atlas.height) );
			piece.transform.position = new Vector3 
				(
					layers[i].offset.x/adjustement.x / pixelsPerUnit,
					-(layers[i].offset.y + layers[i].size.y)/adjustement.y / pixelsPerUnit,
					0
				);


			// Generate shadow as darkened copy of originalsprite
			if (useShadows)
			{
				shadow = Instantiate(piece);
				shadow.transform.parent = piece.transform;
				shadow.transform.localPosition = shadowOffset;
				shadow.name = piece.name + "_Shadow";
				shadow.GetComponent<SpriteRenderer>().color = shadowColor;
			}

			// Assign custom material to puzzle-piece (if neended)
			if (material) 
				spriteRenderer.material = material; 

		}

        return puzzle;
	}

	//-----------------------------------------------------------------------------------------------------
}