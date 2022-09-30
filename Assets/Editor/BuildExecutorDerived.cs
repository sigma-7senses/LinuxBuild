#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static partial class BuildExecutor
{
    #region Partial

    static partial void InitializeBuildPlayerOption(
        ref BuildPlayerOptions buildPlayerOptions,
        in IReadOnlyDictionary<string, string> args
    )
    {
        buildPlayerOptions.target = DetectTarget(args);
        buildPlayerOptions.scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        buildPlayerOptions.locationPathName = args[OptionOutput];
        if (args.ContainsKey(OptionDebug))
        {
            buildPlayerOptions.options |= BuildOptions.Development;
            buildPlayerOptions.options |= BuildOptions.AllowDebugging;
        }

        if (buildPlayerOptions.target == BuildTarget.StandaloneLinux64)
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server;
    }

    #endregion

    #region ParseArg

    private const string OptionDebug = "debug";
    private const string OptionOutput = "out";

    private static readonly Dictionary<string, BuildTarget> BuildTargets
        = new Dictionary<string, BuildTarget>
        {
            {"android", BuildTarget.Android},
            {"ios", BuildTarget.iOS},
            {"webgl", BuildTarget.WebGL},
            {"linux", BuildTarget.StandaloneLinux64},
        };

    // -target android
    private const string OptionTarget = "target";

    private static BuildTarget DetectTarget(IReadOnlyDictionary<string, string> args)
    {
        if (args.TryGetValue(OptionTarget, out var rawTarget)
            && BuildTargets.TryGetValue(rawTarget.ToLower(), out var value))
        {
            return value;
        }
        return BuildTarget.NoTarget;
    }

    #endregion


    #region EditorMenu

    [MenuItem("Tools/Build/Android")]
    private static void BuildAndroid()
    {
        var dic = new Dictionary<string, string>();
        dic[OptionTarget] = "android";

        BuildInternal(dic);
    }

    [MenuItem("Tools/Build/iOS")]
    private static void BuildiOS()
    {
        var dic = new Dictionary<string, string>();
        dic[OptionTarget] = "ios";

        BuildInternal(dic);
    }

    [MenuItem("Tools/Build/WebGL")]
    private static void BuildWebGL()
    {
        var dic = new Dictionary<string, string>();
        dic[OptionTarget] = "webgl";

        BuildInternal(dic);
    }

    [MenuItem("Tools/Build/Linux")]
    private static void BuildLinux()
    {
        var dic = new Dictionary<string, string>();
        dic[OptionTarget] = "linux";

        BuildInternal(dic);
    }

    #endregion
}

#endif