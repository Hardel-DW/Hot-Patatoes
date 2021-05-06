using Essentials.Options;
using HardelAPI.CustomRoles;
using HardelAPI.Enumerations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PatateChaud {

    [RegisterInCustomRoles(typeof(HotPatatoes))]
    public class HotPatatoes : CustomRole<HotPatatoes> {
        // Color: f5d142ff
        public static CustomOptionHeader HotPatatoesHeader = CustomOptionHeader.AddHeader("<color=#f5d142ff>Hot Patatoes Options :</color>");
        public static CustomNumberOption HotPatatoesPercent = CustomOption.AddNumber("Hot Patatoes Apparition", 0f, 0f, 100f, 5f);
        public static CustomNumberOption HotPatatoesCooldwon = CustomOption.AddNumber("Hot Patatoes Apparition", 30f, 10f, 120f, 5f);
        public static CustomToggleOption DontParent = CustomOption.AddToggle("Dont give the patato to parent", true);

        public HotPatatoes() : base() {
            GameOptionFormat();
            Side = PlayerSide.Everyone;
            LooseRole = true;
            RoleActive = true;
            ForceExiledReveal = true;
            GiveTasksAt = Moment.Never;
            ShowIntroCutScene = false;
            Color = new Color(1f, 1f, 1f, 1f);
            Name = "Patato Holders";
            NumberPlayers = 1;
        }

        public override void OnInfectedStart() {
            PercentApparition = (int) HotPatatoesPercent.GetValue();
        }

        public override void OnUpdate(PlayerControl Player) {
            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId) {
                if (Player.Data.IsImpostor)
                    Color = Palette.ImpostorRed;
                else
                    Color = Palette.White;
            }
        }

        public override void OnGameStarted() {
            PatateChaud.Button.button.MaxTimer = (int) HotPatatoesCooldwon.GetValue();
            PatateChaud.Button.allPlayersTargetable = PlayerControl.AllPlayerControls.ToArray().ToList();
        }

        public override void OnPlayerDisconnect(PlayerControl Player) {
            if (HasRole(Player)) {
                List<PlayerControl> users = PlayerControl.AllPlayerControls.ToArray().ToList().Where(p => !p.Data.IsDead && !p.Data.Disconnected).ToList();
                PlayerControl newUsers = users[new System.Random().Next(users.Count)];
                PatateChaud.Button.allPlayersTargetable = PlayerControl.AllPlayerControls.ToArray().ToList();

                Instance.AllPlayers = new List<PlayerControl>() { newUsers };
                Instance.DefineVisibleByWhitelist();
            }
        }

        private void GameOptionFormat() {
            HotPatatoesHeader.HudStringFormat = (option, name, value) => $"\n{name}";

            HotPatatoesPercent.ValueStringFormat = (option, value) => $"{value}%";
            HotPatatoesCooldwon.ValueStringFormat = (option, value) => $"{value}s";
        }
    }
}
