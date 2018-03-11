using System;
using System.Collections.Generic;
using System.Diagnostics;
using BrunoMikoski.Pahtfinding.Gameplay;
using UnityEditor;
using Object = UnityEngine.Object;

namespace BrunoMikoski.Profiler
{
    public static class ProfilerController 
    {

        [MenuItem("Profiler/Debug Average time for Biggest Path")]
        public static void ProfileBiggestPath()
        {
            GameplayController gameplayController = Object.FindObjectOfType<GameplayController>();
            ProfileAction( 100, () =>
            {
                gameplayController.PrintBiggestPath();
            });
        }
        
        public static void ProfileAction( int numberOfTimes, Action targetAction )
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            targetAction();
            
            Stopwatch stopwatch = new Stopwatch();
            
            List<long> allTicks  = new List<long>();
            for ( int i = 0; i < numberOfTimes; i++ )
            {
                stopwatch.Reset();
                stopwatch.Start();

                targetAction();
                
                stopwatch.Stop();
                allTicks.Add( stopwatch.ElapsedTicks );
            }

            long average = GetAverage( allTicks );
            UnityEngine.Debug.Log( "Average Ticks: " + average );
        }

        private static long GetAverage( List<long> allTicks )
        {
            allTicks.Sort();
            allTicks.RemoveAt( allTicks.Count - 1 );
            allTicks.RemoveAt( 0 );
            long totalTime = 0;
            for ( int i = allTicks.Count - 1; i >= 0; i-- )
            {
                totalTime += allTicks[i];
            }

            return totalTime / allTicks.Count;
        }
    }
}

