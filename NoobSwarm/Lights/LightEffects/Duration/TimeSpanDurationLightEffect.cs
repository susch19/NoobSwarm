using NoobSwarm.Lights.LightEffects.Wrapper;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects.Duration
{
    public class TimeSpanDurationLightEffectWrapper : LightEffectWrapper
    {
        public TimeSpan ActiveTime { get; set; }

        private DateTime startTime = DateTime.Now;

        public TimeSpanDurationLightEffectWrapper(TimeSpan activeTime) : base()
        {
            ActiveTime = activeTime;
            Activated += TimeSpanDurationLightEffectWrapper_Activated;
        }


        public TimeSpanDurationLightEffectWrapper(LightEffect childEffect, TimeSpan activeTime) : base(childEffect)
        {
            ActiveTime = activeTime;
            Activated += TimeSpanDurationLightEffectWrapper_Activated;
        }

        private void TimeSpanDurationLightEffectWrapper_Activated(object? sender, bool e)
        {
            if (!e)
                return;
            startTime = DateTime.Now;
        }


        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (startTime.Add(ActiveTime) > DateTime.Now)
                base.Next(currentColors, counter, elapsedMilliseconds, stepInrease, pressed);
            else 
                Active = false;

        }

    }
}
