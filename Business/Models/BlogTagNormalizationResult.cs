namespace TrainingTest.Business.Models;

public class BlogTagNormalizationResult
{
    public int Scanned { get; set; }
    public int Changed { get; set; }
    public int Unchanged { get; set; }
    public int Failed { get; set; }
    public bool Stopped { get; set; }
    public List<string> Messages { get; } = [];

    public string ToSummary() => string.Join("\n",
        new[]
        {
            $"Normalize blog tags {(Stopped ? "stopped" : "completed")}",
            $"Posts: {Scanned} scanned, {Changed} changed, {Unchanged} unchanged, {Failed} failed"
        }.Concat(Messages));
}
