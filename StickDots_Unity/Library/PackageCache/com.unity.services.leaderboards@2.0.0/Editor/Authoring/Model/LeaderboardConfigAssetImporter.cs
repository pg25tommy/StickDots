using System;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.Services.Leaderboards.Editor.Authoring.Model
{
    [ScriptedImporter(1, LeaderboardAssetsExtensions.configExtension)]
    class LeaderboardConfigAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx.assetPath == null)
            {
                throw new FileLoadException("Impossible to load the asset.");
            }

            var asset = ScriptableObject.CreateInstance<LeaderboardConfigAsset>();
            asset.Path = ctx.assetPath;
            ctx.AddObjectToAsset("MainAsset", asset);
            ctx.SetMainObject(asset);
            // TODO: prevent people from initializing IDeploymentItems here,
            // it's better to do it on the observable asset side in the added event
        }
    }
}
