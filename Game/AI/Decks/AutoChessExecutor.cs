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

            AddExecutor(ExecutorType.SpSummon); 
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
                得到某个位置的卡片的函数（从神数不神那借来的）
                    GetZoneCards(CardLocation loc, ClientField player)
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
                        if (cards.Count() > 0)
                        {
                            AI.SelectNextCard(cards);
                            return true;
                        }
                        return false;
                    }
                    if (Card.Id == 57736667 || Card.Id == 36148308)
                    {
                        cards = GetZoneCards(CardLocation.Onfield, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    if (Card.Id == 99013397 || Card.Id == 70423794 || Card.Id == 22147147)
                    {
                        cards = GetZoneCards(CardLocation.Onfield, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget() && (card.HasType(CardType.Spell) || card.HasType(CardType.Trap))).ToList();
                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    return DefaultDontChainMyself();
                }
                else if (Card.Location == CardLocation.Grave || Card.Location == CardLocation.Removed)
                {
                    if (Card.Id == 64867422)
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget() && card.IsFaceup()).ToList();
                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    if (Card.Id == 66947913)
                    {
                        cards = GetZoneCards(CardLocation.MonsterZone, Enemy).Where(card => card != null && !card.IsShouldNotBeTarget()).ToList();
                        if (cards.Count() > 0)
                        {
                            AI.SelectCard(cards);
                            return true;
                        }
                        return false;
                    }
                    return DefaultDontChainMyself();
                }
                return false;
            }
            return false;
        }

        private bool ActivateFunction()
        {
            if (Card.HasType(CardType.Equip) || (Card.HasType(CardType.Pendulum) && Card.Location == CardLocation.Hand && ActivateDescription == 1160))
                return false;
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
                    List<ClientCard> cards = GetZoneCards(CardLocation.Onfield, Enemy);
                    cards = cards.Where(tcard => tcard != null && !tcard.IsShouldNotBeTarget()).ToList();
                    if (cards.Count <= 0) return false;
                    AI.SelectCard(cards);
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
