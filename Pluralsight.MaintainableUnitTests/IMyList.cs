public interface IMyList
{
    int Length { get; }

    void Append(int value);
    void AppendMany(int[] values);
    int ElementAt(int index);
}