using System;

public sealed class SystemRandomSingleton
{
    private Random random;
    
    public uint NextUInt 
    {
        get
        {
            lock(padlock)
            {
                return (uint)(this.random.Next(Int32.MinValue, Int32.MaxValue) + (uint)Int32.MaxValue);
            }
        }
    }

    public double NextDouble 
    {
        get
        {
            lock(padlock)
            {
                return this.random.NextDouble();
            }
        }
    }

    public float NextFloat 
    {
        get
        {
            lock(padlock) 
            { 
                return Convert.ToSingle(this.random.NextDouble()); 
            }
        }
    }

    private SystemRandomSingleton()
    {
        this.random = new Random();
    }

    private static readonly object padlock = new object();

    private static SystemRandomSingleton instance = null;

    public static SystemRandomSingleton Instance
    {
        get
        {
            lock(padlock)
            {
                if(instance == null)
                {
                    instance = new SystemRandomSingleton();
                }
                
                return instance;
            }
        }
    }
}
