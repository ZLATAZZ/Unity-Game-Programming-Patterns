using System;
using UnityEngine;

namespace Patterns.Command.Customization.BodyMaterial
{
    [Serializable]
    public sealed class MaterialSlotTarget
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private int _materialIndex;

        public Renderer Renderer => _renderer;
        public int MaterialIndex => _materialIndex;
    }
}