using System;
using System.Collections.Generic;
using System.Linq;
using HKMirror;
using Modding;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding.Menu;
using Modding.Menu.Config;
using UnityEngine;
using UnityEngine.UI;

namespace GlassSoulsMod
{
    public class GlassSoul : Mod, ICustomMenuMod,IGlobalSettings<GlobalSettings>,ITogglableMod
    {
        public static GlobalSettings settings { get; set; } = new ();
        public void OnLoadGlobal(GlobalSettings s) => settings = s;
        public GlobalSettings OnSaveGlobal() => settings;

        public override string GetVersion() => "v1.1.0 - 2";

        private static string[] Cyclone = new[]
        {
            "Hit L", "Hit R",
        };
        
        private static string[] Nart = new[]
        {
            "Great Slash", "Dash Slash", Cyclone[0],Cyclone[1]
        };

        private static string[] Slash = new[]
        {
            "Slash", "DownSlash", "UpSlash"
        };
        

        public override void Initialize()
        {
            Log("Initializing Glass Soul");
            ModHooks.TakeHealthHook += OnHealthTaken;
            ModHooks.HitInstanceHook += IncreaseDamage;
        }

        private HitInstance IncreaseDamage(Fsm owner, HitInstance hit)
        {
            if (!settings._1_ExtraDamage_PerHealth) return hit;

            int increase = PlayerDataAccess.health + PlayerDataAccess.healthBlue - 4;
            int nailDamage = PlayerDataAccess.nailDamage;

            if (Nart.Contains(hit.Source.name))
            {
                float Damage = (float) (nailDamage + increase) * (Cyclone.Contains(hit.Source.name) ? 1.25f : 2.5f);
                
                if (PlayerDataAccess.equippedCharm_6)
                {
                    Damage *= 1.75f;
                }

                hit.DamageDealt = Mathf.RoundToInt(Damage);
            }
            else if (Slash.Contains(hit.Source.name))
            {
                hit.DamageDealt += increase;
            }

            return hit;
        }

        private int OnHealthTaken(int damage)
        {
            Log("GlassSoulsMod: On health taken");
            PlayerDataAccess.health = 0;
            return 0;
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            var dels = toggleDelegates.Value;
            Action<MenuSelectable> cancelAction = _ =>
            {
                UIManager.instance.UIGoToDynamicMenu(modListMenu);
            };
            return new MenuBuilder(UIManager.instance.UICanvas.gameObject, "Glass Souls Mod")
                .CreateTitle("Glass Souls Mod", MenuTitleStyle.vanillaStyle)
                .CreateContentPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 903f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -60f)
                    )
                ))
                .CreateControlPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 259f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -502f)
                    )
                ))
                .SetDefaultNavGraph(new ChainedNavGraph())
                .AddContent(
                    RegularGridLayout.CreateVerticalLayout(105f),
                    c =>
                    {
                        c.AddHorizontalOption(
                            "Mod Enabled",
                            new HorizontalOptionConfig
                            {
                                Label = "Mod Enabled",
                                Options = new string[] {"Off", "On"},
                                ApplySetting = (_, i) => dels.SetModEnabled(i == 1),
                                RefreshSetting = (s, _) => s.optionList.SetOptionTo(dels.GetModEnabled() ? 1 : 0),
                                CancelAction = cancelAction
                            }, out var ModEnabled);
                        ModEnabled.menuSetting.RefreshValueFromGameSettings();
                        c.AddHorizontalOption(
                            "Extra Damage",
                            new HorizontalOptionConfig
                            {
                                Label = "Have 1 extra damage per health",
                                Options = new[] {"True", "False"},
                                ApplySetting = (_, i) => { settings._1_ExtraDamage_PerHealth = i == 0; },
                                RefreshSetting = (s, _) =>
                                    s.optionList.SetOptionTo(settings._1_ExtraDamage_PerHealth ? 0 : 1),
                                CancelAction = cancelAction,
                                Style = HorizontalOptionStyle.VanillaStyle,
                                Description = new DescriptionInfo
                                {
                                    Text = "Enables you to do 1 extra damage per extra health"
                                }
                            }, out var ExtraDamage);
                        ExtraDamage.menuSetting.RefreshValueFromGameSettings();
                        c.AddMenuButton(
                            "DiscordButton",
                            new MenuButtonConfig
                            {
                                Label = "Need More Help? or Have Suggestions?",
                                CancelAction = cancelAction,
                                SubmitAction = _ => Application.OpenURL("https://discord.gg/F6Y5TeFQ8j"),
                                Proceed = true,
                                Style = MenuButtonStyle.VanillaStyle,
                                Description = new DescriptionInfo
                                {
                                    Text = "Join the Hollow Knight Modding Discord."
                                }
                            });
                    }).AddControls(
                    new SingleContentLayout(new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -64f)
                    )), c => c.AddMenuButton(
                        "BackButton",
                        new MenuButtonConfig
                        {
                            Label = "Back",
                            CancelAction = cancelAction,
                            SubmitAction = cancelAction,
                            Style = MenuButtonStyle.VanillaStyle,
                            Proceed = true
                        })).Build();
        }

        public bool ToggleButtonInsideMenu => true;

        public void Unload()
        {
            Log("UnLoading Glass Soul");
            ModHooks.HitInstanceHook -= IncreaseDamage;
            ModHooks.TakeHealthHook -= OnHealthTaken;
        }
    }

    public class GlobalSettings
    {
        public bool _1_ExtraDamage_PerHealth = true;
    }
}
