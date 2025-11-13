using System.Collections.Generic;
using UnityEngine;

public enum SpawnPatternType
{
    Alternating,
    Burst,
    Focused,
    Surround,
    Escort
}

public struct SpawnInstruction
{
    public int SpawnPointIndex;
    public float Delay;
    public bool ForceElite;
    public bool ForceMiniBoss;
}

/// <summary>
/// Generates procedural spawn plans (path order + tempo) per wave.
/// Combined with AdaptiveWaveDifficulty to create readable but varied challenges.
/// </summary>
public static class ProceduralSpawnPatternPlanner
{
    public static List<SpawnInstruction> BuildPattern(
        SpawnPatternType pattern,
        int totalEnemies,
        float baseDelay,
        int spawnPointCount,
        int eliteBudget,
        int miniBossBudget)
    {
        List<SpawnInstruction> plan = new List<SpawnInstruction>(Mathf.Max(0, totalEnemies));

        if (totalEnemies <= 0)
        {
            return plan;
        }

        spawnPointCount = Mathf.Max(1, spawnPointCount);
        baseDelay = Mathf.Max(0.05f, baseDelay);

        switch (pattern)
        {
            case SpawnPatternType.Burst:
                BuildBurstPlan(plan, totalEnemies, baseDelay, spawnPointCount);
                break;
            case SpawnPatternType.Focused:
                BuildFocusedPlan(plan, totalEnemies, baseDelay, spawnPointCount);
                break;
            case SpawnPatternType.Surround:
                BuildSurroundPlan(plan, totalEnemies, baseDelay, spawnPointCount);
                break;
            case SpawnPatternType.Escort:
                BuildEscortPlan(plan, totalEnemies, baseDelay, spawnPointCount);
                break;
            default:
                BuildAlternatingPlan(plan, totalEnemies, baseDelay, spawnPointCount);
                break;
        }

        AssignPriorityTargets(plan, eliteBudget, miniBossBudget, pattern);
        return plan;
    }

    static void BuildAlternatingPlan(List<SpawnInstruction> plan, int totalEnemies, float baseDelay, int spawnPointCount)
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            plan.Add(new SpawnInstruction
            {
                SpawnPointIndex = i % spawnPointCount,
                Delay = baseDelay
            });
        }
    }

    static void BuildBurstPlan(List<SpawnInstruction> plan, int totalEnemies, float baseDelay, int spawnPointCount)
    {
        int remaining = totalEnemies;

        while (remaining > 0)
        {
            int burstSize = Mathf.Clamp(Random.Range(2, 5), 1, remaining);
            int pathIndex = Random.Range(0, spawnPointCount);

            for (int i = 0; i < burstSize; i++)
            {
                bool lastInBurst = i == burstSize - 1;
                float delay = baseDelay * (lastInBurst ? 1.3f : 0.4f);

                plan.Add(new SpawnInstruction
                {
                    SpawnPointIndex = pathIndex,
                    Delay = delay
                });
            }

            remaining -= burstSize;
        }
    }

    static void BuildFocusedPlan(List<SpawnInstruction> plan, int totalEnemies, float baseDelay, int spawnPointCount)
    {
        int mainLane = Random.Range(0, spawnPointCount);
        int flankLane = (mainLane + Random.Range(1, spawnPointCount)) % spawnPointCount;

        for (int i = 0; i < totalEnemies; i++)
        {
            bool useMain = Random.value > 0.35f;
            plan.Add(new SpawnInstruction
            {
                SpawnPointIndex = useMain ? mainLane : flankLane,
                Delay = baseDelay * (useMain ? 0.85f : 1.1f)
            });
        }
    }

    static void BuildSurroundPlan(List<SpawnInstruction> plan, int totalEnemies, float baseDelay, int spawnPointCount)
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            bool endOfCycle = (i + 1) % spawnPointCount == 0;
            plan.Add(new SpawnInstruction
            {
                SpawnPointIndex = i % spawnPointCount,
                Delay = endOfCycle ? baseDelay * 1.4f : baseDelay * 0.6f
            });
        }
    }

    static void BuildEscortPlan(List<SpawnInstruction> plan, int totalEnemies, float baseDelay, int spawnPointCount)
    {
        int escortGroup = Mathf.Clamp(totalEnemies / 3, 2, 6);

        for (int i = 0; i < totalEnemies; i++)
        {
            float delay = baseDelay;

            if (i < escortGroup)
            {
                delay *= 0.75f;
            }
            else if (i == escortGroup)
            {
                delay *= 1.5f;
            }
            else
            {
                delay *= 1.1f;
            }

            plan.Add(new SpawnInstruction
            {
                SpawnPointIndex = i % spawnPointCount,
                Delay = delay
            });
        }
    }

    static void AssignPriorityTargets(List<SpawnInstruction> plan, int eliteBudget, int miniBossBudget, SpawnPatternType pattern)
    {
        if (plan.Count == 0)
        {
            return;
        }

        if (miniBossBudget > 0)
        {
            int spacing = Mathf.Max(1, plan.Count / (miniBossBudget + 1));
            for (int i = 0; i < miniBossBudget; i++)
            {
                int index = Mathf.Clamp(spacing * (i + 1) - 1, 0, plan.Count - 1);
                if (pattern == SpawnPatternType.Escort && i == miniBossBudget - 1)
                {
                    index = plan.Count - 1;
                }

                var instruction = plan[index];
                instruction.ForceMiniBoss = true;
                instruction.Delay *= 1.4f;
                plan[index] = instruction;
            }
        }

        if (eliteBudget > 0)
        {
            int spacing = Mathf.Max(1, plan.Count / (eliteBudget + 1));
            for (int i = 0; i < eliteBudget; i++)
            {
                int index = Mathf.Clamp(spacing * i, 0, plan.Count - 1);

                // Avoid overriding a mini-boss slot.
                while (index > 0 && plan[index].ForceMiniBoss)
                {
                    index--;
                }

                var instruction = plan[index];
                if (instruction.ForceMiniBoss)
                {
                    continue;
                }

                instruction.ForceElite = true;
                plan[index] = instruction;
            }
        }
    }
}
