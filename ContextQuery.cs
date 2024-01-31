namespace ECSGameEngine;

public partial class EngineContext
{
    private Dictionary<TypeKey, List<List<Component>>> componentCache = new();
    public List<List<Component>> Query(params Type[] types)
    {
        var typeKey = new TypeKey(types);

        if (this.componentCache.TryGetValue(typeKey, out var cache))
        {
            if (cache == null) this.componentCache.Remove(typeKey);
            else return cache;
        }

        if (types.Length == 0)
        {
            return new List<List<Component>>();
        }

        List<List<Component>> groupList = new();

        if (this.componentArrays.TryGetValue(types[0], out var components))
        {
            groupList = components.Select(c => new List<Component>() { c }).ToList();
        }
        else
        {
            return new List<List<Component>>();
        }

        if (types.Length == 1)
        {
            return groupList;
        }

        foreach (var type in types)
        {
            if (type == types[0]) continue;

            List<Component> nextList = this.componentArrays[type];
            int lastAdd = 0;

            foreach (var group in groupList)
            {
                for (int i = lastAdd; i < nextList.Count; i++)
                {
                    var next = nextList[i];

                    if (group[0].entityID == next.entityID)
                    {
                        group.Add(next);
                        lastAdd = i + 1;
                        goto outer;
                    }
                }
                groupList.Remove(group);
            outer:
                continue;
            }
        }

        this.componentCache.Add(typeKey, groupList);

        return groupList;
    }

    public List<T> Query<T>() where T : Component
    {
        if (!this.componentArrays.TryGetValue(typeof(T), out var components))
        {
            return new List<T>();
        }
        return new EntityQuery<T, Component>(components).ToList();
    }

    private Dictionary<TypeKey, List<(Component, Component)>> componentSet2 = new();
    public List<(T1, T2)> Query<T1, T2>()
        where T1 : Component
        where T2 : Component
    {
        var typeKey = new TypeKey(typeof(T1), typeof(T2));
        if (componentSet2.TryGetValue(typeKey, out var cache))
        {
            if (cache != null) return cache.Select(tup => ((T1)tup.Item1, (T2)tup.Item2)).ToList();
            WriteLine("Got cache, but it's null?");
            if (!componentSet2.Remove(typeKey))
            {
                WriteLine("But also it's not there??");
            }
        }


        if (!this.componentArrays.TryGetValue(typeof(T1), out var components1) ||
        !this.componentArrays.TryGetValue(typeof(T2), out var components2)) return new List<(T1, T2)>();

        var joined = from c1 in components1
                     join c2 in components2 on c1.entityID equals c2.entityID
                     select
                     (
                         (T1)c1,
                         (T2)c2
                     );

        var joinedList = joined.ToList();

        componentSet2.Add(typeKey, joinedList.Select(tup => ((Component)tup.Item1, (Component)tup.Item2)).ToList());

        return joinedList;
    }

    private Dictionary<TypeKey, List<(Component, Component, Component)>> componentSet3 = new();
    public List<(T1, T2, T3)> Query<T1, T2, T3>()
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        var typeKey = new TypeKey(typeof(T1), typeof(T2), typeof(T3));
        if (componentSet3.TryGetValue(typeKey, out var cache))
        {
            if (cache != null) return cache.Select(tup => ((T1)tup.Item1, (T2)tup.Item2, (T3)tup.Item3)).ToList();
            WriteLine("Got cache, but it's null?");
            if (!componentSet3.Remove(typeKey))
            {
                WriteLine("But also it's not there??");
            }
        }

        if (!this.componentArrays.TryGetValue(typeof(T1), out var components1) ||
            !this.componentArrays.TryGetValue(typeof(T2), out var components2) ||
            !this.componentArrays.TryGetValue(typeof(T3), out var components3)) return new List<(T1, T2, T3)>();

        var joinedList = from c1 in components1
                         join c2 in components2 on c1.entityID equals c2.entityID
                         join c3 in components3 on c1.entityID equals c3.entityID
                         select
                         (
                             (T1)c1,
                             (T2)c2,
                             (T3)c3
                         );

        componentSet3.Add(typeKey, joinedList.Select(tup => ((Component)tup.Item1, (Component)tup.Item2, (Component)tup.Item3)).ToList());

        return joinedList.ToList();
    }

    private Dictionary<TypeKey, List<(Component, Component, Component, Component)>> componentSet4 = new();
    public List<(T1, T2, T3, T4)> Query<T1, T2, T3, T4>()
        where T1 : Component
        where T2 : Component
        where T3 : Component
        where T4 : Component
    {
        var typeKey = new TypeKey(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        if (componentSet4.TryGetValue(typeKey, out var cache))
        {
            if (cache != null) return cache.Select(tup => ((T1)tup.Item1, (T2)tup.Item2, (T3)tup.Item3, (T4)tup.Item4)).ToList();
            WriteLine("Got cache, but it's null?");
            if (!componentSet4.Remove(typeKey))
            {
                WriteLine("But also it's not there??");
            }
        }

        if (!this.componentArrays.TryGetValue(typeof(T1), out var components1) ||
            !this.componentArrays.TryGetValue(typeof(T2), out var components2) ||
            !this.componentArrays.TryGetValue(typeof(T3), out var components3) ||
            !this.componentArrays.TryGetValue(typeof(T4), out var components4)) return new List<(T1, T2, T3, T4)>();

        var joinedList = from c1 in components1
                         join c2 in components2 on c1.entityID equals c2.entityID
                         join c3 in components3 on c1.entityID equals c3.entityID
                         join c4 in components4 on c1.entityID equals c4.entityID
                         select
                         (
                             (T1)c1,
                             (T2)c2,
                             (T3)c3,
                             (T4)c4
                         );

        componentSet4.Add(typeKey, joinedList.Select(tup => ((Component)tup.Item1, (Component)tup.Item2, (Component)tup.Item3, (Component)tup.Item4)).ToList());

        return joinedList.ToList();
    }
}

class TypeKey
{
    private readonly Type[] types;

    public TypeKey(params Type[] types)
    {
        // Sort the types to ensure consistent hash code
        this.types = types.OrderBy(t => t.FullName).ToArray();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (Type type in types)
            {
                hash = hash * 23 + type.GetHashCode();
            }
            return hash;
        }
    }

    public Type[] GetTypes()
    {
        return types;
    }

    public override bool Equals(object obj)
    {
        if (obj is TypeKey other)
        {
            if (this.types.Length != other.types.Length)
                return false;

            for (int i = 0; i < this.types.Length; i++)
            {
                if (this.types[i] != other.types[i])
                    return false;
            }
            return true;
        }
        return false;
    }
}