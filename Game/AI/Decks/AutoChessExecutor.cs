using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using static WindBot.Game.AI.Decks.TimeThiefExecutor;

namespace WindBot.Game.AI.Decks
{
    [Deck("AutoChess", "AI_Test", "Test")]
    public class AutoChessExecutor : DefaultExecutor
    {
         public AutoChessExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {

            AddExecutor(ExecutorType.SpSummon, SPSummonFunction); 
            AddExecutor(ExecutorType.Activate, PendulumActivateFunction);
            AddExecutor(ExecutorType.Activate, EquipEffectActivateFunction);
            AddExecutor(ExecutorType.Activate, EquipActivateFunction);
            AddExecutor(ExecutorType.Activate, ActivateFunction);

            AddExecutor(ExecutorType.Summon, MonsterSummon);
            AddExecutor(ExecutorType.MonsterSet, MonsterSet);
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, MonsterRepos);

            AddExecutor(ExecutorType.Activate, _CardId.MysticalSpaceTyphoon, DefaultMysticalSpaceTyphoon);
            AddExecutor(ExecutorType.Activate, _CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.GalaxyCyclone, DefaultGalaxyCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.BookOfMoon, DefaultBookOfMoon);
            AddExecutor(ExecutorType.Activate, _CardId.CompulsoryEvacuationDevice, DefaultCompulsoryEvacuationDevice);
            AddExecutor(ExecutorType.Activate, _CardId.CallOfTheHaunted, DefaultCallOfTheHaunted);
            AddExecutor(ExecutorType.Activate, _CardId.Scapegoat, DefaultScapegoat);
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, _CardId.GhostOgreAndSnowRabbit, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, _CardId.GhostBelle, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, _CardId.BreakthroughSkill, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnJudgment, DefaultSolemnJudgment);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnWarning, DefaultSolemnWarning);
            AddExecutor(ExecutorType.Activate, _CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, _CardId.TorrentialTribute, DefaultTorrentialTribute);
            AddExecutor(ExecutorType.Activate, _CardId.HeavyStorm, DefaultHeavyStorm);
            AddExecutor(ExecutorType.Activate, _CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, _CardId.HammerShot, DefaultHammerShot);
            AddExecutor(ExecutorType.Activate, _CardId.DarkHole, DefaultDarkHole);
            AddExecutor(ExecutorType.Activate, _CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, _CardId.SmashingGround, DefaultSmashingGround);
            AddExecutor(ExecutorType.Activate, _CardId.PotOfDesires, DefaultPotOfDesires);
            AddExecutor(ExecutorType.Activate, _CardId.AllureofDarkness, DefaultAllureofDarkness);
            AddExecutor(ExecutorType.Activate, _CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, _CardId.InterruptedKaijuSlumber, DefaultInterruptedKaijuSlumber);

            AddExecutor(ExecutorType.SpSummon, _CardId.JizukirutheStarDestroyingKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.GadarlatheMysteryDustKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.RadiantheMultidimensionalKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.KumongoustheStickyStringKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.ThunderKingtheLightningstrikeKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.DogorantheMadFlameKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.SpSummon, _CardId.SuperAntiKaijuWarMachineMechaDogoran, DefaultKaijuSpsummon);

            AddExecutor(ExecutorType.SpSummon, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
        }

        private List<int> HintMsgForEnemy = new List<int>
        {
            HintMsg.Release, HintMsg.Destroy, HintMsg.Remove, HintMsg.ToGrave, HintMsg.ReturnToHand, HintMsg.ToDeck,
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Disable
        };

        private List<int> HintMsgForDeck = new List<int>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.Remove, HintMsg.AddToHand, HintMsg.FusionMaterial
        };

        private List<int> HintMsgForSelf = new List<int>
        {
            HintMsg.Equip
        };

        private List<int> HintMsgForMaterial = new List<int>
        {
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Release
        };

        private List<int> HintMsgForMaxSelect = new List<int>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.AddToHand, HintMsg.FusionMaterial, HintMsg.Destroy
        };

        private bool p_summoning = false;

        /*
        函数目录    Function Catalogue
            卡片过滤函数
                碰瓷怪检测
                    BlackmailAttackerSunmmon(ClientCard card)
                    BlackmailAttackerSunmmon2(int cardId)
                    BlackmailAttacker(ClientCard card, ClientField player)
                不可通招怪兽检测
                    DontSummon(ClientCard card)
                反转怪兽检测
                    FilpMonster(ClientCard card)
                装备检测
                    EquipForEnemy(ClientCard card)
                卡片选择检测
                    EnemyCardTarget(ClientCard card, bool chkc, CardLocation loc, CardType[] type, CardPosition[] pos)
                得到某个位置的卡片的函数（从神数不神那借来的）
                    GetZoneCards(CardLocation loc, ClientField player)
                系统提示检测
                    HintFunction(int hint, int last, int[] except)
            卡片发动过滤函数
                灵摆刻度设置
                    PendulumActivateFunction()
                装备卡的发动
                    EquipActivateFunction()
                装备卡的效果的发动
                    EquipEffectActivateFunction()
                其他
                    ActivateFunction()
            怪兽相关函数
                改变表示形式
                    MonsterRepos()
                通常召唤
                    MonsterSummon()
                盖放
                    MonsterSet()
                特殊召唤
                    SPSummonFunction()
            OnSelect函数重写
                选择卡片
                    OnSelectCard(IList<ClientCard> _cards, int min, int max, int hint, bool cancelable)
                选择效果的选项
                    OnSelectOption(IList<int> options)
                选择特殊召唤时的表示形式
                    OnSelectPosition(int cardId, IList<CardPosition> positions)
                选择攻击对象
                    OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        */

        private bool BlackmailAttackerSunmmon(ClientCard card)
        {
            int[] cardsname = new[] {34031284, 35494087, 54366836, 94004268, 97403510, 59627393, 93730230, 69058960, 95442074, 24874631};
            foreach(int cardname in cardsname)
            {
                if (card.Id == cardname) return true;
            }

            if ((card.HasSetcode(0x10) && GetZoneCards(CardLocation.MonsterZone, Bot).Any(scard => scard != null && scard.Id == 29552709 && scard.IsFaceup() && !scard.IsDisabled()) && Duel.Player == 0) || card.HasSetcode(0x2a) || card.HasSetcode(0x1a5) || card.HasSetcode(0x18d) || card.HasSetcode(0x4a))
                return true;

            return false;
        }

        private bool BlackmailAttackerSunmmon2(int cardId)
        {
            YGOSharp.OCGWrapper.NamedCard card = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (card == null) return false;
            int[] cardsname = new[] {34031284, 35494087, 54366836, 94004268, 97403510, 59627393, 93730230, 69058960, 95442074, 24874631};
            foreach(int cardname in cardsname)
            {
                if (cardId == cardname) return true;
            }

            if ((card.HasSetcode(0x10) && GetZoneCards(CardLocation.MonsterZone, Bot).Any(scard => scard != null && scard.Id == 29552709 && scard.IsFaceup() && !scard.IsDisabled()) && Duel.Player == 0) || card.HasSetcode(0x2a) || card.HasSetcode(0x1a5) || card.HasSetcode(0x18d) || card.HasSetcode(0x4a))
                return true;

            return false;
        }

        private bool BlackmailAttacker(ClientCard card, ClientField player)
        {
            if (!card.IsDisabled())
            {
                if (card.HasSetcode(0x4a) || card.Id == 34031284 || card.Id == 35494087 || card.Id == 54366836 || card.Id == 94004268 || card.Id == 97403510
                    || (card.HasSetcode(0x18d) && card.EquipCards.Count()>0)
                    || (card.Id == 59627393 && card.Overlays.Count()>0)
                    || (card.Id == 93730230 && card.Overlays.Count()>0)
                    || (card.Id == 69058960 && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 95442074 && scard.IsFaceup()))
                    || (card.Id == 95442074 && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 69058960 && scard.IsFaceup()))
                )
                    return true;
            }

            if (card.IsFaceup())
            {
                if ((card.HasSetcode(0x1a5) && GetZoneCards(CardLocation.SpellZone, player).Any(scard => scard != null && scard.Id == 65261141 && scard.IsFaceup() && !scard.IsDisabled()))
                    || (card.HasSetcode(0x2a) && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 17285476 && scard.IsFaceup() && !scard.IsDisabled()))
                    || (card.Id == 24874631 && GetZoneCards(CardLocation.SpellZone, player).Any(scard => scard != null && scard.Id == 24874630 && scard.IsFaceup() && !scard.IsDisabled()))
                    || (card.HasSetcode(0x10) && GetZoneCards(CardLocation.MonsterZone, player).Any(scard => scard != null && scard.Id == 29552709 && scard.IsFaceup() && !scard.IsDisabled()))
                )
                    return true;
            }

            return false;
        }


        private bool DontSummon(ClientCard card)
        {
            if (card.HasSetcode(0x40) || card.HasSetcode(0xa4) || card.HasSetcode(0xd3)) return true;
            int[] cardsname = new[] {74762582, 90179822, 16759958, 26964762, 42352091, 2511, 74018812, 76214441, 62886670, 69105797, 32391566, 94076521, 73625877, 1980574, 42090294, 68823957, 34976176, 89785779, 76133574, 3248469, 87102774
            , 57647597, 37961969, 51993760, 87988305, 38339996, 37629703, 58131925, 71133680, 42790071, 34475451, 63009228, 24725825, 48427163, 86028783, 51852507, 29280589, 87462901, 73640163, 68120130, 84813516, 55461064, 59042331, 26775203, 89169343
            , 67750322, 68819554, 26084285, 15613529, 19096726, 59546797, 12235475, 38695361, 37742478, 26914168, 43534808, 13313278, 99581584, 04192696, 89662736, 81109178, 18444902, 04807253, 12423762, 72318602, 86613346, 82489470, 16223761, 08152834/*时尚小垃圾*/
            , 97268402/*效果遮蒙者*/, 24508238/*D.D.乌鸦*/, 94145021/*锁鸟*/
            , 14558127, 14558128, 52038441, 52038442, 59438930, 59438931, 60643553, 60643554, 62015408, 62015409, 73642296, 73642297/*手坑六姐妹*/
            , 15721123, 23434538, 25137581, 46502744, 80978111, 87170768, 94081496/*xx的G*/
            , 17266660, 21074344, 94689635/*byd原来宣告者没字段啊*/
            , 18964575, 20450925, 19665973, 28427869, 27352108/*攻宣坑*/
            };
            foreach(int cardname in cardsname)
            {
                if (card.Id == cardname) return true;
            }

            return false;
        }

        private bool FilpMonster(ClientCard card)
        {
            if (card.HasType(CardType.Flip)) return true;
            int[] cardsname = new[] {20073910, 89707961, 41753322, 86346363, 75421661, 87483942, 40659562, 41039846, 72370114, 92693205, 22134079, 16509093, 96352326, 923596, 47111934, 81306586, 26016357, 52323207, 64804316, 75209824, 71071546, 92736188, 16279989, 97584500, 72913666, 71415349, 51196805, 85463083, 41872150, 75109441, 3510565, 15383415, 2326738, 80885284, 84472026, 93294869, 27491571, 54490275, 36239585, 2694423, 81278754, 24101897, 46925518, 99641328, 61318483, 54512827, 81907872, 98707192
            };
            foreach(int cardname in cardsname)
            {
                if (card.Id == cardname) return true;
            }

            return false;
        }

        private bool EquipForEnemy(ClientCard card)
        {
            int[] cardsname = new[] {33453260, 79912449, 32919136, 45986603, 45247637, 44092304, 46967601, 94303232, 56948373, 69954399, 83584898, 62472614, 75560629, 23842445, 24668830, 98867329, 50152549, 62472614
            };
            foreach(int cardname in cardsname)
            {
                if (card.Id == cardname) return true;
            }

            return false;
        }

        private bool EnemyCardTarget(ClientCard card, bool chkc, CardLocation loc, CardType[] type, CardPosition[] pos)
        {
            int a = 0;
            int b = 0;
            int[] cardsname;
            foreach (CardType ty in type)
            {
                if (ty == CardType.Spell)
                    a++;
                if (ty == CardType.Trap)
                    a = a + 2;
                if (ty == CardType.Spell || ty == CardType.Trap || ty == CardType.Monster)
                    a = a + 3;
            }
            foreach (CardPosition po in pos)
            {
                if (po == CardPosition.FaceUp)
                    b++;
                if (po == CardPosition.FaceDown)
                    b = b + 2;
            }
            if ((loc & CardLocation.MonsterZone) > 0 && (loc & CardLocation.SpellZone) > 0)
            {
                if (chkc)
                {
                    if (a == 1 && b == 1)
                    {
                        if (card.Id == 76137614)
                            return true;
                    }
                    else if (a == 2 && b == 1)
                    {
                        if (card.Id == 5640330 && ActivateDescription == Util.GetStringId(5640330, 1))
                            return true;
                    }
                    else if (a == 3 && b == 1)
                    {
                        cardsname = new[] {10071151, 14883228, 43785278, 44852429, 69452756, 76137614, 80019195, 80275707, 84565800, 85800949
                        };
                        foreach(int cardname in cardsname)
                        {
                            if (card.Id == cardname) return true;
                        }
                        if ((card.Id == 5133471 && ActivateDescription == Util.GetStringId(5133471, 1)) || (card.Id == 17241941 && ActivateDescription == Util.GetStringId(17241941, 2)))
                            return true;
                    }
                    else if (a == 3 && b == 2)
                    {
                        cardsname = new[] {25955749, 18489208, 20351153, 40736921, 61831093, 76515293
                        };
                        foreach(int cardname in cardsname)
                        {
                            if (card.Id == cardname) return true;
                        }
                        if ((card.Id == 5133471 && ActivateDescription == Util.GetStringId(5133471, 0))
                        || (card.Id == 64398890 && ActivateDescription == Util.GetStringId(64398890, 0))
                        || (card.Id == 73213494 && ActivateDescription == Util.GetStringId(73213494, 1))
                        )
                            return true;
                    }
                    else if (a == 3 && b == 3)
                    {
                        cardsname = new[] {5318639, 51232472, 6983839, 8267140, 22923081, 29223325, 71413901, 76471944, 89172051, 43898403
                        };
                        foreach(int cardname in cardsname)
                        {
                            if (card.Id == cardname) return true;
                        }
                        if ((card.Id == 41470137 && ActivateDescription == Util.GetStringId(41470137, 0))
                        || (card.Id == 98558751 && ActivateDescription == Util.GetStringId(98558751, 0))
                        || (card.Id == 49456901 && ActivateDescription == Util.GetStringId(49456901, 0))
                        || (card.Id == 53618197 && ActivateDescription == Util.GetStringId(53618197, 0))
                        || (card.Id == 71100270 && ActivateDescription == Util.GetStringId(71100270, 0))
                        || (card.Id == 85252081 && ActivateDescription == Util.GetStringId(85252081, 0))
                        || (card.Id == 89423971 && ActivateDescription == Util.GetStringId(89423971, 0))
                        || (card.Id == 19025379 && ActivateDescription == Util.GetStringId(19025379, 1))
                        || (card.Id == 64182380 && ActivateDescription == Util.GetStringId(64182380, 1))
                        || (card.Id == 75425320 && ActivateDescription == Util.GetStringId(75425320, 2))
                        || (card.Id == 10117149 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 15710054 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 37991342 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 94454495 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 35035481 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 75782277 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 51531505 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 97692972 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 18444733 && card.Location == CardLocation.SpellZone && card.IsFaceup())
                        || (card.Id == 49430782 && card.Location == CardLocation.Grave)
                        || (card.Id == 77723643 && card.Location == CardLocation.Grave)
                        || (card.Id == 83656563 && card.Location == CardLocation.Grave)
                        || (card.Id == 60362066 && card.Location != CardLocation.Grave)
                        )
                            return true;
                    }
                    else if (a == 6 && b == 1)
                    {
                        cardsname = new[] {744887, 20403123, 20745268, 23204029, 38694052, 38904695, 43892408, 48905153, 89883517, 93379652, 
                        };
                        foreach(int cardname in cardsname)
                        {
                            if (card.Id == cardname) return true;
                        }
                        if ((card.Id == 5795980 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 9940036 && ActivateDescription == Util.GetStringId(9940036, 2))
                        || (card.Id == 13364097 && ActivateDescription == Util.GetStringId(13364097, 0))
                        || (card.Id == 50078320 && ActivateDescription == Util.GetStringId(50078320, 0))
                        || (card.Id == 50907446 && ActivateDescription == Util.GetStringId(50907446, 0))
                        || (card.Id == 65398390 && ActivateDescription == Util.GetStringId(65398390, 0))
                        || (card.Id == 29601381 && ActivateDescription == Util.GetStringId(29601381, 1))
                        || (card.Id == 48461764 && ActivateDescription == Util.GetStringId(48461764, 1))
                        || (card.Id == 94073244 && ActivateDescription == Util.GetStringId(94073244, 1))
                        || (card.Id == 13317419 && card.Location == CardLocation.SpellZone && card.IsFaceup())
                        || (card.Id == 37491810 && card.Location == CardLocation.SpellZone && card.IsFaceup())
                        || (card.Id == 38761908 && card.Location == CardLocation.SpellZone)
                        )
                            return true;
                    }
                    else if (a == 6 && b == 2)
                    {
                        cardsname = new[] {60229110, 93554166, 99940363, 15545291, 58475908
                        };
                        foreach(int cardname in cardsname)
                        {
                            if (card.Id == cardname) return true;
                        }
                        if ((card.Id == 20281581 && ActivateDescription == Util.GetStringId(20281581, 1))
                        || (card.Id == 31467949 && ActivateDescription == Util.GetStringId(31467949, 1))
                        || (card.Id == 32912040 && ActivateDescription == Util.GetStringId(32912040, 0))
                        )
                            return true;
                    }
                    else if (a == 6 && b == 3)
                    {
                        cardsname = new[] {744887, 4178474, 5168381, 9765723, 13298352, 18318842, 19943114, 22850702, 28016193, 28112535, 28423537, 28711704, 29477860, 29479265, 32785578, 33846209, 33964637
                        , 35252119, 35330871, 38265153, 38342335, 43215738, 43912676, 44553392, 47264717, 48626373, 55794644, 56804361, 65192027, 65892310, 68182934, 69764158, 71279983, 72959823, 73964868
                        , 76552147, 77449773, 83102080, 84125619, 84453939, 89211486, 97317530, 14154221, 17494901, 31677606, 66789970, 72044448, 78156759, 84290642, 85893201 
                        };
                        foreach(int cardname in cardsname)
                        {
                            if (card.Id == cardname) return true;
                        }
                        if (card.Id == 1561110 && ActivateDescription == Util.GetStringId(1561110, 0)
                        || (card.Id == 4779823 && ActivateDescription == Util.GetStringId(4779823, 0))
                        || (card.Id == 23626223 && ActivateDescription == Util.GetStringId(23626223, 0))
                        || (card.Id == 26692769 && ActivateDescription == Util.GetStringId(26692769, 0))
                        || (card.Id == 32617464 && ActivateDescription == Util.GetStringId(32617464, 0))
                        || (card.Id == 32939238 && ActivateDescription == Util.GetStringId(32939238, 0))
                        || (card.Id == 36609518 && ActivateDescription == Util.GetStringId(36609518, 0))
                        || (card.Id == 38267552 && ActivateDescription == Util.GetStringId(38267552, 0))
                        || (card.Id == 50056656 && ActivateDescription == Util.GetStringId(50056656, 0))
                        || (card.Id == 79194594 && ActivateDescription == Util.GetStringId(79194594, 0))
                        || (card.Id == 84815190 && ActivateDescription == Util.GetStringId(84815190, 0))
                        || (card.Id == 90835938 && ActivateDescription == Util.GetStringId(90835938, 0))
                        || (card.Id == 98462037 && ActivateDescription == Util.GetStringId(98462037, 0))
                        || (card.Id == 43227 && ActivateDescription == Util.GetStringId(43227, 1))
                        || (card.Id == 2530830 && ActivateDescription == Util.GetStringId(2530830, 1))
                        || (card.Id == 6764709 && ActivateDescription == Util.GetStringId(6764709, 1))
                        || (card.Id == 12444060 && ActivateDescription == Util.GetStringId(12444060, 1))
                        || (card.Id == 10406322 && ActivateDescription == Util.GetStringId(10406322, 1))
                        || (card.Id == 13482262 && ActivateDescription == Util.GetStringId(13482262, 1))
                        || (card.Id == 20563387 && ActivateDescription == Util.GetStringId(20563387, 1))
                        || (card.Id == 21113684 && ActivateDescription == Util.GetStringId(21113684, 1))
                        || (card.Id == 22110647 && ActivateDescription == Util.GetStringId(22110647, 1))
                        || (card.Id == 22908820 && ActivateDescription == Util.GetStringId(22908820, 1))
                        || (card.Id == 28373620 && ActivateDescription == Util.GetStringId(28373620, 1))
                        || (card.Id == 34481518 && ActivateDescription == Util.GetStringId(34481518, 1))
                        || (card.Id == 40732515 && ActivateDescription == Util.GetStringId(40732515, 1))
                        || (card.Id == 46294982 && ActivateDescription == Util.GetStringId(46294982, 1))
                        || (card.Id == 61307542 && ActivateDescription == Util.GetStringId(61307542, 1))
                        || (card.Id == 63101468 && ActivateDescription == Util.GetStringId(63101468, 1))
                        || (card.Id == 73667937 && ActivateDescription == Util.GetStringId(73667937, 1))
                        || (card.Id == 81055000 && ActivateDescription == Util.GetStringId(81055000, 1))
                        || (card.Id == 83533296 && ActivateDescription == Util.GetStringId(83533296, 1))
                        || (card.Id == 87188910 && ActivateDescription == Util.GetStringId(87188910, 1))
                        || (card.Id == 90579153 && ActivateDescription == Util.GetStringId(90579153, 1))
                        || (card.Id == 95207988 && ActivateDescription == Util.GetStringId(95207988, 1))
                        || (card.Id == 5973663 && ActivateDescription == Util.GetStringId(5973663, 1))

                        || (card.Id == 11132674 && ActivateDescription == Util.GetStringId(11132674, 2))
                        || (card.Id == 28798938 && ActivateDescription == Util.GetStringId(28798938, 2))
                        || (card.Id == 30989084 && ActivateDescription == Util.GetStringId(30989084, 2))
                        || (card.Id == 37495766 && ActivateDescription == Util.GetStringId(37495766, 2))
                        || (card.Id == 91336701 && ActivateDescription == Util.GetStringId(91336701, 2))
                        || (card.Id == 73734821 && ActivateDescription == Util.GetStringId(73734821, 3))
                        || (card.Id == 13073850 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 15130912 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 35187185 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 47021196 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 47963370 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 56638325 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 66698383 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 74122412 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 86585274 && card.Location == CardLocation.MonsterZone)
                        || (card.Id == 36148308 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 54807656 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 57736667 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 60176682 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 69207766 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 71817640 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 71832012 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 77103950 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 87091930 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 88667504 && card.Location == CardLocation.SpellZone)
                        || (card.Id == 35834119 && card.Location == CardLocation.Grave)
                        || (card.Id == 44536921 && card.Location == CardLocation.Grave)
                        || (card.Id == 80532587 && card.Location == CardLocation.Grave)
                        || (card.Id == 95440946 && card.Location == CardLocation.Hand)
                        || (card.Id == 53804307 && card.Location == CardLocation.Hand)
                        || (card.Id == 28865322 && card.Location != CardLocation.Hand)
                        )
                            return true;
                    }
                }

            }

            return false;
        }

        private List<ClientCard> GetZoneCards(CardLocation loc, ClientField player)
        {
            List<ClientCard> res = new List<ClientCard>();
            List<ClientCard> temp = new List<ClientCard>();
            if ((loc & CardLocation.Hand) > 0) { temp = player.Hand.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.MonsterZone) > 0) { temp = player.GetMonsters(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.SpellZone) > 0) { temp = player.GetSpells(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.Grave) > 0) { temp = player.Graveyard.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.Removed) > 0) { temp = player.Banished.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            if ((loc & CardLocation.Extra) > 0) { temp = player.ExtraDeck.Where(card => card != null).ToList(); if (temp.Count() > 0) res.AddRange(temp); }
            return res;
        }

        private bool HintFunction(int hint, int last, int[] except)
        {
            for (int i = 500; i <= 500 + last; i++)
            {
                foreach (int ex in except)
                {
                    if (i == ex)
                        i++;
                }
                if (hint == i)
                    return true;
            }
            return false;
        }

        private bool PendulumActivateFunction()
        {
            if (Card.HasType(CardType.Pendulum) && Card.Location == CardLocation.Hand && ActivateDescription == 1160)
            {
                if (Card.Location != CardLocation.Hand || Bot.HasInSpellZone(Card.Id))
                    return false;

                ClientCard l = Util.GetPZone(0, 0);
                ClientCard r = Util.GetPZone(0, 1);
                if (l == null && r == null)
                    return true;
                if (l == null && r.RScale != Card.LScale)
                {
                    if (r.RScale > Card.LScale)
                        return r.RScale != Card.LScale + 1;
                    else
                        return r.RScale != Card.LScale - 1;
                }
                if (r == null && l.LScale != Card.RScale)
                {
                    if (l.LScale > Card.RScale)
                        return l.LScale != Card.RScale + 1;
                    else
                        return l.LScale != Card.RScale - 1;
                }
                return false;
            }
            return false;
        }
        private bool EquipActivateFunction()
        {
            if (Card.HasType(CardType.Equip))
            {
                List<ClientCard> cards = new List<ClientCard>();
                if (Card.Location == CardLocation.Hand)
                {
                    if (Card.Id == 43527730)
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget()).ToList();
                        if (cards.Count() == 0)
                            cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget()).ToList();
                        else
                        {
                            cards.Sort(CardContainer.CompareCardAttack);
                            cards.Reverse();
                        }

                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    else if (Card.Id == 41927278)
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget()).ToList();
                        if (cards.Count() == 0)
                            cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget() && card.HasSetcode(0x18d)).ToList();
                        else
                        {
                            cards.Sort(CardContainer.CompareCardAttack);
                            cards.Reverse();
                        }

                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    else if (EquipForEnemy(Card))
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget()).ToList();
                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    cards = GetZoneCards(CardLocation.MonsterZone, Bot).Where(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget()).ToList();
                    if (cards.Count() > 0)
                    {
                        AI.SelectCard(cards);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
        private bool EquipEffectActivateFunction()
        {
            if (Card.HasType(CardType.Equip))
            {
                List<ClientCard> cards = new List<ClientCard>();
                if(Card.Location == CardLocation.SpellZone)
                {
                    if (Card.Id == 43527730)
                    {
                        return !Card.EquipTarget.HasSetcode(0x18d);
                    }
                    if (Card.Id == 32939238)
                    {
                        cards = GetZoneCards(CardLocation.Onfield, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
                        return cards.Count() > 0;
                    }
                    if (Card.Id == 57736667 || Card.Id == 36148308)
                    {
                        cards = GetZoneCards(CardLocation.Onfield, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
                        return cards.Count() > 0;
                    }
                    if (Card.Id == 99013397 || Card.Id == 70423794 || Card.Id == 22147147)
                    {
                        cards = GetZoneCards(CardLocation.Onfield, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget() && (card.HasType(CardType.Spell) || card.HasType(CardType.Trap))).ToList();
                        return cards.Count() > 0;
                    }
                    return DefaultDontChainMyself();
                }
                else if (Card.Location == CardLocation.Grave || Card.Location == CardLocation.Removed)
                {
                    if (Card.Id == 64867422)
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget() && card.IsFaceup()).ToList();
                        return cards.Count() > 0;
                    }
                    if (Card.Id == 66947913)
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
                        return cards.Count() > 0;
                    }
                    return DefaultDontChainMyself();
                }
                return false;
            }
            return false;
        }

        private bool ActivateFunction()
        {
            IList<ClientCard> cards = new List<ClientCard>();

            if (Card.HasType(CardType.Equip) || (Card.HasType(CardType.Pendulum) && Card.Location == CardLocation.Hand && ActivateDescription == 1160))
                return false;
            if (EnemyCardTarget(Card, true, CardLocation.Onfield, new[] { CardType.Spell, CardType.Trap }, new[] { CardPosition.FaceUp }))
                return GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && (card.HasType(CardType.Spell) || card.HasType(CardType.Trap)) && card.IsFaceup() && !card.IsShouldNotBeTarget());
            if (EnemyCardTarget(Card, true, CardLocation.Onfield, new[] { CardType.Spell, CardType.Trap }, new[] { CardPosition.FaceDown }))
                return GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && (card.HasType(CardType.Spell) || card.HasType(CardType.Trap)) && card.IsFacedown() && !card.IsShouldNotBeTarget());
            if (EnemyCardTarget(Card, true, CardLocation.Onfield, new[] { CardType.Spell, CardType.Trap }, new[] { CardPosition.FaceUp, CardPosition.FaceDown }))
                return GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && (card.HasType(CardType.Spell) || card.HasType(CardType.Trap)) && !card.IsShouldNotBeTarget());
            if (EnemyCardTarget(Card, true, CardLocation.Onfield, new[] { CardType.Spell, CardType.Trap, CardType.Monster }, new[] { CardPosition.FaceUp }))
                return GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && card.IsFaceup() && !card.IsShouldNotBeTarget());
            if (EnemyCardTarget(Card, true, CardLocation.Onfield, new[] { CardType.Spell, CardType.Trap, CardType.Monster }, new[] { CardPosition.FaceDown }))
                return GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && card.IsFacedown() && !card.IsShouldNotBeTarget());
            if (EnemyCardTarget(Card, true, CardLocation.Onfield, new[] { CardType.Spell, CardType.Trap, CardType.Monster }, new[] { CardPosition.FaceUp, CardPosition.FaceDown }))
                return GetZoneCards(CardLocation.Onfield, Enemy).Any(card => card != null && !card.IsShouldNotBeTarget());

            //优化单卡是怎么想的啊喂(#`O′)
            if (Card.Id == 60461804)
            {
                if (Card.Location == CardLocation.Grave)
                {
                    return true;
                }

                ClientCard target = Util.GetProblematicEnemyCard(2500);
                if (target != null && !Util.ChainContainPlayer(0))
                {
                    AI.SelectCard(Card);
                    AI.SelectNextCard(target);
                    return true;
                }

                return false;
            }
            else if (Card.Id == 84815190)
            {
                if (ActivateDescription == Util.GetStringId(84815190, 0))
                {
                    cards = GetZoneCards(CardLocation.Onfield, Enemy);
                    cards = cards.Where(tcard => tcard != null && !tcard.IsShouldNotBeTarget()).ToList();
                    if (cards.Count <= 0) return false;
                    //AI.SelectCard(cards);
                    return true;
                }
                else if (ActivateDescription == Util.GetStringId(84815190, 1))
                {
                    return Duel.LastChainPlayer == 1;
                }
                return false;
            }
            return DefaultDontChainMyself();
        }
        private bool MonsterRepos()
        {
            if (Duel.Phase == DuelPhase.Main1 && (FilpMonster(Card)) && Card.IsFacedown())
            {
                return true;
            }
            else if (Duel.Phase == DuelPhase.Main2)
            {
                if (BlackmailAttacker(Card, Bot))
                {
                    if (Card.IsFaceup() && Card.IsAttack())
                        return false;
                    if (Card.IsFaceup() && Card.IsDefense())
                        return true;
                }
                else if (Card.Defense > Card.Attack)
                    return DefaultMonsterRepos();
                else
                {
                    if (Card.IsFaceup() && Card.IsAttack())
                        return Bot.LifePoints <= 1500 && GetZoneCards(CardLocation.MonsterZone, Enemy).Count(card => card != null && card.Attack < Card.Attack) == 0 && GetZoneCards(CardLocation.MonsterZone, Enemy).Count() > 0 && Card.Attack < 1500;
                    else
                        return Bot.LifePoints > 1500 ||  GetZoneCards(CardLocation.MonsterZone, Enemy).Count(card => card != null && card.Attack < Card.Attack) > 0 || GetZoneCards(CardLocation.MonsterZone, Enemy).Count() == 0 || Card.Attack >= 1500;
                }
                return false;
            }

            return false;
        }

        private bool MonsterSummon()
        {
            if (DontSummon(Card))
                return false;

            if (BlackmailAttackerSunmmon(Card))
                return DefaultMonsterSummon();
            else if (FilpMonster(Card))
                return false;
            else if (Card.Level > 4 || Bot.LifePoints > 1500)
                return DefaultMonsterSummon();
            else if (Bot.LifePoints <= 1500 && GetZoneCards(CardLocation.MonsterZone, Enemy).Count(card => card != null && card.Attack < Card.Attack) > 0 || GetZoneCards(CardLocation.MonsterZone, Enemy).Count() == 0 || Card.Attack >= 1500)
                return DefaultMonsterSummon();

            return false;
        }

        private bool MonsterSet()
        {
            if (FilpMonster(Card))
                return DefaultMonsterSummon();
            if (Card.HasSetcode(0x40)) return false;
            return DefaultMonsterSummon() && (Bot.LifePoints <= 1500 || (GetZoneCards(CardLocation.MonsterZone, Bot).Count() == 0 && Bot.LifePoints <= 4000));
        }

        private bool SPSummonFunction()
        {
            ClientCard l = Util.GetPZone(0, 0);
            ClientCard r = Util.GetPZone(0, 1);
            if ((Card == l || Card == r) && l != null && r != null)
            {
                int lowscales = 0;
                int highscales = 0;
                if (l.LScale > r.RScale)
                {
                    lowscales = r.RScale;
                    highscales = l.LScale;
                }
                else
                {
                    lowscales = l.LScale;
                    highscales = r.RScale;
                }
                List<ClientCard> cards = GetZoneCards(CardLocation.Hand, Bot).Where(card => card != null && !DontSummon(card) && card.Level > lowscales && card.Level < highscales).ToList();
                p_summoning = true;
                return cards.Count() > 0;
            }
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, int hint, bool cancelable)
        {
            if (Duel.Phase == DuelPhase.BattleStart)
                return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>(_cards);
            if (max > cards.Count)
                max = cards.Count;

            if (p_summoning || ((Card == Util.GetPZone(0, 0) || Card == Util.GetPZone(0, 1)) && hint == HintMsg.SpSummon) && Card.HasType(CardType.Pendulum))
            {
                List<ClientCard> result = new List<ClientCard>();
                List<ClientCard> scards = cards.Where(card => card != null && !DontSummon(card)).ToList();
                p_summoning = false;
                if (scards.Count > 0) return Util.CheckSelectCount(result, scards, 1, 1);
                else if (min == 0) return result;
            }

            if (HintFunction(hint, 13, new[]{506}) && !cards.Any(card => card != null && card.Controller == 1) && !cards.Any(card => card != null && card.Location != CardLocation.Hand))
            {
                IList<ClientCard> scards = cards.Where(card => card != null && !card.HasSetcode(0x40)).ToList();
                if (scards.Count() < min)
                {
                    IList<ClientCard> scards2 = cards.Where(card => card != null && card.HasSetcode(0x40)).ToList();
                    if (scards2.Count() > 0)
                    {
                        foreach (ClientCard card in scards2)
                        {
                            if (scards.Count() < min)
                                scards.Add(card);
                        }
                    }
                }
                if (scards.Count() >= min)
                    return Util.CheckSelectCount(scards,cards,min,max);
            }

            if ((HintFunction(hint, 13, new[]{501, 506, 508, 509}) || hint == 573) && cards.Any(card => card != null && card.Controller == 1) && cards.Any(card => card != null && card.Controller == 0) && !cards.Any(card => card != null && card.Location != CardLocation.Onfield))
            {
                IList<ClientCard> scards = cards.Where(card => card != null && card.Controller == 1).ToList();
                if (scards.Count() < min)
                {
                    IList<ClientCard> scards2 = cards.Where(card => card != null && card.Controller == 0).ToList();
                    if (scards2.Count() > 0)
                    {
                        foreach (ClientCard card in scards2)
                        {
                            if (scards.Count() < min)
                                scards.Add(card);
                        }
                    }
                }
                if (scards.Count() >= min)
                    return Util.CheckSelectCount(scards,cards,min,max);
            }

            if ((hint == 503 || hint == 507) && cards.Any(card => card != null && card.Controller == 1) && cards.Any(card => card != null && card.Controller == 0) && cards.Count(card => card != null && (card.Location == CardLocation.Grave || card.Location == CardLocation.Onfield)) == cards.Count())
            {
                IList<ClientCard> scards = cards.Where(card => card != null && card.Controller == 1).ToList();
                if (scards.Count() < min)
                {
                    IList<ClientCard> scards2 = cards.Where(card => card != null && card.Controller == 0).ToList();
                    if (scards2.Count() > 0)
                    {
                        foreach (ClientCard card in scards2)
                        {
                            if (scards.Count() < min)
                                scards.Add(card);
                        }
                    }
                }
                if (scards.Count() >= min)
                    return Util.CheckSelectCount(scards,cards,min,max);
            }

            if (HintMsgForEnemy.Contains(hint))
            {
                IList<ClientCard> enemyCards = cards.Where(card => card.Controller == 1).ToList();

                // select enemy's card first
                while (enemyCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = enemyCards[Program.Rand.Next(enemyCards.Count)];
                    selected.Add(card);
                    enemyCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForDeck.Contains(hint))
            {
                IList<ClientCard> deckCards = cards.Where(card => card.Location == CardLocation.Deck).ToList();

                // select deck's card first
                while (deckCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = deckCards[Program.Rand.Next(deckCards.Count)];
                    selected.Add(card);
                    deckCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForSelf.Contains(hint))
            {
                IList<ClientCard> botCards = cards.Where(card => card.Controller == 0).ToList();

                // select bot's card first
                while (botCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = botCards[Program.Rand.Next(botCards.Count)];
                    selected.Add(card);
                    botCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForMaterial.Contains(hint))
            {
                IList<ClientCard> materials = cards.OrderBy(card => card.Attack).ToList();

                // select low attack first
                while (materials.Count > 0 && selected.Count < min)
                {
                    ClientCard card = materials[0];
                    selected.Add(card);
                    materials.Remove(card);
                    cards.Remove(card);
                }
            }

            // select random cards
            while (selected.Count < min)
            {
                ClientCard card = cards[Program.Rand.Next(cards.Count)];
                selected.Add(card);
                cards.Remove(card);
            }

            if (HintMsgForMaxSelect.Contains(hint))
            {
                // select max cards
                while (selected.Count < max)
                {
                    ClientCard card = cards[Program.Rand.Next(cards.Count)];
                    selected.Add(card);
                    cards.Remove(card);
                }
            }
            if (hint == HintMsg.SpSummon && max == 1)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.IsCode(60461804))
                        return new List<ClientCard>(new[] { card });
                }
            }

            return selected;
        }

        public override int OnSelectOption(IList<int> options)
        {
            return Program.Rand.Next(options.Count);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (BlackmailAttackerSunmmon2(cardId))
                    return CardPosition.FaceUpAttack;
                else if (Duel.Player == 1 && Duel.Phase == DuelPhase.Battle)
                    return CardPosition.FaceUpDefence;

                if (Bot.LifePoints <= 1500)
                {
                    if (cardData.Attack >= 1800 && cardData.Attack > cardData.Defense)
                        return CardPosition.FaceUpAttack;
                    else
                        return CardPosition.FaceUpDefence;
                }
                else
                {
                    if (cardData.Defense >= 2000 && cardData.Defense > cardData.Attack)
                        return CardPosition.FaceUpDefence;
                    else
                        return CardPosition.FaceUpAttack;
                }
            }
            return 0;
        }
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            if (attacker.CanDirectAttack)
                return AI.Attack(attacker, null);

            int atk = attacker.Attack;
            if (!attacker.IsAttack())
                atk = attacker.GetDefensePower();

            List<ClientCard> cards1 = defenders.Where(defender => defender != null && !BlackmailAttacker(defender, Enemy) && defender.IsFaceup() && defender.GetDefensePower() >= atk).ToList();
            List<ClientCard> cards2 = defenders.Where(defender => defender != null && !BlackmailAttacker(defender, Enemy) && defender.IsFaceup() && defender.IsAttack() && defender.Attack < atk).ToList();
            cards2.Sort(CardContainer.CompareCardAttack);

            if (BlackmailAttacker(attacker, Bot))
            {
                if (cards1.Count() > 0 && cards2.Count() > 0)
                {
                    int dam1 = cards1[0].GetDefensePower();
                    int dam2 = cards2[0].GetDefensePower();

                    if (dam1 - atk > atk - dam2)
                        return AI.Attack(attacker, cards1[0]);
                    else
                        return AI.Attack(attacker, cards2[0]);
                }
                else if (cards1.Count() > 0)
                    return AI.Attack(attacker, cards1[0]);
                else if (cards2.Count() > 0)
                    return AI.Attack(attacker, cards2[0]);

                return AI.Attack(attacker, defenders[0]);
            }

            if (defenders.Any(defender => defender != null && defender.HasSetcode(0x10) && BlackmailAttacker(defender, Enemy)))
            {
                List<ClientCard> scards = defenders.Where(defender => defender != null && defender.Id == 69058960 && !defender.IsDisabled() && defender.IsFaceup()).ToList();
                if (scards.Count() > 0)
                    return base.OnSelectAttackTarget(attacker, scards);
                else if (defenders.Any(defender => defender != null && !BlackmailAttacker(defender, Enemy)))
                    foreach (ClientCard defender in defenders)
                    {
                        if (defender == null || !base.OnPreBattleBetween(attacker, defender) || BlackmailAttacker(defender, Enemy))
                            continue;

                        if ((atk > defender.GetDefensePower()) || (atk >= defender.GetDefensePower() && attacker.IsLastAttacker && defender.IsAttack()) || (defender.IsFacedown()))
                            return AI.Attack(attacker, defender);
                    }

                return null;
            }
            foreach (ClientCard defender in defenders)
            {
                if (defender == null || !base.OnPreBattleBetween(attacker, defender) || BlackmailAttacker(defender, Enemy))
                    continue;

                if ((atk > defender.GetDefensePower()) || (atk >= defender.GetDefensePower() && attacker.IsLastAttacker && defender.IsAttack()) || (defender.IsFacedown()))
                    return AI.Attack(attacker, defender);
            }

            return null;
        }
    }
}
