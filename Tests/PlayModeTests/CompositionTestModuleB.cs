using Leap.Forward.Composition;

public partial class CompositionTestModuleB : ModuleBase<CompositionTestContainer>
{
    public CompositionTestModuleA ModuleA { get; private set; }

    public override void SetupModule()
    {
        ModuleA = Container.ModuleA;
    }
}
