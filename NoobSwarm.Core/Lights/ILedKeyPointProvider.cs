using System.Collections.Generic;

namespace NoobSwarm.Lights
{
    public interface ILedKeyPointProvider
    {
        IReadOnlyCollection<LedKeyPoint> GetLedKeyPoints();
    }
}