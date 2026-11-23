using System;
using CharacterModifierBytecodeDemo.Bytecode;

namespace CharacterModifierBytecodeDemo.Character
{
    public sealed class CharacterBytecodeSymbolTable : IBytecodeSymbolTable
    {
        public bool TryResolveProperty(string symbol, out byte propertyId)
        {
            if (Enum.TryParse(symbol, true, out CharacterPropertyId property))
            {
                propertyId = (byte)property;
                return true;
            }

            propertyId = default;
            return false;
        }
    }
}
