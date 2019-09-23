using System;
using Common.Hashes;
using MessagePack;
using static Gantt.Jobs.JobStatus;

namespace Gantt.Jobs
{
  [MessagePackObject]
  public struct Job : IEquatable<Job>
  {
    [Key(0)] public JobStatus Status;
    [Key(0)] public int Progress;
    [Key(1)] public int Amount;
    
    public Job(int total)
    {
      Amount = total;
      Progress = 0;
      Status = Free;
    }

    public JobStatus Perform()
    {
      Status = InProgress;
      
      if (++Progress == Amount)
        Status = Done;

      return Status;
    }

    public override bool Equals(object other) => 
      other is Job job && Equals(job);

    public override string ToString() => 
      $"{Progress} / {Amount}";

    public bool Equals(Job other) =>
      Amount == other.Amount;

    public override int GetHashCode() =>
      Hash.Of(Amount);
  }
}