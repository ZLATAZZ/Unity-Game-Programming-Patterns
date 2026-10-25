using System;
using Patterns.DoubleBuffer.Core;
using SlapSimulationMode = Patterns.DoubleBuffer.Core.SimulationMode;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.DoubleBuffer.Unity.UI
{
    public sealed class SlapStageCardView : MonoBehaviour
    {
        [SerializeField] private Text _nameLabel;
        [SerializeField] private Text _orderLabel;
        [SerializeField] private Text _reactionsLabel;
        [SerializeField] private SlapActorRowView[] _rows;

        public void Configure(Text nameLabel, Text orderLabel, Text reactionsLabel, SlapActorRowView[] rows)
        {
            _nameLabel = nameLabel;
            _orderLabel = orderLabel;
            _reactionsLabel = reactionsLabel;
            _rows = rows;
        }

        public void Render(SlapStage stage)
        {
            _nameLabel.text = stage.Mode == SlapSimulationMode.DoubleBuffered
                ? "DOUBLE BUFFER"
                : "IMMEDIATE UPDATE";
            _orderLabel.text = stage.Order == UpdateOrder.Forward
                ? "HARRY > BALDY > CHUMP"
                : "CHUMP > BALDY > HARRY";

            for (int index = 0; index < _rows.Length; index++)
            {
                ActorId actorId = (ActorId)index;
                bool reacted = false;

                foreach (SlapReaction reaction in stage.Reactions)
                {
                    if (reaction.Source == actorId)
                    {
                        reacted = true;
                        break;
                    }
                }

                _rows[index].Render(stage.GetActor(actorId), reacted);
            }

            if (stage.Reactions.Count == 0)
            {
                _reactionsLabel.text = "REACTIONS   None yet";
                return;
            }

            string reactions = "REACTIONS   ";

            for (int index = 0; index < stage.Reactions.Count; index++)
            {
                SlapReaction reaction = stage.Reactions[index];
                reactions += reaction.Source + " > " + reaction.Target;

                if (index < stage.Reactions.Count - 1)
                {
                    reactions += "  |  ";
                }
            }

            _reactionsLabel.text = reactions;
        }
    }
}
