#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;

/// <summary>
/// /opt/unity/Editor/Unity -projectPath "" -batchmode -nographics -quit -executeMethod BuildExecutor.Build
/// <see cref="ParseArg"/>
/// </summary>
public static partial class BuildExecutor
{
    #region Build

    public static void Build()
    {
        var args = ParseArg();
        BuildInternal(args);
    }

    private static void BuildInternal(IReadOnlyDictionary<string, string> args)
    {
        var buildPlayerOptions = new BuildPlayerOptions();
        InitializeBuildPlayerOption(ref buildPlayerOptions, in args);
        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        switch (buildReport.summary.result)
        {
            case BuildResult.Succeeded:
                // NOP
                break;
            case BuildResult.Unknown:
            case BuildResult.Failed:
            case BuildResult.Cancelled:
            default:
                EditorApplication.Exit(1);
                break;
        }
    }

    #endregion

    #region Partial

    static partial void InitializeBuildPlayerOption(
        ref BuildPlayerOptions buildPlayerOptions,
        in IReadOnlyDictionary<string, string> args
    );

    #endregion

    #region ParseArg

    private const string ExecuteMethod = "-executeMethod";

    /// <summary>
    /// -optionKey optionValue -> (optionKey, optionValue)
    /// -optionKey             -> (optionKey, null)
    /// </summary>
    private static IReadOnlyDictionary<string, string> ParseArg()
    {
        var args = Environment.GetCommandLineArgs();
        var index = Array.FindIndex(args, s => string.Equals(s, ExecuteMethod));
        if (index < 0)
        {
            throw new IndexOutOfRangeException();
        }

        const string prefixOption = "-";

        var parsedArg = new Dictionary<string, string>();
        for (var count = index + 2; count < args.Length; count++)
        {
            var rawArg = args[count];
            if (string.IsNullOrEmpty(rawArg) || !rawArg.StartsWith(prefixOption))
            {
                continue;
            }
            string optionValue = null;
            if (count + 1 < args.Length)
            {
                var rawValue = args[count + 1];
                if (!string.IsNullOrEmpty(rawValue) && !rawValue.StartsWith(prefixOption))
                {
                    optionValue = rawValue;
                }
            }
            parsedArg[rawArg.Substring(prefixOption.Length)] = optionValue;
        }

        return parsedArg;
    }

    #endregion
}
#endif