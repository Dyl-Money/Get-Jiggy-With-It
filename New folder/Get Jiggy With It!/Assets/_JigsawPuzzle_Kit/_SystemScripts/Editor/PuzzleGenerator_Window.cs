//----------------------------------------------------------------------------------------------------------------------------------------------------------
// Editor script - Generates puzzle atlas and textures according to settings, create/place puzzle-pieces in the scene
//----------------------------------------------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



[ExecuteInEditMode]
public class PuzzleGenerator_Window : EditorWindow 
{

	static Texture2D image;				// Will be used as main puzzle image
	static Texture2D subElement;		// Will be used for sub-elements generation
	static Material material;			// Puzzle custom material
    static Material assembledMaterial;  // Material for assembled pieces
    static int cols = 2;				// Puzzle grid columns number
	static int rows = 2;				// Puzzle rows columns number

	static int elementBaseSize = 200;	// Size of puzzle piece base
	static int maxAtlasSize = 8192;		// Max Atlas size
	static int pixelsPerUnit = 200;		// Sprites resolution

    static bool generateBackground;     // Automatically generate puzzle background fronm the source image

    // Atlas and shadow settings
    static string texturePath = "Assets/_Atlas.png";
	static bool useShadows;
	static Vector3 shadowOffset = new Vector3(0.1f, -0.1f, 1);
	static Color shadowColor = new Color(0, 0, 0, 0.5f);

	// Important internal variables - please don't change them blindly
	//static PuzzleGenerator_Window window;	 
	static Vector2 windowSize = new Vector2(450, 377);
	static Vector2 newWindowSize;
	static Color guiColor;
	static Texture2D oldSubElement;	
	static int oldElementBaseSize;
	static Texture2D piecePreview;

    GUIStyle guiStyle  = new GUIStyle();
	 Rect[] atlasRects; 
	 Texture2D atlas;
     

	// Contatins data about whole puzzle
	PuzzleElement[] puzzleGrid;
    PuzzleController puzzle;

    static bool initialized;


    //----------------------------------------------------------------------------------------------------
    // Initialize menu window
    [MenuItem ("Tools/Puzzle Generator")]
	static void OnEnable ()
	{
        // Get existing open window or if none, make a new one:		
        PuzzleGenerator_Window window = (PuzzleGenerator_Window)EditorWindow.GetWindow(typeof(PuzzleGenerator_Window), true, "PuzzleGenerator", true);
        window.position = new Rect((Screen.width - windowSize.x) / 2 + 100, (Screen.height - windowSize.y) / 2 + 200, windowSize.x, windowSize.y);

        if (!initialized)
        {
            guiColor = GUI.color;

            PuzzleUtitlities.AddPuzzleTags();
            initialized = true;
        }
	}


	//----------------------------------------------------------------------------------------------------
	// Draw whole interface
	void OnGUI()
	{  
		newWindowSize = windowSize; 

		// General image settings
		EditorGUILayout.Space();	  
		EditorGUILayout.LabelField("General Puzzle settings", EditorStyles.boldLabel);	 
		GUILayout.BeginHorizontal("box");
			image = EditorGUILayout.ObjectField(new GUIContent("Puzzle image", "Image will be used as main puzzle image") ,image, typeof(Texture2D), false) as Texture2D;
			EditorGUILayout.Space();

			GUILayout.BeginVertical();
				cols = EditorGUILayout.IntField(new GUIContent("Columns amount:", "Desired amount of elements(pieces) in a row of new puzzle"), cols, GUILayout.MaxWidth(200));	 
				rows = EditorGUILayout.IntField(new GUIContent("Rows amount:", "Desired amount of elements(pieces) in a column of new puzzle"), rows, GUILayout.MaxWidth(200));

				cols = Mathf.Clamp (cols, 2, 35);
				rows = Mathf.Clamp (rows, 2, 35);	

			GUILayout.EndVertical();
		GUILayout.EndHorizontal();


		// Puzzle-pieces settings
		EditorGUILayout.Space();	  
		EditorGUILayout.LabelField("Puzzle-pieces settings", EditorStyles.boldLabel);	 
		GUILayout.BeginVertical ("box");
			GUILayout.BeginHorizontal();
				subElement = EditorGUILayout.ObjectField(new GUIContent("Sub-element image", "Image will be used for puzzle pieces sub-elements generation") ,subElement, typeof(Texture2D), false) as Texture2D;

				if (subElement) 
				{
					newWindowSize.y = newWindowSize.y + 15;
					EditorGUILayout.LabelField("Puzzle-piece sample:");	 
					// Prepare new puzzle-piece preview if needed
					if (elementBaseSize != oldElementBaseSize  ||  subElement != oldSubElement)  
						try
						{
                            elementBaseSize = Mathf.Clamp(elementBaseSize, subElement.width * 2, subElement.width * 4);
                            subElement = TextureUtility.PrepareAsset(subElement);
							piecePreview = GenerateExamplePiece (subElement, elementBaseSize);
							oldSubElement = subElement;
							oldElementBaseSize = elementBaseSize;
						}
						catch(System.Exception ex)
							{
								EditorUtility.DisplayDialog("ERROR", "SOMETHING GONE WRONG! \n \n" + ex.Message, "OK");
								subElement = null;
							}

					if (piecePreview)
                        EditorGUI.DrawTextureTransparent(new Rect(352,136,90,90), piecePreview, ScaleMode.ScaleToFit);
				}
				else
					EditorGUILayout.LabelField("<< Choose image to see sample");	

			GUILayout.EndHorizontal();

			EditorGUILayout.Space(); 
			EditorGUILayout.Space(); 
			if (subElement)
                elementBaseSize = EditorGUILayout.IntSlider(new GUIContent("Base element size:", "Size of puzzle piece base (without sub-elements)"), elementBaseSize, subElement.width*2, subElement.width*4, GUILayout.MaxWidth(340));
		GUILayout.EndVertical();


		// Important generation settings
		EditorGUILayout.Space();	
		EditorGUILayout.LabelField("Generation settings", EditorStyles.boldLabel);	 
		// Sizes
		GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					pixelsPerUnit = EditorGUILayout.IntField(new GUIContent("Sprites size:", "Use this pixelsPerUnit value for sprites(puzzle pieces) generation - affects size in game-world (smaller value - bigger pieces)"), pixelsPerUnit, GUILayout.MaxWidth(200));
					pixelsPerUnit = Mathf.Clamp (pixelsPerUnit, 10, 1024);
					maxAtlasSize = EditorGUILayout.IntField(new GUIContent("Atlas size:", "Maximal allowed size for generated atlas texture - affects quality"), maxAtlasSize, GUILayout.MaxWidth(200));	
					maxAtlasSize = Mathf.Clamp (maxAtlasSize, 256, 8192);
				GUILayout.EndVertical();

				EditorGUILayout.Space();

				// Optional shadow settings
				GUILayout.BeginVertical();

                    generateBackground = EditorGUILayout.Toggle(new GUIContent("Generate Background", "Automatically generate puzzle background fronm the source image"), generateBackground);

                    useShadows = EditorGUILayout.Toggle(new GUIContent("Generate Shadows", "Generate additional sprites to simulate shadows under puzzle-pieces"), useShadows);
					if (useShadows)
					{
						shadowOffset = EditorGUILayout.Vector3Field(new GUIContent("Shadows offset", "Offset of shadow relatively to puzzle-piece"), shadowOffset);
						shadowColor =  EditorGUILayout.ColorField(new GUIContent("Shadows color", "Please pay attention to Alpha"), shadowColor);
						newWindowSize.y = newWindowSize.y + 50;
					}
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Additional		
			texturePath = EditorGUILayout.TextField(new GUIContent("Atlas name:", "Name of generated atlas-asset, should be in 'Assets' folder. You can put it into subfolder adding path in front of it"), texturePath);
			if(File.Exists(texturePath)) 
			{
				guiStyle.normal.textColor = Color.red; 
				guiStyle.fontSize = 10;
				newWindowSize.y = newWindowSize.y + 20;
				EditorGUILayout.LabelField("Atlas with this name already exists - Generation will overwrite it!", guiStyle);	

			}

			EditorGUILayout.Space();
			material = EditorGUILayout.ObjectField(new GUIContent("Use material:", "This custom Material will be used for generated puzzle-elements") ,material, typeof(Material), false) as Material;

            EditorGUILayout.Space();
            assembledMaterial = EditorGUILayout.ObjectField(new GUIContent("Use assemble material:", "This custom Material will be used for assembled state of puzzle-elements"), assembledMaterial, typeof(Material), false) as Material;

        GUILayout.EndVertical();


		// Generation trigger-button
		if (image  &&  subElement) 
		{
			newWindowSize.y = newWindowSize.y + 35; 	  
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUI.color = Color.yellow;
			if (GUILayout.Button("GENERATE PUZZLE")) 
				CreatePuzzle ();

			GUI.color = guiColor;
		}


		if (this.position.size != newWindowSize)  
			this.position = new Rect(this.position.x, this.position.y, newWindowSize.x, newWindowSize.y);	 

	}


	//============================================================================================================================================================
	// Aggregate function, that processes whole generation
	void CreatePuzzle () 
	{
		Random.InitState(System.DateTime.Now.Millisecond);

		puzzleGrid = new PuzzleElement[cols*rows];

		try 
		{
			image = TextureUtility.PrepareAsset(image);

			GeneratePuzzlePieces (cols, rows, subElement, elementBaseSize,  image);
			CreateAtlas ();
			ConvertToSprites();
            puzzle = CreateGameObjects().AddComponent<PuzzleController>();
            puzzle.pieceMaterial_assembled = assembledMaterial;
            puzzle.Prepare();

            if (generateBackground)
                puzzle.GenerateBackground(image);

        }
		catch(System.Exception ex)
			{
				EditorUtility.DisplayDialog ("ERROR", "SOMETHING GONE WRONG! \n \n" + ex.Message, "OK");
			}


		EditorUtility.ClearProgressBar(); 
	}

	//-----------------------------------------------------------------------------------------------------
	// Generate puzzle-pieces gameObjects and compose them in the scene
	GameObject CreateGameObjects () 
	{
		// Load sprites assets
		Object[] spritePieces = AssetDatabase.LoadAllAssetsAtPath(texturePath);
		ArrayUtility.RemoveAt(ref spritePieces, 0);

        // Calculate final sprite size, taking into account pixelsPerUnit and possible atlas scale
        Vector2 spriteBaseSize = new Vector2(
                                                image.width  / (float)cols / pixelsPerUnit / (puzzleGrid[0].texture.width  / (spritePieces[0] as Sprite).rect.width),
                                                image.height / (float)rows / pixelsPerUnit / (puzzleGrid[0].texture.height / (spritePieces[0] as Sprite).rect.height)
                                            );


        GameObject puzzle = new GameObject();
		GameObject piece;
		GameObject shadow;
		SpriteRenderer spriteRenderer;
        SpriteRenderer shadowRenderer;


        puzzle.name = "Puzzle_" + image.name + "_" + cols.ToString() + "x" + rows.ToString();

		// Go through array and create gameObjects
		for (int y = 0; y < rows; y++) 
			for (int x = 0; x < cols; x++)
			{    
				EditorUtility.DisplayProgressBar( "Step 4/4", "Create GameObjects...", (y*cols + x+0.1f)/(rows*cols+0.1f));

				// Generate sprite
				piece = new GameObject();
				piece.name = "piece_" + x.ToString() + "x" + y.ToString();
				piece.transform.SetParent(puzzle.transform);

				piece.transform.position = new Vector3(x * spriteBaseSize.x, -y * spriteBaseSize.y, 0);

                spriteRenderer = piece.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = spritePieces[y * cols + x] as Sprite;
 

                // Generate shadow as darkened copy of originalsprite
                if (useShadows)
				{
					shadow = Instantiate(piece);
					shadow.transform.parent = piece.transform;
					shadow.transform.localPosition = shadowOffset;
					shadow.name = piece.name + "_Shadow";

                    shadowRenderer = shadow.GetComponent<SpriteRenderer>();
                    shadowRenderer.color = shadowColor;
                    shadowRenderer.sortingOrder = -1;
                }

				// Assign custom material to puzzle-piece (if neended)
				if (material)
					spriteRenderer.material = material; 

			}
        
        return puzzle;
	}

	//-----------------------------------------------------------------------------------------------------
	// Convert atlas texture to Multiple sprite sheet		
	void ConvertToSprites () 
	{
        // Create and initialize sprites
        SpriteMetaData[] sprites  = new SpriteMetaData[atlasRects.Length];
		for (int i = 0; i < sprites.Length; i++) 
		{
			sprites[i].alignment = (int)SpriteAlignment.Custom;
			sprites[i].name = "piece_" + i.ToString();
			sprites[i].pivot = new Vector2 (puzzleGrid[i].pivot.x, puzzleGrid[i].pivot.y);
			sprites[i].rect = new Rect(atlasRects[i].x * atlas.width, atlasRects[i].y * atlas.height, atlasRects[i].width * atlas.width, atlasRects[i].height * atlas.height);

			EditorUtility.DisplayProgressBar("Step 3/4", "Convert textures to Sprites...", i/(sprites.Length-1.1f));
		}

		// Modify the importer settings
		TextureImporter atlasTextureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        atlasTextureImporter.isReadable = true;
        atlasTextureImporter.mipmapEnabled = false;
        atlasTextureImporter.alphaIsTransparency = true;
        //  atlasTextureImporter.maxTextureSize = maxAtlasSize;
        atlasTextureImporter.wrapMode = TextureWrapMode.Clamp;
        atlasTextureImporter.filterMode = FilterMode.Trilinear;
        atlasTextureImporter.spritePixelsPerUnit = pixelsPerUnit;
        atlasTextureImporter.textureType = TextureImporterType.Sprite;
		atlasTextureImporter.spriteImportMode = SpriteImportMode.Multiple;
		atlasTextureImporter.spritesheet = sprites;


        AssetDatabase.WriteImportSettingsIfDirty(texturePath);
		AssetDatabase.ImportAsset(texturePath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

    //-----------------------------------------------------------------------------------------------------
    // Pack Textures(puzzle-pieces) to atlas 
    void CreateAtlas()
    {
        // Import all textures to textureArray
        Texture2D[] textureArray = new Texture2D[rows * cols];
        for (int i = 0; i < textureArray.Length; i++)
        {
            textureArray[i] = puzzleGrid[i].texture;
            EditorUtility.DisplayProgressBar("Step 2/4  (This may take awhile)", "Save textures to atlas...", i / (textureArray.Length - 1.1f));
        }

        // Make a new atlas texture
        atlas = new Texture2D(1, 1);
        atlasRects = atlas.PackTextures(textureArray, 3);

        // Scale atlas if source bigger than chosen atlas size  
        if (maxAtlasSize < atlas.width  ||  maxAtlasSize < atlas.height)      
            if (atlas.width == atlas.height)
                atlas = TextureUtility.Scale(atlas, maxAtlasSize, maxAtlasSize);
            else
                if (atlas.width > atlas.height)
                    atlas = TextureUtility.Scale(atlas, maxAtlasSize, Mathf.RoundToInt(atlas.height / (atlas.width / (float)maxAtlasSize)));
                else
                    atlas = TextureUtility.Scale(atlas, Mathf.RoundToInt(atlas.width / (atlas.height / (float)maxAtlasSize)), maxAtlasSize);
            

		byte[] atlasPng  = atlas.EncodeToPNG();

		if (File.Exists(texturePath))
			File.Delete(texturePath);

		File.WriteAllBytes(texturePath, atlasPng);

		AssetDatabase.ImportAsset(texturePath);
        AssetDatabase.Refresh();
    }
    
    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Generate puzzle-pieces textures and order them in puzzleGrid
    Vector2 GeneratePuzzlePieces(int _cols, int _rows, Texture2D _subElement, int _elementBaseSize, Texture2D _image)
    {
        int top, left, bottom, right;

        // Calculate piece aspect-ratio accordingly to image size    
        Vector2 elementSizeRatio = new Vector2(_image.width / (float)_cols / elementBaseSize, _image.height / (float)_rows / _elementBaseSize);


        // Prepare sub-element variants
        Color[] subElementPixels = _subElement.GetPixels();
        Color[] topPixels = subElementPixels;
        Color[] leftPixels = TextureUtility.Rotate90(subElementPixels, _subElement.width, _subElement.height, false);


        // Generation													                                          
        for (int y = 0; y < _rows; y++)
            for (int x = 0; x < _cols; x++)
            {
                EditorUtility.DisplayProgressBar("Step 1/4", "Generating textures...", (y * _cols + x) / (_rows * _cols + 0.1f));

                // Calculate shape - which type/variant of sub-elements should be  used for top/left/bottom/right parts of piece (accordingly to shapes of surrounding puzzle-pieces) 
                //	(0 - flat, 1-convex, 2-concave)	
                top     =  y > 0            ?  -puzzleGrid[((y - 1) * _cols + x)].bottom  :  0;
                left    =  x > 0            ?  -puzzleGrid[(y * _cols + x - 1)].right     :  0;
                bottom  =  y < (_rows - 1)  ?  Random.Range(-1, 1) * 2 + 1  :  0;
                right   =  x < (_cols - 1)  ?  Random.Range(-1, 1) * 2 + 1  :  0;


                // Prepare element mask 
                puzzleGrid[y * _cols + x] = new PuzzleElement(
                                                                top, left, bottom, right,
                                                                _elementBaseSize,
                                                                _subElement,
                                                                topPixels, leftPixels
                                                             );

                // Extract and mask image-piece to be used as puzzle-piece texture
                puzzleGrid[y * _cols + x].texture = ExtractFromImage(_image, puzzleGrid[y * _cols + x], x, y, _elementBaseSize, elementSizeRatio);

                // Set pivot to Left-Top corner of puzzle-piece base
                puzzleGrid[y * _cols + x].pivot = new Vector2(
                                                                ((float)puzzleGrid[y * _cols + x].pixelOffset.x / puzzleGrid[y * _cols + x].texture.width * elementSizeRatio.x),
                                                                (1.0f - (float)puzzleGrid[y * _cols + x].pixelOffset.y / puzzleGrid[y * _cols + x].texture.height * elementSizeRatio.y)
                                                            );
            }


        return elementSizeRatio;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------  
    // Extract and mask image-piece to be used as puzzle-piece texture
    Texture2D ExtractFromImage(Texture2D _image, PuzzleElement _puzzlElement, int _x, int _y, int _elementBaseSize, Vector2 _elementSizeRatio)
    {
        // Get proper piece of image 
        Color[] pixels = _image.GetPixels
                            (
                                (int)((_x * _elementBaseSize - _puzzlElement.pixelOffset.x) * _elementSizeRatio.x),
                                (int)(_image.height - (_y + 1) * _elementBaseSize * _elementSizeRatio.y - _puzzlElement.pixelOffset.height * _elementSizeRatio.y),
                                (int)(_puzzlElement.maskWidth * _elementSizeRatio.x),
                                (int)(_puzzlElement.maskHeight * _elementSizeRatio.y)
                            );


        Texture2D result = new Texture2D(
                                            (int)(_puzzlElement.maskWidth * _elementSizeRatio.x),
                                            (int)(_puzzlElement.maskHeight * _elementSizeRatio.y)
                                        );

        // Apply mask
        result.wrapMode = TextureWrapMode.Clamp;
        _puzzlElement.ApplyMask(pixels, ref result);
        
        return result;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Generates example puzzle-piece/ to be shown as preview in Editor window
    Texture2D GenerateExamplePiece(Texture2D _subElement, int _elementBaseSize)
    {
        PuzzleElement puzzlePiece;

        // Calculate randomly which sub-element variants should be used
        int top = Random.Range(-1, 1) * 2 + 1;
        int left = Random.Range(-1, 1) * 2 + 1;
        int bottom = Random.Range(-1, 1) * 2 + 1;
        int right = Random.Range(-1, 1) * 2 + 1;


        // Prepare sub-element variants
        Color[] subElementPixels = _subElement.GetPixels();
        Color[] topPixels = subElementPixels;
        Color[] leftPixels = TextureUtility.Rotate90(subElementPixels, _subElement.width, _subElement.height, false);


        // Prepare element mask  
        puzzlePiece = new PuzzleElement
            (
                top, left, bottom, right,
                _elementBaseSize,
                _subElement,
                topPixels, leftPixels
            );

        // Create resultTexture
        puzzlePiece.texture = new Texture2D(puzzlePiece.maskWidth, puzzlePiece.maskHeight);

        Color[] piecePreviewColor = new Color [puzzlePiece.maskWidth*puzzlePiece.maskHeight];
        for (int i = 0; i < piecePreviewColor.Length; i++)
                piecePreviewColor[i] = Color.black;     

        puzzlePiece.ApplyMask (piecePreviewColor, ref puzzlePiece.texture);


        return puzzlePiece.texture; 
	}  

	//----------------------------------------------------------------------------------------------------

}