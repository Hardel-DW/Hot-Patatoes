using HardelAPI.CustomRoles;
using HardelAPI.Utility;
using HarmonyLib;
using Hazel;
using System.Collections.Generic;
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
                Plugin.LoadSpriteFromEmbeddedResources("RolesMods.Resources.potato.png", 16f),
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
                        PlayerControl target = PlayerControlUtils.GetClosestPlayer(PlayerControl.LocalPlayer, allPlayersTargetable);
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
                }
            }
        }

        private static void RpcSendPatatoes() {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SendPatato, SendOption.Reliable, -1);
            messageWriter.Write(closestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
    }
}