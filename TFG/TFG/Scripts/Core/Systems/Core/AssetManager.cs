using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace TFG.Scripts.Core.Systems.Core;

public class AssetManager(ContentManager contentManager)
{
    // We need a content manager to load assets. 
    // We need a dictionary to store assets and their references.
    private Dictionary<string, object> _loadedAssets = new();
    // And also a dictionary to store the number of references to each asset.
    private Dictionary<string, int> _assetReferenceCounts = new();

    #region Methods

    // Load an asset.
    public T Load<T>(string assetName) where T : class
    {
        // If the asset is already loaded, increment its reference count and return it.
        if(_loadedAssets.TryGetValue(assetName, out var asset))
        {
            _assetReferenceCounts[assetName]++;
            return asset as T;
        }
        // If the asset is not loaded, create a new instance of it.
        // Load the asset using the content manager.
        var loadedAsset = contentManager.Load<T>(assetName);
        
        // Store the loaded asset and set its reference count to 1.
        _loadedAssets[assetName] = loadedAsset;
        _assetReferenceCounts[assetName] = 1;
        return loadedAsset;
    }
    
    // Unload an asset.
    public void Unload(string assetName)
    {
        if (_assetReferenceCounts.TryGetValue(assetName, out var count))
        {
            //Reduce the usage count of the asset. And save it.
            count--;
            _assetReferenceCounts[assetName] = count;
            
            //If the usage count is 0, unload the asset.
            if (count == 0)
            {
                _loadedAssets.Remove(assetName);
                _assetReferenceCounts.Remove(assetName);
                contentManager.Unload();
            }
        }
    }

    #endregion
    
    
}