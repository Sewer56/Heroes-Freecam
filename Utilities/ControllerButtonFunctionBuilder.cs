using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Input.Common.Controller_Inputs_Substructures;

namespace Reloaded_Mod_Template.Utilities
{
    /// <summary>
    /// Builds individual functions that retrieve an individual <see cref="JoystickButtons"/> button for a given <see cref="ControllerButton"/> property.
    /// </summary>
    public static class ControllerButtonFunctionBuilder
    {
        /// <summary>
        /// Retrieves a function that returns an individual <see cref="JoystickButtons"/> button for a given <see cref="ControllerButton"/> property.
        /// </summary>
        public static Func<JoystickButtons, bool> FromControllerButton(ControllerButton controllerButton)
        {
            switch (controllerButton)
            {
                case ControllerButton.ButtonA: return buttons => buttons.ButtonA;
                case ControllerButton.ButtonB: return buttons => buttons.ButtonB;
                case ControllerButton.ButtonX: return buttons => buttons.ButtonX;
                case ControllerButton.ButtonY: return buttons => buttons.ButtonY;
                case ControllerButton.ButtonLb: return buttons => buttons.ButtonLb;
                case ControllerButton.ButtonRb: return buttons => buttons.ButtonRb;
                case ControllerButton.ButtonBack: return buttons => buttons.ButtonBack;
                case ControllerButton.ButtonStart: return buttons => buttons.ButtonStart;
                case ControllerButton.ButtonLs: return buttons => buttons.ButtonLs;
                case ControllerButton.ButtonRs: return buttons => buttons.ButtonRs;
                case ControllerButton.ButtonGuide: return buttons => buttons.ButtonGuide;
                case ControllerButton.DpadUp: return buttons => buttons.DpadUp;
                case ControllerButton.DpadLeft: return buttons => buttons.DpadLeft;
                case ControllerButton.DpadRight: return buttons => buttons.DpadRight;
                case ControllerButton.DpadDown: return buttons => buttons.DpadDown;
                default: return buttons => false;
            }
        }
    }
}
