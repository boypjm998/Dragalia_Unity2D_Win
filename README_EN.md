# DL_2D_Win (Not Realeased) (2024/4/1）

[中文版](./README.md)

## v0.5.1 - v0.5.5

**New Contents**
1. New adventurers: Sazanka, Gala Laxi
2. New story quests: Bloodstained Auspecalia, Exorcism En Masse
3. New quests: Asura's Blinding Light (Master), Iblis's Surging Cascade (Master), Primal Brunhilda's Trial (Standard, Expert, Master)
4. New achievement: Archdemon Slayer

**Adjustments**
1. Reduced the damage dealt by Louise's skill 1 to targets afflicted with Poison affliction.
2. In quest Origa's Trial: Legend, the duration of the "Demon's Seal" effect from the boss's ability "Satan's Yoke+" has been extended to 240s (the duration remains unchanged in Legend+ difficulty).
3. Slightly increased the maximum HP of the boss in Origa's Trial: Legend, while slightly reduced its attack power.
4. Slightly increased the damage multiplier of Althe's skill 1 against targets afflicted with Paralysis affliction.
5. Slightly reduced the damage multiplier of Louise's skill 1 against targets afflicted with Poison affliction.
6. Adjusted the VFX of the Poison affliction.
7. Adjusted the calculation method of adventurer abilities, changed multiple lists to dictionaries. (No changes to in-game effects.)
8. Greatly reduced the HP of enemies in the quest "Demon's Footprints".
9. In quest Zena's Trial: Legend, the cast speed of "Glorious Sanctuary" has been increased.
10. Slightly increased the damage multipliers of Gala Cleo's skill 1 and skill 3.
11. Reduced the boss's maximum HP and OD gauge limit in quest Sheila's Trial: Legend.
12. Slightly reduced the damage dealt by Halloween Elisanne's skill "Mischief Maker" to enemies afflicted with Paralysis affliction.
13. In quest Zena's Trial: Legend +, when the boss uses "Glorious Sanctuary", it will now also gain a movement speed increasing buff.
14. Adjusted Ilia's ability "Enigma": The charge speed is now increased by 200% when the combo count is above 20, changed from 30.
15. Increased the damage multiplier and maximum HP recovery of Heinwald's skill "Call of Chaos", extended the duration of the "Abyssal Connection" buff to 30 seconds, and also increased the bonus damage of his skill "Void Resonance" that based on missing HP.
16. Adjusted the map scene of the quest "Asura's Blinding Light".
17. Reduced the damage dealt by Fjorm's skills "Frigid Smash" and "Ice Mirror" to enemies afflicted with Frostbite affliction.
18. Increased the cooldown of Fjorm's ability "Frostbite = User Strength & Critical Rate" (5s -> 8s).
19. Adjusted Fjorm's ability "Last Bravery". The original effects "+40% Strength during quest" and "+30% Defense during quest" have been changed to: "+20% Defense during quest", "+20% Strength during quest", "+40% Defense for 30 seconds", "+20% Strength for 30 seconds". In addition, slightly reduced the damage reduction effect of this adventurer when their HP is low.
20. Adjusted the damage reduction effect of Cleo's ability "Supreme Sorcerer": it now reduces damage by 20% when HP is above 30%. Meanwhile, the defense reduction effect of the reflect damage has been increased from 5% to 10%.
21. Increased the damage multipliers of some enemy attacks in the quest "Primal Midgardsormr's Trial".
22. Adjusted the required quest for upgrading Sheila's skill "Blazing Blitz II": you now need to clear "Primal Brunhilda's Trial (Master)".

**Bug Fixes**
1. Fixed the issue where the boss's skill "Frozen Blizzard" behaved abnormally in quest Origa's Trial: Legend.
2. Fixed the issue where Origa could unconditionally use skills while in dragondrive.
3. Fixed the issue where the quest guide button disappeared in the quest "Surtr's Devouring Flames".
4. Fixed the issue where the boss would skip some actions after a part was broken in the quest "Asura's Blinding Light".
5. Fixed the issue where the OD gauge reduction effect of Alex's skill chain damage was lower than expected.
6. Fixed the issue where the attack reduction effect and damage reduction effect of Cleo's ability "Supreme Sorcerer" failed to work.

---

## v0.4.1 - v0.5.0

**New Contents**
1. New adventurers: Curran, Gala Alex, Louise, Ieyasu, Heinwald, Lathna, Grace, Fjorm, Eirene, Sarisse.
2. New challenge quests: Primal Zodiark's Trial, Fallen Angle of Twilight, Lilith's Encroaching Shadow, Jaldabaoth's Piercing Gale, Surtr's Devouring Flames, Origa's Trial (Legend, Legend +).
3. New story quest: Rally
4. New skill upgrades: Bondforged Zethia (Healing Hand), Alex (Emergency Treatment).
5. Other new contents:
(1) Added new nodes to the Ability Tree, including new nodes for the weapon type "Axe".
(2) Added some new achievements.
(3) Added countdown warning in quests.
(4) Added Quest Auto Clear (When you fully clear a high-difficulty quest, all lower-difficulty quests of this boss will be automatically considered as full-cleared).
(5) Added camera zoom function in quests.
(6) Added adventurer tutorials in the training mode.

**Adjustments**
1. Reduced the boss's Poison resistance in Zena's Trial. (Expert, Master: 100->0) (Legend and above: 200->99)
2. Reduced the boss's Poison resistance in Sinister of Domination: Water Chapter. (100->80)
3. Reduced the healing amount of the boss's skill "Healing Hand" in Zethia's Trial: Legend +.
4. Slightly increased the difficulty of the quest Zethia's Trial: Legend +.
5. Adjusted the balance of some adventurers, and adjusted the required quests for skill upgrades.
6. Reduced the ability values of the adventurer Althe.
7. Greatly increased the boss stats for Legend+ difficulty.
8. Increased the cast speed of Curran's skill 2.
9. Modified the 3-star clear conditions for some quests.
10. Added UI prompt for skill upgrade after clearing a quest.
11. Reset the visual effect of Bondforged Zethia's skill "Ring of Affection".
12. Reduced the stats and difficulty of the quest Zethia's Trial: Master.
13. Optimized UI scaling.
14. Added new beginner guidance.
15. You will now get an extra Crown after clearing the prologue.
16. Increased the damage multiplier of Cleo's skill "Ancient Aegis" and the reflect damage multiplier of his ability "Supreme Sorcerer".
17. Optimized the settings menu: Conflicting gamepad key bindings will now be marked in red.
18. Reduced the HP of enemies in some quests.
19. Adjusted the visual effect when gaining the "Prayer's Power" buff in the quest Zethia's Trial: Legend.
20. Adjusted some of the boss's actions in the quest Zethia's Trial: Legend +, reduced the difficulty.
21. Adjusted the UI display position of the Life Shield.
22. Adjusted the dash distance of the force strike for the Lance weapon.
23. You can now use keyboard or gamepad to switch key bindings in the main menu, but the adaptation is still not perfect, there are some issues with button navigation, and it can't adapt to scroll bars yet.
24. Increased the hit range of Notte's normal attacks while in Metamorphosis.
25. Greatly increased the HP of "Weak Point" in the quest Fallen Angle of Conflict.
26. Adjusted the attack buff provided by Cleo's "Altered Strike", it will no longer be affected by "Nihility".
27. Increased the OD gauge reduction multiplier of Alex's skills. (0.7->0.9)
28. Adjusted the movement logic of Ezelith's skill "Howling Meteor", it will no longer fall off the platform when moving at the edge of the platform.
29. Optimized the attack feel of Notte: when using her skill 2 in Metamorphosis form, you can control the attack direction with direction keys.
30. Increased the HP of the first phase boss in the quest Lilith's Encroaching Shadow.
31. Increased the OD gauge reduction multiplier of Alex's skills. (0.9->0.95)
32. Increased the trigger window time of Alex's skill chain from 3s to 4s.
33. Adjusted the quest "Demon's Footprints": the NPC Gabriel's normal attacks now also dispel enemy buffs.
34. Slightly reduced the boss's HP in Jaldabaoth's Piercing Gale (Master).

**Bug Fixes**
1. Fixed the issue where the force strike of the Sword weapon dealt higher damage to the OD gauge than expected.
2. Fixed the issue where the boss stopped all actions after using "Healing Hand" in the quest Zethia's Trial: Legend +.
3. Fixed the issue where Energy and Inspiration effects failed to work when the player was afflicted with Creeping Corrosion affliction.
4. Fixed the issue where gravity stopped working after dashing attack in the air and rolling.
5. Fixed the issue where Cleo's Altered Strike buff area fell out of the map in the prologue quest.
6. Fixed the issue where some of the boss's actions could be interrupted in the quest Zethia's Trial: Legend.
7. Fixed the issue where the Drastic Force effect failed to work when "Weak Point" took damage in the quest Fallen Angle of Conflict.
8. Fixed the issue where Cleo's skill "Ancient Aegis" could hit the same target multiple times.
9. Fixed the issue where the facial animations of some story characters behaved abnormally.
10. Fixed the abnormal gamepad key binding settings.
11. Fixed the issue where the displayed quest details did not match the actual quest content in some quests.
12. Fixed the issue where the boss ability menu was blocked.
13. Fixed the issue where the damage taken increase effect worked incorrectly when the "Rule of Creation: Mana Amplification" effect was active in the quest Zena's Trial: Legend.
14. Fixed the issue where the duration of the "Uriel's Wrath" debuff failed to refresh when enemies gained it repeatedly.
15. Fixed the issue where the gamepad UI mapping was abnormal.
16. Fixed the issue where the boss HP bar animation got stuck when the boss recovered HP while being attacked.
17. Fixed the issue where the boss had abnormal actions in the quest Zena's Trial: Legend.
18. Fixed the issue where the healing area left by Zena's "Glorious Sanctuary" could not remove the Creeping Corrosion debuff when healing the player's own HP.
19. Fixed the issue where the orientation of some quest introductions was wrong.
20. Fixed the issue where some attacks could not hit newly spawned enemies.
21. Fixed the issue where some enemies had an extremely low chance to be counterattacked by normal attacks.
22. Fixed the issue where some bosses would skip some actions when taking damage.
23. Fixed the issue where the boss's skill "Frozen Blizzard" behaved abnormally in quest Origa's Trial: Legend.
24. Fixed the issue where Origa could unconditionally use skills while in Dragondrive mode.


## v0.4.0

**New Contents**

1. New quest: Ilia's Trial: Legend
2. New quest: Zethia's Trial: Legend +
3. Added a dedicated "Up Key". Now Notte(Metamorphosis) can use Up Key to fly upward instead of Jump Key.

**Adjustments**

1. Adventurer's gravity scale will slightly decrease when they use dash attack in the air.
2. Sheila can cancel her warp movement by pressing direction keys when using her 7th standard attack combo.
3. Increases the attack range of Sheila's some standard attacks.
4. When targets are afflicted with Bog, they will take 30% extra damage, instead of 50% now.
5. Slightly increases the stadnard attack rate of adventurers who wield lance.

## v0.3.9

**New Contents**

1. New quest: Primal Jupiter's Trial
2. New quest: Fallen Angle of Conflict

**Adjustments**

1. Optimized Gala Zethia's movement of her 3rd combo.
2. Increases the strength up effect of Bondforged Zethia's 4th skill. (15%->30%)

## v0.3.8

**New Contents**

1. New adventurer: Summer Elisanne
2. New quest: Fallen Angle of Solitude

**Bug Fixes**

1. In Sinister of Domination quests, music may plays incorrectly.
2. In quest Zena's Trial: Legend, adventurers can gain buffs from Glorious Sanctuary even if they're afflicted with "Energy Overloaded" debuff.
3. When adventurers gain buffs from targets other than themselves, their Buff Time +X% abilities still take effect.

---

## v0.3.7

**New Contents**

1. New adventurer: Origa
2. New adventurer: Gala Cleo

**Bug Fixes**

1. Ilia's visual effect animations may lost when using specific attacks.
2. In quest Ilia's Trial, enemies may behave abnormally under certain circumstances.
3. In quest Zena's Trial: Legend, when adventures are defeated while afflicted with "Energy Overloaded", their HP are reset to 1 after reviving.
4. In quest Zethia's Trial: Legend, boss probably stops all actions after afflicted with "Nihility" under certain circumstances.

---

## v0.3.6

**New Contents**

1. New quest: Origa's Trial (Master, Expert)
2. New affliction: Bog

**Bug Fixes**

1. When opening the game for the first time, the achievement was not initialized, resulting in the crash when completing the quest.
2. Notte could not attacked by enemy attacks when they "can forcibly purge shapeshifting" under certain circumstances.
3. When adventurers are hit by attacks that would inflict blindness, their sleep resistacen are mistakenly used to determine its infliction chance.

---

## v0.3.5

**Adjustments**

1. Increases the difficulty in Primal Midgardsormr's Trial and Ilia's Trial

**Bug Fixes**

1. Stormlash & Shadowblight Resistances does not work.
2. Some texts displayed incorrectly in English version.

---

## v0.3.4

**New Contents**

1. Some new achievements.

**Adjustments**

1. Slightly reduced the difficulty of quest Zethia's Trial: Legend.

**Bug Fixes**

1. In quest Zena's Trial: Legend, the stun affliction inflicted by Bondforged Zethia's skill "Resplendent Glare" could not trigger the effect of her ability "Rule of Creation: Causal Bond".

---

## v0.3.3

**New Contents**

1. New skill upgrade: Notte(Emergency Treatment)

**Adjustments**

1. Optimized the visual effects in prologue quest.
2. Added special thank list.

**Bug Fixes**

1. Notte can't gain buff from "Glorious Sanctuary" in quest"Zena's Trial: Legend" when in Metamorphosis.

---

## v0.3.2

**New Contents**

1. New skill upgrade: Zena(Twilight Crown), Sheila(Blazing Blitz).
2. Added DPS statistic in training mode.

---

## v0.3.1

**New Contents**

1. New quest: Zena's Trial: Legend + (Extremely high difficulty)
2. New skill upgrade: Gala Zethia's second skill.
3. Added more nodes to the ability tree.

**Adjustments**

1. Optimize the UI display when using gamepad.

**Bug Fixes**

1. Some bosses behave abnormally when afflicted by Freeze, Stun or Sleep.
2. Camera offset doesn't work when following enemies.

---

## v0.3.0

**New Contents**

1. New quest: Zethia's Trial: Legend
2. New adventurer: Zethia(Bondforged)
3. New feature: Ability Tree. (Boost all adventurers.)
4. Support Gamepad(Game Controller) Input

**Bug Fixes**

1. Sheila will fall from the platforms when using her 4th and 7th combo in some situation.

---

## v0.2.9

**New Contents**

1. New quest: To Claim Happiness
2. New quest: Sheila's Trial: Legend + (Extremely high difficulty)
3. New feature: Skill upgrade. Now Ilia can upgrade her skill3 after clearing specific quest.

**Adjustments**

1. Modified Ezelith's first ability: Reduced Defense Punisher -> Reduced Defense & Break Punisher.

**Bug Fixes**

1. Pinon won't remove her Gabriel's blessing buff on being attacked even if she is not in the zone that created by her 3rd skill.

---

## v0.2.8

**New Contents**

1. New quest: The Demon's False Love (Expert, Standard)
2. New adventurer: Regina

**Adjustments**

1. Modified the action pattern of Zethia(Enemy) in quest "Zethia's Trial: Master".
2. Zethia(Adventurer)'s summon gauge will charge 30% on quest starts.

---

## v0.2.7

**New Contents**

1. Achievement system
2. New adventurer: Elisanne(Halloween)

**Bug Fixes**

1. Fixed the issue that Sheila will fall from the platforms when using skill "Carmine Rush" or "Bright Carmine Rush".
2. Increase enemies' strength and HP in quest "Sheila's Trial: Legend".

---

## v0.2.6

**New Contents**

1. New quest: Zethia's Trial (Expert, Master)
2. New adventurer: Gala Zethia (Blade)

**Adjustments**

1. Modified the algorithm of ability effect calculation to optimize game performance
2. Adjusted the action logic of the adventurer "Sheila", fixed the issue where the adventurer would move to the platform below when performing some attacks at the edge of the platform.

**Bug Fixes**

1. Fixed the issue where the adventurer: Pinon, would automatically release the attack when her force strike was interrupted at more than one fs level.

---

## v0.2.5

**New Contents**

1. New story quest: Demon's Footprints

**Adjustments**

1. Adjusted the calculation method of DOT-type afflictions to optimize game performance.
2. Adjusted the skill effects and damage multipliers of the adventurer "Pinon".

---

## v0.2.4

**New Contents**

1. New adventurer: Pinon
2. New adventurer: Fleur

**Bug Fixes**

1. Notte would use skills during the shapeshifting process, which would consume the SP of the human form skills but could not release the skill.
2. Some text modifications.

---

## v0.2.3

**New Contents**

1. New adventurer: Notte

**Bug Fixes**

1. In Zena's Trial Legend, the BOSS may behave abnormally under certain circumstances.
2. When the player uses Sheila and continuously holds down the dodge and attack key, the actions will interrupt each other indefinitely.
3. In Sheila's Trial Legend, the background turns to black when the player clears the quest.
4. In Sheila's Trial Legend, after the Boss uses the skill "Blazing Azure" and enters the break state, Boss will behave abnormally under certain circumtances.
5. In Sheila's Trial Legend, the color of the small map mark of "Blazing Azure" is displayed incorrectly.

**Adjustments**

1. Modify Ilia's ability 'Critical Output', increase the critical damage to the "Flashburn" foes: 30% => 50%.
2. Modify Zena's ability 'Auspex's Prayer', add an additional effect of increasing the critical rate by 8% when combo counts more than 15.
3. With the strengthening of some adventurers, the HP of some Enemies is increased.
4. Now some enemy attacks will forcibly cancel the player's shapeshifting.
5. In Zena's Trial Legend, when the player gets the gain from 'Glorious Sanctuary', there will be special visual effects to display its buff count.

---

## v0.2.2

**Adjustments**

1. Adjust the difficulty of "Primal Midgardsormr's Trial".
2. In "Primal Midgardsormr's Trial Master", optimize the visual effect for some enemy skills.
3. Optimized the problem that when the attack hits the target, if the target is more than one, the hit SE will be superimposed and played, causing the volume to be too large.
4. The weapon "Manacaster" adds a passive ability to increase the reduction rate of the enemy's OD gauge by 20%.
5. Lowered the damage of the BOSS's skill "Dual Flame Clash" in 'Sheila's Trial Legend'.

**Bug Fixes**

1. Some Buff Log displays abnormally in the English version.
