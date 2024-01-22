using Minimal_Vehicle_API.Domain.Enums;

namespace Minimal_Vehicle_API.Domain.ModelViews
{
    public record AdminModelView
    {
        public int Id { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Profile { get; set; } = default!;
    }
}
