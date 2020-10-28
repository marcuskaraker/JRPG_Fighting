using UnityEngine;
using System.Collections.Generic;

public struct TargetOrder
{
    public Tile targetTile;
    public int orderID;

    public TargetOrder(Tile targetTile, int orderID)
    {
        this.targetTile = targetTile;
        this.orderID = orderID;
    }
}

public class CommandInvoker
{
    public List<TargetOrder> targetTiles = new List<TargetOrder>();

    Queue<Command> commands = new Queue<Command>();

    public void AddCommand(Command command)
    {
        commands.Enqueue(command);
    }

    public void AddTargetTile(Tile tile)
    {
        targetTiles.Add(new TargetOrder(tile, targetTiles.Count));
    }

    public void RemoveTargetTile(Tile tile)
    {
        targetTiles.RemoveAll(x => x.targetTile == tile);
    }

    public void RemoveTargetTile(int order)
    {
        targetTiles.RemoveAll(x => x.orderID == order);
    }

    /// <summary>
    /// Executes the command in the front of the command queue.
    /// </summary>
    /// <returns>true if a command was executed.</returns>
    public bool ExecuteCommand()
    {
        if (commands.Count > 0)
        {
            Command command = commands.Dequeue();
            command.Execute(this);
            return true;
        }

        return false;
    }
}
