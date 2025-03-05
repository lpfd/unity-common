using Leap.Forward.Composition;

public partial class MyModuleB : ModuleBase<MyContainer>
{
    private MyModuleA _a;

    public override void SetupModules()
    {
        this._a = Container.A;

        base.SetupModules();
    }
}

public interface ICooldownModifier
{
    float ModifyCooldown(float value);
}

public partial class CooldownModifier : ModuleBase<MyContainer>, ICooldownModifier
{
    private MyModuleA _a;

    public float ModifyCooldown(float value)
    {
        return value * 0.5f;
    }
}