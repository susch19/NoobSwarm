using NonSucking.Framework.Extension.IoC;
using NoobSwarm.Hotkeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NoobSwarm.Lights.LightEffects;
using static NoobSwarm.MakroManager;

namespace NoobSwarm.Lights
{
    public class AddRemoveLightCommand : IHotkeyCommand
    {
        private LightService? lightService;

        public HotKeyType HotKeyType { get; set; }
        public LightEffect? Effect { get; set; }
        public bool InsertAtEnd { get; set; }

        public int InsertPosition { get; set; }

        public AddRemoveLightCommand()
        {
        }

        public AddRemoveLightCommand(LightEffect effect)
        {
            Effect = effect;
        }

        public void Execute()
        {
            lightService ??= TypeContainer.GetOrNull<LightService>();
            if (lightService is null)
            {
                return;
            }

            if (Effect is not null)
            {
                if (lightService.Contains(Effect))
                    lightService.RemoveLightEffect(Effect);
                else
                {
                    if (InsertAtEnd)
                        lightService.AddToEnd(Effect);
                    else if (InsertPosition > -1)
                        lightService.AddAtPosition(Effect, InsertPosition);
                    else
                        lightService.AddToStart(Effect);
                }
            }
        }

        public void Deserialize()
        {
        }

        public void Serialize()
        {
        }
    }
}