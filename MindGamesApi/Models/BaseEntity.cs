using System;

namespace MindGamesApi.Models;

public class BaseEntity
{
    public Guid Id { get; set; }

    protected BaseEntity(Guid id) => this.Id = id;
}
