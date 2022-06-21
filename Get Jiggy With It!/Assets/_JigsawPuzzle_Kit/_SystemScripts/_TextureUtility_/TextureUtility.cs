//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
// Script contains bunch of utilities to manipulate Texture2D assets
//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEngine.Networking;



public static class TextureUtility 
{


	// Enums for more convenient setup
	public enum BooleanOperation {Union, Intersection, Subtraction};
	public enum Alignment {TopLeft, TopRight, TopCenter, BottomLeft, BottomRight, BottomCenter};
	public enum BlendMode {Normal, Additive, Subtraction, Multiply, Subdivide, MaskAlpha} 



	#if UNITY_EDITOR && !UNITY_WEBPLAYER
	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Prepare the texture to be operable by TextureUtility
	public static Texture2D PrepareAsset (Texture2D _source)
	{
		string texturePath = AssetDatabase.GetAssetPath(_source);

		// Modify the importer settings
		TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
		if (!textureImporter.isReadable)
		{
			// The most important - texture should be readable!
			textureImporter.isReadable = true;
			textureImporter.mipmapEnabled = false;
			textureImporter.alphaIsTransparency = true;
			textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

			AssetDatabase.WriteImportSettingsIfDirty(texturePath);
			AssetDatabase.ImportAsset(texturePath);
			AssetDatabase.Refresh();

			return _source;
		}	

		return AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
	}


	//-----------------------------------------------------------------------------------------------------
	// Save texture as asset to path in Assets folder
	 public static void SaveAsAsset (Texture2D _source, string _path) 
	{
		if (!_source)
		{
			Debug.LogWarning("Saving is impossible! Absent _source texture");
			return;
		}

		if (_path == "")
		{
			Debug.LogWarning("_path is empty! Saved as Assets/_newTexture.png");
			_path = "Assets/_newTexture.png";
		}

		// Save texture
		byte[] assetPng = _source.EncodeToPNG();

		if(File.Exists(_path) == true)
		{
			File.Delete(_path);
			AssetDatabase.Refresh();
		}

		File.WriteAllBytes(_path, assetPng);

		//yield WaitForEndOfFrame();
		AssetDatabase.ImportAsset(_path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

	}
#endif

    //-----------------------------------------------------------------------------------------------------
    // Save texture as PNG to path in Assets folder
    public static void SaveAsPNG(Texture2D _texture, string _filename)
    {
        var bytes = _texture.EncodeToPNG();
        var file = File.Open(Application.dataPath + "/" + _filename + ".png", FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }

    //-----------------------------------------------------------------------------------------------------
    // Try to load texturefrom external source
    // For local files use _path like: "file:///" + Application.persistentDataPath + "/image.png";   or full path, as instance   "file://c://Users/.../image.png"
    public static Texture2D LoadTexture(string _path)
    {
        Texture2D texture = null;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_path);
   

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning("Probably this is wrong external source: " + _path);
            Debug.Log("DON'T FORGET: the path should starts from 'http://'(for online image) or from 'file://'(for local)");
            Debug.Log(www.error);
        }
        else
            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

        return texture;
    }
    
    //---------------------------------------------------------------------------------------------------------------------------------------------------------- 
    // Create new Texture2D/PixelArray filled by custom color
    public static Texture2D CreateTexture (int _width, int _height, Color _color)
	{
		Texture2D result = new Texture2D(_width, _height);
		Color[] resultPixels = new Color [_width * _height];

		for (int i = 0; i < resultPixels.Length; i++ )
            resultPixels[i] = _color;

		result.SetPixels(resultPixels);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] CreatePixelArray (int _width, int _height, Color _color)
	{
		Color[] resultPixels = new Color [_width * _height];
		for (int i = 0; i < resultPixels.Length; i++ ) resultPixels[i] = _color;


		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Clear Texture2D/PixelArray by custom color
	 public static Texture2D Clear (Texture2D _source, Color _color)
	{
		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] resultPixels = new Color [_source.width * _source.height];

		for (int i = 0; i < resultPixels.Length; i++ ) resultPixels[i] = _color;
		result.SetPixels(resultPixels);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] Clear (Color[] _source, Color _color)
	{
		Color[] resultPixels = _source;
		for (int i = 0; i < resultPixels.Length; i++ ) resultPixels[i] = _color;

		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Invert Texture2D/PixelArray transparency
	 public static Texture2D InvertTransparency (Texture2D _source)
	{
		Color[] pixels = _source.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
            pixels[i].a = Mathf.Abs(pixels[i].a - 1.0f);

		Texture2D result = new Texture2D(_source.width, _source.height);	

		result.SetPixels(pixels);
		result.Apply();

		return result; 
	}

	//-------------------------------
	 public static Color[] InvertTransparency (Color[] _source)
	{
		Color[] resultPixels = new Color[_source.Length];
		for (int i = 0; i < resultPixels.Length; i++) resultPixels[i].a = Mathf.Abs(_source[i].a - 1.0f);

		return resultPixels; 
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Make custom color of Texture2D/PixelArray completely transparent (Alpha-key)
	 public static Texture2D MakeColorTransparent (Texture2D _source, Color _color)
	{
		Color[] pixels = _source.GetPixels();
		Texture2D result = new Texture2D(_source.width, _source.height);	

		for (int i = 0; i < pixels.Length; i++) 
			if (pixels[i] == _color) pixels[i].a = 0;


		result.SetPixels(pixels);
		result.Apply();

		return result; 
	}

	//------------------------------- 
	 public static Color[] MakeColorTransparent (Color[] _source, Color _color)
	{
		Color[] resultPixels = new Color[_source.Length];

		for (int i = 0; i < resultPixels.Length; i++) 
			if (resultPixels[i] == _color) _source[i].a = 0;

		return resultPixels; 
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 	
	// Change Texture2D/PixelArray HSB (allows to adjust HUE, Saturation and Brightness)
	 public static Texture2D ChangeHSB(Texture2D _source, float _hue, float _saturation, float _brightness)
	{
		Texture2D result = new Texture2D(_source.width, _source.height, TextureFormat.RGBA32, false);
		Color[] sourcePixels = _source.GetPixels();
		Color[] resultPixels = new Color[_source.width * _source.height];
		Vector4 hsba;

		for (int i = 0; i < sourcePixels.Length; i++) 
		{
			hsba = ColorToHSBA(sourcePixels[i]);
			hsba.x += _hue;
			hsba.y += _saturation;
			hsba.z += _brightness;
			resultPixels[i] = HSBAtoColor(hsba);
		}


		result.SetPixels(resultPixels);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] ChangeHSB(Color[] _source, float _hue, float _saturation, float _brightness)
	{
		Color[] resultPixels = new Color[_source.Length];
		Vector4 hsba;

		for (int i = 0; i < _source.Length; i++) 
		{
			hsba = ColorToHSBA(_source[i]);
			hsba.x += _hue;
			hsba.y += _saturation;
			hsba.z += _brightness;
			resultPixels[i] = HSBAtoColor(hsba);
		}


		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Change Texture2D/PixelArray contrast
	 public static Texture2D Contrast (Texture2D _source, float _contrast)
	{
		_contrast = _contrast < 0 ? 0: _contrast;

		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] sourcePixels = _source.GetPixels();
		Color[] resultPixels = new Color[_source.width * _source.height];
		Color averageColor = GetAverageColor(sourcePixels);

		for (int i = 0; i < sourcePixels.Length; i++) 
			resultPixels[i] = new Color(
											averageColor.r + (sourcePixels[i].r - averageColor.r) * _contrast, 
											averageColor.g + (sourcePixels[i].g - averageColor.g) * _contrast, 
											averageColor.b + (sourcePixels[i].b - averageColor.b) * _contrast, 
											sourcePixels[i].a
										);


		result.SetPixels(resultPixels, 0);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] Contrast (Color[] _source, float _contrast)
	{
		_contrast = _contrast < 0 ? 0: _contrast;

		Color[] resultPixels = new Color[_source.Length];
		Color averageColor = GetAverageColor(_source);

		for (int i = 0; i < _source.Length; i++) 
			resultPixels[i] = new Color(
											averageColor.r + (_source[i].r - averageColor.r) * _contrast, 
											averageColor.g + (_source[i].g - averageColor.g) * _contrast, 
											averageColor.b + (_source[i].b - averageColor.b) * _contrast, 
											_source[i].a
										);

		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Leave in Texture2D/PixelArray within pixels with color only within custom diapason (all other pixel become grayscale)
	 public static Texture2D ColorDiapason (Texture2D _source, Color _colorStart, Color _colorEnd)
	{
		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] sourcePixels = _source.GetPixels();
		Color[] resultPixels = new Color[_source.width * _source.height];

		//Color averageColor = GetAverageColor(sourcePixels);
		float pixelAverageIntensity;
		float pixelHue;
		Vector2 hueDiapason = new Vector2(ColorToHSBA(_colorStart).x, ColorToHSBA(_colorEnd).x);


		for (int i = 0; i < sourcePixels.Length; i++) 
		{
			pixelAverageIntensity = (sourcePixels[i].r + sourcePixels[i].g + sourcePixels[i].b) / 3;
			pixelHue = ColorToHSBA(sourcePixels[i]).x;
			if (pixelHue >= hueDiapason.x  &&  pixelHue <= hueDiapason.y)
				resultPixels[i] = sourcePixels[i];
			else // Grayscale
				resultPixels[i] = new Color(
												pixelAverageIntensity, 
												pixelAverageIntensity, 
												pixelAverageIntensity, 
												sourcePixels[i].a
											);
		}

		result.SetPixels(resultPixels, 0);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] ColorDiapason (Color[] _source, Color _colorStart, Color _colorEnd)
	{
		Color[] resultPixels = new Color[_source.Length];

		//Color averageColor = GetAverageColor(_source);
		float pixelAverageIntensity;
		float pixelHue;
		Vector2 hueDiapason = new Vector2(ColorToHSBA(_colorStart).x, ColorToHSBA(_colorEnd).x);	


		for (int i = 0; i < _source.Length; i++) 
		{
			pixelAverageIntensity = (_source[i].r + _source[i].g + _source[i].b) / 3;
			pixelHue = ColorToHSBA(_source[i]).x;
			if (pixelHue >= hueDiapason.x  &&  pixelHue <= hueDiapason.y)
				resultPixels[i] = _source[i];
			else
				resultPixels[i] = new Color(
												pixelAverageIntensity, 
												pixelAverageIntensity, 
												pixelAverageIntensity, 
												_source[i].a
											);
		}

		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Colorize Texture2D/PixelArray by custom color and intensity 
	 public static Texture2D Colorize (Texture2D _source, Color _color, float _intensity)
	{
		_intensity = _intensity < 0 ? 0: _intensity;

		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] sourcePixels = _source.GetPixels();
		Color[] resultPixels = new Color[_source.width * _source.height];


		for (int i = 0; i < sourcePixels.Length; i++) 
			resultPixels[i] = new Color(
											sourcePixels[i].r * _color.r * _intensity, 
											sourcePixels[i].g * _color.g * _intensity, 
											sourcePixels[i].b * _color.b * _intensity, 
											sourcePixels[i].a
										);


		result.SetPixels(resultPixels, 0);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] Colorize (Color[] _source, Color _color, float _intensity)
	{
		Color[] resultPixels = new Color[_source.Length];

		_intensity = _intensity < 0 ? 0: _intensity;

		for (int i = 0; i < _source.Length; i++) 
			resultPixels[i] = new Color(
											_source[i].r * _color.r * _intensity, 
											_source[i].g * _color.g * _intensity, 
											_source[i].b * _color.b * _intensity, 
											_source[i].a
										);


		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Turn Texture2D/PixelArray to grayscale
	 public static Texture2D Grayscale (Texture2D _source)
	{
		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] sourcePixels = _source.GetPixels();
		Color[] resultPixels = new Color[_source.width * _source.height];
		float pixelAverageIntensity;


		for (int i = 0; i < sourcePixels.Length; i++) 
		{
			pixelAverageIntensity = (sourcePixels[i].r + sourcePixels[i].g + sourcePixels[i].b) / 3;
			resultPixels[i] = new Color(
											pixelAverageIntensity, 
											pixelAverageIntensity, 
											pixelAverageIntensity, 
											sourcePixels[i].a
										);
		}

		result.SetPixels(resultPixels, 0);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] Grayscale (Color[] _source)
	{
		Color[] resultPixels = new Color[_source.Length];
		float pixelAverageIntensity;


		for (int i = 0; i < _source.Length; i++) 
		{
			pixelAverageIntensity = (_source[i].r + _source[i].g + _source[i].b) / 3;
			resultPixels[i] = new Color(
											pixelAverageIntensity, 
											pixelAverageIntensity, 
											pixelAverageIntensity, 
											_source[i].a
										);
		}

		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Turn Texture2D/PixelArray to negative of itself
	 public static Texture2D Negative (Texture2D _source)
	{
		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] sourcePixels = _source.GetPixels();
		Color[] resultPixels = new Color[_source.width * _source.height];
		Color averageColor = GetAverageColor(sourcePixels);


		for (int i = 0; i < sourcePixels.Length; i++) 
			resultPixels[i] = new Color(
											sourcePixels[i].r - (sourcePixels[i].r - averageColor.r) * 3, 
											sourcePixels[i].g - (sourcePixels[i].g - averageColor.g) * 3, 
											sourcePixels[i].b - (sourcePixels[i].b - averageColor.b) * 3, 
											sourcePixels[i].a
										);


		result.SetPixels(resultPixels, 0);
		result.Apply();

		return result;
	}

	//-------------------------------
	 public static Color[] Negative (Color[] _source)
	{
		Color[] resultPixels = new Color[_source.Length];
		Color averageColor = GetAverageColor(_source);


		for (int i = 0; i < _source.Length; i++) 
			resultPixels[i] = new Color(
											_source[i].r - (_source[i].r - averageColor.r) * 3, 
											_source[i].g - (_source[i].g - averageColor.g) * 3, 
											_source[i].b - (_source[i].b - averageColor.b) * 3, 
											_source[i].a
										);

		return resultPixels;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Flip Texture2D horizontally
	 public static Texture2D FlipHorizontally (Texture2D _source)
	{
		Texture2D result = new Texture2D(_source.width, _source.height);

		for(int x = 0; x < _source.width; x++)
			for(int y = 0; y < _source.height; y++)
				result.SetPixel(_source.width-x-1, y, _source.GetPixel(x,y));


		result.Apply();

		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Flip Texture2D vertically
	 public static Texture2D FlipVertically (Texture2D _source)
	{
		Texture2D result = new Texture2D(_source.width, _source.height);

		for(int x = 0; x < _source.width; x++)
			for(int y = 0; y < _source.height; y++)
				result.SetPixel(x, _source.height-y-1, _source.GetPixel(x,y));


		result.Apply();

		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Change Texture2D canvas width/height by expanding in specified directions (Texture2D doesn't scale image itself)
	 public static Texture2D Expand (Texture2D _source, int _newWidth, int _newHeight, Alignment _sourceAlignment)
	{

		if ((_newWidth < _source.width)  ||  (_newHeight < _source.height) )
		{
			Debug.LogWarning("Expand is impossible! New size should be bigger than _source rect");
			return null;
		}


		Color[] sourcePixels = _source.GetPixels();
		Texture2D result = CreateTexture (_newWidth, _newHeight, new Color(1.0f, 0.0f, 0.0f, 0.0f));


		switch (_sourceAlignment)  
		{
		case Alignment.TopLeft:
			result.SetPixels(0, _newHeight-_source.height, _source.width, _source.height, sourcePixels);
			break;

		case Alignment.TopRight:
			result.SetPixels(_newWidth-_source.width, _newHeight-_source.height, _source.width, _source.height, sourcePixels);
			break;

		case Alignment.TopCenter:
			result.SetPixels((_newWidth-_source.width)/2, _newHeight-_source.height, _source.width, _source.height, sourcePixels);
			break;

		case Alignment.BottomLeft:
			result.SetPixels(0, 0, _source.width, _source.height, sourcePixels);
			break;

		case Alignment.BottomRight:
			result.SetPixels(_newWidth-_source.width, 0, _source.width, _source.height, sourcePixels);
			break;

		case Alignment.BottomCenter:
			result.SetPixels((_newWidth-_source.width)/2, 0, _source.width, _source.height, sourcePixels);
			break;
		}	


		result.Apply();

		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Crop Texture2D by extracting it piece in specified rectangle
	 public static Texture2D Crop (Texture2D _source, Rect _rect)
	{
		if (_rect.x < 0  ||  _rect.y < 0  ||  (_rect.width < 0 || (_rect.x+_rect.width > _source.width))  ||  (_rect.height<0 || (_rect.y+_rect.height > _source.height)) )
		{
			Debug.LogWarning("Crop is impossible! Crop _rect should be within _source rect");
			return null;
		}


		Color[] neededPixels = _source.GetPixels(Mathf.CeilToInt(_rect.x), Mathf.CeilToInt(_rect.y), Mathf.CeilToInt(_rect.width), Mathf.CeilToInt(_rect.height));
		Texture2D result = new Texture2D(Mathf.CeilToInt(_rect.width), Mathf.CeilToInt(_rect.height));


		result.SetPixels(neededPixels);
		result.Apply();

		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Automatically crop transparent pixels surrounding image
	 public static Texture2D AutoCropTransparency (Texture2D _source)
	{
		Color[] sourcePixels = _source.GetPixels();
		Rect cropRect = new Rect (0, 0, _source.width, _source.height) ;

		int x = 0;
		int y = 0;


		while (cropRect.x == 0 && x < _source.width)
		{
			for (y = 0; y < _source.height; y++)
				if (sourcePixels[y*_source.width + x].a != 0) cropRect.x = x;
			x++; 
		} 

		x = _source.width; 
		while (cropRect.width == _source.width  &&  x > cropRect.x)
		{
			x--; 
			for (y = 0; y < _source.height; y++)
				if (sourcePixels[y*_source.width + x].a != 0) cropRect.width = x - cropRect.x;
		}



		y = 0;
		while (cropRect.y == 0 && y < _source.height)
		{
			for (x = 0; x < _source.width; x++)
				if (sourcePixels[y*_source.width + x].a != 0) cropRect.y = y;
			y++; 
		}


		y = _source.height; 
		while (cropRect.height == _source.height  &&  y > cropRect.y)
		{
			y--; 
			for (x = 0; x < _source.width; x++)
				if (sourcePixels[y*_source.width + x].a != 0) cropRect.height = y - cropRect.y;

		}


		return Crop (_source, cropRect);

	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Automatically crop pixels(custom color) surrounding image
	 public static Texture2D AutoCropColor (Texture2D _source, Color _color)
	{
		Color[] sourcePixels = _source.GetPixels();
		Rect cropRect = new Rect (0, 0, _source.width, _source.height) ;

		int x = 0;
		int y = 0;


		while (cropRect.x == 0 && x < _source.width)
		{
			for (y = 0; y < _source.height; y++)
				if (sourcePixels[y*_source.width + x] != _color) cropRect.x = x;
			x++; 
		} 

		x = _source.width; 
		while (cropRect.width == _source.width  &&  x > cropRect.x)
		{
			x--; 
			for (y = 0; y < _source.height; y++)
				if (sourcePixels[y*_source.width + x] != _color) cropRect.width = x - cropRect.x;
		}



		y = 0;
		while (cropRect.y == 0 && y < _source.height)
		{
			for (x = 0; x < _source.width; x++)
				if (sourcePixels[y*_source.width + x] != _color) cropRect.y = y;
			y++; 
		}


		y = _source.height; 
		while (cropRect.height == _source.height  &&  y > cropRect.y)
		{
			y--; 
			for (x = 0; x < _source.width; x++)
				if (sourcePixels[y*_source.width + x] != _color) cropRect.height = y - cropRect.y;

		}


		return Crop (_source, cropRect);

	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Apply transparency(Alpha-channel) mask to Texture2D
	public static Texture2D ApplyMask (Texture2D _source, Texture2D _mask)
	{
		Color[] pixels = _source.GetPixels();
		Texture2D result = new Texture2D(_source.width, _source.height);	

        int width = result.width;
        int height = result.height;

		Color[] maskPixels = _mask.GetPixels ();

        int maskWidth = _mask.width;
        int maskHeight = _mask.height;


        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
				pixels [y * width + x].a = maskPixels [(y * maskHeight / height) * maskWidth + (x * maskWidth / width)].a;
            }
			
			
	   /*	Alternative version - slower, but a bit more precise
	      Vector2 fraction;
		  for (int y = 0; y < result.height; y++) 
			for (int x = 0; x < result.width; x++) 
			{
				fraction.x = Mathf.Clamp01 (x / (float)(result.width + 1.0f));
				fraction.y = Mathf.Clamp01 (y / (float)(result.height + 1.0f));

				pixels[y * result.width + x].a = _mask.GetPixelBilinear(fraction.x, fraction.y).a;
			}*/
		
			
		result.SetPixels(pixels);
		result.Apply();

		return result; 
	}

	//----------
	public static void ApplyMask (Color[] _sourcePixels, Texture2D _mask, ref Texture2D _result)
	{
		int width = _result.width;
		int height = _result.height;

		Color[] maskPixels = _mask.GetPixels ();

		int maskWidth = _mask.width;
		int maskHeight = _mask.height;


		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
			{
				_sourcePixels [y * width + x].a = maskPixels [(y * maskHeight / height) * maskWidth + (x * maskWidth / width)].a;
			}


		_result.SetPixels(_sourcePixels);
		_result.Apply();

	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Merge 2 Textures by apply Boolean operation(based on Alpha-channel) to them. Result-texture size will be expanded if needed.
	// Union - result Texture will contains all non-transparent pixels of both textures
	// Intersection - result Texture will contains only overlapped non-transparent pixels of textures
	// Subtraction - result Texture will contains only non-transparent pixels which aren't overlapped
	 public static Texture2D ApplyBooleanOperation (BooleanOperation _operationType, Texture2D _base, Texture2D _addition, Vector2 _additionOffset)
	{
		_additionOffset = new Vector2 (Mathf.CeilToInt(_additionOffset.x), Mathf.CeilToInt(_additionOffset.y));


		Vector2 sizeIncrement 	 = new Vector2 (Mathf.Clamp(_addition.width + Mathf.Abs(_additionOffset.x) - _base.width, 0, Mathf.Infinity), Mathf.Clamp(_addition.height + Mathf.Abs(_additionOffset.y) - _base.height, 0, Mathf.Infinity));
		Vector2 resultSize 		 = new Vector2 (_base.width + sizeIncrement.x, _base.height + sizeIncrement.y); 
		Vector2 basePosition	 = new Vector2 (_additionOffset.x > 0 ? 0 : sizeIncrement.x, _additionOffset.y > 0 ? 0 : sizeIncrement.y); 
		Vector2 additionPosition = new Vector2 (_additionOffset.x > 0 ? Mathf.Abs(_additionOffset.x) : 0, _additionOffset.y > 0 ? Mathf.Abs(_additionOffset.y) : 0); 


		Texture2D result 		= new Texture2D(Mathf.CeilToInt(resultSize.x), Mathf.CeilToInt(resultSize.y));	
		Color[] resultPixels 	= new Color[Mathf.CeilToInt(resultSize.x * resultSize.y)];
		Color[] basePixels 		= _base.GetPixels();
		Color[] additionPixels 	= _addition.GetPixels();


		result.SetPixels(0, 0, Mathf.CeilToInt(resultSize.x), Mathf.CeilToInt(resultSize.y), resultPixels);
		result.SetPixels(Mathf.CeilToInt(basePosition.x), Mathf.CeilToInt(basePosition.y), Mathf.CeilToInt(_base.width), Mathf.CeilToInt(_base.height), basePixels);
		resultPixels = result.GetPixels();


		Color currentPixelColor;
		if (_operationType == BooleanOperation.Intersection)
		{
			Texture2D texture = new Texture2D(Mathf.CeilToInt(resultSize.x), Mathf.CeilToInt(resultSize.y));	
			Color[] pixels = new Color[Mathf.CeilToInt(resultSize.x * resultSize.y)];
			texture.SetPixels(0, 0, Mathf.CeilToInt(resultSize.x), Mathf.CeilToInt(resultSize.y), pixels);
			texture.SetPixels(Mathf.CeilToInt(additionPosition.x) , Mathf.CeilToInt(additionPosition.y), Mathf.CeilToInt(_addition.width), Mathf.CeilToInt(_addition.height), additionPixels);
			pixels = texture.GetPixels();

			for (int i = 0; i < pixels.Length; i++)
                resultPixels[i] = (resultPixels[i].a == 0  ||  pixels[i].a == 0)  ?  new Color(0,0,0,0)  :  pixels[i];

			result.SetPixels(0, 0, Mathf.CeilToInt(resultSize.x), Mathf.CeilToInt(resultSize.y), resultPixels);
		}
		else
			if (_operationType == BooleanOperation.Union)
			{
				for (int y = 0; y < _addition.height; ++y) 
					for (int x = 0; x < _addition.width; ++x) 
					{
						currentPixelColor = additionPixels[y*_addition.width + x].a == 0  ?  resultPixels[Mathf.CeilToInt((y+additionPosition.y)*resultSize.x + (x+additionPosition.x))]  :  additionPixels[Mathf.CeilToInt(y*_addition.width + x)];
						result.SetPixel (Mathf.CeilToInt(x+additionPosition.x), Mathf.CeilToInt(y+additionPosition.y), currentPixelColor);
					}
			}
			else
				for (int y = 0; y < _addition.height; ++y) 
					for (int x = 0; x < _addition.width; ++x) 
					{
						currentPixelColor = additionPixels[y*_addition.width + x].a == 0  ?  resultPixels[Mathf.CeilToInt((y+additionPosition.y)*resultSize.x + (x+additionPosition.x))]  :  additionPixels[Mathf.CeilToInt(y*_addition.width + x)];
						if (resultPixels[Mathf.CeilToInt((y+additionPosition.y)*resultSize.x + (x+additionPosition.x))].a !=0  &&  additionPixels[Mathf.CeilToInt(y*_addition.width + x)].a != 0)
                            currentPixelColor.a = 0;

						result.SetPixel (Mathf.CeilToInt(x+additionPosition.x), Mathf.CeilToInt(y+additionPosition.y), currentPixelColor);
					}


		result.Apply();

		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Rotate Texture2D on custom angle
	 public static Texture2D Rotate(Texture2D _source, float _angle)
	{
		Color[] sourcePixels = _source.GetPixels();
		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] pixels = result.GetPixels();


		int rotatedX;
		int rotatedY;

		for (int y = 0; y < result.height; y++)
			for (int x = 0; x < result.width; x++)
			{
				rotatedX = Mathf.CeilToInt(Mathf.Cos(Mathf.Deg2Rad*_angle) * (x-result.width/2)  +  Mathf.Sin(Mathf.Deg2Rad*_angle) * (y-result.height/2)  +  result.width/2);
				rotatedY = Mathf.CeilToInt(-Mathf.Sin(Mathf.Deg2Rad*_angle) * (x-result.width/2)  +  Mathf.Cos(Mathf.Deg2Rad*_angle) * (y-result.height/2)  +  result.height/2);

				if (rotatedX > -1  &&  rotatedX < result.width  &&  rotatedY > -1  &&  rotatedY < result.height)  
					pixels[y*result.width + x] = sourcePixels[Mathf.CeilToInt(Mathf.Clamp(rotatedY, 0, result.height-1)*result.width + Mathf.Clamp(rotatedX, 0.0f, result.width-1))];
				else 
					pixels[y*result.width + x] = new Color(0,0,0,0);

			}


		result.SetPixels(pixels);
		result.Apply();

		return result;
	} 

	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Rotate Texture2D on 90 degree angle
	public static Texture2D Rotate90(Texture2D _source, bool _clockwise = true)
	{
		Color32[] original = _source.GetPixels32();
		Color32[] rotated = new Color32[original.Length];

		int rotatedPixelId, originalPixelId;
		int width = _source.width;
		int height = _source.height;

		Texture2D result = new Texture2D(height, width);


		for (int y = 0; y < height; ++y)
			for (int x = 0; x < width; ++x)
			{
				rotatedPixelId = (x + 1) * height - y - 1;
				originalPixelId = _clockwise ? original.Length - 1 - (y * width + x) : y * width + x;
				rotated[rotatedPixelId] = original[originalPixelId];
			}


		result.SetPixels32(rotated);
		result.Apply();

		return result;

	}

    //----------
    public static Color[] Rotate90(Color[] _source, int _width, int _height, bool _clockwise = true)
    {
        Color[] result = new Color[_source.Length];
        int rotatedPixelId, originalPixelId;

        if (_clockwise)
            for (int y = 0; y < _height; ++y)
                for (int x = 0; x < _width; ++x)
                {
                    rotatedPixelId = (x + 1) * _height - y - 1;
                    originalPixelId = _source.Length - 1 - (y * _width + x);
                    result[rotatedPixelId] = _source[originalPixelId];
                }
        else
            for (int y = 0; y < _height; ++y)
                for (int x = 0; x < _width; ++x)
                {
                    rotatedPixelId = (x + 1) * _height - y - 1;
                    originalPixelId = y * _width + x;
                    result[rotatedPixelId] = _source[originalPixelId];
                }


        return result;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Scale Texture2D with new width/height
    public static Texture2D Scale (Texture2D _source, int _targetWidth, int _targetHeight)
	{
		if (_targetWidth <= 0  ||  _targetHeight <= 0)
		{
			Debug.LogWarning("Scale is impossible! Target size should be at least 1x1");
			return null;
		}


		Texture2D result = new Texture2D(_targetWidth, _targetHeight);
		Color[] pixels = new Color[_targetWidth * _targetHeight];

		Vector2 fraction;

		for (int y = 0; y < result.height; y++) 
			for (int x = 0; x < result.width; x++) 
			{
				fraction.x = Mathf.Clamp01 (x / (result.width + 0.0f));
				fraction.y = Mathf.Clamp01 (y / (result.height + 0.0f));

				// Get the relative pixel positions
				pixels[y * result.width + x] = _source.GetPixelBilinear(fraction.x, fraction.y);
			}

		result.SetPixels(pixels);
		result.Apply();

		return result;
	}


	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// Stroke(colorize edge pixels) Texture2D by color with specified thickness and blendMode
	 public static Texture2D Stroke (Texture2D _source, int _thickness, Color _color, BlendMode _blendMode) 
	{
		Texture2D result = new Texture2D(_source.width, _source.height);
		Color[] pixels = _source.GetPixels();

		for (int x = 0; x < result.width; x++)
			for (int y = 0; y < result.height; y++)
				for (int i = 0; i < _thickness; i++)
					if (
						pixels[y*result.width + x].a > 0.01f
						  &&  
						(
							(x+i < result.width  &&  pixels[y*result.width + x+i].a < 0.01f) || 
							(x-i >=0  &&  pixels[y*result.width + x-i].a < 0.01f) ||
							(y-i >=0  &&  pixels[(y-i)*result.width + x].a < 0.01f) ||
							(y+i < result.height &&  pixels[(y+i)*result.width + x].a < 0.01f) ||
							x-i == 0 || x+i == result.width-1  ||  y-i == 0 || y+i == result.height-1
						) 
					) 
						pixels[y*result.width + x] = BlendColors(pixels[y*result.width + x], _color, _blendMode);



		result.SetPixels(pixels);
		result.Apply();

		return result;

	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Get average color in PixelsArray
	 public static Color GetAverageColor(Color[] _pixels)
	{
		Color averageColor = new Color();

		for (int i = 0; i < _pixels.Length; i++) 
		{
			averageColor.r += _pixels[i].r;
			averageColor.g += _pixels[i].g;
			averageColor.b += _pixels[i].b;
			averageColor.a += _pixels[i].a;
		}

		averageColor /= _pixels.Length;

		return averageColor;
	}


	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// Blend 2 colors using one of BlendModes
	 public static Color BlendColors (Color _color1, Color _color2, BlendMode _blendMode)
	{
		Color result = _color1;

		switch (_blendMode)
		{
		case BlendMode.Normal:
			result = _color2;
			break;

		case BlendMode.Additive:
			result += _color2;
			break;

		case BlendMode.Subtraction:
			result -= _color2;
			break;

		case BlendMode.Multiply:
			result *= _color2;
			break;

		case BlendMode.Subdivide:
			result.r /= _color2.r;
			result.g /= _color2.g;
			result.b /= _color2.b;
			result.a /= _color2.a;
			break;

		case BlendMode.MaskAlpha:
			result.a = _color2.a;
			break;

		} 

		return result;			 
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Convert Color to HSBA (Vector4)
	 public static Vector4 ColorToHSBA (Color _color)
	{
		float minValue = Mathf.Min(_color.r, Mathf.Min(_color.g, _color.b));
		float maxValue = Mathf.Max(_color.r, Mathf.Max(_color.g, _color.b));
		float delta = maxValue - minValue;
		// hsba
		Vector4 result = new Vector4 (0, 0, maxValue, _color.a);


		// Calculate the HUE in degrees
		if (maxValue == _color.r) 
		{
			if (_color.g >= _color.b)
				result.x = (delta == 0) ? 0 : 60.0f * (_color.g - _color.b) / delta;
			else 
				if (_color.g < _color.b)
					result.x = 60.0f * (_color.g - _color.b) / delta + 360;
		} 
		else 
			if (maxValue == _color.g)
				result.x = 60.0f * (_color.b - _color.r) / delta + 120;
			else 				
				result.x = 60.0f * (_color.r - _color.g) / delta + 240;


		result.x /= 360;

		// Calculate saturation 
		result.y = (maxValue == 0)  ?  0  :  1.0f - (minValue / maxValue);


		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 
	// Convert HSBA(Vector4) to Color
	 public static Color HSBAtoColor(Vector4 hsba)
	{
		Color result = new Color(hsba.z, hsba.z, hsba.z, hsba.w);

		// if Saturation > 0
		if(hsba.y > 0) 
		{ 
			// Calculate sector
			float secPos = (hsba.x * 360.0f) / 60.0f;
			int secNr = Mathf.FloorToInt(secPos);
			float secPortion = secPos - secNr;

			// Calculate axes
			float p = hsba.z * (1.0f - hsba.y);
			float q = hsba.z * (1.0f - (hsba.y * secPortion));
			float t = hsba.z * (1.0f - (hsba.y * (1.0f - secPortion)));


			// Calculate RGB
			switch (secNr) 
			{
			case 1:
				result = new Color(q, hsba.z, p, hsba.w);
				break;

			case 2:
				result = new Color(p, hsba.z, t, hsba.w);
				break;

			case 3:
				result = new Color(p, q, hsba.z, hsba.w);
				break;

			case 4:
				result = new Color(t, p, hsba.z, hsba.w);
				break;

			case 5:
				result = new Color(hsba.z, p, q, hsba.w); 
				break;

			default:
				result = new Color(hsba.z, t, p, hsba.w);
				break;

			}

		}

		return result;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------- 

}
