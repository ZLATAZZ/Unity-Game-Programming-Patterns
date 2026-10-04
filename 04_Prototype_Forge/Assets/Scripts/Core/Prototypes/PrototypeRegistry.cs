using System;
using System.Collections.Generic;

namespace PrototypeForge.Core.Prototypes
{
    public sealed class PrototypeRegistry<TPrototype> where TPrototype : class, IPrototype<TPrototype>
    {
        private readonly Dictionary<PrototypeId, TPrototype> _prototypes = new();
        private readonly List<PrototypeId> _registrationOrder = new();

        public int Count => _prototypes.Count;

        public void Register(TPrototype prototype)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }

            if (prototype.Id.IsEmpty)
            {
                throw new InvalidOperationException("A prototype cannot have an empty ID.");
            }

            if (!_prototypes.TryAdd(prototype.Id, prototype))
            {
                throw new InvalidOperationException($"A prototype with ID '{prototype.Id}' is already registered.");
            }

            _registrationOrder.Add(prototype.Id);
        }

        public TPrototype Create(PrototypeId id)
        {
            if (!_prototypes.TryGetValue(id, out TPrototype prototype))
            {
                throw new KeyNotFoundException($"Prototype '{id}' is not registered.");
            }

            return prototype.Clone();
        }

        public IReadOnlyList<TPrototype> CreateAll()
        {
            List<TPrototype> clones = new(_registrationOrder.Count);

            for (int i = 0; i < _registrationOrder.Count; i++)
            {
                clones.Add(Create(_registrationOrder[i]));
            }

            return clones;
        }
    }
}