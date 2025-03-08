using Leap.Forward.Composition;

public partial class MyModuleB : ModuleBase<MyContainer>
{
    private MyModuleA _a;

    public override void SetupModule()
    {
        this._a = Container.A;

        base.SetupModule();
    }
}
