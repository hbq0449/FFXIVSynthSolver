﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Engine
{

  public abstract class BuffAction : Action
  {
    public BuffAction()
    {
      usageChecks.Add(delegate(State state)
                      {
                        // Don't allow this buff to be used if this buff is already active.
                        return !IsBuffActive(state);
                      });
      successOperators.Add(delegate(State originalState, State newState) 
                           {
                             ApplyBuff(newState);
                           });
    }

    public abstract uint GetTurnsRemaining(State state);
    public abstract void SetTurnsRemaining(State state, uint turns);

    public bool IsBuffActive(State state)
    {
      return GetTurnsRemaining(state) > 0;
    }

    // Called when the Step advances while the buff is active.
    public virtual void TickBuff(State state)
    {
      uint remain = GetTurnsRemaining(state);
      if (remain > 0)
        SetTurnsRemaining(state, remain-1);
    }

    // Called when a buff is applied for the first time.
    public virtual void ApplyBuff(State state)
    {
      SetTurnsRemaining(state, Attributes.BuffDuration + 1);
    }
  }

  [SynthAction(ActionType.Buff, Name = "Steady Hand", CP = 22, BuffDuration=5)]
  public class SteadyHand : BuffAction
  {
    public SteadyHand()
    {
      // Don't allow Steady hand with < 50 durability (since it would waste one of its procs on
      // restoring durability) unless we're in a bind and we need progress quickly.
      usageChecks.Add(delegate(State state)
                      {
                        if (state.Durability >= 50)
                          return true;
                        // If we have enough CP to restore durability, it's not urgent so don't
                        // allow Steady hand to be used.
                        if (state.CP >= Compute.CP(92, state))
                          return false;

                        // Otherwise either we must use SH for safety reasons, or there's no additional harm
                        // in doing so, so allow it to be used.
                        return true;
                      });
    }
    public override uint GetTurnsRemaining(State state) { return state.SteadyHandTurns; }
    public override void SetTurnsRemaining(State state, uint turns) { state.SteadyHandTurns = turns; }
  }

  // TODO
  [SynthAction(ActionType.Buff, Name = "Inner Quiet", CP = 18, Disabled = true, BuffDuration=0)]
  public class InnerQuiet : BuffAction
  {
    public override uint GetTurnsRemaining(State state) { return 0; }
    public override void SetTurnsRemaining(State state, uint turns) { }
  }

  // TODO
  [SynthAction(ActionType.Buff, Name = "Manipulation", CP = 88, BuffDuration = 3)]
  public class Manipulation : BuffAction
  {
    public override uint GetTurnsRemaining(State state)
    {
      return state.ManipulationTurns;
    }

    public override void SetTurnsRemaining(State state, uint turns)
    {
      state.ManipulationTurns = turns;
    }

    public override void TickBuff(State state)
    {
      base.TickBuff(state);

      // Only perform the actual effect if this was not the first tick.
      if (GetTurnsRemaining(state) <= Attributes.BuffDuration)
        state.Durability = Math.Min(state.MaxDurability, state.Durability + 10);
    }
  }

  [SynthAction(ActionType.Buff, Name="Ingenuity", CP = 24, BuffDuration=3)]
  public class Ingenuity : BuffAction
  {
    public Ingenuity()
    {
      // Only allow ingenuity to run if we're at least 2 levels below the synth
      usageChecks.Add(delegate(State state) { return state.LevelSurplus <= -2; });
    }
    public override uint GetTurnsRemaining(State state)
    {
      return state.IngenuityTurns;
    }

    public override void SetTurnsRemaining(State state, uint turns)
    {
      state.IngenuityTurns = turns;
    }
  }

  [SynthAction(ActionType.Buff, Name = "Great Strides", CP = 32, BuffDuration = 3)]
  public class GreatStrides : BuffAction
  {
    public GreatStrides()
    {
      usageChecks.Add(delegate(State state) { return state.Quality <= state.MaxQuality; });
    }

    public override uint GetTurnsRemaining(State state)
    {
      return state.GreatStridesTurns;
    }

    public override void SetTurnsRemaining(State state, uint turns)
    {
      state.GreatStridesTurns = turns;
    }
  }
}
