namespace Database;
using Microsoft.EntityFrameworkCore;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
}