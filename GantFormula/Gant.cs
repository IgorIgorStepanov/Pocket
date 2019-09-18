using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GantFormula.Extensions;
using static System.Threading.Tasks.Task;

namespace GantFormula
{
  public class Gant : IGant
  {
    public GantSolution Best => _best;

    public long TotalTasks;
    public long RunningTasks;

    private readonly List<Developer> _developers;
    private readonly List<Qa> _qas;
    private readonly List<JiraTask> _tasks;

    private GantSolution _best;
    private int NextId = 1;
    
    private readonly object _lock = new object();

    public Gant(List<Developer> developers, List<Qa> qas, List<JiraTask> tasks)
    {
      _developers = developers;
      _qas = qas;
      _tasks = tasks;
    }
    
    public void Calculate()
    {
      Interlocked.Increment(ref TotalTasks);
      Interlocked.Increment(ref RunningTasks);

      Run(() =>
        {
          new GantSolution(_developers, _qas, _tasks)
          {
            Id = NextId,
            Gant = this,
          }.Calculate();

          Interlocked.Decrement(ref RunningTasks);
        }
      );
    }
    
    public void Continue(GantSolution solution, Combination devCombination = null, Combination qaCombination = null)
    {
      var solutionVariant = solution.Copy();
      
      solutionVariant.Id = Interlocked.Increment(ref NextId);
      solutionVariant.Gant = this;
      
      solutionVariant.DevCombination = devCombination;
      solutionVariant.QaCombination = qaCombination;

      Interlocked.Increment(ref TotalTasks);
      Interlocked.Increment(ref RunningTasks);

      Run(() =>
      {
        solutionVariant.Calculate();
        Interlocked.Decrement(ref RunningTasks);
      });
    }

    public void Add(GantSolution solutionVariant)
    {
      if (solutionVariant.Day >= _best?.Day) 
        return;
      
      GantSolution initial;
      do
      {
        initial = _best;
        
        if (solutionVariant.Day >= initial?.Day)
          break;
      }
      while (initial != Interlocked.CompareExchange(ref _best, solutionVariant, initial));
    }
  }
}