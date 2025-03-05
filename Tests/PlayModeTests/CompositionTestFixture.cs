using Leap.Forward.Composition;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

[TestFixture]
public class CompositionTestFixture
{
    GameObject rootGameObject;

    [TearDown]
    public void TearDown()
    {
        // Destroy the GameObjects after the test
        Object.DestroyImmediate(rootGameObject);
    }

    [Test]
    public void CreateContainerFromCode()
    {
        rootGameObject = new GameObject("ParentObject");
        var container = rootGameObject.AddComponent<CompositionTestContainer>();

        Assert.IsTrue(container.IsInitialized);
        Assert.IsNull(container.ModuleA);
    }

    [Test]
    public void TestLateModuleActivation()
    {
        rootGameObject = new GameObject("ParentObject");
        var childObject = new GameObject("ChildObject");
        childObject.transform.SetParent(rootGameObject.transform);

        var container = rootGameObject.AddComponent<CompositionTestContainer>();
        var module = childObject.AddComponent<CompositionTestModuleA>();

        Assert.AreEqual(container, module.Container);
        Assert.AreEqual(module, container.ModuleA);
    }

    [Test]
    public void TestLateContainerActivation()
    {
        UnityEngine.TestTools.LogAssert.Expect("ChildObject of CompositionTestModuleA: No contaier found in the parent game objects.");

        rootGameObject = new GameObject("ParentObject");
        var childObject = new GameObject("ChildObject");
        childObject.transform.SetParent(rootGameObject.transform);

        var module = childObject.AddComponent<CompositionTestModuleA>();
        var container = rootGameObject.AddComponent<CompositionTestContainer>();

        Assert.AreEqual(container, module.Container);
        Assert.AreEqual(module, container.ModuleA);
    }


    [Test]
    public void TestLateModuleBActivation()
    {
        rootGameObject = new GameObject("ParentObject");
        var childObject = new GameObject("ChildObject");
        childObject.transform.SetParent(rootGameObject.transform);

        var container = rootGameObject.AddComponent<CompositionTestContainer>();
        var moduleA = childObject.AddComponent<CompositionTestModuleA>();
        var moduleB = childObject.AddComponent<CompositionTestModuleB>();

        Assert.AreEqual(container, moduleA.Container);
        Assert.AreEqual(moduleA, container.ModuleA);
        Assert.AreEqual(container, moduleB.Container);
        Assert.AreEqual(moduleB, container.ModulesB.FirstOrDefault());
        Assert.AreEqual(moduleA, moduleB.ModuleA);
    }

    [Test]
    public void ModulesAtSameLevelAsContainer()
    {
        rootGameObject = new GameObject("ParentObject");

        var container = rootGameObject.AddComponent<CompositionTestContainer>();
        var moduleA = rootGameObject.AddComponent<CompositionTestModuleA>();
        var moduleB = rootGameObject.AddComponent<CompositionTestModuleB>();

        Assert.AreEqual(container, moduleA.Container);
        Assert.AreEqual(moduleA, container.ModuleA);
        Assert.AreEqual(container, moduleB.Container);
        Assert.AreEqual(moduleB, container.ModulesB.FirstOrDefault());
        Assert.AreEqual(moduleA, moduleB.ModuleA);
    }
}


