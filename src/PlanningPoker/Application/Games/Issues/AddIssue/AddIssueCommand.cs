﻿#region

using PlanningPoker.Application.Abstractions.Commands;

#endregion

namespace PlanningPoker.Application.Games.Issues.AddIssue;

public class AddIssueCommand(string gameId, string name, string? link = null, string? description = null)
    : Command
{
    public string GameId { get; } = gameId;
    public string Name { get; } = name;
    public string? Link { get; } = link;
    public string? Description { get; } = description;
}