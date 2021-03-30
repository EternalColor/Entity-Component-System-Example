using System.Collections;
using System.Collections.Generic;

public class RenderTextureCacheSingleton
{
    private IDictionary<int, UnityEngine.RenderTexture> cachedRenderTextures;

    public UnityEngine.RenderTexture GetOrCreateRenderTexture(int key, int width, int height, int depth)
    {
        lock(padlock)
        {
            if(this.cachedRenderTextures == null)
            {
                this.cachedRenderTextures = new Dictionary<int, UnityEngine.RenderTexture>();
            }

            if(!this.cachedRenderTextures.ContainsKey(key))
            {
                UnityEngine.RenderTexture renderTexture = new UnityEngine.RenderTexture(width, height, depth);
                renderTexture.Create();

                this.cachedRenderTextures.Add(key, renderTexture);
            }

            return this.cachedRenderTextures[key];
        }
    }

    private RenderTextureCacheSingleton()
    {
    }

    private static readonly object padlock = new object();

    private static RenderTextureCacheSingleton instance = null;

    public static RenderTextureCacheSingleton Instance
    {
        get
        {
            lock(padlock)
            {
                if(instance == null)
                {
                    instance = new RenderTextureCacheSingleton();
                }
                
                return instance;
            }
        }
    }
}
