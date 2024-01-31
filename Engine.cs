global using Raylib_cs;
using System.Collections;

namespace ECSGameEngine;

public class EntityQuery<T, U> : IEnumerable<T> where T : U
{
    private List<U> items = new List<U>();
    public EntityQuery(List<U> list)
    {
        this.items = list;
    }
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in items)
        {
            yield return (T)item!;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public partial class EngineContext
{
    private uint currentEntityID = 0;
    public Dictionary<Type, List<Component>> componentArrays = new();
    private Dictionary<Type, object> resources = new();
    private Dictionary<TypeKey, Dictionary<Type, List<Component>>> componentSets = new();

    public EngineContext(Dictionary<Type, object> resources)
    {
        this.resources = resources;
    }

    public T? GetResource<T>()
    {
        if (resources.TryGetValue(typeof(T), out var resource))
            return (T)resource;
        return default;
    }

    public void AddEntity(params Component?[] addedComponents)
    {
        Component[] components = addedComponents.Where(comp => comp != null).ToArray();

        foreach (var component in components)
        {
            component.entityID = this.currentEntityID;
            var type = component.GetType();
            if (this.componentArrays.ContainsKey(type))
            {
                this.componentArrays[type].Add(component);
            }
            else
            {
                this.componentArrays[type] = new() { component };
            }
        }

        var set = new HashSet<Type>(components.Select(comp => comp.GetType()));

    outerloop:
        foreach (var key in componentCache.Keys)
        {
            foreach (var type in key.GetTypes())
            {
                if (!set.Contains(type)) goto outerloop;
            }

            if (!this.componentCache.Remove(key)) WriteLine("Components not removed from set 2!");
        }

    outerloop2:
        foreach (var key in componentSet2.Keys)
        {
            foreach (var type in key.GetTypes())
            {
                if (!set.Contains(type)) goto outerloop2;
            }

            if (!this.componentSet2.Remove(key)) WriteLine("Components not removed from set 2!");
        }

    outerloop3:
        foreach (var key in componentSet3.Keys)
        {
            foreach (var type in key.GetTypes())
            {
                if (!set.Contains(type)) goto outerloop3;
            }

            if (!this.componentSet3.Remove(key)) WriteLine("Components not removed from set 3!");
        }

    outerloop4:
        foreach (var key in componentSet4.Keys)
        {
            foreach (var type in key.GetTypes())
            {
                if (!set.Contains(type)) goto outerloop4;
            }

            if (!this.componentSet4.Remove(key)) WriteLine("Components not removed from set 4!");
        }

        this.currentEntityID++;
    }
}

public class Engine
{
    private EngineContext context;
    private List<SystemFunction> startupSystems = new();
    private List<SystemFunction> updateSystems = new();
    private Dictionary<Type, object> resources = new();
    public delegate void SystemFunction(EngineContext ctx);
    public Engine()
    {
        this.context = new EngineContext(resources);
    }
    public Engine AddStartupSystems(params SystemFunction[] systems)
    {
        this.startupSystems.AddRange(systems);
        return this;
    }
    private void RunStartupSystems()
    {
        foreach (var func in this.startupSystems)
        {
            func(this.context);
        }
    }
    public Engine AddUpdateSystems(params SystemFunction[] systems)
    {
        this.updateSystems.AddRange(systems);
        return this;
    }
    public Engine AddResources(params object[] resources)
    {
        foreach (var resource in resources)
        {
            this.resources[resource.GetType()] = resource;
        }

        return this;
    }
    private void RunUpdateSystems()
    {
        foreach (var func in this.updateSystems)
        {
            func(this.context);
        }
    }
    public void Run()
    {
        Raylib.InitWindow(Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT, Constants.WINDOW_TITLE);
        Raylib.SetTargetFPS(Constants.TARGET_FPS);

        this.RunStartupSystems();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Constants.BACKGROUND_COLOR);
            this.RunUpdateSystems();
            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
