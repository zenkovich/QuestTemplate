using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class StaticBuilder
{
    static string staticResourcesPath = "kiss-static";
    static string dataPath = "Assets/Data";
    static string assetBundlesPath = "Assets/Bundles";
    static string framesPath = "/inventory/frames/mobile";
    static string framePrefabName = "FrameObject.prefab";

    static int frameSpriteSheetColumns = 50;
    static int frameSpriteSheetRows = 2;
    static int idleFramesCount = 50;
    static int specialFramesCount = 50;
    static int animationFPS = 25;
    static string[] nonAnimatedFrameLevels = { "level-1", "level-2" };

    static float referenceAvatarSizeSize = 98;
    static Dictionary<string, Vector2> screenResolutions = new Dictionary<string, Vector2>() {
        { "1x", new Vector2(360, 640) },
        { "2x", new Vector2(720, 1280) },
        { "3x", new Vector2(1080, 1920) },
        { "ldpi", new Vector2(240, 480) },
        { "mdpi", new Vector2(360, 640) },
        { "hdpi", new Vector2(540, 960) },
        { "xhdpi", new Vector2(720, 1280) },
        { "xxhdpi", new Vector2(1080, 1920) },
        { "xxxhdpi", new Vector2(1440, 2560) }
    };
    static string referenceScreenResolution = "hdpi";

    [MenuItem("Build/Run android")]
    public static void RebuildNonForcible()
    {
        Rebuild(false, true, BuildTarget.Android);
    }

    [MenuItem("Build/Run android forcible")]
    public static void RebuildForcible()
    {
        Rebuild(true, true, BuildTarget.Android);
    }

    [MenuItem("Build/Run android without bundles")]
    public static void RebuildNonForcibleWOBundles()
    {
        Rebuild(false, false, BuildTarget.Android);
    }

    [MenuItem("Build/Run android forcible without bundles")]
    public static void RebuildForcibleWOBundles()
    {
        Rebuild(true, false, BuildTarget.Android);
    }

    [MenuItem("Build/Clear frames")]
    public static void ClearFramesMenuItem()
    {
        AssetDatabase.StartAssetEditing();

        ClearFrames(dataPath + framesPath);

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    [MenuItem("Build/Clear bundles")]
    public static void ClearBundlesMenuItem()
    {
        Directory.Delete(assetBundlesPath, true);
        AssetDatabase.Refresh();
    }

    [MenuItem("Build/Clear bundles")]
    public static void ClearBundlesMenu()
    {
        AssetDatabase.StartAssetEditing();

        FramesDirectoriesListingFunc listFunc = (frameDirectory, progress, frameId, level, gender, language, resolution) =>
        {
            var files = Directory.GetFiles(frameDirectory);
            foreach (var file in files)
            {
                bool cancelled = EditorUtility.DisplayCancelableProgressBar("Clear bundles", frameDirectory, progress);

                AssetImporter imp = AssetImporter.GetAtPath(file);
                imp.assetBundleName = "";

                if (cancelled)
                    throw new Exception("Stopped by user");
            }

        };

        ListFramesDirectories(dataPath + framesPath, listFunc);

        AssetDatabase.StopAssetEditing();
        EditorUtility.ClearProgressBar();
    }

    public static void Rebuild(bool forcible, bool buildBundles, BuildTarget buildTarget)
    {
        CopyFramesAssetsFromStatic(staticResourcesPath + framesPath, dataPath + framesPath);

        RebuildFrames(forcible, dataPath + framesPath);

        if (buildBundles)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    BuildAndroidAssetBundles();
                    break;

                default:
                    Debug.LogError("Unsupported bundles platform:" + buildTarget.ToString());
                    break;
            }
        }
    }

    private static void BuildAndroidAssetBundles()
    {
        Dictionary<string, TextureImporterFormat> formats = new Dictionary<string, TextureImporterFormat>() {
            { "PVR", TextureImporterFormat.PVRTC_RGBA4 },
            { "DXT", TextureImporterFormat.DXT5 }
        };

        foreach (var kv in formats)
        {
            TextureImporterPlatformSettings textureSettings = new TextureImporterPlatformSettings();
            textureSettings.maxTextureSize = 2048;
            textureSettings.name = "Android";
            textureSettings.format = kv.Value;

            //SetFramesTexturesPlatformSettings(dataPath + framesPath, textureSettings);

            Debug.Log("Building android asset bundles for " + kv.Key);

            string assetsBundlesPath = assetBundlesPath + "/Android/" + kv.Key;
            if (!Directory.Exists(assetsBundlesPath))
                Directory.CreateDirectory(assetsBundlesPath);

            //             var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            //             var manifest = BuildPipeline.BuildAssetBundles(assetBundlesPath + "/Android/" + kv.Key, options, BuildTarget.Android);
            //             
            FramesDirectoriesListingFunc listFunc = (frameDirectory, progress, frameId, level, gender, language, resolution) =>
            {
                bool cancelled = EditorUtility.DisplayCancelableProgressBar("Building bundle", frameDirectory, progress);

                var files = Directory.GetFiles(frameDirectory);
                foreach (var file in files)
                {
                    AssetImporter imp = AssetImporter.GetAtPath(file);
                    imp.assetBundleName = "frames/" + frameId + "/" + level + "/" + gender + "/" + language + "/" + resolution;
                }

                var options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
                var manifest = BuildPipeline.BuildAssetBundles(assetBundlesPath + "/Android/" + kv.Key, options, BuildTarget.Android);

                foreach (var file in files)
                {
                    AssetImporter imp = AssetImporter.GetAtPath(file);
                    imp.assetBundleName = "";
                }

                if (cancelled)
                    throw new Exception("Stopped by user");
            };

            ListFramesDirectories(dataPath + framesPath, listFunc);
        }
    }

    private static void SetFramesTexturesPlatformSettings(string directory, TextureImporterPlatformSettings settings)
    {
        AssetDatabase.StartAssetEditing();

        FramesDirectoriesListingFunc listFunc = (frameDirectory, progress, frameId, level, gender, language, resolution) =>
        {

            string frameDataPath = dataPath + "/" + frameId + "/" + level + "/" + gender + "/" + language + "/" + resolution;

            var files = Directory.GetFiles(frameDirectory);
            foreach (var file in files)
            {
                var importer = AssetImporter.GetAtPath(file) as TextureImporter;

                if (importer == null)
                    continue;

                bool cancelled = EditorUtility.DisplayCancelableProgressBar("Setting texture format: " + settings.format.ToString(), file, progress);

                var currSettings = importer.GetPlatformTextureSettings(settings.name);

                if (currSettings.format == settings.format && currSettings.maxTextureSize == settings.maxTextureSize)
                    continue;

                importer.SetPlatformTextureSettings(settings);

                if (cancelled)
                    throw new Exception("Stopped by user");
            }
        };

        ListFramesDirectories(directory, listFunc);

        AssetDatabase.StopAssetEditing();
    }

    public delegate void FramesDirectoriesListingFunc(string directory, float progress, string frameId, string level, string gender, string language, string resolution);

    private static void ListFramesDirectories(string directory, FramesDirectoriesListingFunc listFunction)
    {
        var framesDirectories = Directory.GetDirectories(directory);
        for (int i = 0; i < framesDirectories.Length; i++)
        {
            float frameDirectoryProgressStep = 1.0f/(float)framesDirectories.Length;
            float frameDirectoryProgress = (float)i*frameDirectoryProgressStep;
            string frameId = Path.GetFileNameWithoutExtension(framesDirectories[i]);

            var levelsDirectories = Directory.GetDirectories(framesDirectories[i]);
            for (int j = 0; j < levelsDirectories.Length; j++)
            {
                float levelDirectoryProgressStep = 1.0f/(float)levelsDirectories.Length*frameDirectoryProgressStep;
                float levelDirectoryProgress = (float)j*levelDirectoryProgressStep + frameDirectoryProgress;
                string level = Path.GetFileNameWithoutExtension(levelsDirectories[j]);

                var genderDirectories = Directory.GetDirectories(levelsDirectories[j]);
                for (int k = 0; k < genderDirectories.Length; k++)
                {
                    float genderDirectoryProgressStep = 1.0f/(float)genderDirectories.Length*levelDirectoryProgressStep;
                    float genderDirectoryProgress = (float)k*genderDirectoryProgressStep + levelDirectoryProgress;
                    string gender = Path.GetFileNameWithoutExtension(genderDirectories[k]);

                    var languageDirectories = Directory.GetDirectories(genderDirectories[k]);
                    for (int l = 0; l < languageDirectories.Length; l++)
                    {
                        float languageDirectoryProgressStep = 1.0f/(float)languageDirectories.Length*genderDirectoryProgressStep;
                        float languageDirectoryProgress = (float)l*languageDirectoryProgressStep + genderDirectoryProgress;
                        string language = Path.GetFileNameWithoutExtension(languageDirectories[l]);

                        var resolutionDirectories = Directory.GetDirectories(languageDirectories[l]);
                        for (int m = 0; m < resolutionDirectories.Length; m++)
                        {
                            float resolutionDirectoryProgressStep = 1.0f/(float)resolutionDirectories.Length*languageDirectoryProgressStep;
                            float resolutionDirectoryProgress = (float)m*resolutionDirectoryProgressStep + languageDirectoryProgress;
                            string resolution = Path.GetFileNameWithoutExtension(resolutionDirectories[m]);

                            listFunction(resolutionDirectories[m], resolutionDirectoryProgress, frameId, level, gender, language, resolution);
                        }
                    }
                }
            }
        }
    }

    public static void RebuildFrames(bool forcible, string directory)
    {
        AssetDatabase.StartAssetEditing();

        if (forcible)
            ClearFrames(directory);

        try
        {
            FramesDirectoriesListingFunc listFunc = (frameDirectory, progress, frameId, level, gender, language, resolution) =>
            {
                bool cancelled = EditorUtility.DisplayCancelableProgressBar("Rebuilding frames", frameDirectory, progress);

                if (!File.Exists(frameDirectory + "/" + framePrefabName))
                    GenerateFramePrefab(frameDirectory, frameId, level, gender, language, resolution);

                if (cancelled)
                    throw new Exception("Stopped by user");
            };

            ListFramesDirectories(directory, listFunc);
        }
        catch (Exception ex)
        {
            Debug.LogError("Frames rebuild failed: " + ex.Message);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    public static void CopyFramesAssetsFromStatic(string staticPath, string dataPath)
    {
        AssetDatabase.StartAssetEditing();

        try
        {
            FramesDirectoriesListingFunc listFunc = (frameDirectory, progress, frameId, level, gender, language, resolution) =>
            {
                bool cancelled = EditorUtility.DisplayCancelableProgressBar("Copying frames resources", frameDirectory, progress);

                string frameDataPath = dataPath + "/" + frameId + "/" + level + "/" + gender + "/" + language + "/" + resolution;

                if (Directory.Exists(frameDataPath))
                    return;

                Directory.CreateDirectory(frameDataPath);

                var files = Directory.GetFiles(frameDirectory);
                foreach (var file in files)
                    File.Copy(file, frameDataPath + "/" + Path.GetFileName(file), true);

                if (cancelled)
                    throw new Exception("Stopped by user");
            };

            ListFramesDirectories(staticPath, listFunc);
        }
        catch (Exception ex)
        {
            Debug.LogError("Frames resources copying failed: " + ex.Message);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    public static void ClearFrames(string directory)
    {
        try
        {
            FramesDirectoriesListingFunc listFunc = (frameDirectory, progress, frameId, level, gender, language, resolution) =>
            {
                bool cancelled = EditorUtility.DisplayCancelableProgressBar("Clearing frames", frameDirectory, progress);

                var files = Directory.GetFiles(frameDirectory);
                foreach (var file in files)
                {
                    if (file.EndsWith(".prefab") || file.EndsWith(".anim") || file.EndsWith(".controller") || file.Contains("frame_"))
                        AssetDatabase.DeleteAsset(file);
                }

                if (cancelled)
                    throw new Exception("Stopped by user");
            };

            ListFramesDirectories(directory, listFunc);
        }
        catch (Exception ex)
        {
            Debug.LogError("Frames clearing failed: " + ex.Message);
        }

        EditorUtility.ClearProgressBar();
    }


    private static void GenerateFramePrefab(string directory, string frameId, string level, string gender, string language, string resolution)
    {
        bool isSingleFrame = nonAnimatedFrameLevels.Contains(level);

        // create object
        GameObject frameObject = new GameObject("FrameObject");

        // generate image and animations of needed
        Image image = frameObject.AddComponent<Image>();
        var files = Directory.GetFiles(directory).ToList().FindAll(x => !x.EndsWith(".meta"));
        if (files.Count == 1)
        {
            if (isSingleFrame)
                GenerateFramePrefabFromSingleImage(files, resolution, frameObject, image);
            else
                GenerateFramePrefabFromAtlas(directory, files, resolution, frameObject, image);
        }
        else GenerateFramePrefabFromMultipleImages(directory, files, resolution, frameObject, image);

        // adjust object size
        Vector2 frameImageSize = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
        float sizeCoef = 1;
        if (screenResolutions.ContainsKey(resolution))
            sizeCoef = screenResolutions[resolution].y/screenResolutions[referenceScreenResolution].y;

        (frameObject.transform as RectTransform).sizeDelta = frameImageSize/sizeCoef;

        // create prefab and destroy object from scene
        PrefabUtility.CreatePrefab(directory + "/" + framePrefabName, frameObject, ReplacePrefabOptions.Default);
        GameObject.DestroyImmediate(frameObject);

        // assign bundle name and sprite packing tag to assets
        files = Directory.GetFiles(directory).ToList().FindAll(x => !x.EndsWith(".meta"));
        foreach (var file in files)
        {
            var imp = AssetImporter.GetAtPath(file);

            if (imp != null)
            {
                imp.assetBundleName = "frames/" + frameId + "/" + level + "/" + gender + "/" + language + "/" + resolution;

                TextureImporter texImp = imp as TextureImporter;
                if (texImp != null)
                {
                    texImp.spritePackingTag = "frame_" + frameId + "_" + level + "_" + gender + "_" + language + "_" + resolution;
                    texImp.spritePackingTag = "";
                }
            }
        }
    }

    private static void GenerateFramePrefabFromSingleImage(List<string> directoryFiles, string resolutionName, GameObject frameObject, Image imageComponent)
    {
        imageComponent.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(directoryFiles[0]);
    }

    private static void GenerateFramePrefabFromAtlas(string directory, List<string> directoryFiles, string resolutionName, GameObject frameObject, Image imageComponent)
    {
        var image = System.Drawing.Image.FromFile(directoryFiles[0]);

        int frameWidth = image.Width/frameSpriteSheetColumns;
        int frameHeight = image.Height/frameSpriteSheetRows;

        int iFrame = 0;
        for (int row = 0; row < frameSpriteSheetRows; row++)
        {
            for (int col = 0; col < frameSpriteSheetColumns; col++)
            {
                System.Drawing.Bitmap frame = GetSubBitmap(image as System.Drawing.Bitmap, new System.Drawing.Rectangle(col*frameWidth, row*frameHeight, frameWidth, frameHeight));

                string path = directory + "/frame_" + iFrame + ".png";
                frame.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                directoryFiles.Add(path);
                iFrame++;
            }
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.StartAssetEditing();

        GenerateFramePrefabFromMultipleImages(directory, directoryFiles, resolutionName, frameObject, imageComponent);
    }

    private static void GenerateFramePrefabFromMultipleImages(string directory, List<string> directoryFiles, string resolutionName, GameObject frameObject, Image imageComponent)
    {
        List<string> idleFrames = new List<string>();
        List<string> specialFrames = new List<string>();
        bool isNameWithFrames = directoryFiles.FindIndex(x => x.Contains("frame_0.png")) >= 0;

        if (isNameWithFrames)
        {
            for (int i = 0; i < idleFramesCount; i++)
                idleFrames.Add(directory + "/frame_" + i + ".png");

            for (int i = 0; i < specialFramesCount; i++)
                specialFrames.Add(directory + "/frame_" + (idleFramesCount + i) + ".png");
        }
        else
        {
            for (int i = 0; i < idleFramesCount; i++)
                idleFrames.Add(directory + "/" + i + ".png");
            
            for (int i = 0; i < specialFramesCount; i++)
                specialFrames.Add(directory + "/" + (idleFramesCount + i) + ".png");
        }

        AnimationClip idleClip = CreateSpriteSheetAnimation(idleFrames, directory + "/Idle.anim");
        AnimationClip specialClip = CreateSpriteSheetAnimation(specialFrames, directory + "/Special.anim");

        imageComponent.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(idleFrames[0]);

        Animator animator = frameObject.AddComponent<Animator>();

        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(directory + "/Controller.controller");
        animator.runtimeAnimatorController = controller;

        var idleState = controller.AddMotion(idleClip as Motion);
        var specialState = controller.AddMotion(specialClip as Motion);

        var idleToSpecial = idleState.AddTransition(specialState);
        idleToSpecial.hasExitTime = true;

        var specialToIdle = specialState.AddTransition(idleState);
        specialToIdle.hasExitTime = true;

        controller.AddParameter("special", AnimatorControllerParameterType.Trigger);

        idleToSpecial.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1, "special");
    }

    private static AnimationClip CreateSpriteSheetAnimation(List<string> frameNames, string path)
    {
        AnimationClip clip = new AnimationClip();
        ObjectReferenceKeyframe[] frames = new ObjectReferenceKeyframe[frameNames.Count];
        for (int i = 0; i < frameNames.Count; i++)
        {
            frames[i].time = (float)i*(1.0f/(float)animationFPS);
            frames[i].value = AssetDatabase.LoadAssetAtPath<Sprite>(frameNames[i]);
        }

        var curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(Image);
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";

        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, frames);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        clip.wrapMode = WrapMode.Loop;

        AssetDatabase.CreateAsset(clip, path);

        return clip;
    }

    static private System.Drawing.Bitmap GetSubBitmap(System.Drawing.Bitmap srcBitmap, System.Drawing.Rectangle section)
    {
        var bmp = new System.Drawing.Bitmap(section.Width, section.Height);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.DrawImage(srcBitmap, 0, 0, section, System.Drawing.GraphicsUnit.Pixel);
        g.Dispose();

        return bmp;
    }
}
