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

namespace ColorDrop
{
	public class ColorDrop : NeosMod
	{
		public override string Name => "ColorDrop";
		public override string Author => "art0007i";
		public override string Version => "1.1.0";
		public override string Link => "https://github.com/art0007i/ColorDrop/";
		public override void OnEngineInit()
		{
			Harmony harmony = new Harmony("me.art0007i.ColorDrop");
			harmony.PatchAll();

		}
		[HarmonyPatch(typeof(PrimitiveTryParsers))]
		[HarmonyPatch("GetParser", typeof(string))]
		class PrimitiveTryParsers_GetParser_Patch
		{
			private static readonly PrimitiveTryParsers.TryParser ColorParser = delegate (string str, out object parsed)
			{
				color c;
				if (color.TryParse(str, out c))
				{
					parsed = c;
					return true;
				}
				parsed = null;
				return false;
			};

			private static readonly PrimitiveTryParsers.TryParser ColorXParser = delegate (string str, out object parsed)
			{
				colorX cx;
				if (colorX.TryParse(str, out cx))
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
		[HarmonyPatch(typeof(ColorMemberEditor))]
		[HarmonyPatch("BuildUI")]
		class ColorMemberEditor_BuildUI_Patch
		{
			private static bool Prefix(UIBuilder ui, ColorMemberEditor __instance)
			{
				RelayRef<IField> this_target = (RelayRef<IField>)AccessTools.Field(__instance.GetType(), "_target").GetValue(__instance);
				Sync<string> path = (Sync<string>)AccessTools.Field(__instance.GetType(), "_path").GetValue(__instance);
				ButtonEventHandler openPickerMethod = (ButtonEventHandler)AccessTools.Method(__instance.GetType(), "OpenColorPicker").CreateDelegate(typeof(ButtonEventHandler), __instance);
				FieldDrive<color> colorDrive = (FieldDrive<color>)AccessTools.Field(__instance.GetType(), "_colorDrive").GetValue(__instance);
				FieldDrive<color> colorDriveNoAlpha = (FieldDrive<color>)AccessTools.Field(__instance.GetType(), "_colorDriveNoAlpha").GetValue(__instance);

				ui.HorizontalLayout(2f, 0f, null);
				ui.Text("R", true, null, true, null);
				ui.Style.FlexibleWidth = 10f;
				ui.PrimitiveMemberEditor(this_target.Target, path + ".r", true, "0.##");
				ui.Style.FlexibleWidth = -1f;
				ui.Text("G", true, null, true, null);
				ui.Style.FlexibleWidth = 10f;
				ui.PrimitiveMemberEditor(this_target.Target, path + ".g", true, "0.##");
				ui.Style.FlexibleWidth = -1f;
				ui.Text("B", true, null, true, null);
				ui.Style.FlexibleWidth = 10f;
				ui.PrimitiveMemberEditor(this_target.Target, path + ".b", true, "0.##");
				ui.Style.FlexibleWidth = -1f;
				ui.Text("A", true, null, true, null);
				ui.Style.FlexibleWidth = 10f;
				ui.PrimitiveMemberEditor(this_target.Target, path + ".a", true, "0.##");
				ui.Style.FlexibleWidth = -1f;
				ui.Style.MinWidth = 64f;
				ui.Panel();
				Button button = ui.Button(null, openPickerMethod);
				var text = button.Slot.AttachComponent<Text>();
				var textField = button.Slot.AttachComponent<TextField>();
				textField.Text = text;
				var primEditor = button.Slot.AttachComponent<PrimitiveMemberEditor>();
				(AccessTools.Field(primEditor.GetType(), "_textEditor").GetValue(primEditor) as SyncRef<TextEditor>).Target = (textField.Editor);
				(AccessTools.Field(primEditor.GetType(), "_target").GetValue(primEditor) as RelayRef<IField>).Target = this_target.Target;
				(AccessTools.Field(primEditor.GetType(), "_textDrive").GetValue(primEditor) as FieldDrive<string>).Target = text.Content;
				textField.Enabled = false;
				text.Enabled = false;
				ui.NestInto(button.Slot);
				RectTransform rect;
				RectTransform rect2;
				ui.SplitHorizontally(0.5f, out rect, out rect2, 0f);
				ui = new UIBuilder(rect);
				colorDriveNoAlpha.ForceLink(ui.Image().Tint);
				ui = new UIBuilder(rect2);
				var img = ui.Image(NeosAssets.Common.Backgrounds.TransparentLight64, null);
				img.PreserveAspect.Value = false;
				ui.Nest();
				colorDrive.ForceLink(ui.Image().Tint);

				return false;
			}
		}
	}
}