using HardelAPI.CustomRoles;
using HardelAPI.Utility;
using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PatateChaud {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class Button {
        public static CooldownButton button;
        public static PlayerControl closestPlayer;
        public static List<PlayerControl> allPlayersTargetable = new List<PlayerControl>();

        public static void Postfix(HudManager __instance) {
            button = new CooldownButton
                (() => OnClick(),
                HotPatatoes.HotPatatoesCooldwon.GetValue(),
                Plugin.LoadSpriteFromEmbeddedResources("PatateChaude.Resources.potato.png", 16f),
                250,
                new Vector2(0f, 0f),
                __instance,
                () => OnUpdate(button)
            );
        }

        private static void OnClick() {
            if (allPlayersTargetable == null)
                return;

            RpcSendPatatoes();
        }

        private static void OnUpdate(CooldownButton button) {
            if (HotPatatoes.Instance.AllPlayers != null && PlayerControl.LocalPlayer != null) {
                if (HotPatatoes.Instance.HasRole(PlayerControl.LocalPlayer)) {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                        button.SetCanUse(false);
                    else
                        button.SetCanUse(!MeetingHud.Instance);

                    if (allPlayersTargetable != null) {
                        PlayerControl target = PlayerControlUtils.GetClosestPlayer(PlayerControl.LocalPlayer, allPlayersTargetable, 1f);
                        if (closestPlayer != null) {
                            button.isDisable = false;
                            closestPlayer.myRend.material.SetFloat("_Outline", 0f);
                        } else {
                            button.isDisable = true;
                        }

                        if (target != null) {
                            target.myRend.material.SetFloat("_Outline", 1f);
                            target.myRend.material.SetColor("_OutlineColor", HotPatatoes.Instance.Color);
                            closestPlayer = target;
                        } else {
                            closestPlayer = null;
                        }
                    }
                } else {
                    button.SetCanUse(false);
                }

                if (closestPlayer != null && !HotPatatoes.Instance.HasRole(PlayerControl.LocalPlayer)) {
                    closestPlayer.myRend.material.SetFloat("_Outline", 0f);
                    closestPlayer = null;
                }
            } else {
                button.SetCanUse(false);
            }
        }

        private static void RpcSendPatatoes() {
            if (closestPlayer != null) {
                Button.allPlayersTargetable = PlayerControl.AllPlayerControls.ToArray().ToList();
                if (HotPatatoes.DontParent.GetValue())
                    Button.allPlayersTargetable.RemovePlayer(closestPlayer);

                HotPatatoes.Instance.AllPlayers = new List<PlayerControl>() { closestPlayer };
                HotPatatoes.Instance.DefineVisibleByWhitelist();

                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SendPatato, SendOption.Reliable, -1);
                messageWriter.Write(closestPlayer.PlayerId);
                messageWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            }

        }
    }
}