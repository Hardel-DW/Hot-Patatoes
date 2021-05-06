using BepInEx;
using BepInEx.IL2CPP;
using Essentials.Options;
using HarmonyLib;
using Reactor;
using System;
using System.IO;
using System.Reflection;
using UnhollowerBaseLib;
using UnityEngine;

namespace PatateChaud {
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(HardelAPI.Plugin.Id)]
    public class Plugin : BasePlugin {
        public const string Id = "fr.hardel.hotpatatoes";

        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load() {
            RegisterInIl2CppAttribute.Register();
            Harmony.PatchAll();
            CustomOption.ShamelessPlug = false;
        }

        public static Sprite LoadSpriteFromEmbeddedResources(string resource, float PixelPerUnit) {
            try {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream(resource);
                byte[] image = new byte[myStream.Length];
                myStream.Read(image, 0, (int) myStream.Length);
                Texture2D myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                LoadImage(myTexture, image, true);
                return Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
            } catch { }
            return null;
        }

        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable) {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>) data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }
    }
}
