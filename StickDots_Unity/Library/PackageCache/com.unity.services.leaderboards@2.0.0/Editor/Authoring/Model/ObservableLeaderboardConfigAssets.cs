using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Leaderboards.Assets;
using UnityEngine;

namespace Unity.Services.Leaderboards.Editor.Authoring.Model
{
    /// <summary>
    /// This class serves to track creation and deletion of assets of the
    /// associated service type
    /// </summary>
    sealed class ObservableLeaderboardConfigAssets : ObservableCollection<IDeploymentItem>, IDisposable
    {
        readonly ObservableAssets<LeaderboardConfigAsset> m_LeaderboardsAssets;

        public ObservableLeaderboardConfigAssets()
        {
            m_LeaderboardsAssets = new ObservableAssets<LeaderboardConfigAsset>();

            foreach (var asset in m_LeaderboardsAssets)
            {
                OnNewAsset(asset);
            }
            m_LeaderboardsAssets.CollectionChanged += LeaderboardsAssetsOnCollectionChanged;
        }

        public void Dispose()
        {
            m_LeaderboardsAssets.CollectionChanged -= LeaderboardsAssetsOnCollectionChanged;
        }

        void OnNewAsset(LeaderboardConfigAsset asset)
        {
            asset.Model.FromJsonFile(asset.Path);

            Add(asset.Model);
        }

        void LeaderboardsAssetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems.Cast<LeaderboardConfigAsset>())
                {
                    Remove(oldItem.Model);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems.Cast<LeaderboardConfigAsset>())
                {
                    OnNewAsset(newItem);
                }
            }
        }
    }
}
