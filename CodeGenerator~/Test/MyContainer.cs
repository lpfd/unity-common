using Leap.Forward.Composition;
using System.Collections.ObjectModel;

public partial class MyContainer : ContainerBase<MyContainer>
{
    [AttachedModule]
    private MyModuleA _a;

    [AttachedModule]
    private ObservableCollection<MyModuleB> _b;

    public MyContainer()
    {
        
    }
}
