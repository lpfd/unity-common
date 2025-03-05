# Composition Unity Library Documentation

The Composition library facilitates the creation of modular and maintainable game architectures by leveraging composition over inheritance. Developers can define complex game entities — such as characters or items — by composing them from interchangeable modules. This approach enhances code reusability, scalability, and clarity.

## Overview

The library comprises three primary components:

1. **ContainerBase\<T\>**: Serves as the foundational class for composite entities. Developers can inherit from this class to define logical entities like characters, items, etc.

2. **ModuleBase\<T\>**: Acts as the base class for individual modules. Developers can create modules such as health managers, hit boxes, audio players, inventories, movement controllers, etc., by inheriting from this class.

3. **Code Generator**: Automates the integration of modules within containers. By marking container fields with the `AttachedModule` attribute, the code generator produces the necessary properties and code, streamlining the development process.

## Getting Started

### Creating a Container

To define a composite entity, create a class that inherits from the `ContainerBase<T>` base class. This class will act as a container for various modules.

```csharp
public class Character : ContainerBase<Character>
{
    // Container-specific logic
}
```


### Developing Modules

Modules encapsulate distinct functionalities and can interact with other modules within the same container. To create a module, inherit from the `ModuleBase<T>` base class and override the `SetupModule` method for initialization and inter-module communication.

```csharp
public interface IHealthManager
{
     void TakeDamage(int amount);
}

public class HealthManager : ModuleBase<Character>, IHealthManager
{
    public int health = 100;

    public override void SetupModule()
    {
        // Initialization logic
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // Handle entity death
        }
    }
}
```



```csharp
public class Hitbox : ModuleBase<Character>
{
    private IHealthManager healthManager;

    public override void SetupModule()
    {
        // Discover and store reference to HealthManager module
        healthManager = Container.HealthManager;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Example damage value
        int damage = 10;
        healthManager?.TakeDamage(damage);
    }
}
```


### Integrating Modules into a Container

To attach modules to a container, define fields in the container class and annotate them with the `AttachedModule` attribute. The code generator will process these annotations to integrate the modules seamlessly.

```csharp
public class Character : ContainerBase<Character>
{
    [AttachedModule]
    private IHealthManager healthManager;

    [AttachedModule]
    private List<Hitbox> hitboxes = new List<Hitbox>();

    // Additional container logic
}
```


After marking the fields with the `AttachedModule` attribute, run the code generator to produce the necessary properties and integration code.

## Module Interaction

Modules can discover and interact with each other via Container within the `SetupModule` method. This mechanism allows modules to establish references to other modules they depend on, facilitating cohesive functionality.

```csharp
public override void SetupModule()
{
    // Discover and store reference to HealthManager module
    healthManager = Container.HealthManager;
}
```

In this example, the `Hitbox` module retrieves a reference to the `HealthManager` module during its setup phase, enabling it to report damage.

## Code Generation

The code generator simplifies module integration by automatically generating the necessary boilerplate code. By marking container fields with the `AttachedModule` attribute, developers can focus on implementing functionality without worrying about manual module registration.

**Steps to Use Code Generation**:

1. Annotate module fields in the container with the `AttachedModule` attribute.

2. The code generation assembly will generate the required properties and integration code, streamlining the development process.

## Best Practices

- **Single Responsibility**: Design each module to handle a specific aspect of functionality, promoting modularity and ease of maintenance.

- **Loose Coupling**: Utilize the module discovery mechanism to interact with other modules, reducing dependencies and enhancing flexibility.

- **Initialization**: Perform module setup and inter-module references within the `SetupModule` method to ensure all modules are initialized before use.

By adhering to these practices, developers can create scalable and maintainable game architectures using the Composition Unity Library.

## Conclusion

The Composition Unity Library offers a structured approach to building modular game entities through the use of containers and modules. With automatic code generation and a focus on composition, developers can efficiently create complex, maintainable, and scalable game systems. 