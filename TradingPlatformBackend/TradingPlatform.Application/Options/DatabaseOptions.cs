using System.ComponentModel.DataAnnotations;

namespace TradingEngine.Application.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    [Required]
    public string DefaultConnection { get; set; } = string.Empty;
    
    public const string MigrationsAssembly = "TradingEngine.Infrastructure";
}
