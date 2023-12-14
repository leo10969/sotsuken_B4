using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class iOSBuildCheck : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
        {
            bool proceedWithBuild = EditorUtility.DisplayDialog(
                "iOS Build Confirmation",
                "実験モードにしましたか？",
                "Yes",
                "No"
            );

            if (!proceedWithBuild)
            {
                throw new BuildFailedException("iOS build was cancelled by the user.");
            }
        }
    }
}
