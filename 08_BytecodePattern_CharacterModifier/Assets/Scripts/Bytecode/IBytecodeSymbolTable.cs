namespace CharacterModifierBytecodeDemo.Bytecode
{
    public interface IBytecodeSymbolTable
    {
        bool TryResolveProperty(string symbol, out byte propertyId);
    }
}
