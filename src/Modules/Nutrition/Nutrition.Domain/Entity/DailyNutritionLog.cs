using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Nutrition.Domain.Enums;
using Nutrition.Domain.Events;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Domain.Entity
{
    public class DailyNutritionLog : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal? DailyCalorieGoal { get; private set; }
        public string? Note { get; private set; }

        private readonly List<LogEntry> _entries = new();
        public IReadOnlyList<LogEntry> Entries => _entries.AsReadOnly();

        private DailyNutritionLog() { }

        public static DailyNutritionLog Create(Guid userId, DateTime date, decimal? dailyCalorieGoal, string? note)
        {
            var log = new DailyNutritionLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date.Date,
                DailyCalorieGoal = dailyCalorieGoal,
                Note = note
            };

            log.AddDomainEvent(new DailyNutritionLogCreatedEvent(log.Id, userId));

            return log;
        }

        public void Update(decimal? dailyCalorieGoal, string? note)
        {
            DailyCalorieGoal = dailyCalorieGoal;
            Note = note;

            AddDomainEvent(new DailyNutritionLogUpdatedEvent(Id));
        }

        public void Delete(string deletedBy)
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new DailyNutritionLogDeletedEvent(Id, deletedBy));
        }

        public LogEntry AddEntry(Guid foodId, string foodName, decimal quantity, ServingUnit servingUnit, MacroNutrients macros)
        {
            var entry = new LogEntry(Guid.NewGuid(), Id, foodId, foodName, quantity, servingUnit, macros);
            _entries.Add(entry);

            AddDomainEvent(new DailyNutritionLogEntryChangedEvent(Id));

            return entry;
        }

        public void RemoveEntry(Guid entryId)
        {
            var entry = _entries.SingleOrDefault(x => x.Id == entryId);
            if (entry is null)
            {
                return;
            }

            _entries.Remove(entry);

            AddDomainEvent(new DailyNutritionLogEntryChangedEvent(Id));
        }

        public LogEntry UpdateEntryQuantity(Guid entryId, decimal quantity, MacroNutrients macros)
        {
            var entry = _entries.SingleOrDefault(x => x.Id == entryId)
                        ?? throw new DomainNotFoundException("LogEntry", entryId, "DailyNutritionLog", Id);

            entry.UpdateQuantity(quantity, macros);

            AddDomainEvent(new DailyNutritionLogEntryChangedEvent(Id));

            return entry;
        }
    }
}
