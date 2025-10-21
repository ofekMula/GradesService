using System.ComponentModel.DataAnnotations;

public record PaginationDto(
    [Range(0, int.MaxValue, ErrorMessage = "skip must be >= 0")]
    int Skip = 0,

    [Range(1, 200, ErrorMessage = "take must be between 1 and 200")]
    int Take = 50
);
