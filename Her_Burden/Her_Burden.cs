//If you haven't done so yet, run the setup.bat file in your project/libs folder to acquire the needed references.
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace CustomItemPlugin
{
    [R2APISubmoduleDependency(nameof(ResourcesAPI))]
    //[BepInPlugin(GUID, Name, Version)]
    //[BepInDependency(R2API.R2API.PluginGUID)]

    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(
        //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). Java package notation is commonly used, which is "com.[your name here].[your plugin name here]"
        "com.OkIgotIt2018.Her_Burden",
        //The name is the name of the plugin that's displayed on load
        "Her_Burden",
        //The version number just specifies what version the plugin is.
        "1.0.0")]
    //Like seriously, if we see this boilerplate on thunderstore, we will deprecate this mod. Change that name!
    //If you want to test package uploading in general, try using beta.thunderstore.io

    //We will be using 3 modules from R2API: ItemAPI to add our item, ItemDropAPI to have our item drop ingame, and AssetPLus to add our language tokens.
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI))]


    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class CustomItemPlugin : BaseUnityPlugin
    {
        //We need our item definition to persist through our functions, and therefore make it a class field.
        private static ItemDef myItemDef;

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            R2API.LanguageAPI.Add("HERBURDEN_NAME", "Her Burden");
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Her_Burden.Resources.herburden"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@Her_Burden", bundle);
                ResourcesAPI.AddProvider(provider);
            }
            //First let's define our item
            myItemDef = new ItemDef
            {
                name = "HERBURDEN",
                //More on these later
                nameToken = "HERBURDEN_NAME",
                pickupToken = "HERBURDEN_PICKUP",
                descriptionToken = "HERBURDEN_DESC",
                loreToken = "HERBURDEN_LORE",
                //The tier determines what rarity the item is: Tier1=white, Tier2=green, Tier3=red, Lunar=Lunar, Boss=yellow, and finally NoTier is generally used for helper items, like the tonic affliction
                tier = ItemTier.Lunar,
                //You can create your own icons and prefabs through assetbundles, but to keep this boilerplate brief, we'll be using question marks.
                //pickupIconPath = "Textures/MiscIcons/texMysteryIcon",
                pickupIconPath = "@Her_Burden:Assets/Import/herburdenicon/itemIcon.png",
                //pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupModelPath = "@Her_Burden:Assets/Import/herburden/her_burden.blend",
                //Can remove determines if a shrine of order, or a printer can take this item, generally true, except for NoTier items.
                canRemove = true,
                //Hidden means that there will be no pickup notification, and it won't appear in the inventory at the top of the screen. This is useful for certain noTier helper items, such as the DrizzlePlayerHelper.
                hidden = false
            };
            //Now let's turn the tokens we made into actual strings for the game:
            AddTokens();
            AddLocation();

            //You can add your own display rules here, where the first argument passed are the default display rules: the ones used when no specific display rules for a character are found.
            //For this example, we are omitting them, as they are quite a pain to set up.
            //var displayRules = new ItemDisplayRuleDict(null);

            //Then finally add it to R2API
            //ItemAPI.Add(new CustomItem(myItemDef, rules));

            //But now we have defined an item, but it doesn't do anything yet. So we'll need to define that ourselves.
            //~~~~On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            //CharacterBody_OnInventoryChanged(CharacterBody self);
            //On.RoR2.PickupDropletController.CreatePickupDroplet += DropThing;
            On.RoR2.GenericPickupController.GrantItem += (orig, self, body, inventory) =>
            {
                if (body.inventory.GetItemCount(myItemDef.itemIndex) > 0)
                    self.pickupIndex = PickupCatalog.FindPickupIndex(myItemDef.itemIndex);
                orig(self, body, inventory);
            };
            WhoKnows();
        }

        /*private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            Chat.AddMessage("Good1");
            orig(self);
        }*/
        public static void WhoKnows()
        {

            IL.RoR2.CharacterBody.RecalculateStats += (il) => {
                ILCursor c = new ILCursor(il);
                ILLabel dioend = il.DefineLabel(); //end label

                try
                {
                    c.Index = 0;

                    // find the section that the code will be injected					
                    c.GotoNext(
                        MoveType.Before,
                        x => x.MatchLdarg(0), // 1388	0D5A	ldarg.0
                        x => x.MatchLdcI4(0x21), // 1389	0D5B	ldc.i4.s	0x21
                        x => x.MatchCallvirt<CharacterBody>("HasBuff") // 1390	0D5D	call	instance bool RoR2.CharacterBody::HasBuff(valuetype RoR2.BuffIndex)
                    );

                    if (c.Index != 0)
                    {
                        c.Index++;

                        // this block is just "If artifact isn't enabled, jump to dioend label". In an item case, this should be the part where you check if you have the items or not.
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, bool>>((cb) => {
                            int items = cb.master.inventory.GetItemCount(myItemDef.itemIndex);
                            if (items > 0) return true;
                            return false;
                        });
                        c.Emit(OpCodes.Brfalse, dioend);

                        // this.moveSpeed
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_moveSpeed")); // 1406	0D8A	call	instance float32 RoR2.CharacterBody::get_moveSpeed()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.MoveSpeed *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) => {
                            float speedmultiplier = 1;
                            if (cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(myItemDef.itemIndex);
                                if (itemCount > 0)
                                {
                                    speedmultiplier *= Mathf.Pow(0.975f, itemCount);
                                }
                            }
                            return speedmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_moveSpeed", BindingFlags.Instance | BindingFlags.NonPublic));

                        // this.maxHealth
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth")); // 1414 0DA3	call	instance float32 RoR2.CharacterBody::get_maxHealth()

                        // get the inventory count for the item, calculate multiplier, return a float value
                        // This is essentially `this.maxHealth *= multiplier;`
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>((cb) => {
                            float healthmultiplier = 1;
                            if (cb.master.inventory)
                            {
                                int itemCount = cb.master.inventory.GetItemCount(myItemDef.itemIndex);
                                if (itemCount > 0)
                                {
                                    healthmultiplier *= Mathf.Pow(1.05f, itemCount);
                                }
                            }
                            return healthmultiplier;
                        });
                        c.Emit(OpCodes.Mul);
                        c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("set_maxHealth", BindingFlags.Instance | BindingFlags.NonPublic));

                        c.MarkLabel(dioend); // end label
                    }

                }
                catch (Exception ex) { Debug.LogError(ex); }
            };
        }
        //This function adds the tokens from the item to assetplus, the comments in here are a style guide, but is very opiniated. Make your own judgements!
        private void AddTokens()
        {
            //The Name should be self explanatory
            R2API.AssetPlus.Languages.AddToken("HERBURDEN_NAME", "Her Burden");
            //The Pickup is the short text that appears when you first pick this up. This text should be short and to the point, nuimbers are generally ommited.
            R2API.AssetPlus.Languages.AddToken("HERBURDEN_PICKUP", "Increase HP and decrease move speed.\nAll item drops are now:<color=#307FFF>Her Burden</color>");
            //The Description is where you put the actual numbers and give an advanced description.
            R2API.AssetPlus.Languages.AddToken("HERBURDEN_DESC", "Increase HP and decrease move speed.\nAll item drops are now:<color=#307FFF>Her Burden</color> (Plus Numbers)");
            //The Lore is, well, flavor. You can write pretty much whatever you want here.
            R2API.AssetPlus.Languages.AddToken("HERBURDEN_LORE", "None");

        }

        public static void AddLocation()
        {
            GameObject followerPrefab = Resources.Load<GameObject>("@Her_Burden:Assets/Import/herburden/her_burden.blend");
            Vector3 generalScale = new Vector3(.0125f, .0125f, .0125f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.1f, 0.1f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.1f, 0.1f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "LowerArmR",
                    localPos = new Vector3(-2f, 6f, 0f),
                    localAngles = new Vector3(45f, -90f, 0f),
                    localScale = generalScale * 10
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.1f, 0.1f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.1f, 0.1f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.25f, 0.05f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "WeaponPlatform",
                    localPos = new Vector3(0.2f, 0.05f, 0.2f),
                    localAngles = new Vector3(-45f, 0f, 0f),
                    localScale = generalScale * 2
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.2f, 0.2f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Hip",
                    localPos = new Vector3(0f, 3.5f, 0f),
                    localAngles = new Vector3(135f, -0.05f, 0f),
                    localScale = generalScale * 10
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = followerPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0f, 0.1f, 0.1f),
                    localAngles = new Vector3(180f, -0.05f, 0f),
                    localScale = generalScale
                }
            });
            ItemAPI.Add(new CustomItem(myItemDef, rules));
        }
    }
}