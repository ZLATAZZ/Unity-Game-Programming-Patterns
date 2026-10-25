using Patterns.DoubleBuffer.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.DoubleBuffer.Unity.UI
{
    public sealed class SlapActorRowView : MonoBehaviour
    {
        [SerializeField] private Text _nameLabel;
        [SerializeField] private Text _currentLabel;
        [SerializeField] private Text _nextLabel;
        [SerializeField] private Text _actionLabel;

        private static readonly Color Active = new Color(0.36f, 0.91f, 0.72f);
        private static readonly Color Inactive = new Color(0.51f, 0.58f, 0.71f);
        private static readonly Color Queued = new Color(1f, 0.76f, 0.40f);

        public void Configure(Text nameLabel, Text currentLabel, Text nextLabel, Text actionLabel)
        {
            _nameLabel = nameLabel;
            _currentLabel = currentLabel;
            _nextLabel = nextLabel;
            _actionLabel = actionLabel;
        }

        public void Render(SlapActor actor, bool reacted)
        {
            _nameLabel.text = actor.Id.ToString();
            _currentLabel.text = actor.CurrentSlapped ? "TRUE" : "FALSE";
            _currentLabel.color = actor.CurrentSlapped ? Active : Inactive;
            _nextLabel.text = actor.NextSlapped ? "TRUE" : "FALSE";
            _nextLabel.color = actor.NextSlapped ? Queued : Inactive;
            _actionLabel.text = reacted ? "REACTED" : actor.NextSlapped ? "QUEUED" : "-";
            _actionLabel.color = reacted ? Active : actor.NextSlapped ? Queued : Inactive;
        }
    }
}
