using System.Collections.Generic;

public interface IMyList
{
    int Length { get; }
    int Count { get; }

    void Append(int value);
    void AppendMany(int[] values);
    int ElementAt(int index);
    int GetFirst();
}