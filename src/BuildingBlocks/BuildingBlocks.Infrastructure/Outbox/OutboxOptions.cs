namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxOptions
    {
        public const string SectionName = "Outbox";
        public int IntervalInSeconds { get; set; } = 10;
        public int BatchSize { get; set; } = 20;
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Tek bir tetikleme döngüsünde (interval delay'i beklemeden) art arda işlenebilecek
        /// maksimum dolu batch sayısı. Batch tam dolduğu sürece bir sonraki batch hemen işlenir
        /// (drain); bu tavan, birikmiş mesaj çok fazla olduğunda DB'yi süresiz sorgulamayı önler.
        /// </summary>
        public int MaxDrainIterations { get; set; } = 10;
    }
}