using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reloaded.Input;
using Reloaded.Input.Common;
using Reloaded.Input.Common.ControllerInputs;
using Reloaded.Input.Common.Controller_Inputs_Substructures;
using Reloaded_Mod_Template.Utilities;
using static Reloaded_Mod_Template.Utilities.ControllerButtonFunctionBuilder;

namespace Reloaded_Mod_Template
{
    /// <summary>
    /// Spawns a thread in the background that optionally controls the Heroes camera.
    /// </summary>
    public class ReloadedController
    {
        // Reloaded's Controller Manager
        public ControllerManager ControllerManager = new ControllerManager();
        public GameController GameController;

        private Thread _controllerUpdateThread;

        // Reloaded Controller
        public ReloadedController()
        {
            GameController = new GameController(ProcessController);
        }

        // Handle Controller
        private void ProcessController()
        {
            // Only process inputs ingame.
            if (!GameController.IsInMenu() && Reloaded.Native.Functions.WindowProperties.IsWindowActivated())
            {
                var inputs = ControllerManager.GetInput(0);

                // Toggle Freeze On/Off
                CheckControllerPressAndWaitForRelease(FromControllerButton(ControllerButton.ButtonLs), () =>
                {
                    if (GameController.IsGameFrozen)
                        GameController.UnFreezeGame();
                    else
                        GameController.FreezeGame();
                });

                // Toggle Camera On/Off
                CheckControllerPressAndWaitForRelease(FromControllerButton(ControllerButton.ButtonRs), () =>
                {
                    if (GameController.IsCameraEnabled)
                        GameController.FreezeCamera();
                    else
                        GameController.UnFreezeCamera();

                });

                // Process Remaining Inputs
                if (!GameController.IsCameraEnabled && !GameController.IsPaused())
                    ProcessCustomCamera(inputs);
            }
        }

        // Handles the custom camera movement when camera is frozen.
        private void ProcessCustomCamera(ControllerInputs inputs)
        {
            // Camera Movement Sticks
            GameController.MoveForward(ScaleReloadedAxis(-1 * inputs.LeftStick.GetY()));
            GameController.MoveLeft(ScaleReloadedAxis(-1 * inputs.LeftStick.GetX()));

            // Camera Rotation Sticks
            GameController.RotateUp(ScaleReloadedAxis(-1 * inputs.RightStick.GetY()));
            GameController.RotateRight(ScaleReloadedAxis(-1 * inputs.RightStick.GetX()));

            // Camera Roll Triggers
            GameController.RotateRoll(ScaleReloadedAxis(inputs.GetLeftTriggerPressure()));
            GameController.RotateRoll(ScaleReloadedAxis(-1 * inputs.GetRightTriggerPressure()));

            // Lift Camera Up (RB)
            if (inputs.ControllerButtons.ButtonRb)
                GameController.MoveUp(1F);

            // Lift Camera Down (LB)
            if (inputs.ControllerButtons.ButtonLb)
                GameController.MoveUp(-1F);

            // Modify Move Speed (DPAD LR)
            if (inputs.ControllerButtons.DpadLeft)
                GameController.MoveSpeed -= (GameController.MoveSpeed * 0.011619440F); // Calculated using Geometric Progression; Approx 1 second for 2x increase.

            if (inputs.ControllerButtons.DpadRight)
                GameController.MoveSpeed += (GameController.MoveSpeed * 0.011619440F); // Calculated using Geometric Progression; Approx 1 second for 2x increase.

            // Modify Rotate Speed (DPAD UD)
            if (inputs.ControllerButtons.DpadUp)
                GameController.RotateSpeed += (GameController.RotateSpeed * 0.011619440F);

            if (inputs.ControllerButtons.DpadDown)
                GameController.RotateSpeed -= (GameController.RotateSpeed * 0.011619440F);

            // Reset Camera (X)
            if (inputs.ControllerButtons.ButtonX)            
                GameController.ResetCamera();

            // Toggle HUD (B)
            CheckControllerPressAndWaitForRelease(FromControllerButton(ControllerButton.ButtonB), () =>
            {
                GameController.EnableHUD = !GameController.EnableHUD;
            });

            // Teleport Character (A)
            if (inputs.ControllerButtons.ButtonA)
                GameController.TeleportCharacterToCamera();

            // Reset Roll (Y)
            if (inputs.ControllerButtons.ButtonY)
                GameController.ResetCameraRoll();
        }

        /* Utility Methods */

        /// <summary>
        /// Checks if a button is pressed and if so, performs an action specified by the <see cref="functionToPerform"/>
        /// parameter; then waits for button release.
        ///
        /// This function blocks the thread.
        /// </summary>
        /// <param name="controllerCheckFunction">A function which checks if a controller button has been pressed. This can be generated with <see cref="ControllerButtonFunctionBuilder"/></param>
        /// <param name="functionToPerform">The action to perform if the button has been pressed.</param>
        public void CheckControllerPressAndWaitForRelease(Func<JoystickButtons, bool> controllerCheckFunction, Action functionToPerform)
        {
            if (controllerCheckFunction(ControllerManager.GetInput(0).ControllerButtons))
            {
                functionToPerform();

                // Wait for release.
                while (controllerCheckFunction(ControllerManager.GetInput(0).ControllerButtons))
                    Thread.Sleep(16);
            }
        }

        /// <summary>
        /// Normalizes a Reloaded Mod Loader controller axis to a maximum of 1F.
        /// </summary>
        /// <param name="axisToScale"></param>
        private float ScaleReloadedAxis(float axisToScale)
        {
            return (axisToScale / ControllerCommon.AxisMaxValueF);
        }

    }
}
