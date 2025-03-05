using Leap.Forward.Composition;
using System.Collections.Generic;

public partial class CompositionTestContainer : ContainerBase<CompositionTestContainer>
{
    [AttachedModule]
    private CompositionTestModuleA moduleA;

    [AttachedModule]
    private List<CompositionTestModuleB> modulesB = new List<CompositionTestModuleB>();

    // Update is called once per frame
    void Update()
    {
    }
}