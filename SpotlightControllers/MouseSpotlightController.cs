﻿namespace Spotlight.SpotlightControllers
{
    using System.Windows.Forms;

    using Rage;
    using Rage.Native;

    using Spotlight.Core;

    internal class MouseSpotlightController : SpotlightController
    {
        readonly Keys modifierKey;
        readonly Keys toggleKey;

        protected MouseSpotlightController()
        {
            modifierKey = Plugin.Settings.GeneralSettingsIniFile.ReadEnum<Keys>("Mouse", "Modifier", Keys.LControlKey);
            toggleKey = Plugin.Settings.GeneralSettingsIniFile.ReadEnum<Keys>("Mouse", "Toggle", Keys.I);
        }

        public override bool ShouldToggleSpotlight() => Utility.IsKeyDownWithModifier(toggleKey, modifierKey);

        bool hasMoved = false;
        Rotator rotationDelta;
        protected override void UpdateControlsInternal(VehicleSpotlight spotlight)
        {
            hasMoved = false;
            float pitch = 0.0f, yaw = 0.0f;

            if (modifierKey == Keys.None || Game.IsKeyDownRightNow(modifierKey))
            {
                Game.DisableControlAction(0, GameControl.LookLeftRight, true);
                Game.DisableControlAction(0, GameControl.LookUpDown, true);

                float leftRight = NativeFunction.Natives.GetDisabledControlNormal<float>(0, (int)GameControl.LookLeftRight) * spotlight.Data.MovementSpeed;
                float upDown = NativeFunction.Natives.GetDisabledControlNormal<float>(0, (int)GameControl.LookUpDown) * spotlight.Data.MovementSpeed;
                
                yaw = -leftRight;
                pitch = -upDown;
            }

            if (pitch != 0.0f || yaw != 0.0f)
                hasMoved = true;
            rotationDelta = new Rotator(pitch, 0.0f, yaw);
        }


        protected override bool GetUpdatedRotationDeltaInternal(VehicleSpotlight spotlight, out Rotator rotation)
        {
            rotation = rotationDelta;
            return hasMoved;
        }
    }
}
