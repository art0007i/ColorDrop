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
            Harmony harmony = new Harmony("me.art0007i.ColorDrop");
            harmony.PatchAll();

        }

        [HarmonyPatch(typeof(PrimitiveTryParsers), "GetParser", typeof(string))]
        class PrimitiveTryParsers_GetParser_Patch
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

            public static bool Prefix(string typename, ref PrimitiveTryParsers.TryParser __result)
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
        class ColorMemberEditor_BuildUI_Patch
        {
            private static bool Prefix(ColorMemberEditor __instance, UIBuilder ui, RelayRef<IField> ____target, Sync<string> ____path, FieldDrive<color> ____colorDrive, FieldDrive<color> ____colorDriveNoAlpha)
            {
                var openPickerMethod = AccessTools.MethodDelegate<ButtonEventHandler>(__instance.GetType().GetMethod("OpenColorPicker", AccessTools.allDeclared), __instance);

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
                Button button = ui.Button(null, openPickerMethod);
                var text = button.Slot.AttachComponent<Text>();
                var textField = button.Slot.AttachComponent<TextField>();
                textField.Text = text;
                var primEditor = button.Slot.AttachComponent<PrimitiveMemberEditor>();
                (AccessTools.Field(primEditor.GetType(), "_textEditor").GetValue(primEditor) as SyncRef<TextEditor>).Target = (textField.Editor);
                (AccessTools.Field(primEditor.GetType(), "_target").GetValue(primEditor) as RelayRef<IField>).Target = ____target.Target;
                (AccessTools.Field(primEditor.GetType(), "_textDrive").GetValue(primEditor) as FieldDrive<string>).Target = text.Content;
                textField.Enabled = false;
                text.Enabled = false;
                ui.NestInto(button.Slot);
                RectTransform rect;
                RectTransform rect2;
                ui.SplitHorizontally(0.5f, out rect, out rect2, 0f);
                ui = new UIBuilder(rect);
                ____colorDriveNoAlpha.ForceLink(ui.Image().Tint);
                ui = new UIBuilder(rect2);
                var img = ui.Image(NeosAssets.Common.Backgrounds.TransparentLight64, null);
                img.PreserveAspect.Value = false;
                ui.Nest();
                ____colorDrive.ForceLink(ui.Image().Tint);

                return false;
            }
        }
    }
}