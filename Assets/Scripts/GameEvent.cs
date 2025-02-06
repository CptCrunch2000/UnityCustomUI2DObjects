using System;

public class GameEvent
{
    private event Action action = delegate { };

    public void Publish()
    {
        action?.Invoke();
    }

    public void Add(Action subscriber)
    {
        action += subscriber;
    }

    public void Remove(Action subscriber)
    {
        action -= subscriber;
    }
}

public class GameEvent<T>
{
    private event Action<T> action;

    public void Publish(T param)
    {
        action?.Invoke(param);
    }

    public void Add(Action<T> subscriber)
    {
        action += subscriber;
    }

    public void Remove(Action<T> subscriber)
    {
        action -= subscriber;
    }
}

public static partial class Events
{
    public static readonly GameEvent<PieChartPiece> onPieChartPieceAdded = new();
    public static readonly GameEvent<int> onPieChartPieceRemove = new();
}