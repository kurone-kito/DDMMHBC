using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ddmmhbc.EditorTools
{
  /// <summary>
  /// Creates the first presentation scene and regenerates its replaceable Cube sprite.
  /// </summary>
  public static class RenderCubeProxy
  {
    private const string BootScenePath = "Assets/Scenes/Boot.unity";
    private const string CubeProxyPath = "Assets/Art/Placeholders/CubeProxy.png";
    private const int ProxyWidth = 512;
    private const int ProxyHeight = 768;

    [MenuItem("DDMMHBC/Art/Render Cube Proxy")]
    public static void GenerateCubeProxy()
    {
      var renderScene = OpenWorkingScene(out var singleSceneMode, out var activeSceneToRestore);
      var succeeded = false;
      try
      {
        RenderCubeProxyInScene(renderScene);
        succeeded = true;
      }
      finally
      {
        if (singleSceneMode)
        {
          if (succeeded)
          {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
          }
          else if (renderScene.IsValid() && renderScene.isLoaded)
          {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
          }
        }
        else
        {
          CloseGeneratedScene(renderScene, activeSceneToRestore);
        }
      }
    }

    private static void RenderCubeProxyInScene(Scene scene)
    {
      EnsureAssetFolder("Assets/Art/Placeholders");

      var temporaryObjects = new List<GameObject>();
      RenderTexture renderTexture = null;
      Texture2D image = null;
      Material proxyMaterial = null;
      var previousActiveTexture = RenderTexture.active;

      try
      {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SceneManager.MoveGameObjectToScene(cube, scene);
        temporaryObjects.Add(cube);
        cube.name = "Cube Proxy Source";
        cube.transform.position = Vector3.zero;
        cube.transform.rotation = Quaternion.Euler(22f, -32f, 8f);
        UnityEngine.Object.DestroyImmediate(cube.GetComponent<Collider>());

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
          throw new InvalidOperationException("The URP/Lit shader is unavailable.");
        }

        proxyMaterial = new Material(shader) { name = "Temporary Cube Proxy Material" };
        proxyMaterial.SetColor("_BaseColor", new Color(0.24f, 0.82f, 1f, 1f));
        cube.GetComponent<MeshRenderer>().sharedMaterial = proxyMaterial;

        var keyLightObject = new GameObject("Proxy Key Light");
        SceneManager.MoveGameObjectToScene(keyLightObject, scene);
        temporaryObjects.Add(keyLightObject);
        keyLightObject.transform.rotation = Quaternion.Euler(38f, -28f, 0f);
        var keyLight = keyLightObject.AddComponent<Light>();
        keyLight.type = LightType.Directional;
        keyLight.intensity = 1.8f;
        keyLight.color = new Color(1f, 0.91f, 0.78f);

        var fillLightObject = new GameObject("Proxy Fill Light");
        SceneManager.MoveGameObjectToScene(fillLightObject, scene);
        temporaryObjects.Add(fillLightObject);
        fillLightObject.transform.rotation = Quaternion.Euler(315f, 145f, 0f);
        var fillLight = fillLightObject.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.intensity = 0.65f;
        fillLight.color = new Color(0.44f, 0.77f, 1f);

        var cameraObject = new GameObject("Proxy Render Camera");
        SceneManager.MoveGameObjectToScene(cameraObject, scene);
        temporaryObjects.Add(cameraObject);
        cameraObject.transform.position = new Vector3(0f, 0f, -6f);
        var camera = cameraObject.AddComponent<Camera>();
        camera.enabled = false;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        camera.orthographic = true;
        camera.orthographicSize = 1.55f;
        camera.aspect = (float)ProxyWidth / ProxyHeight;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 20f;
        camera.allowHDR = false;
        camera.allowMSAA = false;
        camera.overrideSceneCullingMask = EditorSceneManager.GetSceneCullingMask(scene);

        var additionalCameraData = cameraObject.AddComponent<UniversalAdditionalCameraData>();
        additionalCameraData.renderType = CameraRenderType.Base;
        additionalCameraData.renderPostProcessing = false;

        renderTexture = new RenderTexture(
          ProxyWidth,
          ProxyHeight,
          24,
          RenderTextureFormat.ARGB32,
          RenderTextureReadWrite.sRGB)
        {
          name = "Temporary Cube Proxy Render Texture",
          antiAliasing = 1,
          useMipMap = false
        };
        renderTexture.Create();

        var request = new UniversalRenderPipeline.SingleCameraRequest();
        if (!RenderPipeline.SupportsRenderRequest(camera, request))
        {
          throw new InvalidOperationException(
            "The active render pipeline does not support a URP single-camera request.");
        }

        request.destination = renderTexture;
        RenderPipeline.SubmitRenderRequest(camera, request);

        RenderTexture.active = renderTexture;
        image = new Texture2D(ProxyWidth, ProxyHeight, TextureFormat.RGBA32, false, false)
        {
          name = "Cube Proxy PNG Source"
        };
        image.ReadPixels(new Rect(0f, 0f, ProxyWidth, ProxyHeight), 0, 0, false);
        image.Apply(false, false);
        ValidateRenderedCube(image);
        File.WriteAllBytes(GetProjectFilePath(CubeProxyPath), image.EncodeToPNG());

        AssetDatabase.ImportAsset(CubeProxyPath, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(CubeProxyPath) as TextureImporter;
        if (importer == null)
        {
          throw new InvalidOperationException("Unity did not create a texture importer for the proxy.");
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        if (AssetDatabase.LoadAssetAtPath<Sprite>(CubeProxyPath) == null)
        {
          throw new InvalidOperationException("The generated Cube proxy was not imported as a Sprite.");
        }

        Debug.Log($"Generated the Cube proxy Sprite at {CubeProxyPath}.");
      }
      finally
      {
        RenderTexture.active = previousActiveTexture;
        if (renderTexture != null)
        {
          renderTexture.Release();
          UnityEngine.Object.DestroyImmediate(renderTexture);
        }

        if (image != null)
        {
          UnityEngine.Object.DestroyImmediate(image);
        }

        if (proxyMaterial != null)
        {
          UnityEngine.Object.DestroyImmediate(proxyMaterial);
        }

        for (var index = temporaryObjects.Count - 1; index >= 0; index--)
        {
          if (temporaryObjects[index] != null)
          {
            UnityEngine.Object.DestroyImmediate(temporaryObjects[index]);
          }
        }
      }
    }

    [MenuItem("DDMMHBC/Scene/Create Boot Scene")]
    public static void CreateBootScene()
    {
      if (File.Exists(GetProjectFilePath(BootScenePath)))
      {
        throw new InvalidOperationException(
          $"Refusing to overwrite the existing scene at {BootScenePath}.");
      }

      var bootScene = OpenWorkingScene(out var singleSceneMode, out var sceneToRestore);
      var saved = false;
      try
      {
        bootScene.name = "Boot";
        RenderCubeProxyInScene(bootScene);
        var proxySprite = AssetDatabase.LoadAssetAtPath<Sprite>(CubeProxyPath);
        if (proxySprite == null)
        {
          throw new InvalidOperationException("The generated Cube proxy was not imported as a Sprite.");
        }

        CreateBootCamera(bootScene);
        CreateBackdrop(bootScene);
        CreatePresentationCanvas(bootScene, proxySprite);

        if (!EditorSceneManager.SaveScene(bootScene, BootScenePath))
        {
          throw new InvalidOperationException($"Unity could not save {BootScenePath}.");
        }

        var scenes = new List<EditorBuildSettingsScene>
        {
          new EditorBuildSettingsScene(BootScenePath, true)
        };
        foreach (var scene in EditorBuildSettings.scenes)
        {
          if (!string.Equals(scene.path, BootScenePath, StringComparison.Ordinal))
          {
            scenes.Add(scene);
          }
        }

        EditorBuildSettings.scenes = scenes.ToArray();
        AssetDatabase.SaveAssets();
        saved = true;
        Debug.Log($"Created {BootScenePath} and placed it first in Build Settings.");
      }
      finally
      {
        if (singleSceneMode)
        {
          if (!saved && bootScene.IsValid() && bootScene.isLoaded)
          {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
          }
        }
        else
        {
          CloseGeneratedScene(bootScene, sceneToRestore);
        }
      }
    }

    [MenuItem("DDMMHBC/Scene/Validate Boot Scene")]
    public static void ValidateBootScene()
    {
      if (!File.Exists(GetProjectFilePath(BootScenePath)))
      {
        throw new FileNotFoundException($"The boot scene does not exist at {BootScenePath}.");
      }

      var scene = SceneManager.GetSceneByPath(BootScenePath);
      var openedForValidation = !scene.IsValid() || !scene.isLoaded;
      var activeSceneToRestore = SceneManager.GetActiveScene();
      if (openedForValidation)
      {
        scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Additive);
      }

      try
      {
        ValidateBootScene(scene);
      }
      finally
      {
        if (openedForValidation && scene.IsValid() && scene.isLoaded)
        {
          EditorSceneManager.CloseScene(scene, true);
        }

        if (activeSceneToRestore.IsValid() && activeSceneToRestore.isLoaded)
        {
          SceneManager.SetActiveScene(activeSceneToRestore);
        }
      }
    }

    private static void ValidateBootScene(Scene scene)
    {
      if (!scene.IsValid() || !scene.isLoaded)
      {
        throw new InvalidOperationException("Unity could not open the boot scene.");
      }

      var buildScenes = EditorBuildSettings.scenes;
      if (buildScenes.Length == 0 || !buildScenes[0].enabled || buildScenes[0].path != BootScenePath)
      {
        throw new InvalidOperationException("Boot.unity is not the first enabled Build Settings scene.");
      }

      var rootObjects = scene.GetRootGameObjects();
      Camera mainCamera = null;
      Canvas presentationCanvas = null;
      var meshRendererCount = 0;
      var missingReferenceCount = 0;
      foreach (var rootObject in rootObjects)
      {
        var transforms = rootObject.GetComponentsInChildren<Transform>(true);
        foreach (var item in transforms)
        {
          if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject) != 0)
          {
            throw new InvalidOperationException($"{item.name} has a missing script reference.");
          }

          if (item.TryGetComponent<Camera>(out var candidateCamera) && candidateCamera.CompareTag("MainCamera"))
          {
            mainCamera = candidateCamera;
          }

          if (item.TryGetComponent<Canvas>(out var candidateCanvas))
          {
            presentationCanvas = candidateCanvas;
          }

          meshRendererCount += item.GetComponents<MeshRenderer>().Length;
          foreach (var component in item.GetComponents<Component>())
          {
            if (component == null)
            {
              throw new InvalidOperationException($"{item.name} has a missing component reference.");
            }

            var serializedComponent = new SerializedObject(component);
            var property = serializedComponent.GetIterator();
            while (property.NextVisible(true))
            {
              if (property.propertyType == SerializedPropertyType.ObjectReference
                && property.objectReferenceValue == null
                && property.objectReferenceInstanceIDValue != 0)
              {
                missingReferenceCount++;
                throw new InvalidOperationException(
                  $"{item.name}.{property.propertyPath} contains a missing object reference.");
              }
            }
          }
        }
      }

      if (mainCamera == null || !mainCamera.orthographic || mainCamera.GetComponent<UniversalAdditionalCameraData>() == null)
      {
        throw new InvalidOperationException("The URP orthographic Main Camera is missing or incomplete.");
      }
      ValidateBackdropRendering(mainCamera);

      var canvasScaleFactor = presentationCanvas == null ? 0f : presentationCanvas.scaleFactor;
      var canvasLocalScale = presentationCanvas == null
        ? Vector3.zero
        : presentationCanvas.transform.localScale;
      if (presentationCanvas == null
        || presentationCanvas.renderMode != RenderMode.ScreenSpaceOverlay
        || !IsFinitePositive(canvasScaleFactor)
        || !IsFinitePositive(canvasLocalScale)
        || !IsApproximately(canvasLocalScale, Vector3.one * canvasScaleFactor))
      {
        throw new InvalidOperationException(
          $"The overlay presentation Canvas is missing, misconfigured, or has an invalid scale (factor: {canvasScaleFactor}, local: {canvasLocalScale}).");
      }

      var scaler = presentationCanvas.GetComponent<CanvasScaler>();
      if (scaler == null || scaler.referenceResolution != new Vector2(1920f, 1080f))
      {
        throw new InvalidOperationException("The presentation Canvas reference resolution is not 1920×1080.");
      }

      var playfield = presentationCanvas.transform.Find("Vertical Playfield");
      var playfieldRect = playfield == null ? null : playfield.GetComponent<RectTransform>();
      var playfieldWidth = playfieldRect == null ? 0f : playfieldRect.rect.width;
      var playfieldHeight = playfieldRect == null ? 0f : playfieldRect.rect.height;
      var playfieldAspectRatio = IsFinitePositive(playfieldHeight)
        ? playfieldWidth / playfieldHeight
        : float.NaN;
      if (playfieldRect == null
        || !IsFinitePositive(playfieldWidth)
        || !IsFinitePositive(playfieldHeight)
        || float.IsNaN(playfieldAspectRatio)
        || float.IsInfinity(playfieldAspectRatio)
        || !IsApproximately(playfieldRect.anchorMin, new Vector2(0.5f, 0.5f))
        || !IsApproximately(playfieldRect.anchorMax, new Vector2(0.5f, 0.5f))
        || !IsApproximately(playfieldRect.pivot, new Vector2(0.5f, 0.5f))
        || !IsApproximately(playfieldRect.anchoredPosition, Vector2.zero)
        || Math.Abs(playfieldAspectRatio - 0.75f) > 0.001f)
      {
        throw new InvalidOperationException(
          "The playfield is not centered with a 3:4 aspect ratio.");
      }

      var spriteObject = playfield.Find("Cube Proxy Sprite");
      var spriteImage = spriteObject == null ? null : spriteObject.GetComponent<Image>();
      if (spriteImage == null
        || spriteImage.sprite == null
        || AssetDatabase.GetAssetPath(spriteImage.sprite) != CubeProxyPath)
      {
        throw new InvalidOperationException("The playfield does not reference the generated Cube proxy Sprite.");
      }

      if (playfield.GetComponentInChildren<MeshRenderer>(true) != null
        || playfield.GetComponentInChildren<MeshFilter>(true) != null)
      {
        throw new InvalidOperationException(
          "The playfield must use the static Cube proxy Sprite instead of a live mesh.");
      }

      if (meshRendererCount < 6 || missingReferenceCount != 0)
      {
        throw new InvalidOperationException("The scene has no 3D backdrop or contains missing references.");
      }

      Debug.Log(
        $"Boot scene validation passed: URP camera, 3:4 playfield, Cube Sprite, and 3D backdrop (Canvas scale factor: {canvasScaleFactor}).");
    }

    private static void ValidateBackdropRendering(Camera camera)
    {
      const int width = 320;
      const int height = 180;
      var target = new RenderTexture(
        width,
        height,
        24,
        RenderTextureFormat.ARGB32,
        RenderTextureReadWrite.sRGB)
      {
        antiAliasing = 1,
        useMipMap = false
      };
      Texture2D capture = null;
      var previousActiveTexture = RenderTexture.active;
      try
      {
        target.Create();
        var request = new UniversalRenderPipeline.SingleCameraRequest();
        if (!RenderPipeline.SupportsRenderRequest(camera, request))
        {
          throw new InvalidOperationException("URP cannot render the boot camera to a texture.");
        }

        request.destination = target;
        RenderPipeline.SubmitRenderRequest(camera, request);
        RenderTexture.active = target;
        capture = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
        capture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
        capture.Apply(false, false);

        var colors = new HashSet<uint>();
        foreach (var pixel in capture.GetPixels32())
        {
          colors.Add(((uint)pixel.r << 24) | ((uint)pixel.g << 16) | ((uint)pixel.b << 8) | pixel.a);
          if (colors.Count >= 8)
          {
            break;
          }
        }

        if (colors.Count < 8)
        {
          throw new InvalidOperationException("The boot camera did not render visible 3D backdrop geometry.");
        }
      }
      finally
      {
        RenderTexture.active = previousActiveTexture;
        if (target != null)
        {
          target.Release();
          UnityEngine.Object.DestroyImmediate(target);
        }

        if (capture != null)
        {
          UnityEngine.Object.DestroyImmediate(capture);
        }
      }
    }

    private static bool IsApproximately(Vector2 actual, Vector2 expected)
    {
      return Math.Abs(actual.x - expected.x) <= 0.001f
        && Math.Abs(actual.y - expected.y) <= 0.001f;
    }

    private static bool IsApproximately(Vector3 actual, Vector3 expected)
    {
      return Math.Abs(actual.x - expected.x) <= 0.001f
        && Math.Abs(actual.y - expected.y) <= 0.001f
        && Math.Abs(actual.z - expected.z) <= 0.001f;
    }

    private static bool IsFinitePositive(float value)
    {
      return value > 0f && !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private static bool IsFinitePositive(Vector3 value)
    {
      return IsFinitePositive(value.x) && IsFinitePositive(value.y) && IsFinitePositive(value.z);
    }

    private static Scene OpenWorkingScene(out bool singleSceneMode, out Scene sceneToRestore)
    {
      var emptyUnsavedScenes = new List<Scene>();
      for (var index = 0; index < SceneManager.sceneCount; index++)
      {
        var scene = SceneManager.GetSceneAt(index);
        if (!string.IsNullOrEmpty(scene.path))
        {
          continue;
        }

        if (scene.isDirty || scene.GetRootGameObjects().Length > 0)
        {
          throw new InvalidOperationException(
            "Save or close any unsaved scene with content before running this editor tool.");
        }

        emptyUnsavedScenes.Add(scene);
      }

      var allScenesAreEmpty = SceneManager.sceneCount == 0 || emptyUnsavedScenes.Count == SceneManager.sceneCount;
      if (!allScenesAreEmpty)
      {
        foreach (var emptyScene in emptyUnsavedScenes)
        {
          EditorSceneManager.CloseScene(emptyScene, true);
        }
      }

      sceneToRestore = allScenesAreEmpty ? default : SceneManager.GetActiveScene();
      singleSceneMode = allScenesAreEmpty;
      return EditorSceneManager.NewScene(
        NewSceneSetup.EmptyScene,
        singleSceneMode ? NewSceneMode.Single : NewSceneMode.Additive);
    }

    private static void CloseGeneratedScene(Scene generatedScene, Scene sceneToRestore)
    {
      if (generatedScene.IsValid() && generatedScene.isLoaded)
      {
        EditorSceneManager.CloseScene(generatedScene, true);
      }

      if (sceneToRestore.IsValid() && sceneToRestore.isLoaded)
      {
        SceneManager.SetActiveScene(sceneToRestore);
      }
    }

    private static void ValidateRenderedCube(Texture2D image)
    {
      var pixels = image.GetPixels32();
      for (var x = 0; x < image.width; x++)
      {
        if (pixels[x].a > 8 || pixels[(image.height - 1) * image.width + x].a > 8)
        {
          throw new InvalidOperationException(
            "The URP render produced an opaque pixel on the Cube proxy image border.");
        }
      }

      for (var y = 1; y < image.height - 1; y++)
      {
        var rowStart = y * image.width;
        if (pixels[rowStart].a > 8 || pixels[rowStart + image.width - 1].a > 8)
        {
          throw new InvalidOperationException(
            "The URP render produced an opaque pixel on the Cube proxy image border.");
        }
      }

      var visiblePixelCount = 0;
      var hasColorVariation = false;
      var hasReferenceColor = false;
      var referenceColor = new Color32();
      foreach (var pixel in pixels)
      {
        if (pixel.a <= 8)
        {
          continue;
        }

        visiblePixelCount++;
        if (!hasReferenceColor)
        {
          referenceColor = pixel;
          hasReferenceColor = true;
        }
        else if (Math.Abs(pixel.r - referenceColor.r) > 4
          || Math.Abs(pixel.g - referenceColor.g) > 4
          || Math.Abs(pixel.b - referenceColor.b) > 4)
        {
          hasColorVariation = true;
        }
      }

      if (visiblePixelCount < 500 || !hasColorVariation)
      {
        throw new InvalidOperationException("The URP render did not produce a visible Cube proxy image.");
      }
    }

    private static void CreateBootCamera(Scene scene)
    {
      var cameraObject = new GameObject("Main Camera");
      SceneManager.MoveGameObjectToScene(cameraObject, scene);
      cameraObject.tag = "MainCamera";
      cameraObject.transform.position = new Vector3(0f, 0f, -10f);

      var camera = cameraObject.AddComponent<Camera>();
      camera.clearFlags = CameraClearFlags.SolidColor;
      camera.backgroundColor = new Color(0.018f, 0.032f, 0.085f, 1f);
      camera.orthographic = true;
      camera.orthographicSize = 5.4f;
      camera.nearClipPlane = 0.1f;
      camera.farClipPlane = 100f;

      var additionalCameraData = cameraObject.AddComponent<UniversalAdditionalCameraData>();
      additionalCameraData.renderType = CameraRenderType.Base;
      additionalCameraData.renderPostProcessing = false;
    }

    private static void CreateBackdrop(Scene scene)
    {
      var lightObject = new GameObject("Backdrop Key Light");
      SceneManager.MoveGameObjectToScene(lightObject, scene);
      lightObject.transform.rotation = Quaternion.Euler(38f, -28f, 0f);
      var light = lightObject.AddComponent<Light>();
      light.type = LightType.Directional;
      light.color = new Color(0.47f, 0.72f, 1f);
      light.intensity = 1.1f;
      light.shadows = LightShadows.Soft;

      CreateBackdropCube(scene, "Left Tower", new Vector3(-7.1f, 0f, 3f), new Vector3(1.2f, 8.5f, 0.8f));
      CreateBackdropCube(scene, "Right Tower", new Vector3(7.1f, 0.1f, 4f), new Vector3(1.5f, 7f, 1f));
      CreateBackdropCube(scene, "Left Platform", new Vector3(-5.9f, -4.35f, 2f), new Vector3(2.8f, 0.45f, 1f));
      CreateBackdropCube(scene, "Right Platform", new Vector3(6.1f, -3.8f, 3.5f), new Vector3(2.2f, 0.35f, 0.7f));
      CreateBackdropCube(scene, "Upper Block", new Vector3(5.4f, 3.9f, 4.5f), new Vector3(1.2f, 1.1f, 0.9f));
      CreateBackdropCube(scene, "Lower Block", new Vector3(-5.2f, -2.9f, 4f), new Vector3(0.9f, 1.2f, 0.8f));
    }

    private static void CreateBackdropCube(Scene scene, string name, Vector3 position, Vector3 scale)
    {
      var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      SceneManager.MoveGameObjectToScene(cube, scene);
      cube.name = name;
      cube.transform.position = position;
      cube.transform.localScale = scale;
      UnityEngine.Object.DestroyImmediate(cube.GetComponent<Collider>());
    }

    private static void CreatePresentationCanvas(Scene scene, Sprite proxySprite)
    {
      var canvasObject = new GameObject(
        "Presentation Canvas",
        typeof(RectTransform),
        typeof(Canvas),
        typeof(CanvasScaler));
      SceneManager.MoveGameObjectToScene(canvasObject, scene);

      var canvas = canvasObject.GetComponent<Canvas>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      var scaler = canvasObject.GetComponent<CanvasScaler>();
      scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
      scaler.referenceResolution = new Vector2(1920f, 1080f);
      scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
      scaler.matchWidthOrHeight = 0.5f;
      canvasObject.transform.localScale = Vector3.one;

      var playfieldObject = new GameObject(
        "Vertical Playfield",
        typeof(RectTransform),
        typeof(CanvasRenderer),
        typeof(Image));
      SceneManager.MoveGameObjectToScene(playfieldObject, scene);
      playfieldObject.transform.SetParent(canvasObject.transform, false);
      var playfieldRect = playfieldObject.GetComponent<RectTransform>();
      playfieldRect.anchorMin = new Vector2(0.5f, 0.5f);
      playfieldRect.anchorMax = new Vector2(0.5f, 0.5f);
      playfieldRect.pivot = new Vector2(0.5f, 0.5f);
      playfieldRect.anchoredPosition = Vector2.zero;
      playfieldRect.sizeDelta = new Vector2(810f, 1080f);
      var playfieldImage = playfieldObject.GetComponent<Image>();
      playfieldImage.color = new Color(0.012f, 0.018f, 0.055f, 0.82f);
      playfieldImage.raycastTarget = false;

      var spriteObject = new GameObject(
        "Cube Proxy Sprite",
        typeof(RectTransform),
        typeof(CanvasRenderer),
        typeof(Image));
      SceneManager.MoveGameObjectToScene(spriteObject, scene);
      spriteObject.transform.SetParent(playfieldObject.transform, false);
      var spriteRect = spriteObject.GetComponent<RectTransform>();
      spriteRect.anchorMin = new Vector2(0.5f, 0.5f);
      spriteRect.anchorMax = new Vector2(0.5f, 0.5f);
      spriteRect.pivot = new Vector2(0.5f, 0.5f);
      spriteRect.anchoredPosition = Vector2.zero;
      spriteRect.sizeDelta = new Vector2(300f, 450f);
      var spriteImage = spriteObject.GetComponent<Image>();
      spriteImage.sprite = proxySprite;
      spriteImage.preserveAspect = true;
      spriteImage.raycastTarget = false;
    }

    private static void EnsureAssetFolder(string folderPath)
    {
      var parts = folderPath.Split('/');
      var current = parts[0];
      for (var index = 1; index < parts.Length; index++)
      {
        var next = $"{current}/{parts[index]}";
        if (!AssetDatabase.IsValidFolder(next))
        {
          AssetDatabase.CreateFolder(current, parts[index]);
        }

        current = next;
      }
    }

    private static string GetProjectFilePath(string assetPath)
    {
      var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
      if (string.IsNullOrEmpty(projectRoot))
      {
        throw new InvalidOperationException("Unity did not provide a valid project Assets path.");
      }

      return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
    }
  }
}
