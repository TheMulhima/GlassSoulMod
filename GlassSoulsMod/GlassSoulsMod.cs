using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassSoulsMod
{
    public class GlassSoulsMod : Modding.Mod
    {
        public override void Initialize()
        {
            Modding.ModHooks.ModLog("Initializing Glass Souls");
            Modding.ModHooks.Instance.CharmUpdateHook += OnCharmUpdate;
            Modding.ModHooks.Instance.AttackHook += OnAttack;
            Modding.ModHooks.Instance.TakeHealthHook += OnHealthTaken;
        }

        public void OnCharmUpdate(PlayerData data, HeroController controller)
        {
            Modding.ModHooks.ModLog("GlassSoulsMod: Charm Update");
            data.galienPinned = true;
            data.galienDefeated = 0;
            data.markothPinned = true;
            data.markothDefeated = 0;
            data.noEyesPinned = true;
            data.noEyesDefeated = 0;
            data.mumCaterpillarPinned = true;
            data.mumCaterpillarDefeated = 0;
            data.huPinned = true;
            data.elderHuDefeated = 0;
            data.xeroPinned = true;
            data.xeroDefeated = 0;
            data.aladarPinned = true;
            data.aladarSlugDefeated = 0;
            data.falseKnightDreamDefeated = false;
            data.infectedKnightDreamDefeated = false;
            data.mageLordDreamDefeated = false;
            data.charmCost_12 = 1;
            data.charmCost_29 = 1;
            data.charmCost_3 = 1;
            data.charmCost_34 = 1;
        }

        public void OnAttack(GlobalEnums.AttackDirection dir)
        {
            Modding.ModHooks.ModLog("GlassSoulsMod: On attack");
            PlayerData playerData = PlayerData.instance;
            playerData.nailDamage = 5 + playerData.nailSmithUpgrades * 4;
            playerData.nailDamage += playerData.health + playerData.healthBlue - 4;
        }

        public int OnHealthTaken(int damage)
        {
            Modding.ModHooks.ModLog("GlassSoulsMod: On health taken");
            PlayerData.instance.health = 0;
            return 0;
        }
    }
}
