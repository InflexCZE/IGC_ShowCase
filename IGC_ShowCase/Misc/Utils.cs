using System;
using System.Text;
using System.Collections.Generic;

using VRageMath;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace IGC_ShowCase
{
    public static class Utils
    {
        //#script begin

        public static TBlock GetFirstBlockOfType<TBlock>(this MyGridProgram program) where TBlock : class
        {
            var blocks = GetAllBlocksOfType<TBlock>(program);

            if(blocks.Count == 0)
                throw new InvalidOperationException($"No block of type {typeof(TBlock).Name} found!");

            return blocks[0];
        }

        public static TBlock GetClosestBlockOfType<TBlock>(this MyGridProgram program) where TBlock : class, IMyCubeBlock
        {
            var blocks = GetAllBlocksOfType<TBlock>(program);

            if(blocks.Count == 0)
                throw new InvalidOperationException($"No block of type {typeof(TBlock).Name} found!");

            TBlock block = null;
            var distance = double.MaxValue;
            var myPosition = program.Me.GetPosition();
            foreach(var x in blocks)
            {
                var d = Vector3D.DistanceSquared(x.GetPosition(), myPosition);
                if(d < distance)
                {
                    block = x;
                    distance = d;
                }
            }

            return block;
        }

        public static List<TBlock> GetAllBlocksOfType<TBlock>(this MyGridProgram program) where TBlock : class
        {
            var blocks = new List<TBlock>();
            program.GridTerminalSystem.GetBlocksOfType(blocks);

            return blocks;
        }

        //#script end
    }

    //#script begin

    public class LCDLogger
    {
        public const int MaxTextFeedLength = 16;

        public IMyTextPanel LCD { get; }
        public Queue<string> TextFeed { get; } = new Queue<string>(MaxTextFeedLength);

        private StringBuilder Buffer = new StringBuilder();

        public LCDLogger(IMyTextPanel LCD)
        {
            this.LCD = LCD;
        }

        public void WriteLine(string str)
        {
            while(this.TextFeed.Count >= MaxTextFeedLength)
                this.TextFeed.Dequeue();

            this.TextFeed.Enqueue(str);

            foreach(var line in this.TextFeed)
            {
                this.Buffer.AppendLine(line);
            }

            this.LCD.WritePublicText(this.Buffer);
            this.Buffer.Clear();
        }
    }

    //#script end
}
