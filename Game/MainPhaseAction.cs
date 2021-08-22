﻿namespace WindBot.Game
{
    public class MainPhaseAction
    {
        public enum MainAction
        {
            Summon = 3,
            SpSummon = 1,
            Repos = 2,
            SetMonster = 0,
            SetSpell = 4,
            Activate = 5,
            ToBattlePhase = 7,
            ToEndPhase = 6
        }

        public MainAction Action { get; private set; }
        public int Index { get; private set; }

        public MainPhaseAction(MainAction action)
        {
            Action = action;
            Index = 0;
        }

        public MainPhaseAction(MainAction action, int index)
        {
            Action = action;
            Index = index;
        }

        public MainPhaseAction(MainAction action, int[] indexes)
        {
            Action = action;
            Index = indexes[(int)action];
        }

        public int ToValue()
        {
            return (Index << 16) + (int)Action;
        }
    }
}
