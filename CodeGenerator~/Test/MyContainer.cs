using Leap.Forward.Composition;
using System.Collections.ObjectModel;

public partial class MyContainer : ContainerBase<MyContainer>
{
    [AttachedModule]
    private MyModuleA _a;

    [AttachedModule]
    private ObservableCollection<MyModuleB> _b;

    [AttachedModule]
    private ObservableCollection<ICooldownModifier> _cooldownModifiers = new ObservableCollection<ICooldownModifier>();
    

    public float EvaluateCooldown(float value)
    {
        foreach (var modifer in _cooldownModifiers)
        {
            value = modifer.ModifyCooldown(value);
        }
        return value;
    }

    public MyContainer()
    {
        
    }
}
