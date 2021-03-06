﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Engine
{

  public enum SynthesisStatus
  {
    BUSTED,
    IN_PROGRESS,
    COMPLETED
  }

  public struct StateDetails
  {
    public ulong w1;
    public ulong w2;
    public ulong w3;

    public ulong RawWord1 { get { return w1; } }
    public ulong RawWord2 { get { return w2; } }

    public ulong RawWord3 { get { return w3; } }

    /* 
       | max progress (7 bits) | progress (7 bits) | max durability (7 bits) | durability (7 bits) | maxcp (9 bits) | cp (9 bits) | control (9 bits) | craftsmanship (9 bits) |
     */

    private static readonly int kCraftsmanshipOffset = 0;
    private static readonly int kCraftsmanshipLength = 9;
    private static readonly int kControlOffset = kCraftsmanshipOffset + kCraftsmanshipLength;
    private static readonly int kControlLength = 9;
    private static readonly int kCPOffset = kControlOffset + kControlLength;
    private static readonly int kCPLength = 9;
    private static readonly int kMaxCPOffset = kCPOffset + kCPLength;
    private static readonly int kMaxCPLength = 9;
    private static readonly int kDurabilityOffset = kMaxCPOffset + kMaxCPLength;
    private static readonly int kDurabilityLength = 7;
    private static readonly int kMaxDurabilityOffset = kDurabilityOffset + kDurabilityLength;
    private static readonly int kMaxDurabilityLength = 7;
    private static readonly int kProgressOffset = kMaxDurabilityOffset + kMaxDurabilityLength;
    private static readonly int kProgressLength = 7;
    private static readonly int kMaxProgressOffset = kProgressOffset + kProgressLength;
    private static readonly int kMaxProgressLength = 7;

    /* 
       | steady hand (3 bits) | ingenuity (2 bits) | great strides (2 bits) | manipulation (2 bits) | condition (2 bits) | crafter level (6 bits) | synth level (6 bits) |
     */
    private static readonly int kSynthLevelOffset = 0;
    private static readonly int kSynthLevelLength = 6;
    private static readonly int kQualityOffset = kSynthLevelOffset + kSynthLevelLength;
    private static readonly int kQualityLength = 11;
    private static readonly int kMaxQualityOffset = kQualityOffset + kQualityLength;
    private static readonly int kMaxQualityLength = 11;
    private static readonly int kCrafterLevelOffset = kMaxQualityOffset + kMaxQualityLength;
    private static readonly int kCrafterLevelLength = 6;
    private static readonly int kConditionOffset = kCrafterLevelOffset + kCrafterLevelLength;
    private static readonly int kConditionLength = 2;
    private static readonly int kManipulationOffset = kConditionOffset + kConditionLength;
    private static readonly int kManipulationLength = 3;
    private static readonly int kGreatStridesOffset = kManipulationOffset + kManipulationLength;
    private static readonly int kGreatStridesLength = 3;
    private static readonly int kIngenuityOffset = kGreatStridesOffset + kGreatStridesLength;
    private static readonly int kIngenuityLength = 3;
    private static readonly int kSteadyHandOffset = kIngenuityOffset + kIngenuityLength;
    private static readonly int kSteadyHandLength = 3;
    private static readonly int kInnerQuietOffset = kSteadyHandOffset + kSteadyHandLength;
    private static readonly int kInnerQuietLength = 6;
    private static readonly int kWasteNotOffset = kInnerQuietOffset + kInnerQuietLength;
    private static readonly int kWasteNotLength = 3;
    private static readonly int kSteadyHand2Offset = kWasteNotOffset + kWasteNotLength;
    private static readonly int kSteadyHand2Length = 3;
    private static readonly int kIngenuity2Offset = kSteadyHand2Offset + kSteadyHand2Length;
    private static readonly int kIngenuity2Length = 3;


    private static readonly int kComfortZoneOffset = 0;
    private static readonly int kComfortZoneLength = 4;
    private static readonly int kInnovationOffset = kComfortZoneOffset + kComfortZoneLength;
    private static readonly int kInnovationLength = 3;
    private static readonly int kWasteNot2Offset = kInnovationOffset + kInnovationLength;
    private static readonly int kWasteNot2Length = 4;


    private uint Retrieve(ulong bitfield, int bitoffset, int bitlength)
    {
      ulong mask = ((1UL << bitlength) - 1UL) << bitoffset;
      ulong result = bitfield & mask;
      return (uint)(result >> bitoffset);
    }

    private void Assign(ref ulong bitfield, int bitoffset, int bitlength, uint value)
    {
      Debug.Assert(value < (1 << bitlength));

      ulong mask1 = ((1UL << bitlength) - 1UL) << bitoffset;  // 0000000000011111000000
      ulong mask2 = ~mask1;                                   // 1111111111100000111111

      // Clear the old value
      bitfield &= mask2;

      ulong assignmentMask = (ulong)value << bitoffset;
      // Or in the new value
      bitfield |= assignmentMask;
    }

    public uint Craftsmanship
    {
      get { return Retrieve(w1, kCraftsmanshipOffset, kCraftsmanshipLength); }
      set { Assign(ref w1, kCraftsmanshipOffset, kCraftsmanshipLength, value); }
    }

    public uint Control
    {
      get { return Retrieve(w1, kControlOffset, kControlLength); }
      set { Assign(ref w1, kControlOffset, kControlLength, value); }
    }

    public uint CP
    {
      get { return Retrieve(w1, kCPOffset, kCPLength); }
      set { Assign(ref w1, kCPOffset, kCPLength, value); }
    }

    public uint MaxCP
    {
      get { return Retrieve(w1, kMaxCPOffset, kMaxCPLength); }
      set { Assign(ref w1, kMaxCPOffset, kMaxCPLength, value); }
    }

    public uint Durability
    {
      get { return Retrieve(w1, kDurabilityOffset, kDurabilityLength); }
      set { Assign(ref w1, kDurabilityOffset, kDurabilityLength, value); }
    }

    public uint MaxDurability
    {
      get { return Retrieve(w1, kMaxDurabilityOffset, kMaxDurabilityLength); }
      set { Assign(ref w1, kMaxDurabilityOffset, kMaxDurabilityLength, value); }
    }

    public uint Progress
    {
      get { return Retrieve(w1, kProgressOffset, kProgressLength); }
      set { Assign(ref w1, kProgressOffset, kProgressLength, value); }
    }

    public uint MaxProgress
    {
      get { return Retrieve(w1, kMaxProgressOffset, kMaxProgressLength); }
      set { Assign(ref w1, kMaxProgressOffset, kMaxProgressLength, value); }
    }

    public uint SynthLevel
    {
      get { return Retrieve(w2, kSynthLevelOffset, kSynthLevelLength); }
      set { Assign(ref w2, kSynthLevelOffset, kSynthLevelLength, value); }
    }

    public uint Quality
    {
      get { return Retrieve(w2, kQualityOffset, kQualityLength); }
      set { Assign(ref w2, kQualityOffset, kQualityLength, value); }
    }

    public uint MaxQuality
    {
      get { return Retrieve(w2, kMaxQualityOffset, kMaxQualityLength); }
      set { Assign(ref w2, kMaxQualityOffset, kMaxQualityLength, value); }
    }

    public uint CrafterLevel
    {
      get { return Retrieve(w2, kCrafterLevelOffset, kCrafterLevelLength); }
      set { Assign(ref w2, kCrafterLevelOffset, kCrafterLevelLength, value); }
    }

    public int LevelSurplus
    {
      get
      {
        int result = (int)CrafterLevel - (int)SynthLevel;
        if (IngenuityTurns > 0)
          return result = Math.Max(result, 0);
        return result;
      }
    }

    public Condition Condition
    {
      get { return (Condition)Retrieve(w2, kConditionOffset, kConditionLength); }
      set { Assign(ref w2, kConditionOffset, kConditionLength, (uint)value); }
    }

    public uint ManipulationTurns
    {
      get { return Retrieve(w2, kManipulationOffset, kManipulationLength); }
      set { Assign(ref w2, kManipulationOffset, kManipulationLength, value); }
    }

    public uint GreatStridesTurns
    {
      get { return Retrieve(w2, kGreatStridesOffset, kGreatStridesLength); }
      set { Assign(ref w2, kGreatStridesOffset, kGreatStridesLength, value); }
    }

    public uint IngenuityTurns
    {
      get { return Retrieve(w2, kIngenuityOffset, kIngenuityLength); }
      set { Assign(ref w2, kIngenuityOffset, kIngenuityLength, value); }
    }

    public uint SteadyHandTurns
    {
      get { return Retrieve(w2, kSteadyHandOffset, kSteadyHandLength); }
      set { Assign(ref w2, kSteadyHandOffset, kSteadyHandLength, value); }
    }

    private uint InnerQuietValue
    {
      get { return Retrieve(w2, kInnerQuietOffset, kInnerQuietLength); }
      set { Assign(ref w2, kInnerQuietOffset, kInnerQuietLength, value); }
    }

    public uint InnerQuietStacks
    {
      get 
      {
        // The lowest bit is the active flag.
        return InnerQuietValue >> 1;
      }
      set 
      {
        uint iqvalue = InnerQuietValue;
        iqvalue = (iqvalue & 0x1) | (value << 1);
        InnerQuietValue = iqvalue;
      }
    }

    public bool InnerQuietIsActive
    {
      get
      {
        return ((InnerQuietValue & 0x1) == 1) ? true : false;
      }
      set
      {
        uint iqvalue = InnerQuietValue;
        iqvalue &= 0xFFFFFFFE;
        if (value)
          iqvalue |= 0x1;
        InnerQuietValue = iqvalue;
      }
    }

    public uint WasteNotTurns
    {
      get { return Retrieve(w2, kWasteNotOffset, kWasteNotLength); }
      set { Assign(ref w2, kWasteNotOffset, kWasteNotLength, value); }
    }

    public uint SteadyHand2Turns
    {
      get { return Retrieve(w2, kSteadyHand2Offset, kSteadyHand2Length); }
      set { Assign(ref w2, kSteadyHand2Offset, kSteadyHand2Length, value); }
    }

    public uint Ingenuity2Turns
    {
      get { return Retrieve(w2, kIngenuity2Offset, kIngenuity2Length); }
      set { Assign(ref w2, kIngenuity2Offset, kIngenuity2Length, value); }
    }

    public uint ComfortZoneTurns
    {
      get { return Retrieve(w3, kComfortZoneOffset, kComfortZoneLength); }
      set { Assign(ref w3, kComfortZoneOffset, kComfortZoneLength, value); }
    }

    public uint InnovationTurns
    {
      get { return Retrieve(w3, kInnovationOffset, kInnovationLength); }
      set { Assign(ref w3, kInnovationOffset, kInnovationLength, value); }
    }

    public uint WasteNot2Turns
    {
      get { return Retrieve(w3, kWasteNot2Offset, kWasteNot2Length); }
      set { Assign(ref w3, kWasteNot2Offset, kWasteNot2Length, value); }
    }

    public SynthesisStatus Status
    {
      get
      {
        if (Progress < MaxProgress)
        {
          if (Durability == 0)
            return SynthesisStatus.BUSTED;
          else
            return SynthesisStatus.IN_PROGRESS;
        }
        else
          return SynthesisStatus.COMPLETED;
      }
    }
  }

  public class State
  {
    internal StateDetails details;

    private State previousState;
    private Ability leadingAction;
    private uint step;

    internal List<TemporaryEnhancementAbility> tempEffects;

    public State()
    {
      details = new StateDetails();
      tempEffects = new List<TemporaryEnhancementAbility>();
      step = 1;
      previousState = null;
      leadingAction = null;
    }

    // Makes a new state which was arrived at by performing an action from a
    // previous state.
    public State(State oldState, Ability leadingAction)
    {
      this.details = oldState.details;

      this.previousState = oldState;    // do we need to clone here?
      this.leadingAction = leadingAction;

      this.step = oldState.step + 1;
      tempEffects = new List<TemporaryEnhancementAbility>(oldState.tempEffects);
    }

    // Makes an exact copy of the original state
    public State(State oldState)
    {
      Debug.Assert(oldState != null);

      this.details = oldState.details;

      this.previousState = oldState.previousState;    // do we need to clone here?
      this.leadingAction = oldState.leadingAction;
      this.step = oldState.step;
      tempEffects = new List<TemporaryEnhancementAbility>(oldState.tempEffects);
    }

    public override bool Equals(object obj)
    {
      State other = obj as State;
      if (other == null)
        return false;

      if (!details.Equals(other.details))
        return false;

      // State comparison is only concerned with the state details, not the state that we
      // were in before or the action that got us here.  This is because 2 different states
      // can lead to the same state through the use of different actions.  Because of this
      // it's incorrect to compare previousState or leadingAction here.
      return true;
    }

    public override int GetHashCode()
    {
      return details.GetHashCode();
    }

    public uint Step
    {
      get { return step; }
    }
    public State PreviousState
    {
      get { return previousState; }
    }

    public Ability LeadingAction
    {
      get { return leadingAction; }
    }

    public SynthesisStatus Status
    {
      get 
      {
        if (Progress == MaxProgress)
          return SynthesisStatus.COMPLETED;
        if (Durability == 0)
          return SynthesisStatus.BUSTED;
        return SynthesisStatus.IN_PROGRESS;
      }
    }
    public uint Craftsmanship
    {
      get { return details.Craftsmanship; }
      set { details.Craftsmanship = value; }
    }

    public uint Control
    {
      get { return details.Control; }
      set { details.Control = value; }
    }
    public uint CP
    {
      get { return details.CP; }
      set { details.CP = value; }
    }
    public uint MaxCP
    {
      get { return details.MaxCP; }
      set { details.MaxCP = value; }
    }
    public int LevelSurplus
    {
      get { return (int)CrafterLevel - (int)SynthLevel; }
    }
    public uint CrafterLevel
    {
      get { return details.CrafterLevel; }
      set { details.CrafterLevel = value; }
    }
    public uint SynthLevel
    {
      get { return details.SynthLevel; }
      set { details.SynthLevel = value; }
    }

    // The synthesis' stats in this state.
    public uint Durability
    {
      get { return details.Durability; }
      set { details.Durability = value; }
    }
    public uint MaxDurability
    {
      get { return details.MaxDurability; }
      set { details.MaxDurability = value; }
    }
    public uint Quality
    {
      get { return details.Quality; }
      set { details.Quality = value; }
    }
    public uint MaxQuality
    {
      get { return details.MaxQuality; }
      set { details.MaxQuality = value; }
    }
    public uint Progress
    {
      get { return details.Progress; }
      set { details.Progress = value; }
    }
    public uint MaxProgress 
    {
      get { return details.MaxProgress; }
      set { details.MaxProgress = value; }
    }

    public Condition Condition
    {
      get { return details.Condition; }
      set { details.Condition = value; }
    }

    public uint ManipulationTurns
    {
      get { return details.ManipulationTurns; }
      set { details.ManipulationTurns = value; }
    }

    public uint GreatStridesTurns
    {
      get { return details.GreatStridesTurns; }
      set { details.GreatStridesTurns = value; }
    }

    public uint IngenuityTurns
    {
      get { return details.IngenuityTurns; }
      set { details.IngenuityTurns = value; }
    }

    public uint SteadyHandTurns
    {
      get { return details.SteadyHandTurns; }
      set { details.SteadyHandTurns = value; }
    }

    public uint InnerQuietStacks
    {
      get { return details.InnerQuietStacks; }
      set { details.InnerQuietStacks = value; }
    }

    public bool InnerQuietIsActive
    {
      get { return details.InnerQuietIsActive; }
      set { details.InnerQuietIsActive = value; }
    }

    public uint WasteNotTurns
    {
      get { return details.WasteNotTurns; }
      set { details.WasteNotTurns = value; }
    }

    public uint SteadyHand2Turns
    {
      get { return details.SteadyHand2Turns; }
      set { details.SteadyHand2Turns = value; }
    }

    public uint Ingenuity2Turns
    {
      get { return details.Ingenuity2Turns; }
      set { details.Ingenuity2Turns = value; }
    }

    public uint ComfortZoneTurns
    {
      get { return details.ComfortZoneTurns; }
      set { details.ComfortZoneTurns = value; }
    }

    public uint InnovationTurns
    {
      get { return details.InnovationTurns; }
      set { details.InnovationTurns = value; }
    }

    public uint WasteNot2Turns
    {
      get { return details.WasteNot2Turns; }
      set { details.WasteNot2Turns = value; }
    }


    public double Score
    {
      get 
      {
        return Compute.StateScore(this);
      }
    }

    public double FailureProbability 
    {
      get { return Compute.FailureProbability(this);  }
    }

    public double SuccessProbability 
    {
      get { return 1.0 - FailureProbability; }
    }
  }
}
