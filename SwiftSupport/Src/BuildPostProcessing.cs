// Based on // https://github.com/jwtan/SwiftToUnityExample/blob/main/Assets/Plugins/iOS/SwiftToUnity/Editor/SwiftToUnityPostProcess.cs

#if UNITY_EDITOR && PLATFORM_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public static class BuildPostProcessing
{

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
       
        if(buildTarget != BuildTarget.iOS)
        {
            return;
        }
        
        string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

        // to improve
        string projAsText = File.ReadAllText(projPath);

        var proj = new PBXProject();
        proj.ReadFromFile(projPath);
        var targetGUID = proj.GetUnityFrameworkTargetGuid(); 

        //set xcode proj properties
        proj.AddBuildProperty(targetGUID, "DEFINES_MODULE", "YES");

        var moduleFile = path + "/UnityFramework/UnityFramework.modulemap";
        if (!File.Exists(moduleFile))
        {
            FileUtil.CopyFileOrDirectory("Packages/com.freetoolsassociation.swiftsupport/Src/UnityFramework.modulemap", moduleFile);
            proj.AddFile(moduleFile, "UnityFramework/UnityFramework.modulemap");
            proj.AddBuildProperty(targetGUID, "MODULEMAP_FILE", "$(SRCROOT)/UnityFramework/UnityFramework.modulemap");
        }
                
        proj.AddBuildProperty(targetGUID, "SWIFT_VERSION", "5.0");
        proj.SetBuildProperty(targetGUID, "COREML_CODEGEN_LANGUAGE", "Swift");
                
  
         // Headers
        string unityInterfaceGuid = proj.FindFileGuidByProjectPath("Classes/Unity/UnityInterface.h");
        proj.AddPublicHeaderToBuild(targetGUID, unityInterfaceGuid);

        string unityForwardDeclsGuid = proj.FindFileGuidByProjectPath("Classes/Unity/UnityForwardDecls.h");
        proj.AddPublicHeaderToBuild(targetGUID, unityForwardDeclsGuid);

        string unityRenderingGuid = proj.FindFileGuidByProjectPath("Classes/Unity/UnityRendering.h");
        proj.AddPublicHeaderToBuild(targetGUID, unityRenderingGuid);

        string unitySharedDeclsGuid = proj.FindFileGuidByProjectPath("Classes/Unity/UnitySharedDecls.h");
        proj.AddPublicHeaderToBuild(targetGUID, unitySharedDeclsGuid);

        proj.WriteToFile(projPath);

    }
}
#endif //UNITY_EDITOR && PLATFORM_IOS
