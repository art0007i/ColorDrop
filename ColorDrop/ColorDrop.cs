using HarmonyLib;
using NeosModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using FrooxEngine.LogiX;
using FrooxEngine.UIX;
using BaseX;
using System.Globalization;

namespace ColorDrop
{
    public class ColorDrop : NeosMod
    {
        public override string Name => "ColorDrop";
        public override string Author => "art0007i";
        public override string Version => "1.2.0";
        public override string Link => "https://github.com/art0007i/ColorDrop/";

        public override void OnEngineInit()
        {
            var harmony = new Harmony("me.art0007i.ColorDrop");
            harmony.PatchAll();

        }

        [HarmonyPatch(typeof(PrimitiveTryParsers), "GetParser", typeof(string))]
        private static class PrimitiveTryParsers_GetParser_Patch
        {
            private static readonly PrimitiveTryParsers.TryParser ColorParser = delegate (string str, out object parsed)
            {
                if (color.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var c))
                {
                    parsed = c;
                    return true;
                }

                parsed = null;
                return false;
            };

            private static readonly PrimitiveTryParsers.TryParser ColorXParser = delegate (string str, out object parsed)
            {
                if (colorX.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var cx))
                {
                    parsed = cx;
                    return true;
                }

                parsed = null;
                return false;
            };

            private static bool Prefix(string typename, ref PrimitiveTryParsers.TryParser __result)
            {
                switch (typename)
                {
                    case "color":
                        __result = ColorParser;
                        return false;

                    case "colorX":
                        __result = ColorXParser;
                        return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ColorMemberEditor), "BuildUI")]
        private static class ColorMemberEditor_BuildUI_Patch
        {
            private static readonly MethodInfo openColorPickerMethod = typeof(ColorMemberEditor).GetMethod("OpenColorPicker", AccessTools.allDeclared);

            private static bool Prefix(ColorMemberEditor __instance, UIBuilder ui, RelayRef<IField> ____target, Sync<string> ____path, FieldDrive<color> ____colorDrive, FieldDrive<color> ____colorDriveNoAlpha)
            {
                var openPickerMethod = AccessTools.MethodDelegate<ButtonEventHandler>(openColorPickerMethod, __instance);

                ui.HorizontalLayout(2f, 0f, null);

                ui.Text("R", true, null, true, null);
                ui.Style.FlexibleWidth = 10f;
                ui.PrimitiveMemberEditor(____target.Target, ____path + ".r", true, "0.##");

                ui.Style.FlexibleWidth = -1f;
                ui.Text("G", true, null, true, null);
                ui.Style.FlexibleWidth = 10f;
                ui.PrimitiveMemberEditor(____target.Target, ____path + ".g", true, "0.##");

                ui.Style.FlexibleWidth = -1f;
                ui.Text("B", true, null, true, null);
                ui.Style.FlexibleWidth = 10f;
                ui.PrimitiveMemberEditor(____target.Target, ____path + ".b", true, "0.##");

                ui.Style.FlexibleWidth = -1f;
                ui.Text("A", true, null, true, null);
                ui.Style.FlexibleWidth = 10f;
                ui.PrimitiveMemberEditor(____target.Target, ____path + ".a", true, "0.##");

                ui.Style.FlexibleWidth = -1f;
                ui.Style.MinWidth = 64f;
                ui.Panel();

                var button = ui.Button(null, openPickerMethod);
                var text = button.Slot.AttachComponent<Text>();
                text.Enabled = false;

                var textField = button.Slot.AttachComponent<TextField>();
                textField.Enabled = false;
                textField.Text = text;

                var primEditor = Traverse.Create(button.Slot.AttachComponent<PrimitiveMemberEditor>());
                primEditor.Field<SyncRef<TextEditor>>("_textEditor").Value.Target = textField.Editor;
                primEditor.Field<RelayRef<IField>>("_target").Value.Target = ____target.Target;
                primEditor.Field<FieldDrive<string>>("_textDrive").Value.Target = text.Content;

                ui.NestInto(button.Slot);
                ui.SplitHorizontally(0.5f, out var left, out var right, 0f);

                ui = new UIBuilder(left);
                ____colorDriveNoAlpha.ForceLink(ui.Image().Tint);

                ui = new UIBuilder(right);
                var img = ui.Image(NeosAssets.Common.Backgrounds.TransparentLight64, null);
                img.PreserveAspect.Value = false;

                ui.Nest();
                ____colorDrive.ForceLink(ui.Image().Tint);

                return false;
            }
        }
    }
}