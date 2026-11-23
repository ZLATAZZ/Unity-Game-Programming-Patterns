namespace CharacterModifierBytecodeDemo.Bytecode
{
    public interface IBytecodeContext
    {
        float ReadProperty(byte propertyId);
        void WriteProperty(byte propertyId, float value);
    }
}
