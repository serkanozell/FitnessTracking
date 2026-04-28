using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Analytics.GetPersonalRecords;

public sealed record GetPersonalRecordsQuery(int Top = 10)
    : IQuery<Result<IReadOnlyList<PersonalRecordDto>>>;
